using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Clinica.Models
{
    public class ClinicaContext : DbContext
    {
        public ClinicaContext() : base("name = ClinicaContext")
        {
        }

        //la riga sopra è per la connection string

        public virtual DbSet<Utente> Utenti { get; set; }
        public virtual DbSet<Animale> Animali { get; set; }
        public virtual DbSet<Visita> Visite { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
            modelBuilder.Entity<Animale>().ToTable("Animali"); // Usa il nome giusto della tabella nel database
            modelBuilder.Entity<Utente>().ToTable("Utenti"); // Usa il nome giusto della tabella nel database
            modelBuilder.Entity<Visita>().ToTable("Visite"); // Usa il nome giusto della tabella nel database

            // Configura la relazione uno-a-molti tra Animale e Visita
            modelBuilder.Entity<Animale>()
                        .HasMany(a => a.Visite)
                        .WithRequired(v => v.Animale)
                        .HasForeignKey(v => v.AnimaleID);

        }
    }


}
