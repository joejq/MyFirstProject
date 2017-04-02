﻿using Deepleo.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Deepleo.Web
{
    //http://stackoverflow.com/questions/37518770/microsoft-bot-with-direct-line-rest-api-for-newbie-of-bot
    public class MSBot
    {
        public async static Task<string> PostMessage(string message)
        {
            HttpClient client;
            HttpResponseMessage response;

            bool IsReplyReceived = false;

            string ReceivedString = null;

            client = new HttpClient();
            client.BaseAddress = new Uri("https://directline.botframework.com/api/conversations/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "uc0nyMvM0NI.cwA.wYs.B8F1M7cEBm9StsTDG8pmuOjhnxeCJd2LdvNlKVfBgro");
            // for leon
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "OYRQTgHjVFA.cwA.5oI.f82UwclGUjX_eGVEgn428kadOtPqpVHLzO440PUdooQ");
            // for doudou
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "iCoMHD3whk8.cwA.50s.vlUvoUn7IjjRuHkSeNZQ2dOIHHTOftz07bAJRF2WJy8");


            response = await client.GetAsync("/api/tokens/");
            if (response.IsSuccessStatusCode)
            {
                var conversation = new Conversation();
                response = await client.PostAsJsonAsync("/api/conversations/", conversation);
                //response = await client.PostAsync("/api/conversations/", null);
                if (response.IsSuccessStatusCode)
                {
                    Conversation ConversationInfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
                    string conversationUrl = ConversationInfo.conversationId + "/messages/";
                    Message msg = new Message() { text = message };
                    response = await client.PostAsJsonAsync(conversationUrl, msg);
                    if (response.IsSuccessStatusCode)
                    {
                        response = await client.GetAsync(conversationUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageSet BotMessage = response.Content.ReadAsAsync(typeof(MessageSet)).Result as MessageSet;
                            //组织内容 TODO
                            string str = BotMessage.messages[1].text;
                            ReceivedString = str.Replace("\r\n\r\n", "\r\n");
                            //for (int i=1; i<BotMessage.messages.Length; i++)
                            //{
                                
                            //    //str = StrHelper.HandlePic(str);
                            //    //str = StrHelper.HandleLink(str);
                            //    ReceivedString += str + "\r\n";
                            //}
                            IsReplyReceived = true;
                        }
                    }
                }

            }
            return ReceivedString;
        }
    }


    public class Conversation
    {
        public string conversationId { get; set; }
        public string token { get; set; }
        public string eTag { get; set; }
    }

    public class MessageSet
    {
        public Message[] messages { get; set; }
        public string watermark { get; set; }
        public string eTag { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string conversationId { get; set; }
        public DateTime created { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public string channelData { get; set; }
        public string[] images { get; set; }
        public Attachment[] attachments { get; set; }
        public string eTag { get; set; }
    }

    public class Attachment
    {
        public string url { get; set; }
        public string contentType { get; set; }
    }

}