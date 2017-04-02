using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        IBLL.IBackManagerService bmService = new BLL.BackManagerService();
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Alter()
        {
            return View();
        }

        public ActionResult AlterPass()
        {
            string contact = Request["contact"].Trim();
            string pass = Request["pass"].Trim();
            if (string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(pass)) return Content("Fail");
            var temp = bmService.LoadEntities(e => e.Delflag == 0 && e.Bcontact == contact).ToList();
            if (temp.Count <= 0) return Content("Fail");
            Model.BackManager bm = temp[0];
            bm.Bpass = Md5Helper.EncryptString(pass);
            if (bmService.EditEntity(bm))
            {
                return Content("Ok");
            }
            else
            {
                return Content("Fail");
            }
        }

        public ActionResult Validate()
        {
            string username = Request["contact"].Trim();
            string tpass = Request["pass"].Trim();
            /*超级管理员入口  计算机协会会长*/
            string supername = ConfigurationManager.AppSettings["Supername"];
            string superpass = ConfigurationManager.AppSettings["Superpass"];
            if (username == supername && tpass == superpass)
            {
                Session["username"] = "super";
                Session["pass"] = "super";
                return Content("super");
            }
            //加密处理
            string pass = Md5Helper.EncryptString(tpass);            
            /*普通协会管理员入口*/
            var temp = bmService.LoadEntities(e => e.Delflag == 0 && e.Busername == username).ToList();
            if (temp.Count <= 0) return Content("Fail");
            var bm = temp[0];
            if (bm.Bpass == pass)
            {
                Session["username"] = username;
                Session["pass"] = pass;
                Session["cid"] = bm.CID;
                return Content("Ok");
            }
            else
            {
                return Content("Fail");
            }

        }

        public ActionResult GetClubIdByContact()
        {
            string contact = (Request["contact"]).Trim();
            var temp = bmService.LoadEntities(e => e.Delflag == 0 && e.Busername == contact).ToList();
            if (temp.Count > 0)
            {
                var bm = temp[0];
                return Content(bm.CID.ToString());
            }
            else
            {
                return Content("-1");
            }
        }
    }
}