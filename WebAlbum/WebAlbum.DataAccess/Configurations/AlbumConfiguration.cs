using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using WebAlbum.DomainModel;

namespace WebAlbum.DataAccess.Configurations
{
    class AlbumConfiguration : EntityTypeConfiguration<Album>
    {
        public AlbumConfiguration()
        {
            this.HasKey(s => s.AlbumId);

            this.Property(s => s.AlbumId)
                .IsRequired()
                .HasColumnName("Id");

            this.Property(s => s.AlbumTitle)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnName("Title");

            this.Property(s => s.UserId)
                .HasColumnName("OwnerId")
                .IsRequired();
        }
    }
}
