using CalligenceBot.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace CalligenceBot
{
    //LUIS 
    //AppId 50322056-24f5-4634-ba81-3284ccabfd31 
    //Sub 86c7fd147e484c3f897a683f505a34e1

    [LuisModel("50322056-24f5-4634-ba81-3284ccabfd31", "f44431ea4c6a4c4a9f4aa9366ef6a77b")]

    [Serializable]
    public class MyLuisDialog : LuisDialog<object>
    {
        //此为当前访问的协会
        private BotData userData;
        private Activity activity;
        private StateClient stateClient;
        public MyLuisDialog(ref BotData data, Activity ac, StateClient sc)
        {
            this.userData = data;
            this.activity = ac;
            this.stateClient = sc;
        }
        //计数器, 判断自动推送标志 
        //private static int Count = 0;
        private static string WebUrl = "更多社团信息, [欢迎访问站点-校园社团](http://clubweb.azurewebsites.net)";

        /// <summary>
        /// 获取当前会话中的实体，即所要了解的指定协会
        /// </summary>
        /// <param name="result"></param>
        /// <returns>实体/协会名称</returns>
        private string GetEntity(LuisResult result)
        {
            string str = string.Empty;
            var entities = result.Entities;
            foreach (var entity in entities)
            {
                str += entity.Entity;
            }
            return str;
        }

        /// <summary>
        /// 确定实体/协会名称
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<string> GetClubName(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync("访问count:" + UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Count);
            if (string.IsNullOrEmpty(GetEntity(result)))
            {//无法从用户的对话信息中获取相关实体/协会名称
                //使用已经存在的当前协会
                return UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cname;
            }
            else
            {//从用户的对话信息中获取新的相关实体/协会名称
                //更新当前的协会
                string str = GetEntity(result);
                if (str == UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cname) return str;
                var data = GetDataFromAPI.DataToString(UrlHelper.GetClubUrl, "clubname=" + str);
                int id = int.Parse(data.Result);
                if (id > 0)
                {
                    UserHelper.SetNowClub(userData.GetProperty<int>("UserId"), id, str);
                }
                else
                {
                    UserHelper.SetNowClub(userData.GetProperty<int>("UserId"), -1, string.Empty);
                }
            }
            return UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cname;
        }

        [LuisIntent("打招呼")]
        public async Task HelloReply(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(ReplyHelper.MYSELF);
            context.Wait(MessageReceived);
        }

        [LuisIntent("自我介绍")]
        public async Task MyselfReply(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(ReplyHelper.MYSELF);
            context.Wait(MessageReceived);
        }

        [LuisIntent("简介")]
        public async Task MainInfoReply(IDialogContext context, LuisResult result)
        {
            //获取指定协会名称
            string clubname = await GetClubName(context, result);
            //判断是否指定了协会
            if (string.IsNullOrEmpty(clubname))
            {
                await context.PostAsync($"请以正确的打开方式, 如: \"计算机协会的{result.Intents[0].Intent}\", 或者你想要查询的协会暂时不存在");
                return;
            }
            //根据协会id获取有关协会的简介
            var t = await GetDataFromAPI.DataToObject(UrlHelper.GetMainInfo + UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cid, "");
            if (t == null)
            {
                await context.PostAsync($"{clubname}还没有填写其介绍");
                return;
            }
            //获取协会的简介信息
            var mainInfo = JObject.Parse(t.ToString());
            string reply = $"**以下为<{clubname}>的主要信息:**\r\n\r\n";
            string iconUrl = "http://clubweb.azurewebsites.net" + mainInfo["Micon"];
            reply += "会徽 :  ![image](" + iconUrl + ")\r\n\r\n";
            reply += "成立时间 :  " + DateTime.Parse(mainInfo["Mcreatetime"].ToString()).Year + "年\r\n\r\n";
            reply += "挂靠单位 :  " + mainInfo["MbelongTo"] + "\r\n\r\n";
            reply += "合作商 :  " + mainInfo["Mcooperation"] + "\r\n\r\n";
            reply += "[查看具体介绍](http://clubweb.azurewebsites.net/Home/MainInfo?botid=" + UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cid + "&botname=" + clubname + ")";
            await context.PostAsync(reply);
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("会长")]
        public async Task ClubsLeader(IDialogContext context, LuisResult result)
        {
            //获取指定协会名称
            string clubname = await GetClubName(context, result);
            //判断是否指定了协会
            if (string.IsNullOrEmpty(clubname))
            {
                await context.PostAsync($"请以正确的打开方式, 如: \"计算机协会的{result.Intents[0].Intent}\", 或者你想要查询的协会暂时不存在");
                return;
            }
            //根据协会名称获取有关协会的所有信息
            var t = await GetDataFromAPI.DataToObject(UrlHelper.GetLeader + UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cid, "");
            if (t == null)
            {
                await context.PostAsync($"{clubname}还未设置会长信息");
                return;
            }
            //获取协会的会长信息
            var leader = JObject.Parse(t.ToString());
            //组织回复消息
            string reply = $"**<{clubname}>的会长信息如下:**\r\n\r\n";
            string imgUrl = "http://clubweb.azurewebsites.net" + leader["Limage"];
            reply += "![照片](" + imgUrl + ")\r\n\r\n";
            reply += "姓名 :  " + leader["Lname"] + "\r\n\r\n";
            try
            {
                string sexStr = short.Parse(leader["Lsex"].ToString()) == 0 ? "女" : "男";
                reply += "性别 :  " + sexStr + "\r\n\r\n";
            }catch(Exception ex)
            {
                reply += "性别 :  保密" + "\r\n\r\n";
            }
            reply += "专业 :  " + leader["Lmajor"] + "\r\n\r\n";
            reply += "入学时间 :  " + DateTime.Parse(leader["Lregiste"].ToString()).Year + "年\r\n\r\n";
            reply += "联系方式 :  " + leader["Lcontact"] + "\r\n\r\n";
            await context.PostAsync(reply);
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("所有协会")]
        public async Task AllClubs(IDialogContext context, LuisResult result)
        {
            var allClubs = await GetDataFromAPI.DataToJArray(UrlHelper.GetAllClubsUrl, "");
            string reply = "**江苏科技大学(张家港校区)，我知道的协会有:**\r\n\r\n";
            foreach(var club in allClubs)
            {
                reply += club["Cname"];
                reply += ", ";
            }
            await context.PostAsync(reply);
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("动态")]
        public async Task News(IDialogContext context, LuisResult result)
        {
            //获取指定协会名称
            string clubname = await GetClubName(context, result);
            //判断是否指定了协会
            if (string.IsNullOrEmpty(clubname))
            {
                await context.PostAsync($"请以正确的打开方式, 如: \"计算机协会的{result.Intents[0].Intent}\", 或者你想要查询的协会暂时不存在");
                return;
            }
            //根据协会名称获取有关协会的所有信息
            var t = await GetDataFromAPI.DataToJArray(UrlHelper.GetNews + UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cid, "");
            if (t == null)
            {
                await context.PostAsync("此协会还未有动态记录");
                return;
            }
            //获取协会的动态
            var jNews = JArray.Parse(t.ToString());
            string reply = $"**<{clubname}>的最新动态如下**\r\n\r\n";
            string imgUrl;
            string ntitle;
            string detailUrl;
            if (jNews.Count() >= 3)
            {
                for (int i=0; i<3; i++)
                {
                    //init
                    imgUrl = string.Empty;
                    ntitle = string.Empty;
                    detailUrl = string.Empty;
                    var news = jNews[i];
                    imgUrl = "http://clubweb.azurewebsites.net" + news["Nimage"];
                    reply += $"![动态图]({imgUrl}) ";
                    ntitle = news["Ntitle"].ToString();
                    detailUrl = "http://clubweb.azurewebsites.net/More/Details?id=" + news["Nid"] + "&type=news";
                    reply += $"[{ntitle}]({detailUrl})";
                    reply += "  —" + Convert.ToDateTime(news["Ntime"]).ToShortDateString() + "\r\n\r\n";
                    //await context.PostAsync(reply);
                }
            }
            else
            {
                for (int i = 0; i < jNews.Count(); i++)
                {
                    //init
                    imgUrl = string.Empty;
                    ntitle = string.Empty;
                    detailUrl = string.Empty;
                    //organize reply
                    var news = jNews[i];
                    imgUrl = "http://clubweb.azurewebsites.net" + news["Nimage"];
                    reply += $"![动态图]({imgUrl}) ";
                    ntitle = news["Ntitle"].ToString();
                    detailUrl = "http://clubweb.azurewebsites.net/More/Details?id=" + news["Nid"] + "&type=news";
                    reply += $"[{ntitle}]({detailUrl})";
                    reply += "\r\n\r\n" + "——" + Convert.ToDateTime(news["Ntime"]).ToShortDateString() + "\r\n\r\n";
                    //await context.PostAsync(reply);
                }
            }
            await context.PostAsync(reply);
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("成果")]
        public async Task Activities(IDialogContext context, LuisResult result)
        {
            string clubname = await GetClubName(context, result);//get club
            if (string.IsNullOrEmpty(clubname))  //is null
            {
                await context.PostAsync($"请以正确的打开方式, 如: \"计算机协会的{result.Intents[0].Intent}\", 或者你想要查询的协会暂时不存在");
                return;
            }
            var t = await GetDataFromAPI.DataToJArray(UrlHelper.GetActivities + UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cid, ""); //get all content related to club
            if (t == null)
            {
                await context.PostAsync("此协会还未有成果记录");
                return;
            }
            //获取活动
            var jActi = JArray.Parse(t.ToString());
            string reply = $"**<{clubname}>的最近的成果如下**\r\n\r\n";
            string imgUrl;
            string atitle;
            string detailUrl;            
            if (jActi.Count() >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    //init
                    imgUrl = string.Empty;
                    atitle = string.Empty;
                    detailUrl = string.Empty;
                    //organize reply
                    var acti = jActi[i];
                    imgUrl = "http://clubweb.azurewebsites.net" + acti["Aimage"];
                    reply += $"![动态图]({imgUrl}) ";
                    atitle = acti["Atitle"].ToString();
                    detailUrl = "http://clubweb.azurewebsites.net/More/Details?id=" + acti["Aid"] + "&type=activities";
                    reply += $"[{atitle}]({detailUrl})";
                    reply += "  —" + Convert.ToDateTime(acti["Atime"]).ToShortDateString() + "\r\n\r\n";
                    //await context.PostAsync(reply);
                }
            }
            else
            {
                for (int i = 0; i < jActi.Count(); i++)
                {
                    //init
                    imgUrl = string.Empty;
                    atitle = string.Empty;
                    detailUrl = string.Empty;
                    //organize reply
                    var acti = jActi[i];
                    imgUrl = "http://clubweb.azurewebsites.net" + acti["Aimage"];
                    reply += $"![动态图]({imgUrl}) ";
                    atitle = acti["Atitle"].ToString();
                    detailUrl = "http://clubweb.azurewebsites.net/More/Details?id=" + acti["Aid"] + "&type=activities";
                    reply += $"[{atitle}]({detailUrl})";
                    reply += "\r\n\r\n" + "——" + Convert.ToDateTime(acti["Atime"]).ToShortDateString() + "\r\n\r\n";
                    //await context.PostAsync(reply);
                }
            }
            await context.PostAsync(reply);
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("公告")]
        public async Task Notice(IDialogContext context, LuisResult result)
        {
            string clubname = await GetClubName(context, result);//get club
            if (string.IsNullOrEmpty(clubname))  //is null
            {
                await context.PostAsync($"请以正确的打开方式, 如: \"计算机协会的{result.Intents[0].Intent}\", 或者你想要查询的协会暂时不存在");
                return;
            }
            var t = await GetDataFromAPI.DataToJArray(UrlHelper.GetNotice + UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Cid, ""); //get all content related to club
            if (t == null)
            {
                await context.PostAsync("此协会还未发布公告");
                return;
            }
            //含有信息
            var jNotice = JArray.Parse(t.ToString());
            string reply= $"**以下为<{clubname}>最近的公告**\r\n\r\n";
            if (jNotice.Count() >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    //organize reply
                    var notice = jNotice[i];
                    reply += notice["Ncontent"].ToString();
                    reply += "  —" + notice["Subtime"] + "\r\n\r\n";
                    //await context.PostAsync(reply);
                }
            }
            else
            {
                for (int i = 0; i < jNotice.Count(); i++)
                {
                    //organize reply
                    var notice = jNotice[i];
                    reply += notice["Ncontent"].ToString();
                    reply += "  —" + notice["Subtime"]+"\r\n\r\n";
                }
            }
            await context.PostAsync(reply);
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("评价")]
        public async Task Comment(IDialogContext context, LuisResult result)
        {
            string clubname = await GetClubName(context, result);//get club
            if (string.IsNullOrEmpty(clubname))  //is null
            {
                await context.PostAsync($"请以正确的方式询问我, 如: \"计算机协会怎么样?\", 或者你想要查询的协会暂时不存在");
                return;
            }
            await context.PostAsync($"{clubname}固然有他的优势，Bot认为每个社团都都是由其成员精心打造，可谓\"百花齐放，百团争鸣\"");
            context.Wait(MessageReceived);
        }
        
        [LuisIntent("批评")]
        public async Task Criticize(IDialogContext context,LuisResult result)
        {
            Random r = new Random();
            int index = r.Next(0, 3);
            string reply = (ReplyHelper.CRITICISNM[index]);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        //批评 夸奖 致谢 调侃 推荐
        [LuisIntent("夸奖")]
        public async Task Praise(IDialogContext context, LuisResult result)
        {
            Random r = new Random();
            int index = r.Next(0, 3);
            string reply = (ReplyHelper.PRAISE[index]);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        [LuisIntent("致谢")]
        public async Task Appreciate(IDialogContext context, LuisResult result)
        {
            Random r = new Random();
            int index = r.Next(0, 3);
            string reply = (ReplyHelper.APPRECIATION[index]);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        [LuisIntent("调侃")]
        public async Task Kidding(IDialogContext context, LuisResult result)
        {
            Random r = new Random();
            int index = r.Next(0, 3);
            string reply = (ReplyHelper.KIDDING[index]);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        [LuisIntent("其他")]
        public async Task Other(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("对不起，您所说的这句话已经超出了我的业务范围，不过我会更加努力学习");
            await GetDataFromAPI.DataToString(UrlHelper.LearnMoreUrl, "content=" + result.Query);
            context.Wait(MessageReceived);
        }

        [LuisIntent("推荐")]
        public async Task Recommend(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("所有社团都具有各自的特色，每个社团的精彩还需亲自加入才能体会得到");
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"\"{result.Query}\"\r\n\r\n这句话我已经记住了, 我会更加努力地学习");
            await GetDataFromAPI.DataToString(UrlHelper.LearnMoreUrl, "content=" + result.Query);
            context.Wait(MessageReceived);
        }

        //当用于针对一个协会询问5次以上进行 自动推送“查看网站”消息
        private bool AutoMessage()
        {
            if (UserHelper.GetNowClub(userData.GetProperty<int>("UserId")).Count >= 15)
            {
                UserHelper.ClearCount(userData.GetProperty<int>("UserId")); //将当前用户的计数器置0
                return true;
            }
            return false;
        }

    }
}