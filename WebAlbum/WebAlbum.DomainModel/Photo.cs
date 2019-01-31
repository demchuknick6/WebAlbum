using System;

namespace WebAlbum.DomainModel
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public string PhotoTitle { get; set; }
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
    }
}
