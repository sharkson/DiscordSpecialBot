using ChatModels;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;

namespace DiscordSpecialBot.Bot
{
    public class RequiredPropertyResponseService
    {
        DiscordClient discord;
        BotUtilityService utilityService;

        public RequiredPropertyResponseService(DiscordClient discordClient, BotUtilityService botUtilityService)
        {
            discord = discordClient;
            utilityService = botUtilityService;
        }

        public async void hasRequiredPropertyResponse(MessageCreateEventArgs e, ChatResponse chatResponse, ChannelSettings channelSettings)
        {
            if (utilityService.alwaysRespond(e) || chatResponse.confidence > channelSettings.TargetedResponseConfidenceThreshold)
            {
                var typeTime = 0;
                foreach (var chat in chatResponse.response)
                {
                    DiscordEmoji emoji = null;
                    try
                    {
                        emoji = DiscordEmoji.FromName(discord, chat.Trim());
                    }
                    catch (Exception)
                    {

                    }

                    if (emoji != null)
                    {
                        await e.Message.CreateReactionAsync(emoji);
                    }
                    else
                    {
                        await Task.Delay(typeTime).ContinueWith((task) => { e.Channel.TriggerTypingAsync(); });

                        var response = chat.Trim();

                        response = utilityService.formatResponse(e, chat);

                        typeTime += utilityService.getTypeTime(chat);
                        await Task.Delay(typeTime).ContinueWith((task) => { e.Message.RespondAsync(response); });
                    }
                }
            }
        }
    }
}
