using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebAlbum.DataAccess.Configurations;
using WebAlbum.DomainModel;

namespace WebAlbum.DataAccess
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext() :
            base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static DatabaseContext Create() 
        {
            return new DatabaseContext();
        }

        public IDbSet<Album> Albums { get; set; }
        public IDbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
            modelBuilder.Properties<string>().Configure(c => c.HasMaxLength(128));

            modelBuilder.Configurations.Add(new AlbumConfiguration());
            modelBuilder.Configurations.Add(new PhotoConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
