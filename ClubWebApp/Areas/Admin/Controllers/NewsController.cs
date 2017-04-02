using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Areas.Admin.Controllers
{
    [LoginFilterAttribute]
    public class NewsController : Controller
    {
        static int Cid;
        static int pageSize = 15;
        const string newsDefaultUrl = "/Content/images/default/100_80.png";

        IBLL.INewsService newsService = new BLL.NewsService();
        // GET: Admin/News
        public ActionResult Index(int id)
        {
            Cid = id;
            int pageIndex;
            if (!int.TryParse(Request["pageIndex"], out pageIndex))
            {
                pageIndex = 1;
            }
            int total;
            //int cid = Cid;
            //if (Club != null) cid = Club.ID;
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

        public ActionResult AddNews()
        {
            return View();
        }
        //Add submit
        [ValidateInput(false)]
        public ActionResult AddNewsSubmit()
        {
            DateTime date;
            try
            {
                //Debug.Write(Request["date"]);
                date = Convert.ToDateTime(Request["date"]);
            }
            catch (Exception ex)
            {
                date = DateTime.Now;
            }
            string content = Request["content"];
            Model.News news = new Model.News()
            {
                Ntitle = Request["title"],
                Ncontent = content,
                Nimage = string.IsNullOrEmpty(Request["url"]) ? newsDefaultUrl : Request["url"],
                Ntime = date,
                CID = Cid
            };
            #region update club subtime
            IBLL.IClubsService cs = new BLL.ClubsService();
            Model.Clubs c = cs.LoadEntities(e => e.ID == Cid).First();
            c.Subtime = DateTime.Now;
            cs.EditEntity(c);
            #endregion
            if (newsService.AddEntity(news))
            {
                return Content("Ok");
            }
            else
            {
                return Content("Fail");
            }
        }
        //Edit
        public ActionResult EditNews()
        {
            int nid;
            if (int.TryParse(Request["id"], out nid))
            {
                if (nid <= 0) return Content("Error");
                Model.News news = newsService.GetNewsById(nid);
                ViewBag.Nid = news.ID;
                ViewData["Ntitle"] = news.Ntitle;
                ViewData["Nimg"] = news.Nimage;
                ViewData["Ndate"] = news.Ntime;
                ViewData["Ncontent"] = news.Ncontent;
                return View();
            }
            else
            {
                return Content("Error");
            }
        }
        //Edit submit
        [ValidateInput(false)]
        public ActionResult EditNewsSubmit()
        {
            int nid;
            DateTime date;
            try
            {
                date = Convert.ToDateTime(Request["date"]);
            }
            catch (Exception ex)
            {
                date = DateTime.Now;
            }
            
            if (int.TryParse(Request["id"], out nid))
            {
                if (nid <= 0) return Content("Error");
                Model.News news = new Model.News()
                {
                    ID = nid,
                    Ntitle = Request["title"],
                    Ncontent = Request["content"],
                    Nimage = string.IsNullOrEmpty(Request["url"]) ? newsDefaultUrl : Request["url"],
                    Ntime = date,
                    CID = Cid
                };
                if (newsService.EditEntity(news))
                {
                    return Content("Ok");
                }
                else
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("Error!");
            }
        }
        //Delete
        public ActionResult DeleteNews()
        {
            int nid;
            if (int.TryParse(Request["id"], out nid))
            {
                if (nid <= 0) return Content("Error");
                if (newsService.LogicDeleteEntity(nid)) return Content("Ok");
                else return Content("Fail");
            }
            else return Content("Error");
        }
    }
}