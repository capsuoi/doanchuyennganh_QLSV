//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLSinhVien
{
    using System;
    using System.Collections.Generic;
    
    public partial class hocsinh_hoatdong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public hocsinh_hoatdong()
        {
            this.diemdanhs = new HashSet<diemdanh>();
        }
    
        public int mahs_hd { get; set; }
        public int mahocsinh { get; set; }
        public int mahoatdong { get; set; }
        public System.DateTime ngaydangky { get; set; }
        public bool dathamgia { get; set; }
        public Nullable<System.DateTime> ngaythamgia { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<diemdanh> diemdanhs { get; set; }
        public virtual hoatdong hoatdong { get; set; }
        public virtual nguoidung nguoidung { get; set; }
    }
}