using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CalligenceBot.Common;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using System.Diagnostics;
using System.Web;

namespace CalligenceBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static bool firstvisit = true;
        //private static string lastclub = string.Empty;
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //首次访问 进行自我介绍
            if (firstvisit)
            {
                var connector=new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(ReplyHelper.MYSELF));
                firstvisit = false;
            }
            if (activity != null)
            {
                // one of these will have an interface and process it
                switch (activity.GetActivityType())
                {

                    case ActivityTypes.Message:
                        await Conversation.SendAsync(activity, () => new MyLuisDialog());
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

            #region test
            //if (activity.Type == ActivityTypes.Message)
            //{
            //    //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            //    #region test
            //    // calculate something for us to return
            //    //int length = (activity.Text ?? string.Empty).Length;
            //    //await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("正在为你查询服务"));
            //    //Activity reply = null;
            //    //string message = activity.Text;
            //    //string str = string.Empty;
            //    //switch (message)
            //    //{
            //    //    case "所有协会":
            //    //        //await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("进入选择通道"));
            //    //        var allClubs = await GetDataFromAPI.DataToString(UrlHelper.GetAllClubsUrl, "");
            //    //        JArray jClubs = new JArray(allClubs);
            //    //        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("获取到数据"));
            //    //        str = "以下为当前所有的协会:\r\n";
            //    //        foreach (var club in jClubs)
            //    //        {
            //    //            str += club["Cname"];
            //    //            str += "\r\n";
            //    //        }
            //    //        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("回复信息完成"));
            //    //        reply = activity.CreateReply(str);
            //    //        break;
            //    //    case "计算机协会简介":
            //    //        string existMessage = await GetDataFromAPI.DataToString(UrlHelper.IsExistUrl, "clubName=JustCA");
            //    //        str = "以下为计算机协会的基本信息:\r\n";
            //    //        if (existMessage == "No")
            //    //        {
            //    //            reply = activity.CreateReply("抱歉，你所查询的协会或社团还未录入本系统");
            //    //        }
            //    //        else
            //    //        {
            //    //            int id = int.Parse(existMessage);
            //    //            JObject mainInfo = await GetDataFromAPI.DataToObject(UrlHelper.GetMainInfoUrl, "id=" + id);
            //    //            str += "简介:" + mainInfo["Mintroduction"];
            //    //            str += "\r\n";
            //    //            str += "所属院系:" + mainInfo["MbelongTo"];
            //    //            str += "\r\n";
            //    //            str += "合作商:" + mainInfo["Mcooperation"];
            //    //        }
            //    //        reply = activity.CreateReply(str);
            //    //        break;
            //    //}
            //    // return our reply to the user
            //    //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
            //    #endregion
            //    await Conversation.SendAsync(activity, () => new MyLuisDialog());
            //}
            //else
            //{
            //    await HandleSystemMessage(activity);
            //}
            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //return response;
            #endregion
        }
        #region 模板
        //private async Task<Activity> HandleSystemMessage(Activity activity)
        //{
        //    if (activity.Type != ActivityTypes.Message)
        //    {
        //        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
        //        Activity reply = activity.CreateReply($"抱歉,你所发送的内容形式:{activity.Type}  我暂时无法处理,但我会努力学习的,fighting");
        //        await connector.Conversations.ReplyToActivityAsync(reply);
        //    }

        //    #region 模板
        //    /*if (message.Type == ActivityTypes.DeleteUserData)
        //    {
        //        // Implement user deletion here
        //        // If we handle user deletion, return a real message
        //    }
        //    else if (message.Type == ActivityTypes.ConversationUpdate)
        //    {
        //        // Handle conversation state changes, like members being added and removed
        //        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        //        // Not available in all channels
        //    }
        //    else if (message.Type == ActivityTypes.ContactRelationUpdate)
        //    {
        //        // Handle add/remove from contact lists
        //        // Activity.From + Activity.Action represent what happened
        //    }
        //    else if (message.Type == ActivityTypes.Typing)
        //    {
        //        // Handle knowing tha the user is typing
        //    }
        //    else if (message.Type == ActivityTypes.Ping)
        //    {
        //    }*/
        //    #endregion
        //    return null;
        //}
        #endregion
    }
}