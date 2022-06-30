using GiphyDotNet;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using GiphyDotNet.Model.Results;

namespace GiphyCommandBot.Giphy
{
    public class GiphyService
    {
        const string API_KEY = "";

        public static async Task<GiphyRandomResult> FetchGif(string searchQuery)
        {
            var giphy = new GiphyDotNet.Manager.Giphy(API_KEY);

            var searchParameter = new RandomParameter
            {
                Tag = searchQuery
            };

            return await giphy.RandomGif(searchParameter);
        }
    }
}
