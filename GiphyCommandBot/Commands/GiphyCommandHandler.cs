namespace GiphyCommandBot.Commands
{
    using GiphyCommandBot.Models;
    using AdaptiveCards.Templating;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.TeamsFx.Conversation;
    using Newtonsoft.Json;
    using GiphyCommandBot.Giphy;

    /// <summary>
    /// The <see cref="GiphyCommandHandler"/> registers a pattern with the <see cref="ITeamsCommandHandler"/> and 
    /// responds with an Adaptive Card if the user types the <see cref="TriggerPatterns"/>.
    /// </summary>
    public class GiphyCommandHandler : ITeamsCommandHandler
    {
        private readonly ILogger<GiphyCommandHandler> _logger;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "GiphyCard.json");

        public IEnumerable<ITriggerPattern> TriggerPatterns => new List<ITriggerPattern>
        {
            new RegExpTrigger("/giphy")
        };

        public GiphyCommandHandler(ILogger<GiphyCommandHandler> logger)
        {
            _logger = logger;
        }

        public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
        {
            // Fetch command search parameters
            var searchQuery = message.Text.Split(" ")[1];

            // Fetch GIF
            var gif = await GiphyService.FetchGif(searchQuery);
            var gifUrl = gif.Data.Images.Original.Url;

            // Load and populate adaptive card with data
            var cardTemplate = await File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);
            var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
            (
                new GiphyCardModel
                {
                    GifUrl = gifUrl
                }
            );
            var activity = MessageFactory.Attachment
            (
                new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(cardContent),
                }
            );

            // Post message to channel with adaptive card
            return new ActivityCommandResponse(activity);
        }
    }
}
