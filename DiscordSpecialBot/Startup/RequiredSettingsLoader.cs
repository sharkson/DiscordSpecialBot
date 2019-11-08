using DiscordSpecialBot.Models;
using Microsoft.Extensions.Configuration;

namespace DiscordSpecialBot
{
    public class RequiredSettingsLoader
    {
        public BotConfiguration LoadRequiredSettings(IConfiguration configuration, BotConfiguration botConfiguration)
        {     
            botConfiguration.Token = configuration.GetSection("Token").Value;
            botConfiguration.ApiUrl = configuration.GetSection("ApiUrl").Value;
            botConfiguration.BotName = configuration.GetSection("BotName").Value;
            botConfiguration.ChatType = configuration.GetSection("ChatType").Value;

            return botConfiguration;
        }
    }
}
