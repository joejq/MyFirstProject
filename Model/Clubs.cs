﻿//------------------------------------------------------------------------------
// <auto-generated>
//     姝や唬鐮佸凡浠庢ā鏉跨敓鎴愩€?
//
//     鎵嬪姩鏇存敼姝ゆ枃浠跺彲鑳藉鑷村簲鐢ㄧ▼搴忓嚭鐜版剰澶栫殑琛屼负銆?
//     濡傛灉閲嶆柊鐢熸垚浠ｇ爜锛屽皢瑕嗙洊瀵规鏂囦欢鐨勬墜鍔ㄦ洿鏀广€?
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Clubs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clubs()
        {
            this.Activities = new HashSet<Activities>();
            this.BackManager = new HashSet<BackManager>();
            this.ClubLeader = new HashSet<ClubLeader>();
            this.MainInfo = new HashSet<MainInfo>();
            this.News = new HashSet<News>();
            this.Notice = new HashSet<Notice>();

            Subtime = DateTime.Now;
            Delflag = 0;
        }
    
        public int ID { get; set; }
        public string Cname { get; set; }
        public Nullable<System.DateTime> Subtime { get; set; }
        public Nullable<short> Delflag { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Activities> Activities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BackManager> BackManager { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClubLeader> ClubLeader { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MainInfo> MainInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<News> News { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notice> Notice { get; set; }
    }
}