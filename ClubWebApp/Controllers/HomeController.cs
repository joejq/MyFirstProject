using Common;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Controllers
{
    public class HomeController : Controller
    {
        //static int Cid;
        static Clubs Club;
        static bool HaveSearched = false;

        //First Page
        public ActionResult SelectWay()
        {
            return View();
        }
        // GET: Home
        public ActionResult Index()
        {
            IBLL.IClubsService clubsService = new BLL.ClubsService();
            //获取第一个协会 作为整个页面的内容加载依据
            if (Club == null)
            {
                int total;
                Club = clubsService.LoadPageEntities(1, 1, out total, e => e.Delflag == 0, e => e.Subtime, false).First();
                ViewData["Total"] = total;
            }
            Session["CLUB_NAME"] = Club.Cname; //设置Session域 全局共享
            ViewBag.Cid = Club.ID;
            ViewBag.FirstClubName = Club.Cname;
            ViewData["Clubs"] = Clubs();  //获取首页的前5个协会 用于下填充下拉菜单
            return View(); 
        }

        public List<ShowClub> Clubs()
        {
            IBLL.IClubsService clubsService = new BLL.ClubsService();
            List<ShowClub> showClubs = new List<ShowClub>();
            int total;
            if(HaveSearched==false || Club==null)  //仅刷新当前页面
            {
                //正常获取前五个
                var entities = clubsService.LoadPageEntities(1, 5, out total, e => e.Delflag == 0, e => e.Subtime, false);
                var clubs = entities.ToList();
                Club = clubs[0];
                foreach (var c in clubs)
                {
                    showClubs.Add(new ShowClub() { Clubid = c.ID, Clubname = c.Cname });
                }
                HaveSearched = false;  //刷新后将搜索标志消除
            }
            else
            {
                //将已经设置好的全局协会 作为第一个
                showClubs.Add(new ShowClub() { Clubid = Club.ID, Clubname = Club.Cname });
                var entities = clubsService.LoadPageEntities(1, 4, out total, e => e.Delflag == 0, e => e.Subtime, false);
                var clubs = entities.ToList();
                foreach (var c in clubs)
                {
                    showClubs.Add(new ShowClub() { Clubid = c.ID, Clubname = c.Cname });
                }
            }
            return showClubs;
        }

        public ActionResult Search()
        {
            IBLL.IClubsService clubsService = new BLL.ClubsService();
            string searchName = Request["searchName"].Trim();
            var temp = clubsService.LoadEntities(e => e.Delflag == 0 && e.Cname == searchName).ToList();
            if (temp.Count <= 0) return Content("Fail");
            var searchResult = temp[0];
            //标记已搜索
            HaveSearched = true;
            //返回查询的协会id
            return Content(searchResult.ID.ToString());
        }

        public ActionResult ChangeClub()
        {
            int id;
            if(int.TryParse(Request["id"], out id))
            {
                IBLL.IClubsService clubsService = new BLL.ClubsService();
                Club = clubsService.LoadEntities(e => e.ID == id).First();
                Session["CLUB_NAME"] = Club.Cname;
                ViewBag.Cid = Club.ID;
                ViewBag.FirstClubName = Club.Cname;
                return Content("Ok");
            }
            else
            {
                return Content("Fail");
            }
            
        }

        public ActionResult News()
        {
            if (Club == null) return Content("[]");
            IBLL.INewsService newsService = new BLL.NewsService();
            int total;
            var newsList = newsService.LoadPageEntities(1, 8, out total, e => e.Delflag == 0 && e.CID == Club.ID, e => e.Ntime, false).ToList();
            var jNews= JsonConvert.SerializeObject(newsList, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(jNews, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Activities()
        {
            if (Club == null) return Content("[]");
            IBLL.IActivitiesSerivice actiService = new BLL.ActivitiesService();
            int total;
            var actiList = actiService.LoadPageEntities(1, 9, out total, e => e.Delflag == 0 && e.CID == Club.ID, e => e.Atime, false).ToList();
            var jActi= JsonConvert.SerializeObject(actiList, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(jActi, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Notice()
        {
            if (Club == null) return Content("[]");
            IBLL.INoticeService noticeService = new BLL.NoticeService();
            int total;
            var noticeList = noticeService.LoadPageEntities(1, 8, out total, e => e.Delflag == 0 && e.CID == Club.ID, e => e.Subtime, false).ToList();
            //if (noticeList.Count <= 0) return Json(null);
            var jNotice= JsonConvert.SerializeObject(noticeList, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(jNotice, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Leader()
        {
            if (Club == null) return Content("[]");
            IBLL.IClubLeaderService leaderService = new BLL.ClubLeaderService();
            var leaderList = leaderService.LoadEntities(e => e.Delflag == 0 && e.CID == Club.ID).ToList();
            if (leaderList.Count <= 0)
            {
                var jnull = JsonConvert.SerializeObject(leaderList, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return Json(jnull, JsonRequestBehavior.AllowGet);
            }
            var leader = leaderList[0];
            var jLeader = JsonConvert.SerializeObject(leader, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(jLeader, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Displays()
        {
            //相对路径
            string t = "/Content/images/" + Club.Cname + "/display/";
            //绝对路径
            string dir = Server.MapPath(t);
            List<Display> displays = new List<Display>();
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                FileInfo[] files = dirInfo.GetFiles();
                foreach (var f in files)
                {
                    displays.Add(new Display { FilePath = "/Content/images/" + Club.Cname + "/display/" + f.Name });
                }
            }
            catch (Exception ex) { }//未获取到文件
            var jDis = JsonConvert.SerializeObject(displays, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            //if (list.Count <= 0) return Json("{}");
            return Json(jDis, JsonRequestBehavior.AllowGet);
        }

        public ActionResult More()
        {
            string type = Request["type"];
            return Redirect("/More/Index/" + Club.ID + "?type=" + type);
        }

        public ActionResult MainInfo()
        {
            IBLL.IMainInfoService miService = new BLL.MainInfoService();
            MainInfo mainInfo;
            //来自bot请求
            int botRequestId;
            if(int.TryParse(Request["botid"], out botRequestId)){
                mainInfo = miService.GetInfoByCid(botRequestId);
                ViewData["Mname"] = Request["botname"];
                ViewData["Micon"] = mainInfo.Micon;
                ViewData["Mintroduction"] = mainInfo.Mintroduction;
                DateTime date = (DateTime)mainInfo.Mcreatetime;
                ViewData["Mcreatetime"] = date.Year + "年";
                ViewData["MbelongTo"] = mainInfo.MbelongTo;
                ViewData["Mcooperation"] = mainInfo.Mcooperation;
                return View();
            }

            //正常browser请求
            if (Club == null) return Content("Error"); 
            var m = miService.LoadEntities(e=>e.CID==Club.ID).ToList();
            if (m.Count <= 0) return Content("<script>alert('此信息还未填写');</script>");
            mainInfo = m[0];
            ViewData["Mname"] = Club.Cname;
            ViewData["Micon"] = mainInfo.Micon;
            ViewData["Mintroduction"] = mainInfo.Mintroduction;
            DateTime d = (DateTime)mainInfo.Mcreatetime;
            ViewData["Mcreatetime"] = d.Year + "年";
            ViewData["MbelongTo"] = mainInfo.MbelongTo;
            ViewData["Mcooperation"] = mainInfo.Mcooperation;
            return View();
        }
        
    }
}