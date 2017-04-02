using IBLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NewsService : BaseService<News>, INewsService
    {
        public bool DeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            var entity = temp[0];
            return CurrentDal.DeleteEntity(entity);
        }

        public bool LogicDeleteEntity(News entity)
        {
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public bool LogicDeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID && e.Delflag == 0).ToList();
            var entity = temp[0];
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public override void SetCurrentDal()
        {
            this.CurrentDal = new DAL.NewsDal();
        }

        public News GetLatestNews(int CID)
        {
            var temp1 = CurrentDal.LoadEntities(e => e.CID == CID && e.Delflag == 0);
            var temp2 = temp1.OrderByDescending(e => e.Subtime).First();
            return temp2;
        }

        public List<News> GetRecentNews(int CID)
        {
            var temp1 = CurrentDal.LoadEntities(e => e.CID == CID && e.Delflag == 0);
            var count = temp1.Count() > 3 ? 3 : temp1.Count();
            var temp2 = temp1.OrderByDescending(e => e.Subtime).Skip(0).Take(count);
            return temp2.ToList<News>();
        }

        public News GetNewsById(int nid)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == nid && e.Delflag == 0).ToList();
            return temp[0];
        }
    }
}
