using IBLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MainInfoService : BaseService<MainInfo>, IMainInfoService
    {
        public bool DeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            var entity = temp[0];
            return CurrentDal.DeleteEntity(entity);
        }

        public MainInfo GetInfoByCid(int cid)
        {
            var temp = CurrentDal.LoadEntities(e => e.Delflag == 0 && e.CID == cid).ToList();
            if(temp.Count <= 0) return null;
            var entity = temp[0];
            return entity;
        }

        public bool LogicDeleteEntity(MainInfo entity)
        {
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public bool LogicDeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            var entity = temp[0];
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public override void SetCurrentDal()
        {
            this.CurrentDal = new DAL.MainInfoDal();
        }
    }
}
