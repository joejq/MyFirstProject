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
    public class NewsController : ApiController
    {
        private CBEntities db = new CBEntities();

        // GET: api/News
        //public IQueryable<News> GetNews()
        //{
        //    return db.News;
        //}

        // GET: api/News/5
        [ResponseType(typeof(News))]
        public IHttpActionResult GetNews(int id)
        {
            var temp1 = db.News.Where(e => e.Delflag == 0 && e.CID == id);
            int count = temp1.Count() > 3 ? 3 : temp1.Count();
            if (count <= 0) return Ok("");
            var temp2 = temp1.OrderByDescending(e => e.Ntime).Skip(0).Take(count);
            var news = temp2.ToList();
            //var news = db.News.Where(e => e.Delflag == 0 && e.CID == id).ToList();
            //int count = news.Count();
            //if (news.Count <= 0) return Ok("");
            List<BotNews> jNews = new List<BotNews>();
            foreach(var i in news)
            {
                jNews.Add(new BotNews()
                {
                    Nid = i.ID,
                    Ntitle = i.Ntitle,
                    Ntime = (DateTime)i.Ntime,
                    Nimage = i.Nimage
                });
            }
            return Json(jNews, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }       
    }
    public class BotNews
    {
        public int Nid { get; set; }
        public string Ntitle { get; set; }
        public DateTime Ntime { get; set; }
        public string Nimage { get; set; }
    }
}