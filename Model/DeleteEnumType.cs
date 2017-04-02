using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public enum DeleteEnumType
    {
        /// <summary>
        /// 正常存在的数据
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 杯逻辑删除的数据
        /// </summary>
        LogicDelete = 1
    }
}
