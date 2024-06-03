using Azure.AI.OpenAI;
using Serilog;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bot.HttpInfrastructure.Extensions
{
    public static class OpenAIRequestClientExtensions
    {
        private static Regex _markdownReservedRegex = new Regex(@"([\\\*_{}\[\]\(\)#\+-\.!\|<>])");

        public static async Task<string> GetOpenAIResponse(this RequestClient client, string query) 
        {
            var response = await client.OpenAIClient.GetChatCompletionsAsync(new ChatCompletionsOptions
            {
                DeploymentName = "gpt-3.5-turbo",
                Messages = { new ChatRequestSystemMessage(query) }
            });

            Log.Debug(response.Value.Choices.Count.ToString());

            foreach (var item in response.Value.Choices)
            {
                Log.Debug(item.Message.Content);   
            }

            return _markdownReservedRegex.Replace(string.Join(" ", response.Value.Choices.Select(_ => _.Message.Content).ToArray()), "\\$1");
        }
    }
}
