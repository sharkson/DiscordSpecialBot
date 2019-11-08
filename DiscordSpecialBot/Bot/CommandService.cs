using DiscordSpecialBot.Models;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Text.RegularExpressions;

namespace DiscordSpecialBot.Bot
{
    public class CommandService
    {
        BotConfiguration configuration;
        ChannelConfigurationService channelConfigurationService;

        public CommandService(BotConfiguration botConfiguration, ChannelConfigurationService channelConfiguration)
        {
            configuration = botConfiguration;
            channelConfigurationService = channelConfiguration;
        }

        public bool command(MessageCreateEventArgs e, Channel channel)
        {
            var message = e.Message.Content.ToLower();

            if (e.Guild != null && e.Author == e.Guild.Owner)
            {
                if (message.Contains("!quiet"))
                {
                    channel.ChannelSettings.TargetedResponseConfidenceThreshold = 2;
                    var response = DiscordEmoji.FromUnicode("🤐");
                    e.Message.CreateReactionAsync(response);
                    channelConfigurationService.SaveChannel(channel);
                    return true;
                }
                else if (message.Contains("!talk"))
                {
                    channel.ChannelSettings.TargetedResponseConfidenceThreshold = configuration.TargetedResponseConfidenceThreshold;
                    channelConfigurationService.SaveChannel(channel);
                    var response = DiscordEmoji.FromUnicode("👅");
                    e.Message.CreateReactionAsync(response);
                    e.Message.RespondAsync("hey 😉");                  
                    return true;
                }
                else if (SetTargetedResponseConfidenceThreshold(message, channel) || SetReactionConfidenceThreshold(message, channel)) {
                    var response = DiscordEmoji.FromUnicode("👍");
                    e.Message.CreateReactionAsync(response);
                }
            }

            return false;
        }

        private bool SetTargetedResponseConfidenceThreshold(string message, Channel channel)
        {
            Match match = Regex.Match(message, @"!threshold (^?[0-9]*\.?[0-9]+)$");

            if (match.Success)
            {
                var value = double.Parse(match.Groups[1].Value);
                channel.ChannelSettings.TargetedResponseConfidenceThreshold = value;
                channelConfigurationService.SaveChannel(channel);
                return true;
            }

            return false;
        }

        private bool SetReactionConfidenceThreshold(string message, Channel channel)
        {
            Match match = Regex.Match(message, @"!reaction (^?[0-9]*\.?[0-9]+)$");

            if (match.Success)
            {
                var value = double.Parse(match.Groups[1].Value);
                channel.ChannelSettings.ReactionConfidenceThreshold = value;
                channelConfigurationService.SaveChannel(channel);
                return true;
            }

            return false;
        }
    }
}
