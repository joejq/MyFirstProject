using Common;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Areas.Admin.Controllers
{
    [LoginFilterAttribute]
    public class MainController : Controller
    {
        const string iconDefaultUrl = "/Content/images/default/100_100.jpg";
        const string leaderDefaultUrl = "/Content/images/default/150_200.jpg";

        static int pageSize = 15;
        static Clubs Club;
        static MainInfo MainInfo;
        // GET: Admin/Main

        IBLL.IClubsService clubsService = new BLL.ClubsService();
        IBLL.IMainInfoService mainInfoService = new BLL.MainInfoService();
        public ActionResult Index()
        {
            if (Session["cid"] == null)
            {
                return Content("Error");
            }
            int cid;
            if(int.TryParse(Session["cid"].ToString(), out cid)){
                //赋值全局的 协会
                Club = clubsService.GetClubById(cid);
                MainInfo = mainInfoService.GetInfoByCid(Club.ID);
                ViewBag.Cid = Club.ID;
                ViewBag.ClubName = Club.Cname;
                if (MainInfo == null) ViewBag.Img = iconDefaultUrl;
                else ViewBag.Img = MainInfo.Micon;
                return View();
            }
            else
            {
                return Content("Error");
            }
            
        }

        /*ClubInfo*/
        public ActionResult EditClubInfo()
        {
            if (Club == null) return Content("Error");
            Model.MainInfo mainInfo = mainInfoService.GetInfoByCid(Club.ID);
            if (mainInfo != null)
            {
                ViewData["Mid"] = mainInfo.ID;
                ViewData["Micon"] = mainInfo.Micon;
                ViewData["Mintroduction"] = mainInfo.Mintroduction;
                ViewData["Mcreatetime"] = mainInfo.Mcreatetime;
                ViewData["MbelongTo"] = mainInfo.MbelongTo;
                ViewData["Mcooperation"] = mainInfo.Mcooperation;
            }
            else
            {
                ViewData["Mid"] = string.Empty;
                ViewData["Micon"] = string.Empty;
                ViewData["Mintroduction"] = string.Empty;
                ViewData["Mcreatetime"] = string.Empty;
                ViewData["MbelongTo"] = string.Empty;
                ViewData["Mcooperation"] = string.Empty;
            }
            ViewBag.ClubName = Club.Cname;          
            return View();
        }
        [ValidateInput(false)]
        public ActionResult EditClubInfoSubmit()
        {
            Model.MainInfo mainInfo;
            DateTime date;
            try
            {
                date = Convert.ToDateTime(Request["createDate"]);
            }
            catch (Exception ex)
            {
                date = DateTime.Now;
            }
            int mid;
            if(!int.TryParse(Request["id"], out mid))
            {
                //mid 不存在即协会的信息还未添加 此操作是添加协会信息
                mainInfo = new MainInfo()
                {
                    Micon = string.IsNullOrEmpty(Request["url"]) ? iconDefaultUrl : Request["url"],
                    Mintroduction = Request["content"],
                    Mcreatetime = date,
                    MbelongTo = Request["belongTo"],
                    Mcooperation = Request["cooperation"],
                    CID = Club.ID
                };
                if(mainInfoService.AddEntity(mainInfo))
                {
                    return Content("Ok");
                }
                return Content("Fail");
            }
            else
            {//协会信息存在
                mainInfo = new MainInfo()
                {
                    ID = mid,
                    Micon = string.IsNullOrEmpty(Request["url"]) ? iconDefaultUrl : Request["url"],
                    Mintroduction = Request["content"],
                    Mcreatetime = date,
                    MbelongTo = Request["belongTo"],
                    Mcooperation = Request["cooperation"],
                    CID = Club.ID
                };
                if (mainInfoService.EditEntity(mainInfo))
                {
                    return Content("Ok");
                }
                return Content("Fail");
            }
            
        }

        /*LeaderInfo*/
        IBLL.IClubLeaderService leaderService = new BLL.ClubLeaderService();
        public ActionResult EditLeaderInfo()
        {
            if (Club == null) return Content("Error");
            Model.ClubLeader leader = leaderService.GetLeaderByCid(Club.ID);
            if (leader != null)
            {
                ViewData["Lid"] = leader.ID;
                ViewData["Lname"] = leader.Lname;
                ViewData["Lmajor"] = leader.Lmajor;
                ViewData["Lcontact"] = leader.Lcontact;
                ViewData["Lregiste"] = leader.Lregiste;
                ViewData["Lsex"] = leader.Lsex;
                ViewData["Limage"] = leader.Limage;
            }
            else
            {
                ViewData["Lid"] = string.Empty;
                ViewData["Lname"] = string.Empty;
                ViewData["Lmajor"] = string.Empty;
                ViewData["Lcontact"] = string.Empty;
                ViewData["Lregiste"] = string.Empty;
                ViewData["Lsex"] = string.Empty;
                ViewData["Limage"] = string.Empty;
            }
            return View();
        }
        public ActionResult EditLeaderInfoSubmit()
        {
            Model.ClubLeader leader;
            DateTime date;
            try
            {
                date = Convert.ToDateTime(Request["registe"]);
            }
            catch (Exception ex)
            {
                date = DateTime.Now;
            }
            short sex = 1;
            short.TryParse(Request["sex"], out sex);
            int lid; 
            if(!int.TryParse(Request["id"], out lid))
            {//会长信息不存在，则为添加操作
                leader = new ClubLeader()
                {
                    Lname = Request["name"],
                    Lsex = sex,
                    Lmajor = Request["major"],
                    Lcontact = Request["contact"],
                    Lregiste = date,
                    Limage = string.IsNullOrEmpty(Request["url"]) ? leaderDefaultUrl : Request["url"],
                    CID = Club.ID
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
            else
            {  //会长信息存在， 则为更改操作
                leader = new ClubLeader()
                {
                    ID = lid,
                    Lname = Request["name"],
                    Lsex = sex,
                    Lmajor = Request["major"],
                    Lcontact = Request["contact"],
                    Lregiste = date,
                    Limage = string.IsNullOrEmpty(Request["url"]) ? leaderDefaultUrl : Request["url"],
                    CID = Club.ID
                };
                if (leaderService.EditEntity(leader))
                {
                    return Content("Ok");
                }
                else
                {
                    return Content("Fail");
                }
            }
        }             

        /*File ajax*/
        public ActionResult FileUploadAjax()
        {
            string fileType = Request["type"];
            if (string.IsNullOrEmpty(fileType)) fileType = "default";
            //默认为news图片的尺寸
            int width = 100;
            int height = 80;
            switch (fileType)
            {
                case "club": //社团图标尺寸
                    width = 100;
                    height = 100;
                    break;
                case "leader":  //会长照片尺寸
                    width = 150;
                    height = 200;
                    break;
                case "activities": //活动照片尺寸
                    width = 150;
                    height = 150;
                    break;
                case "display":
                    width = 300;
                    height = 310;
                    break;
            }

            HttpPostedFileBase file = Request.Files["imgfile"];
            if (file.ContentLength != 0)
            {
                //获取到文件先进行保存,作为临时图片文件
                string fileName = Path.GetFileName(file.FileName); //获取文件名:文件名+扩展名
                string fileExt = Path.GetExtension(fileName); //获取扩展名
                string tempDir = "/Content/images/temp/"; //临时文件存放的文件夹
                if (!Directory.Exists(Request.MapPath(tempDir)))  //判断文件夹是否存在
                {
                    Directory.CreateDirectory(Request.MapPath(tempDir));
                }
                string fileSource = Server.MapPath(tempDir) + fileName; //临时文件的原路径
                file.SaveAs(fileSource);

                //解决文件重名问题
                string newFileName = DateTime.Now.Hour+""+DateTime.Now.Minute+""+DateTime.Now.Millisecond;
                //将上传的文件保存到不同的文件夹下
                string dir = "/Content/images/" + Club.Cname + "/" + fileType + "/" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "/";
                if (!Directory.Exists(Request.MapPath(dir)))  //判断文件夹是否存在
                {
                    Directory.CreateDirectory(Request.MapPath(dir));
                }
                string url = dir + newFileName + fileExt;  //构建完整路径
                string fileTargetPath = Server.MapPath(url); //物理路径
                //使用工具类制作缩略图,并将图片保存至相应的路径中
                ImageClass.MakeThumbnail(fileSource, fileTargetPath, width, height, "HW");
                System.IO.File.Delete(fileSource);//将临时的图片删除
                return Content(url); //将图片路径返回至前段的data数据 
            }
            else
            {//没有选择图片，可在服务器端写入数据时，设置为默认图片的路径
                return Content("/Content/images/default/" + width + "_" + height + ".png");
            }
        }
       
    }
}