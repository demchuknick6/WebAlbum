using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebAlbum.Web.Areas.Users.Models
{
    public class UserViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        public ICollection<SearchAlbumViewModel> Albums { get; set; }
    }
}