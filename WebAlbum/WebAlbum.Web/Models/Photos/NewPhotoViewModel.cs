using System.ComponentModel.DataAnnotations;

namespace WebAlbum.Web.Models.Photos
{
    public class NewPhotoViewModel
    {
        public int PhotoId { get; set; }

        [Required]
        [Display(Name = "Title")]
        [RegularExpression(@"^[a-zA-Z0-9]+$",
            ErrorMessage = "Characters are not allowed.")]
        [StringLength(20, MinimumLength = 2,
            ErrorMessage = "Value must be between 2 to 20 characters long.")]
        public string PhotoTitle { get; set; }

        public int AlbumId { get; set; }
    }
}