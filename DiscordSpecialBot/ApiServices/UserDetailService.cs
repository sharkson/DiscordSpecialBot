using ChatModels;
using DiscordSpecialBot.Models;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordSpecialBot.ApiServices
{
    public class UserDetailService
    {
        HttpClient client;
        BotConfiguration configuration;

        public UserDetailService(HttpClient httpClient, BotConfiguration botConfiguration)
        {
            client = httpClient;
            configuration = botConfiguration;
        }

        public async Task<bool> HasRequiredPropertyAsync(MessageCreateEventArgs e)
        {
            var userName = e.Author.Username;
            var response = await client.GetAsync(configuration.ApiUrl + "/api/user/" + userName);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(jsonResponse);

            return configuration.RequiredProperyMatches.All(rp => userData.derivedProperties.Any(dp => dp.name == rp));
        }
    }
}
