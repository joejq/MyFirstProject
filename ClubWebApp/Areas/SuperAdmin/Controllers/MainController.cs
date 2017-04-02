using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Areas.SuperAdmin
{
    public class MainController : Controller
    {
        static int pageSize = 15;

        IBLL.IClubsService clubService = new BLL.ClubsService();
        // GET: SuperAdmin/Main
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Clubs()
        {
            IBLL.IClubsService clubService = new BLL.ClubsService();
            IBLL.IClubLeaderService leaderService = new BLL.ClubLeaderService();
            int pageIndex;
            if (!int.TryParse(Request["pageIndex"], out pageIndex))
            {
                pageIndex = 1;
            }
            int total;
            /*获取分页列表*/
            var clubList = clubService.LoadPageEntities(pageIndex, pageSize, out total, e =>e.Delflag == 0, e => e.Subtime, false).ToList();
            List<SuperClub> list = new List<SuperClub>();
            foreach(var c in clubList)
            {
                var leader = leaderService.GetLeaderByCid(c.ID);
                string lname = "";
                string lcontact = "";
                if(leader != null)
                {
                    lname = leader.Lname;
                    lcontact = leader.Lcontact;
                }
                list.Add(new SuperClub() { ID = c.ID, Clubname = c.Cname, Leadername = lname, Contact = lcontact });
            }
            /*获取页码条*/
            int pageCount = (int)Math.Ceiling((double)total / pageSize); //天花板函数-获取大于等于此小数的整数
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            string pageBarString = PageBar.GetPageBarByType(pageIndex, pageCount, "loadClubs");
            var jNews = JsonConvert.SerializeObject(new { clubs = list,  pageBar = pageBarString }, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jNews, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Luis()
        {
            IBLL.ILearnMoreService lmService = new BLL.LearnMoreService();
            int pageIndex;
            if (!int.TryParse(Request["pageIndex"], out pageIndex))
            {
                pageIndex = 1;
            }
            int total;
            /*获取分页列表*/
            var luisList = lmService.LoadPageEntities(pageIndex, pageSize, out total, e => e.Lok == 0, e => e.Subtime, false).ToList();
            /*获取页码条*/
            int pageCount = (int)Math.Ceiling((double)total / pageSize); //天花板函数-获取大于等于此小数的整数
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            string pageBarString = PageBar.GetPageBarByType(pageIndex, pageCount, "loadLuis");
            var jNews = JsonConvert.SerializeObject(new { luis = luisList, pageBar = pageBarString }, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jNews, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteClub()
        {
            IBLL.IBackManagerService bmService = new BLL.BackManagerService();
            int cid;
            if (int.TryParse(Request["id"], out cid))
            {
                if(cid <= 0) return Content("Error");
                if(!bmService.LogicDeleteByCid(cid)) return Content("Fail"); //删除管理员
                if(!clubService.LogicDeleteEntity(cid)) return Content("Fail"); //删除协会
                return Content("Ok");
            }
            else return Content("Error");
        }

        public ActionResult Practise()
        {
            IBLL.ILearnMoreService lmService = new BLL.LearnMoreService();
            int lid;
            if(int.TryParse(Request["id"], out lid))
            {
                if (lid <= 0) return Content("Error");
                var temp = lmService.LoadEntities(e => e.ID == lid).ToList();
                if (temp.Count > 0)
                {
                    var lm = temp[0];
                    lm.Lok = 1;
                    if (lmService.EditEntity(lm)) return Content("Ok");
                }
            }
            return Content("Error");
        }

    }

    public class SuperClub
    {
        public int ID { get; set; }
        public string Clubname { get; set; }
        public string Leadername { get; set; }
        public string Contact { get; set; }
    }
}