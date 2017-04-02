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

namespace CalligenceBot
{
    //LUIS 
    //AppId 50322056-24f5-4634-ba81-3284ccabfd31 
    //Sub 86c7fd147e484c3f897a683f505a34e1

    [LuisModel("50322056-24f5-4634-ba81-3284ccabfd31", "86c7fd147e484c3f897a683f505a34e1")]

    [Serializable]
    public class MyLuisDialog : LuisDialog<object>
    {
        //此为当前访问的协会
        private static string NowClub = string.Empty;
        private static int NowClubId = -1;
        //计数器, 判断自动推送标志 
        private static int Count = 0;
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
        private string GetClubName(LuisResult result)
        {
            if (string.IsNullOrEmpty(GetEntity(result)))
            {//无法从用户的对话信息中获取相关实体/协会名称
                //使用已经存在的当前协会
                if (string.IsNullOrEmpty(NowClub))
                {
                    return string.Empty;
                }
                else
                {
                    return NowClub;
                }
            }
            else
            {//从用户的对话信息中获取新的相关实体/协会名称
                //更新当前的协会
                NowClub = GetEntity(result);
                //重新计数
                Count = 0;
            }
            return NowClub;
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
            await context.PostAsync("请您稍等, 我正在为你获取你所想看到的信息");
            //获取指定协会名称
            string clubname = GetClubName(result);
            //判断是否指定了协会
            if (string.IsNullOrEmpty(clubname))
            {
                await context.PostAsync($"请以正确的打开方式, 如: *计算机协会的{result.Intents[0].Intent}*");
                return;
            }
            //根据协会名称获取有关协会的所有信息
            var club = await GetDataFromAPI.DataToObject(UrlHelper.GetClubUrl, "clubname=" + clubname);
            if (club == null)
            {
                await context.PostAsync("您所查询的协会不存在, 我会尽快完善我的知识库~~");
                return;
            }
            //获取协会的简介信息
            var jArray = JArray.Parse(club["MainInfo"].ToString());
            var mainInfo = jArray[0];
            string reply = $"**以下为<{clubname}>的主要信息:**\r\n\r\n";
            string iconUrl = "http://clubweb.azurewebsites.net" + mainInfo["Micon"];
            reply += "*会徽 :* ![image](" + iconUrl + ")\r\n\r\n";
            reply += "*成立时间 :* " + DateTime.Parse(mainInfo["Mcreatetime"].ToString()).ToLongDateString() + "\r\n\r\n";
            reply += "*挂靠单位 :* " + mainInfo["MbelongTo"] + "\r\n\r\n";
            reply += "*合作伙伴 :* " + mainInfo["Mcooperation"] + "\r\n\r\n";
            reply += "[具体介绍](http://clubweb.azurewebsites.net/Home/MainInfo?botid=" + club["ID"] + "&botname=" + clubname + ")";
            await context.PostAsync(reply);
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("会长")]
        public async Task ClubsLeader(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync("我已经使用全部CPU为你提供想要的信息,片刻就好……");
            //获取指定协会名称
            string clubname = GetClubName(result);
            //判断是否指定了协会
            if (string.IsNullOrEmpty(clubname))
            {
                await context.PostAsync($"请以正确的打开方式, 如:*计算机协会的{result.Intents[0].Intent}*");
                return;
            }
            //根据协会名称获取有关协会的所有信息
            var club = await GetDataFromAPI.DataToObject(UrlHelper.GetClubUrl, "clubname=" + clubname);
            if (club == null)
            {
                await context.PostAsync("您所查询的协会不存在, 我会尽快完善我的知识库~~");
                return;
            }
            //获取协会的会长信息
            var jLeader = club["ClubLeader"];
            //未找到信息
            if (jLeader.Count() <= 0)
            {
                await context.PostAsync("此协会还未注册会长信息");
                return;
            }
            //含有会长信息
            var leader = jLeader[0];
            string first = $"**<{clubname}>的会长信息如下:**";
            await context.PostAsync(first);
            string reply = string.Empty;
            string imgUrl = "http://clubweb.azurewebsites.net" + leader["Limage"];
            reply += "![照片](" + imgUrl + ")\r\n\r\n";
            reply += "*姓名 :* " + leader["Lname"] + "\r\n\r\n";
            try
            {
                string sexStr = short.Parse(leader["Lsex"].ToString()) == 0 ? "女" : "男";
                reply += "*性别 :* " + sexStr + "\r\n\r\n";
            }catch(Exception ex)
            {
                reply += "*性别 :* 保密" + "\r\n\r\n";
            }
            reply += "*专业 :* " + leader["Lmajor"] + "\r\n\r\n";
            reply += "*入学时间 :* " + DateTime.Parse(leader["Lregiste"].ToString()).Year + "年\r\n\r\n";
            reply += "*联系方式 :* " + leader["Lcontact"] + "\r\n\r\n";
            await context.PostAsync(reply);
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("所有协会")]
        public async Task AllClubs(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("正在获取数据……");
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
            string clubname = GetClubName(result);
            //判断是否指定了协会
            if (string.IsNullOrEmpty(clubname))
            {
                await context.PostAsync($"请以正确的打开方式, 如:*计算机协会的{result.Intents[0].Intent}*");
                return;
            }
            //根据协会名称获取有关协会的所有信息
            var club = await GetDataFromAPI.DataToObject(UrlHelper.GetClubUrl, "clubname=" + clubname);
            if (club == null)
            {
                await context.PostAsync("您所查询的协会不存在, 我会尽快完善我的知识库~~");
                return;
            }
            //获取协会的动态
            var jNews = club["News"];
            //未找到信息
            if (jNews.Count() <= 0)
            {
                await context.PostAsync("此协会还未有动态记录");
                return;
            }
            //含有信息
            string first = $"**<{clubname}>的最新动态如下**";
            await context.PostAsync(first);

            string reply;
            string imgUrl;
            string ntitle;
            string detailUrl;          
            if (jNews.Count() >= 3)
            {
                for (int i=0; i<3; i++)
                {
                    //init
                    reply = string.Empty;
                    imgUrl = string.Empty;
                    ntitle = string.Empty;
                    detailUrl = string.Empty;
                    //organize reply
                    var news = jNews[i];
                    imgUrl = "http://clubweb.azurewebsites.net" + news["Nimage"];
                    reply += $"![动态图]({imgUrl}) ";
                    ntitle = news["Ntitle"].ToString();
                    detailUrl = "http://clubweb.azurewebsites.net/More/Details?id=" + news["ID"] + "&type=news";
                    reply += $"[{ntitle}]({detailUrl})";
                    reply += "\r\n\r\n" + "——" + Convert.ToDateTime(news["Ntime"]).ToLongDateString();
                    await context.PostAsync(reply);
                }
            }
            else
            {
                for (int i = 0; i < jNews.Count(); i++)
                {
                    //init
                    reply = string.Empty;
                    imgUrl = string.Empty;
                    ntitle = string.Empty;
                    detailUrl = string.Empty;
                    //organize reply
                    var news = jNews[i];
                    imgUrl = "http://clubweb.azurewebsites.net" + news["Nimage"];
                    reply += $"![动态图]({imgUrl}) ";
                    ntitle = news["Ntitle"].ToString();
                    detailUrl = "http://clubweb.azurewebsites.net/More/Details?id=" + news["ID"] + "&type=news";
                    reply += $"[{ntitle}]({detailUrl})";
                    reply += "\r\n\r\n" + "——" + Convert.ToDateTime(news["Ntime"]).ToLongDateString();
                    await context.PostAsync(reply);
                }
            }
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("活动/竞赛")]
        public async Task Activities(IDialogContext context, LuisResult result)
        {
            string clubname = GetClubName(result);//get club
            if (string.IsNullOrEmpty(clubname))  //is null
            {
                await context.PostAsync($"请以正确的打开方式, 如:*计算机协会的{result.Intents[0].Intent}*");
                return;
            }
            var club = await GetDataFromAPI.DataToObject(UrlHelper.GetClubUrl, "clubname=" + clubname); //get all content related to club
            if (club == null)
            {
                await context.PostAsync("您所查询的协会不存在, 我会尽快完善我的知识库~~");
                return;
            }
            var jActi = club["Activities"];
            //未找到信息
            if (jActi.Count() <= 0)
            {
                await context.PostAsync("此协会还未有活动记录");
                return;
            }
            //含有信息
            string first = $"**<{clubname}>的最近的活动如下**";
            await context.PostAsync(first);

            string reply;
            string imgUrl;
            string atitle;
            string detailUrl;            
            if (jActi.Count() >= 3)
            {

                for (int i = 0; i < 3; i++)
                {
                    //init
                    reply = string.Empty;
                    imgUrl = string.Empty;
                    atitle = string.Empty;
                    detailUrl = string.Empty;
                    //organize reply
                    var acti = jActi[i];
                    imgUrl = "http://clubweb.azurewebsites.net" + acti["Aimage"];
                    reply += $"![动态图]({imgUrl}) ";
                    atitle = acti["Atitle"].ToString();
                    detailUrl = "http://clubweb.azurewebsites.net/More/Details?id=" + acti["ID"] + "&type=activities";
                    reply += $"[{atitle}]({detailUrl})";
                    reply += "\r\n\r\n" + "——" + Convert.ToDateTime(acti["Atime"]).ToLongDateString();
                    await context.PostAsync(reply);
                }
            }
            else
            {
                for (int i = 0; i < jActi.Count(); i++)
                {
                    //init
                    reply = string.Empty;
                    imgUrl = string.Empty;
                    atitle = string.Empty;
                    detailUrl = string.Empty;
                    //organize reply
                    var acti = jActi[i];
                    imgUrl = "http://clubweb.azurewebsites.net" + acti["Aimage"];
                    reply += $"![动态图]({imgUrl}) ";
                    atitle = acti["Atitle"].ToString();
                    detailUrl = "http://clubweb.azurewebsites.net/More/Details?id=" + acti["ID"] + "&type=activities";
                    reply += $"[{atitle}]({detailUrl})";
                    reply += "\r\n\r\n" + "——" + Convert.ToDateTime(acti["Atime"]).ToLongDateString();
                    await context.PostAsync(reply);
                }
            }
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("公告")]
        public async Task Notice(IDialogContext context, LuisResult result)
        {
            string clubname = GetClubName(result);//get club
            if (string.IsNullOrEmpty(clubname))  //is null
            {
                await context.PostAsync($"请以正确的打开方式, 如:*计算机协会的{result.Intents[0].Intent}*");
                return;
            }
            var club = await GetDataFromAPI.DataToObject(UrlHelper.GetClubUrl, "clubname=" + clubname); //get all content related to club
            if (club == null)
            {
                await context.PostAsync("您所查询的协会不存在, 我会尽快完善我的知识库~~");
                return;
            }
            var jNotice = club["Notice"];
            //未找到信息
            if (jNotice.Count() <= 0)
            {
                await context.PostAsync("此协会还未发布公告");
                return;
            }
            //含有信息
            await context.PostAsync($"**以下为<{clubname}>最近的公告**");

            string reply;
            if (jNotice.Count() >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    reply = string.Empty;
                    //organize reply
                    var notice = jNotice[i];
                    reply += notice["Ncontent"].ToString() + "\r\n\r\n";
                    reply += "——" + notice["Subtime"];
                    await context.PostAsync(reply);
                }
            }
            else
            {
                for (int i = 0; i < jNotice.Count(); i++)
                {
                    reply = string.Empty;
                    //organize reply
                    var notice = jNotice[i];
                    reply += notice["Ncontent"].ToString() + "\r\n\r\n";
                    reply += "——" + notice["Subtime"];
                    await context.PostAsync(reply);
                }
            }
            //是否自动推送信息
            if (AutoMessage()) await context.PostAsync(WebUrl);
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context,LuisResult result)
        {
            string res = await GetDataFromAPI.DataToString(UrlHelper.LearnMoreUrl, "content=" + result.Query);
            //await context.PostAsync(res);
            if (res == "Ok")
            {
                await context.PostAsync($"*{result.Query}*\r\n\r\n这句话我已经记住了, 我会更加努力地学习");
            }
            context.Wait(MessageReceived);
        }
        //批评 夸奖 致谢 调侃

        [LuisIntent("批评")]
        public async Task Criticize(IDialogContext context,LuisResult result)
        {
            Random r = new Random();
            int index = r.Next(0, 3);
            string reply = (ReplyHelper.CRITICISNM[index]);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

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

        //当用于针对一个协会询问5次以上进行 自动推送“查看网站”消息
        private bool AutoMessage()
        {
            Count++;
            if (Count >= 5)
            {
                Count = 0;  //重新计数
                return true;
            }
            return false;
        }

    }
}