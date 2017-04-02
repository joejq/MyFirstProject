using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Controllers
{
    public class BotApiController : Controller
    {
        // GET: BotApi
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取当前所有的协会
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllClubs()
        {
            IBLL.IClubsService clubsService = new BLL.ClubsService();
            short delFlag = (short)DeleteEnumType.Normal;
            var allClubs = clubsService.LoadEntities(e => e.Delflag == delFlag).ToList();
            var jAllClubs = JsonConvert.SerializeObject(allClubs, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jAllClubs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证协会是否存在 公用方法
        /// </summary>
        /// <param name="clubName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private bool IsExist(string clubName, out int ID)
        {
            IBLL.IClubsService clubsService = new BLL.ClubsService();
            var temp = clubsService.LoadEntities(e => e.Cname == clubName).ToList();
            if (temp.Count > 0)
            {
                Clubs club = temp[0];
                ID = club.ID;
                return true;
            }
            else
            {
                ID = 0;
                return false;
            }
        }

        /// <summary>
        /// 验证协会是否存在
        /// </summary>
        public ActionResult IsExist()
        {
            string clubName = Request["clubName"];
            //clubName = clubName.Trim();
            string message = string.Empty;
            int ID = 0;
            if(IsExist(clubName, out ID))
            {
                message = ID.ToString();
            }
            else
            {
                message = "No";
            }
            return Content(message);
        }

        /// <summary>
        /// 根据协会ID获取主要信息
        /// </summary>
        /// <returns>主要信息的JSON数据</returns>
        public ActionResult GetMainInfo()
        {
            int ID = int.Parse(Request["id"]);
            IBLL.IMainInfoService mainInfoService = new BLL.MainInfoService();
            var temp = mainInfoService.LoadEntities(e => e.CID == ID).ToList();
            MainInfo mainInfo = temp[0];
            var jMainInfo = JsonConvert.SerializeObject(mainInfo, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jMainInfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据协会ID获取会长信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>会长具体信息的JSON数据</returns>
        public ActionResult GetClubLeader()
        {
            int ID = int.Parse(Request["id"]);
            IBLL.IClubLeaderService clubLeaderService = new BLL.ClubLeaderService();
            var temp = clubLeaderService.LoadEntities(e => e.CID == ID).ToList();
            ClubLeader clubLeader = temp[0];
            var jClubLeader = JsonConvert.SerializeObject(clubLeader, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jClubLeader, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取最新公告
        /// </summary>
        /// <returns>JSON</returns>
        public ActionResult GetLatestNotice()
        {
            int ID = int.Parse(Request["id"]);
            IBLL.INoticeService noticeService = new BLL.NoticeService();
            var latestNotice = noticeService.GetLatestNotice(ID);
            var jLatestNotice = JsonConvert.SerializeObject(latestNotice, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jLatestNotice, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取最近公告
        /// </summary>
        /// <returns>JSON</returns>
        public ActionResult GetRecentNotice()
        {
            int ID = int.Parse(Request["id"]);
            IBLL.INoticeService noticeService = new BLL.NoticeService();
            var recentNotice = noticeService.GetRecentNotice(ID);
            var jRecentNotice = JsonConvert.SerializeObject(recentNotice, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jRecentNotice, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取最新新闻
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLatestNews()
        {
            int ID = int.Parse(Request["id"]);
            IBLL.INewsService newsService = new BLL.NewsService();
            var latestNews = newsService.GetLatestNews(ID);
            var jLatestNews = JsonConvert.SerializeObject(latestNews, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jLatestNews, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取最近新闻
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRecentNews()
        {
            int ID = int.Parse(Request["id"]);
            IBLL.INewsService newsServie = new BLL.NewsService();
            var recentNews = newsServie.GetRecentNews(ID);
            var jRecentNews = JsonConvert.SerializeObject(recentNews, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jRecentNews, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取最新活动
        /// </summary>
        /// <returns>JSON</returns>
        public ActionResult GetLatestActivity()
        {
            int ID = int.Parse(Request["id"]);
            IBLL.IActivitiesSerivice actService = new BLL.ActivitiesService();
            var latestActivity = actService.GetLatestActivity(ID);
            var jLatestActivity = JsonConvert.SerializeObject(latestActivity, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jLatestActivity, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取最近的活动
        /// </summary>
        /// <returns>JSON</returns>
        public ActionResult GetRecentActivities()
        {
            int ID = int.Parse(Request["id"]);
            IBLL.IActivitiesSerivice actService = new BLL.ActivitiesService();
            var recentActivities = actService.GetRecentActivities(ID);
            var jRecentActivities = JsonConvert.SerializeObject(recentActivities, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jRecentActivities, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 深度学习
        /// </summary>
        /// <returns></returns>
        public ActionResult LearnMore()
        {
            string strLearnMore = Request["content"];
            LearnMore learnMore = new Model.LearnMore()
            {
                Lcontent = strLearnMore
            };
            IBLL.ILearnMoreService lmService = new BLL.LearnMoreService();
            if (lmService.AddEntity(learnMore))
            {
                return Content("Ok");
            }
            return Content("Fial");
        }
    }
}