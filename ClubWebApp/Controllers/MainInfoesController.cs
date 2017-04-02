using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Model;
using Newtonsoft.Json;

namespace ClubWebApp.Controllers
{
    public class MainInfoesController : ApiController
    {
        private CBEntities db = new CBEntities();


        // GET: api/MainInfoes/5
        [ResponseType(typeof(MainInfo))]
        public IHttpActionResult GetMainInfo(int id)
        {
            try
            {
                MainInfo mainInfo = db.MainInfo.Where(e => e.Delflag == 0 && e.CID == id).First();
                BotMainInfo jMainInfo = new BotMainInfo()
                {
                    Mid = mainInfo.ID,
                    Micon = mainInfo.Micon,
                    Mintroduction = mainInfo.Mintroduction,
                    Mcreatetime = (DateTime)mainInfo.Mcreatetime,
                    MbelongTo = mainInfo.MbelongTo,
                    Mcooperation = mainInfo.Mcooperation
                };
                return Json(jMainInfo, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch(Exception ex) { }
            return Ok("");
        }
    }
    
    public class BotMainInfo
    {
        public int Mid { get; set; }
        public string Micon { get; set; }
        public string Mintroduction { get; set; }
        public DateTime Mcreatetime { get; set; }
        public string MbelongTo { get; set; }
        public string Mcooperation { get; set; }
    }
}