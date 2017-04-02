using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IBaseService<T> where T : class, new()
    {
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <returns></returns>
        IQueryable<T> LoadEntities(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalCount">所有的记录</param>
        /// <param name="whereLambda">where条件</param>
        /// <param name="orderByLambda">排序依据</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, out int totalCount, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderByLambda, bool isAsc);
        /// <summary>
        /// 真正从数据库中删除数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool DeleteEntity(T entity);
        /// <summary>
        /// 逻辑删除，不从数据库中删除数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool LogicDeleteEntity(T entity);
        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        bool DeleteEntity(int ID);
        /// <summary>
        /// 根据ID逻辑删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        bool LogicDeleteEntity(int ID);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">需要修改的实体</param>
        /// <returns></returns>
        bool EditEntity(T entity);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">需要添加的实体</param>
        /// <returns></returns>
        bool AddEntity(T entity);

    }
}
