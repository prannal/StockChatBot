
namespace LuisBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Net;
    using Newtonsoft.Json.Linq;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using LuisBot;

    [LuisModel("3dd159e3-0967-486d-b769-305dcbc8c2c4 ", "7eed46f6d4c041b1ac856eed7044161c")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        //private const string Entitycompany = "company";

        private const string Entitysymbol = "symbol";


        private IList<string> titleOptions = new List<string> { "“Very stylish, great stay, great staff”", "“good hotel awful meals”", "“Need more attention to little things”", "“Lovely small hotel ideally situated to explore the area.”", "“Positive surprise”", "“Beautiful suite and resort”" };

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("greeting")]
        public async Task greeting(IDialogContext context, LuisResult result)
        {
            string message = $"Hello, how can I help you.";

            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
          


        }

        

        [LuisIntent("search.price")]
        public async Task price(IDialogContext context, LuisResult result)
        {
            EntityRecommendation stockEntityRecommendation;

            if (result.TryFindEntity(Entitysymbol, out stockEntityRecommendation))
            {
                await context.PostAsync($"Please, wait until we load information of stock you want.....");
                await context.PostAsync($"this is search.price intent and entity is '{stockEntityRecommendation.Entity}' ");
                var A = GetPriceData(context, result, stockEntityRecommendation.Entity);
                //await context.PostAsync(B.ToString());

            }

            context.Wait(this.MessageReceived);

        }

        private async Task GetPriceData(IDialogContext context, LuisResult result, string symbol)
        {

            using (var client = new WebClient()) //WebClient  
            {

                EntityRecommendation stockEntityRecommendation;
                client.Headers.Add("Content-Type:application/json"); //Content-Type  
                client.Headers.Add("Accept:application/json");
                if (result.TryFindEntity(Entitysymbol, out stockEntityRecommendation))
                {
                    var json = client.DownloadString("https://api.iextrading.com/1.0/stock/" + symbol + "/previous");
                    JObject jObject = JObject.Parse(json);
                    List<string> list = new List<string>();
                    string conversion = "";
                    conversion = jObject["open"].ToString();
                    list.Add("open: " + conversion + "\n");

                    conversion = jObject["close"].ToString();
                    list.Add("close: " + conversion + "\n");
                    conversion = jObject["high"].ToString();
                    list.Add("high: " + conversion + "\n");
                    conversion = jObject["low"].ToString();
                    list.Add("low: " + conversion + "\n");
                    await context.PostAsync(String.Join("\n", list));

                }



            }

        }

        [LuisIntent("search.company")]
        public async Task company(IDialogContext context, LuisResult result)
        {
            EntityRecommendation stockEntityRecommendation;

            if (result.TryFindEntity(Entitysymbol, out stockEntityRecommendation))
            {
                await context.PostAsync($"Please, wait until we load information of stock you want.....");
                await context.PostAsync($"this is search.company intent and entity is '{stockEntityRecommendation.Entity}' ");
                var B = GetCompanyData(context,result,stockEntityRecommendation.Entity);
               // await this.DisplayHeroCard(context, stockEntityRecommendation.Entity);
                //await context.PostAsync(B.ToString());

            }

            context.Wait(this.MessageReceived);

        }

        private async Task GetCompanyData(IDialogContext context, LuisResult result, string symbol)
        {

            using (var client = new WebClient()) //WebClient  
            {

                EntityRecommendation stockEntityRecommendation;
                client.Headers.Add("Content-Type:application/json"); //Content-Type  
                client.Headers.Add("Accept:application/json");
                if (result.TryFindEntity(Entitysymbol, out stockEntityRecommendation))
                {
                    var json = client.DownloadString("https://api.iextrading.com/1.0/stock/" + symbol + "/company");
                    JObject jObject = JObject.Parse(json);
                    List<string> list = new List<string>();
                    string conversion = "";
                    conversion = jObject["companyName"].ToString();
                    list.Add("Company Name: " + conversion + "\n");

                    conversion = jObject["website"].ToString();
                    list.Add("Website: " + conversion + "\n");
                    conversion = jObject["description"].ToString();
                    list.Add("Description: " + conversion + "\n");
                    conversion = jObject["CEO"].ToString();
                    list.Add("CEO: " + conversion + "\n");
                    conversion = jObject["sector"].ToString();
                    list.Add("Sector: " + conversion + "\n");
                    await context.PostAsync(String.Join("\n", list));

                }



            }

        }

        [LuisIntent("symbol.detail")]
        public async Task detail(IDialogContext context, LuisResult result)
        {
            EntityRecommendation stockEntityRecommendation;

            if (result.TryFindEntity(Entitysymbol, out stockEntityRecommendation))
            {
                await context.PostAsync($"Please, wait until we load information of stock you want.....");
                await context.PostAsync($"this is search.company intent and entity is '{stockEntityRecommendation.Entity}' ");
               // var B = GetCompanyData(context, result, stockEntityRecommendation.Entity);
                await this.DisplayHeroCard(context, stockEntityRecommendation.Entity);
                //await context.PostAsync(B.ToString());

            }

            context.Wait(this.MessageReceived);

        }
        public async Task DisplayHeroCard(IDialogContext context, string symbol)
        {

            var replyMessage = context.MakeMessage();
            Attachment attachment = GetProfileHeroCard(symbol); ;
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
        }
        private static Attachment GetProfileHeroCard(string symbol)
        {
            var heroCard = new HeroCard
            {
                // title of the card  
                Title = symbol,
                //subtitle of the card  
                Images = new List<CardImage> { new CardImage("https://storage.googleapis.com/iex/api/logos/TI.png") },
                // list of buttons   
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Stock Information", value: "https://www.bing.com/search?q="+symbol+"+stock+today&FORM=R5FD7"), new CardAction(ActionTypes.OpenUrl, "Company Detail", value: "https://www.google.com/search?q="+symbol+"+company+detail&ie=utf-8&oe=utf-8&client=firefox-b-ab") }
            };

            return heroCard.ToAttachment();
        }

        private IForm<HotelsQuery> BuildHotelsForm()
        {
            OnCompletionAsyncDelegate<HotelsQuery> processHotelsSearch = async (context, state) =>
            {
                var message = "Searching for hotels";
                if (!string.IsNullOrEmpty(state.Destination))
                {
                    message += $" in {state.Destination}...";
                }
                else if (!string.IsNullOrEmpty(state.AirportCode))
                {
                    message += $" near {state.AirportCode.ToUpperInvariant()} airport...";
                }

                await context.PostAsync(message);
            };

            return new FormBuilder<HotelsQuery>()
                .Field(nameof(HotelsQuery.Destination), (state) => string.IsNullOrEmpty(state.AirportCode))
                .Field(nameof(HotelsQuery.AirportCode), (state) => string.IsNullOrEmpty(state.Destination))
                .OnCompletion(processHotelsSearch)
                .Build();
        }
      

       
            
            private async Task GetCloseData(IDialogContext context, LuisResult result, string symbol)
            {

                using (var client = new WebClient()) //WebClient  
                {

                    EntityRecommendation stockEntityRecommendation;
                    client.Headers.Add("Content-Type:application/json"); //Content-Type  
                    client.Headers.Add("Accept:application/json");
                    if (result.TryFindEntity(Entitysymbol, out stockEntityRecommendation))
                    {
                        var json = client.DownloadString("https://api.iextrading.com/1.0/stock/" + symbol + "/previous");
                        JObject jObject = JObject.Parse(json);
                        List<string> list = new List<string>();
                        string conversion = "";
                        conversion = jObject["open"].ToString();
                        list.Add("open: "+conversion+"\n");

                        conversion = jObject["close"].ToString();
                        list.Add("close: "+conversion + "\n");
                        conversion = jObject["high"].ToString();
                        list.Add("high: "+conversion + "\n");
                        conversion = jObject["low"].ToString();
                        list.Add("low: "+conversion + "\n");
                        await context.PostAsync(String.Join("\n", list));

                     }



                }



            }


        private async Task ResumeAfterHotelsFormDialog(IDialogContext context, IAwaitable<HotelsQuery> result)
        {
            try
            {
                var searchQuery = await result;

                var hotels = await this.GetHotelsAsync(searchQuery);

                await context.PostAsync($"I found {hotels.Count()} hotels:");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();
                 foreach (var hotel in hotels)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = hotel.Name,
                        Subtitle = $"{hotel.Rating} starts. {hotel.NumberOfReviews} reviews. From ${hotel.PriceStarting} per night.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = hotel.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=hotels+in+" + HttpUtility.UrlEncode(hotel.Location)
                            }
                        }
                };

                resultMessage.Attachments.Add(heroCard.ToAttachment());
            }

            await context.PostAsync(resultMessage);
        }
        catch (FormCanceledException ex)
        {
            string reply;

            if (ex.InnerException == null)
            {
                reply = "You have canceled the operation.";
            }
            else
            {
                reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
            }

            await context.PostAsync(reply);
        }
        finally
        {
            context.Done<object>(null);
        }
    }

    private async Task<IEnumerable<Hotel>> GetHotelsAsync(HotelsQuery searchQuery)
    {
        var hotels = new List<Hotel>();

        // Filling the hotels results manually just for demo purposes
        for (int i = 1; i <= 5; i++)
        {
            var random = new Random(i);
            Hotel hotel = new Hotel()
            {
                Name = $"{searchQuery.Destination ?? searchQuery.AirportCode} Hotel {i}",
                Location = searchQuery.Destination ?? searchQuery.AirportCode,
                Rating = random.Next(1, 5),
                NumberOfReviews = random.Next(0, 5000),
                PriceStarting = random.Next(80, 450),
                Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
                };

    hotels.Add(hotel);
            }

hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return hotels;
        }
    }
}
