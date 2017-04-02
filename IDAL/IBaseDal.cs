using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IBaseDal<T> where T:class, new()
    {
        //查询数据
        IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda);
        //分页方法
        IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, s>> orderByLambda, bool isAsc);
        //删除数据
        bool DeleteEntity(T entity);
        //修改数据
        bool EditEntity(T entity);
        //添加数据
        int AddEntity(T entity);
    }
}
