using Common;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Controllers
{
    public class MoreController : Controller
    {
        static int pageSize = 15;
        static int Cid;
        static string Icon = "/Content/images/default/icon.png";
        //const string iconDefaultUrl = "/Content/images/default/icon.png";
        // GET: More
        public ActionResult Index(int id)
        {
            if (id <= 0) return Content("Error, 错误请求");
            //判断请求的内容
            string type = string.Empty;
            type = Request["type"];
            if (type == string.Empty) return Content("Error, 错误请求");
            Cid = id;
            ViewBag.FirstClubName = Session["CLUB_NAME"];
            ViewBag.Cid = id;
            ViewBag.Type = type;
            //获取图标
            IBLL.IMainInfoService miService = new BLL.MainInfoService();
            var temp = miService.LoadEntities(e => e.Delflag == 0 && e.CID == Cid).ToList();
            if (temp.Count > 0) Icon = temp[0].Micon;
            ViewBag.Icon = Icon;
            return View();
        }
        
        public ActionResult LoadNews()
        {
            if (Cid <= 0) return Json(null);
            IBLL.INewsService newsService = new BLL.NewsService();
            int pageIndex;
            if (!int.TryParse(Request["pageIndex"], out pageIndex))
            {
                pageIndex = 1;
            }
            int total;
            /*获取分页列表*/
            var newsList = newsService.LoadPageEntities(pageIndex, pageSize, out total, e => e.CID == Cid && e.Delflag == 0, e => e.Ntime, false).ToList();
            /*获取页码条*/
            int pageCount = (int)Math.Ceiling((double)total / pageSize); //天花板函数-获取大于等于此小数的整数
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            string pageBarString = PageBar.GetPageBarByType(pageIndex, pageCount, "loadNews");
            var jNews = JsonConvert.SerializeObject(new { news = newsList, pageBar = pageBarString }, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jNews, JsonRequestBehavior.AllowGet);
        }        

        public ActionResult LoadActivities()
        {
            if (Cid <= 0) return Json(null);
            IBLL.IActivitiesSerivice actiService = new BLL.ActivitiesService();
            int pageIndex;
            if (!int.TryParse(Request["pageIndex"], out pageIndex))
            {
                pageIndex = 1;
            }
            int total;
            var actiList = actiService.LoadPageEntities(pageIndex, pageSize, out total, e => e.CID == Cid && e.Delflag == 0, e => e.Atime, false).ToList();
            int pageCount = (int)Math.Ceiling((double)total / pageSize);
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            string pageBarString = PageBar.GetPageBarByType(pageIndex, pageCount, "loadActivities");
            var jActi = JsonConvert.SerializeObject(new { activities = actiList, pageBar = pageBarString }, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jActi, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadNotice()
        {
            if (Cid <= 0) return Json(null);
            IBLL.INoticeService noticeService = new BLL.NoticeService();
            int pageIndex;
            if (!int.TryParse(Request["pageIndex"], out pageIndex))
            {
                pageIndex = 1;
            }
            int total;
            var noticeList = noticeService.LoadPageEntities(pageIndex, 10, out total, e => e.CID == Cid && e.Delflag == 0, e => e.Subtime, false).ToList();
            int pageCount = (int)Math.Ceiling((double)total / pageSize);
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            string pageBarString = PageBar.GetPageBarByType(pageIndex, pageCount, "loadNotice");
            var jNotice = JsonConvert.SerializeObject(new { notice = noticeList, pageBar = pageBarString }, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jNotice, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details()
        {
            ViewBag.FirstClubName = Session["CLUB_NAME"];

            string type = Request["type"];
            int id;
            if(!int.TryParse(Request["id"], out id)) return Content("Error, 错误的请求");
            DateTime date;
            switch (type)
            {
                case "news":
                    IBLL.INewsService newService = new BLL.NewsService();
                    News news = newService.GetNewsById(id);
                    ViewData["Title"] = news.Ntitle;
                    date = Convert.ToDateTime(news.Ntime);
                    ViewData["Date"] = date.ToShortDateString();
                    ViewData["Content"] = news.Ncontent;
                    break;
                case "activities":
                    IBLL.IActivitiesSerivice actiService = new BLL.ActivitiesService();
                    Activities acti = actiService.GetActiById(id);
                    ViewData["Title"] = acti.Atitle;
                    date = Convert.ToDateTime(acti.Atime);
                    ViewData["Date"] = date.ToShortDateString();
                    ViewData["Content"] = acti.Acontent;
                    break;
            }
            ViewBag.FirstClubName = Session["CLUB_NAME"];
            ViewBag.Icon = Icon;
            return View();
        }
    }
}