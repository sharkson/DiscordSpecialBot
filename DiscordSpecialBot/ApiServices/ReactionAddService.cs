using ChatModels;
using DiscordSpecialBot.Models;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordSpecialBot.ApiServices
{
    public class ReactionAddService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ReactionAddService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botCnfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botCnfiguration;
        }

        public async Task<bool> AddReactionAsync(MessageReactionAddEventArgs e)
        {
            var reaction = new Reaction();
            reaction.reaction = e.Emoji.Name;
            reaction.user = e.User.Username;
            reaction.time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var chat = utilityService.GetChat(e.Message, e.Message.MentionedUsers);
            var conversationName = utilityService.GetConversationName(e.Message);
            var metadata = utilityService.GetMetadata(e.Message);
            var reactionRequest = new ReactionRequest { reaction = reaction, chat = chat, type = configuration.ChatType, conversationName = conversationName, metadata = metadata };

            var httpContent = utilityService.GetHttpContent(reactionRequest);
            var response = await client.PutAsync(configuration.ApiUrl + "/api/reactionupdate", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var success = JsonConvert.DeserializeObject<bool>(jsonResponse);

            return success;
        }
    }
}
