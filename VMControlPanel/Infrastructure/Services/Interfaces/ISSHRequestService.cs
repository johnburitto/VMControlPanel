﻿using Core.Entities;

namespace Infrastructure.Services.Interfaces
{
    public interface ISSHRequestService
    {
        Task<string> ExecuteCommandAsync(VirtualMachine virtualMachine, string command, CommandType type);
    }

    public enum CommandType
    {
        NotSudo,
        Sudo
    }
}