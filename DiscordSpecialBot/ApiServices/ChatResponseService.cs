using ChatModels;
using DiscordSpecialBot.Models;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordSpecialBot.ApiServices
{
    public class ChatResponseService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ChatResponseService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botCnfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botCnfiguration;
        }

        public async Task<ChatResponse> GetChatResponseAsync(MessageCreateEventArgs e)
        {
            var chat = utilityService.GetChat(e.Message, e.MentionedUsers);
            var conversationName = utilityService.GetConversationName(e.Message);
            var metadata = utilityService.GetMetadata(e.Message);
            var chatRequest = new ChatRequest { chat = chat, type = configuration.ChatType, conversationName = conversationName, metadata = metadata, requestTime = DateTime.Now, exclusiveTypes = configuration.ExclusiveTypes, requiredPropertyMatches = configuration.RequiredProperyMatches };

            var httpContent = utilityService.GetHttpContent(chatRequest);
            var response = await client.PutAsync(configuration.ApiUrl + "/api/chat", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(jsonResponse);

            return chatResponse;
        }
    }
}
