using DiscordSpecialBot.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordSpecialBot.Bot
{
    public class BotUtilityService
    {
        DiscordClient discord;
        BotConfiguration configuration;

        public BotUtilityService(DiscordClient discordClient, BotConfiguration botConfiguration)
        {
            discord = discordClient;
            configuration = botConfiguration;
        }

        public string formatResponse(MessageCreateEventArgs e, string chat)
        {
            var response = chat.Trim();

            if (response.StartsWith("/me "))
            {
                response = Formatter.Italic(response.Replace("/me ", string.Empty));
            }

            var regex = new Regex("@(?<name>[^\\s]+)");
            var results = regex.Matches(response)
                .Cast<Match>()
                .Select(m => m.Groups["name"].Value)
                .ToArray();

            foreach (var userName in results)
            {
                var user = e.Guild.Members.Where(m => m.Username == userName).FirstOrDefault();
                if (user != null)
                {
                    var mention = Formatter.Mention(user);
                    response = response.Replace("@" + userName, mention);
                }
            }

            return response;
        }

        public bool alwaysRespond(MessageCreateEventArgs e)
        {
            if (e.Message.Channel.Type == ChannelType.Private || e.MentionedUsers.Contains(discord.CurrentUser) || e.Message.Content.Contains(discord.CurrentUser.Username) || configuration.NickNames.Any(nickName => e.Message.Content.ToLower().Contains(nickName.ToLower())))
            {
                return true;
            }
            return false;
        }

        public int getTypeTime(string message)
        {
            return message.Length * 80;
        }

        public async void defaultResponse(MessageCreateEventArgs e)
        {
            await e.Channel.TriggerTypingAsync();
            var response = formatResponse(e, configuration.DefaultResponse);
            var typeTime = getTypeTime(response);
            await Task.Delay(typeTime).ContinueWith((task) => { e.Message.RespondAsync(response); });
        }

        public bool ignoreMessage(MessageCreateEventArgs e)
        {
            if (e.Message.MentionedUsers.Any(u => u.Username == discord.CurrentUser.Username))
            {
                return false;
            }
            if (e.Message.Author.IsBot || e.Message.MessageType != MessageType.Default || configuration.IgnoredChannels.Any(channel => channel == e.Message.Channel.Name))
            {
                return true;
            }
            return false;
        }
    }
}
