using System;

namespace WebAlbum.Web.Models.Photos
{
    public class PhotoViewModel
    {

        public int PhotoId { get; set; }
        public string PhotoTitle { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Content { get; set; }
        public string FileName { get; set; }

        public int AlbumId { get; set; }     
    }
}