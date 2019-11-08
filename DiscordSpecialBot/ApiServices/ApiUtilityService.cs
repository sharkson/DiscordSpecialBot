using ChatModels;
using DiscordSpecialBot.Models;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DiscordSpecialBot.ApiServices
{
    public class ApiUtilityService
    {
        long launchTime;
        BotConfiguration configuration;

        public ApiUtilityService(BotConfiguration botConfiguration)
        {
            configuration = botConfiguration;
            launchTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public StringContent GetHttpContent(dynamic request)
        {
            var jsonString = JsonConvert.SerializeObject(request);
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        public Metadata GetMetadata(DiscordMessage message)
        {
            return new Metadata { channelId = message.ChannelId, guildId = message.Channel.GuildId };
        }

        public Chat GetChat(DiscordMessage discordMessage, IReadOnlyList<DiscordUser> mentionedUsers)
        {
            var message = discordMessage.Content;
            foreach (var mention in mentionedUsers)
            {
                message = message.Replace(mention.Mention.Replace("!", ""), "@" + mention.Username);
                message = message.Replace(mention.Mention, "@" + mention.Username);
            }
            return new Chat { botName = configuration.BotName, message = message, user = discordMessage.Author.Username, time = DateTimeOffset.Now.ToUnixTimeMilliseconds() };
        }

        public string GetConversationName(DiscordMessage message)
        {
            return configuration.ChatType + "-discord-" + message.Channel.GuildId + "-" + message.ChannelId + "-" + launchTime;
        }
    }
}
