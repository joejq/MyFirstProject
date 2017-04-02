using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalligenceBot.Common
{
    public class UserHelper
    {
        public static int Count = 1;
        //每个用户数据的UserId属性对应UserList中的key
        public static Dictionary<int, NowClub> UserList = new Dictionary<int, NowClub>();

        public static NowClub GetNowClub(int id)
        {
            if (UserList.ContainsKey(id))
            {
                UserList[id].Count++;
                return UserList[id];
            }
            return new NowClub();
        }

        public static void SetNowClub(int id, int cid, string cname)
        {
            if (UserList.ContainsKey(id))
            {
                try
                {
                    NowClub nc = UserList[id];
                    nc.Cid = cid;
                    nc.Cname = cname;
                    nc.Count = 0;  //访问新的社团，计数器置0
                }
                catch (Exception ex)
                {
                    NowClub club = new NowClub(cid, cname);
                    UserList[id] = club;
                }
            }
            else
            {
                NowClub club = new NowClub(cid, cname);
                UserList.Add(id, club);
            }
        }

        public static void ClearCount(int id)
        {
            if (UserList.ContainsKey(id))
            {
                try
                {
                    NowClub nc = UserList[id];
                    nc.Count = 0;  //计数器置0
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
    }

    public class NowClub
    {
        public NowClub()
        {
            Cid = -1;
            Cname = string.Empty;
            Count = 0;
        }
        public NowClub(int cid, string cname)
        {
            Cid = cid;
            Cname = cname;
            Count = 0;
        }
        public int Cid { get; set; } 
        public string Cname { get; set; }
        //计数器，用户多次访问同一个社团，自动推送站点网址
        public int Count { get; set; }
    }
}