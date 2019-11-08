using ChatModels;
using DiscordSpecialBot.Models;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;

namespace DiscordSpecialBot.Bot
{
    public class BotReactionService
    {
        BotConfiguration configuration;
        EmojiService emojiService;

        public BotReactionService(BotConfiguration botConfiguration, EmojiService botEmojiService)
        {
            configuration = botConfiguration;
            emojiService = botEmojiService;
        }

        public async void reactionResponse(MessageCreateEventArgs e, ChatResponse chatResponse, ChannelSettings channelSettings)
        {
            if (chatResponse.confidence > channelSettings.ReactionConfidenceThreshold)
            {
                var reactions = 0;
                foreach (var chat in chatResponse.response)
                {
                    DiscordEmoji emoji = emojiService.getEmoji(chat.Trim());

                    if (emoji != null)
                    {
                        try
                        {
                            await e.Message.CreateReactionAsync(emoji);
                        }
                        catch (Exception)
                        {

                        }

                        reactions++;
                        if (reactions >= configuration.MaximumReactionsPerMessage)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
