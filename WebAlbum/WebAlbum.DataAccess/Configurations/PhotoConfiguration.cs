using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using WebAlbum.DomainModel;

namespace WebAlbum.DataAccess.Configurations
{
    class PhotoConfiguration : EntityTypeConfiguration<Photo>
    {
        public PhotoConfiguration()
        {
            this.HasKey(s => s.PhotoId);

            this.Property(s => s.PhotoId)
                .IsRequired()
                .HasColumnName("Id");

            this.Property(s => s.PhotoTitle)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnName("Title");
        }
    }
}