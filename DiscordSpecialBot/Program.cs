using DiscordSpecialBot.ApiServices;
using DiscordSpecialBot.Bot;
using DSharpPlus;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    class Program
    {
        static DiscordClient discord;
        static UserDetailService userDetailService;
        static ReactionService reactionService;
        static ReactionAddService reactionAddService;
        static ChatResponseService chatResponseService;
        static ChatUpdateService chatUpdateService;
        static ChannelConfigurationService channelbotConfiguration;
        static BotUtilityService botUtilityService;
        static CommandService commandService;
        static BotReactionService botReactionService;
        static RequiredPropertyResponseService requiredPropertyResponseService;

        static void Main(string[] args)
        {
            Startup startup = new Startup();
            var botConfiguration = startup.Configure();
            channelbotConfiguration = new ChannelConfigurationService(botConfiguration);
            botConfiguration.Channels = channelbotConfiguration.LoadChannels();

            var client = new HttpClient();
            var apiUtilityService = new ApiUtilityService(botConfiguration);

            chatResponseService = new ChatResponseService(client, apiUtilityService, botConfiguration);
            chatUpdateService = new ChatUpdateService(client, apiUtilityService, botConfiguration);
            userDetailService = new UserDetailService(client, botConfiguration);
            reactionService = new ReactionService(client, apiUtilityService, botConfiguration);
            reactionAddService = new ReactionAddService(client, apiUtilityService, botConfiguration);

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = botConfiguration.Token,
                TokenType = TokenType.Bot
            });

            botUtilityService = new BotUtilityService(discord, botConfiguration);
            commandService = new CommandService(botConfiguration, channelbotConfiguration);
            botReactionService = new BotReactionService(botConfiguration, new EmojiService(discord));
            requiredPropertyResponseService = new RequiredPropertyResponseService(discord, botUtilityService);

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            discord.MessageCreated += async e =>
            {
                var channel = channelbotConfiguration.GetChannelConfiguration(e);

                if (e.Message.Author.Username == discord.CurrentUser.Username)
                {
                    await chatUpdateService.UpdateChatAsync(e);
                }
                else if (commandService.command(e, channel))
                {

                }
                else if(!botUtilityService.ignoreMessage(e))
                {
                    var reaction = await reactionService.GetReactionAsync(e);
                    botReactionService.reactionResponse(e, reaction, channel.ChannelSettings);

                    var hasRequiredProperty = await userDetailService.HasRequiredPropertyAsync(e);
                    if(hasRequiredProperty)
                    {
                        var chatResponse = await chatResponseService.GetChatResponseAsync(e);
                        requiredPropertyResponseService.hasRequiredPropertyResponse(e, chatResponse, channel.ChannelSettings);
                    }
                    else
                    {
                        var chatResponse = await chatResponseService.GetChatResponseAsync(e);
                        hasRequiredProperty = await userDetailService.HasRequiredPropertyAsync(e);
                        if(!hasRequiredProperty && botUtilityService.alwaysRespond(e))
                        {
                            botUtilityService.defaultResponse(e);
                        }
                        else
                        {
                            requiredPropertyResponseService.hasRequiredPropertyResponse(e, chatResponse, channel.ChannelSettings);
                        }
                    }
                }
            };

            discord.MessageReactionAdded += async e =>
            {
                await reactionAddService.AddReactionAsync(e);
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
