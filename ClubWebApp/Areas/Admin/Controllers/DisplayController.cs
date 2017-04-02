using Common;
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
    public class DisplayController : Controller
    {
        //static int Cid;
        static string Cname;
        public ActionResult Index()
        {
            int id = Convert.ToInt32(Session["cid"]);
            IBLL.IClubsService cs = new BLL.ClubsService();
            var t = cs.LoadEntities(e => e.Delflag == 0 && e.ID == id).ToList();
            List<Display> displays = new List<Display>();
            if (t.Count>0)
            {
                Cname = t[0].Cname;
                //相对路径
                string tdir = "/Content/images/" + Cname + "/display/";
                //绝对路径
                string dir = Server.MapPath(tdir);
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    FileInfo[] files = dirInfo.GetFiles();
                    foreach (var f in files)
                    {
                        displays.Add(new Display { FilePath = "/Content/images/" + Cname + "/display/" + f.Name });
                    }
                }
                catch (Exception ex) { }//未获取到文件
            }
            var jDis = JsonConvert.SerializeObject(displays, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(jDis, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadDisplay()
        {
            string imgId = Request["imgid"];
            int width = 270;
            int height = 250;

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

                //将上传的文件保存到不同的文件夹下
                string dir = "/Content/images/" + Cname + "/display/";
                if (!Directory.Exists(Request.MapPath(dir)))  //判断文件夹是否存在
                {
                    Directory.CreateDirectory(Request.MapPath(dir));
                }
                string url = dir + imgId + fileExt;  //构建完整路径
                string fileTargetPath = Server.MapPath(url); //物理路径
                //存在同名文件则删除
                if (System.IO.File.Exists(fileTargetPath))
                {
                    System.IO.File.Delete(fileTargetPath);
                }
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