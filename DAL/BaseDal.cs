using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BaseDal<T> where T : class, new()
    {
        protected CBEntities Db = null;
        public BaseDal()
        {
            Db = CallContext.GetData("db") as CBEntities;
            if (Db==null)
            {
                Db = new CBEntities();
                CallContext.SetData("db", Db);
                Db.Configuration.ProxyCreationEnabled = false;
            }
        }

        public int AddEntity(T entity)
        {
            Db.Configuration.ValidateOnSaveEnabled = false;
            Db.Set<T>().Add(entity);
            int count = Db.SaveChanges();
            Db.Configuration.ValidateOnSaveEnabled = true;
            return count;
        }

        public bool DeleteEntity(T entity)
        {
            Db.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
            return Db.SaveChanges() > 0;
        }

        public bool EditEntity(T entity)
        {
            Db.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
            return Db.SaveChanges() > 0;
        }

        public IQueryable<T> LoadEntities(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda)
        {
            return Db.Set<T>().AsNoTracking().Where<T>(whereLambda);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="orderByLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, out int totalCount, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderByLambda, bool isAsc)
        {
            //var t = Db.Set<T>().Where<T>(u => true);
            var temp = Db.Set<T>().Where<T>(whereLambda);
            totalCount = temp.Count();
            if (isAsc) //升序
            {
                temp = temp.OrderBy<T, s>(orderByLambda).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }
            else
            {
                temp = temp.OrderByDescending<T, s>(orderByLambda).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }
            return temp;
        }
    }
}
