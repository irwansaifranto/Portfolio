﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Business.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class UserManagement : DbContext
    {
        public UserManagement()
            : base("name=UserManagementEdmx")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Actions> Actions { get; set; }
        public virtual DbSet<Modules> Modules { get; set; }
        public virtual DbSet<ModulesInRoles> ModulesInRoles { get; set; }
        public virtual DbSet<roles> roles { get; set; }
        public virtual DbSet<user_login_activity> user_login_activity { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<versions> versions { get; set; }
    }
}
