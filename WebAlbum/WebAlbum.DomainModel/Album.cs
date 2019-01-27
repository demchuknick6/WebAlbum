using System;
using System.Collections.Generic;

namespace WebAlbum.DomainModel
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public bool Public { get; set; }
        public DateTime? DateCreated { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
    }
}
