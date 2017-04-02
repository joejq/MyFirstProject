
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalligenceBot.Common
{
    public class ReplyHelper
    {
        //public const string HELLO = "hi, 我是校园协会/社团的服务Bot, 尽管问我一些有关协会/社团的问题, 很高兴能够帮到您";
        public const string MYSELF = "hi, 我是校园协会/社团的服务Bot, 我叫**CalligenceBot**, 主要业务是为大家提供社有关社团的信息。我能做的事有:\r\n\r\n1.查看指定社团的主要信息\r\n\r\n2.查看会长信息\r\n\r\n3.查看最新新闻动态\r\n\r\n4.查看协会的成果/荣誉\r\n\r\n5.查看协会的公告与通知\r\n\r\n 现在你可以用自然语言与我交流哦……";
        public const string LEARNMORE = "抱歉, 我太懂您的意思, 但我努力学习让自己变得更聪明";
        public static string[] CRITICISNM = { "我还不够聪明", "您的批评将有助于我很好的找到自己下一步努力的方向", "皇上，臣妾知错了##" };
        public static string[] PRAISE = { "您的夸奖便是我努力学习变聪明的最大动力！", "我还有更多知识需要去学习领会……", "您对我的肯定便是对创造我的主人的肯定，感动ing！" };
        public static string[] APPRECIATION = { "不用客气，为您服务是CalligenceBot的荣幸。", "您的致谢肯定了我的服务，更是鼓舞了背后的开发者！", "不用客气啦，人家会害羞的^0^" };
        public static string[] KIDDING = { "不要调侃我哦，我可是一个段子Bot哦", "嘿嘿嘿，哈哈哈，呵呵呵", "我想做一个单纯的Bot去服务更多的客户，当然你开心就好" };
    }
}