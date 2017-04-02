using IBLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BackManagerService : BaseService<BackManager>, IBackManagerService
    {
        public bool DeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            if(temp.Count <= 0) return true;
            var entity = temp[0];
            return CurrentDal.DeleteEntity(entity);
        }

        public bool LogicDeleteByCid(int cid)
        {
            var temp = CurrentDal.LoadEntities(e => e.CID == cid && e.Delflag == 0).ToList();
            if (temp.Count <= 0) return true;
            var entity = temp[0];
            return LogicDeleteEntity(entity);
        }

        public bool LogicDeleteEntity(BackManager entity)
        {
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public bool LogicDeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            if(temp.Count <= 0) return true;
            var entity = temp[0];
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public override void SetCurrentDal()
        {
            this.CurrentDal = new DAL.BackManagerDal();
        }
    }
}
