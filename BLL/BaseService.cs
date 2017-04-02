using IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public abstract class BaseService<T> where T : class, new()
    {
        //构造函数初始化数据操作对象
        public BaseService()
        {
            SetCurrentDal();
        }

        public IDAL.IBaseDal<T> CurrentDal { get; set; }

        /// <summary>
        /// 使用抽象类实现不同子类使用不同类型的数据操作对象
        /// </summary>
        public abstract void SetCurrentDal();

        public IQueryable<T> LoadEntities(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda)
        {
            return CurrentDal.LoadEntities(whereLambda);
        }

        public IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, out int totalCount, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderByLambda, bool isAsc)
        {
            return CurrentDal.LoadPageEntities<s>(pageIndex, pageSize, out totalCount, whereLambda, orderByLambda, isAsc);
        }

        public bool DeleteEntity(T entity)
        {
            return CurrentDal.DeleteEntity(entity);
        }


        public bool EditEntity(T entity)
        {
            return CurrentDal.EditEntity(entity);
        }

        public bool AddEntity(T entity)
        {
            return CurrentDal.AddEntity(entity) > 0;
        }
    }
}
