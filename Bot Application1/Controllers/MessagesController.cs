using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using System.Diagnostics;
using CalligenceBot;
using CalligenceBot.Common;
using System.Web;
using Microsoft.Rest;

namespace CalligenceBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            int userId = userData.GetProperty<int>("UserId");

            //await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("入口，ClubName: " + userData.GetProperty<string>("ClubName")+"    ClubId: "+ userData.GetProperty<int>("ClubId")));
            //await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("" + userId));
            if (userId <= 0)
            {   //have no id , first visit
                //首次访问 进行自我介绍
                await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(ReplyHelper.MYSELF));

                //set id, save in UserList
                int id = UserHelper.Count;
                userData.SetProperty<int>("UserId", id);
                UserHelper.UserList.Add(id, new NowClub());
                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                UserHelper.Count++;
            }
            if (activity != null)
            {
                // one of these will have an interface and process it
                switch (activity.GetActivityType())
                {

                    case ActivityTypes.Message:
                        await Conversation.SendAsync(activity, () => new MyLuisDialog(ref userData, activity, stateClient));
                        break;

                    case ActivityTypes.ConversationUpdate:

                    case ActivityTypes.ContactRelationUpdate:

                    case ActivityTypes.Typing:

                    case ActivityTypes.DeleteUserData:

                    default:

                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }

            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}