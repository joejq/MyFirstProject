using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Areas.Register.Controllers
{
    //需要验证
    public class MainController : Controller
    {
        const string iconDefaultUrl = "/Content/images/default/100_100.jpg";
        const string leaderDefaultUrl = "/Content/images/default/150_200.jpg";
        static int Cid;
        // GET: Register/Main
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegisterSubmit()
        {
            IBLL.IClubsService clubService = new BLL.ClubsService();
            IBLL.IBackManagerService bmService = new BLL.BackManagerService();
            //添加协会
            string clubname = (Request["clubname"]).Trim();
            Model.Clubs club = new Model.Clubs()
            {
                Cname = clubname
            };
            //club.Delflag = 1;  //暂时封锁协会，等注册完毕/管理员认证后进行释放
            //添加协会对应的管理员
            if (!clubService.AddEntity(club)) return Content("Fail");
            int cid = clubService.GetIdByClubnameIgnoreDelflag(clubname);
            if (cid <= 0) return Content("Fail");
            Model.BackManager bm = new Model.BackManager()
            {
                Busername = Request["contact"].Trim(),
                Bcontact = Request["email"].Trim(),
                //加密处理
                Bpass = Md5Helper.EncryptString(Request["pass"]),
                CID = cid
            };
            //bm.Delflag = 1;  //暂时封锁管理员，等注册完毕/管理员认证后进行释放
            if (bmService.AddEntity(bm))
            {
                //记录session在项填写详细信息的页面跳转中验证
                Session["Clubname"] = clubname;
                Session["Clubid"] = cid;
                //设置全局变量
                Cid = cid;
                return Content("Ok");
            }
            return Content("Fail");
        }

        public ActionResult IsExist()
        {
            string clubname = Request["clubname"].Trim();
            IBLL.IClubsService clubService = new BLL.ClubsService();
            var temp = clubService.LoadEntities(e => e.Delflag == 0 && e.Cname == clubname).ToList();
            if (temp.Count > 0)
            {//协会已经存在
                return Content("Fail");
            }
            else
            {
                return Content("Ok");
            }
        }
        
        public ActionResult ContactExist()
        {
            string contact = Request["contact"].Trim();
            IBLL.IBackManagerService bmService = new BLL.BackManagerService();
            var temp = bmService.LoadEntities(e => e.Delflag == 0 && e.Busername == contact).ToList();
            if (temp.Count > 0)
            {//手机号已经被注册
                return Content("Fail");
            }
            else
            {
                return Content("Ok");
            }
        }

        public ActionResult EditDetail()
        {
            if (Session["Clubname"] == null || Session["Clubid"] == null)
            {
                return Content("Error, 非法请求");
            }
            ViewData["Clubname"] = Session["Clubname"].ToString();
            ViewBag.Cid = Cid;
            return View();
        }
        [ValidateInput(false)]
        public ActionResult AddMainInfoAndLeader()
        {
            IBLL.IMainInfoService miService = new BLL.MainInfoService();
            IBLL.IClubLeaderService leaderService = new BLL.ClubLeaderService();
            DateTime createTime;
            try
            {
                createTime = Convert.ToDateTime(Request["createDate"]);
            }
            catch(Exception ex)
            {
                createTime = DateTime.Now;
            }
            Model.MainInfo mainInfo = new Model.MainInfo()
            {
                Micon = string.IsNullOrEmpty(Request["logo"]) ? iconDefaultUrl : Request["logo"],
                Mintroduction = Request["introduction"],
                Mcreatetime = createTime,
                MbelongTo = Request["belongTo"],
                Mcooperation = Request["cooperation"],
                CID = Cid
            };
            if(!miService.AddEntity(mainInfo)) return Content("Fail");
            DateTime registe;
            try
            {
                registe = Convert.ToDateTime(Request["registe"]);
            }catch(Exception ex)
            {
                registe = DateTime.Now;
            }
            short sex = 1;
            short.TryParse(Request["sex"], out sex);
            Model.ClubLeader leader = new Model.ClubLeader()
            {
                Lname = Request["leadername"],
                Lmajor = Request["major"],
                Lcontact = Request["contact"],
                Lregiste = registe,
                Lsex = sex,
                Limage = string.IsNullOrEmpty(Request["pic"]) ? leaderDefaultUrl : Request["pic"],
                CID = Cid
            };
            if(leaderService.AddEntity(leader))
            {
                return Content("Ok");
            }
            else
            {
                return Content("Fail");
            }
            
        }    
    }
}