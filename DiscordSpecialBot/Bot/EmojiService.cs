using DSharpPlus;
using DSharpPlus.Entities;
using System;

namespace DiscordSpecialBot.Bot
{
    public class EmojiService
    {
        DiscordClient discord;

        public EmojiService(DiscordClient discordClient)
        {
            discord = discordClient;
        }

        public DiscordEmoji getEmoji(string chat)
        {
            DiscordEmoji emoji;
            try
            {
                if (chat.Contains(":"))
                {
                    emoji = DiscordEmoji.FromName(discord, chat);
                }
                else
                {
                    emoji = DiscordEmoji.FromName(discord, $":{chat}:");
                }
            }
            catch (Exception)
            {
                emoji = DiscordEmoji.FromUnicode(discord, chat);
            }

            return emoji;
        }
    }
}
