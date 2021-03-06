﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventerAPI
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class eventerEntities : DbContext
    {
        public eventerEntities()
            : base("name=eventerEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<user_friends_heap> user_friends_heap { get; set; }
        public virtual DbSet<user_friends> user_friends { get; set; }
        public virtual DbSet<notification> notifications { get; set; }
        public virtual DbSet<admin> admins { get; set; }
        public virtual DbSet<video_event> video_event { get; set; }
        public virtual DbSet<user> users { get; set; }
    
        public virtual int add_user_friends(Nullable<long> user_id)
        {
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("add_user_friends", user_idParameter);
        }
    }
}
