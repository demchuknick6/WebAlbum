using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebAlbum.Web.Models.Albums
{
    public class NewAlbumViewModel
    {
        public int AlbumId { get; set; }

        [Required]
        [Display(Name = "Title")]
        [RegularExpression(@"^[a-zA-Z0-9]+$",
            ErrorMessage = "Characters are not allowed.")]
        [Remote("CheckAlbumTitle", "Albums", 
            ErrorMessage = "Album already exists.")]
        [StringLength(20, MinimumLength = 2, 
            ErrorMessage = "Value must be between 2 to 20 characters long.")]
        public string AlbumTitle { get; set; }

        public bool Public { get; set; }

    }
}