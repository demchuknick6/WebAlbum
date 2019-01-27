using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebAlbum.Web.Models.Photos;

namespace WebAlbum.Web.Areas.Users.Models
{
    public class PhotoListViewModel
    {
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Album Title")]
        public string AlbumTitle { get; set; }

        public IEnumerable<PhotoViewModel> Photos { get; set; }
    }
}