﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrioHelper
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CLASSEUREntities1 : DbContext
    {
        public CLASSEUREntities1()
            : base("name=CLASSEUREntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<SUJET> SUJET { get; set; }
        public virtual DbSet<SUJETARC> SUJETARC { get; set; }
        public virtual DbSet<UTILISATEUR> UTILISATEUR { get; set; }
    }
}