using Core.Entities;
using Infrastructure.Services.Interfaces;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Text.RegularExpressions;

namespace Infrastructure.Services.Impls
{
    public class SSHRequestService : ISSHRequestService
    {
        private Dictionary<string, SshClient> _clients = new Dictionary<string, SshClient>();
        private Dictionary<string, ShellStream> _streams = new Dictionary<string, ShellStream>();
        private Dictionary<TerminalModes, uint> _modes = new Dictionary<TerminalModes, uint> { { TerminalModes.ECHO, 53 } };

        private string _passwordRegex = @"password for [\w\d]+[$#>:]";
        private string _yesOrNoRegex = @"\[Y/n\]";
        private string _endOfCommandRegex = @":~\$";
        private string _someInputRegex = @"[$>]";

        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public Task<string> ExecuteCommandAsync(VirtualMachine virtualMachine, string command, string userId)
        {
            var client = GetClient(virtualMachine, userId);
            var stream = GetStream(client, userId, out string? log);

            return Task.FromResult($"{log}{ExecuteCommand(stream!, command)}");
        }

        private SshClient? GetClient(VirtualMachine virtualMachine, string userId)
        {
            if (_clients.TryGetValue(userId, out SshClient? client))
            {
                return client;
            }
            else
            {
                var method = new PasswordAuthenticationMethod(virtualMachine.UserName, virtualMachine.Password);
                var connection = new ConnectionInfo(virtualMachine.Host, virtualMachine.Port, virtualMachine.UserName, method);

                client = new SshClient(connection);
                _clients.Add(userId, client);

                return client;
            }
        }

        private ShellStream? GetStream(SshClient? client, string userId, out string? log)
        {
            if (_streams.TryGetValue(userId, out ShellStream? stream))
            {
                log = "";

                return stream;
            }
            else
            {
                client!.Connect();

                stream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, _modes);
                _streams.Add(userId, stream);

                EndOfCommand(stream, out log);

                return stream;
            }
        }

        private string ExecuteCommand(ShellStream stream, string command)
        {
            string? log = null;

            stream.WriteLine(command);

            while (true)
            {
                if (EndOfCommand(stream, out log))
                {
                    return log?.Replace(command, "") ?? "";
                }
                else if (PasswordInput(stream, out log))
                {
                    return log?.Replace(command, "") ?? "";
                }
                else if (YesOrNo(stream, out log))
                {
                    return log?.Replace(command, "") ?? "";
                }
                else if (SomeInput(stream, out log))
                {
                    return log?.Replace(command, "") ?? "";
                }
            }
        }

        private bool EndOfCommand(ShellStream stream, out string? log)
        {
            log = stream.Expect(new Regex(_endOfCommandRegex), TimeSpan.FromSeconds(1));

            return log != null;
        }

        private bool PasswordInput(ShellStream stream, out string? log)
        {
            log = stream.Expect(new Regex(_passwordRegex), TimeSpan.FromSeconds(1));

            return log != null;
        }

        private bool YesOrNo(ShellStream stream, out string? log)
        {
            log = stream.Expect(new Regex(_yesOrNoRegex), TimeSpan.FromSeconds(1));

            return log != null;
        }

        private bool SomeInput(ShellStream stream, out string? log)
        {
            log = stream.Expect(new Regex(_someInputRegex), TimeSpan.FromSeconds(1));

            return log != null;
        }
    }
}
