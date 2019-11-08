using DiscordSpecialBot.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DiscordSpecialBot
{
    public class Startup
    {
        public IConfiguration configuration { get; }
        private RequiredSettingsLoader requiredSettingsLoader;
        private OptionalSettingsLoader optionalSettingsLoader;

        public Startup()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            configuration = builder.Build();
            requiredSettingsLoader = new RequiredSettingsLoader();
            optionalSettingsLoader = new OptionalSettingsLoader();
        }

        public BotConfiguration Configure()
        {
            var botConfiguration = new BotConfiguration();
            botConfiguration = requiredSettingsLoader.LoadRequiredSettings(configuration, botConfiguration);
            botConfiguration = optionalSettingsLoader.LoadOptionalSettings(configuration, botConfiguration);

            //TODO: load channel settings from mongodb
            botConfiguration.Channels = new List<Channel>();

            return botConfiguration;
        }
    }
}
