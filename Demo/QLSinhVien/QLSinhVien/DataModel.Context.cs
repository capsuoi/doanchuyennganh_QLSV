﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Ql_SinhVienEntities : DbContext
    {
        public Ql_SinhVienEntities()
            : base("name=Ql_SinhVienEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<diemdanh> diemdanhs { get; set; }
        public virtual DbSet<hoatdong> hoatdongs { get; set; }
        public virtual DbSet<hocsinh_hoatdong> hocsinh_hoatdong { get; set; }
        public virtual DbSet<nguoidung> nguoidungs { get; set; }
    }
}