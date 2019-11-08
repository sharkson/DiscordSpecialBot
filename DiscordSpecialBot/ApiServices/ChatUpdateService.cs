using ChatModels;
using DiscordSpecialBot.Models;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordSpecialBot.ApiServices
{
    public class ChatUpdateService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ChatUpdateService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botCnfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botCnfiguration;
        }

        public async Task<bool> UpdateChatAsync(MessageCreateEventArgs e)
        {
            var chat = utilityService.GetChat(e.Message, e.MentionedUsers);
            var conversationName = utilityService.GetConversationName(e.Message);
            var metadata = utilityService.GetMetadata(e.Message);
            var chatRequest = new ChatRequest { chat = chat, type = configuration.ChatType, conversationName = conversationName, metadata = metadata };

            var httpContent = utilityService.GetHttpContent(chatRequest);
            var response = await client.PutAsync(configuration.ApiUrl + "/api/chatupdate", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var success = JsonConvert.DeserializeObject<bool>(jsonResponse);

            return success;
        }
    }
}
