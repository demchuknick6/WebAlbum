using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebAlbum.Web.Models.Photos;

namespace WebAlbum.Web.Models.Albums
{
    public class AlbumViewModel
    {
        public int AlbumId { get; set; }

        [Required]
        [Display(Name = "Title")]
        [RegularExpression(@"^[a-zA-Z0-9]+$",
            ErrorMessage = "Characters are not allowed.")]
        [StringLength(20, MinimumLength = 2, 
            ErrorMessage = "Value must be between 2 to 20 characters long.")]
        public string AlbumTitle { get; set; }

        public bool Public { get; set; }

        [Display(Name = "Date Created")]
        public DateTime? DateCreated { get; set; }
        
        public string UserId { get; set; }

        public ICollection<PhotoViewModel> Photos { get; set; }
    }
}