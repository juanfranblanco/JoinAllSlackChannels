using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JoinAllSlackChannels
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please input your token id for the slack");
                Console.ReadLine();
            }
            else
            {
                new SlackService().JoinAllChannels(args[0]).Wait();
            }
        }

        public class SlackService
        {
            const string GetAllChannelsUrl = "https://slack.com/api/channels.list?token={0}";
            const string JoinChannelUrl = "https://slack.com/api/channels.join?token={0}&name={1}";
            public async Task<JObject> GetAllChannels(string tokenId)
            {
                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.GetStringAsync(
                    new Uri(String.Format(GetAllChannelsUrl, tokenId))).ConfigureAwait(false);
                    return JObject.Parse(result);
                }
            }

            public async Task JoinAllChannels(string tokenId)
            {
                var allChannels = await GetAllChannels(tokenId).ConfigureAwait(false);
                var channels = allChannels["channels"].Value<JArray>();
                if (channels != null)
                    foreach (var channel in channels)
                    {
                        if (channel["is_member"].Value<bool>() == false)
                        {
                            var name = channel["name"].Value<string>();
                            await JoinChannel(tokenId, name);

                        }
                    }
            }

            public async Task<JObject> JoinChannel(string tokenId, string channelName)
            {
                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.GetStringAsync(
                    new Uri(String.Format(JoinChannelUrl, tokenId, channelName))).ConfigureAwait(false);
                    return JObject.Parse(result);
                }
            }
        }
    }
}
