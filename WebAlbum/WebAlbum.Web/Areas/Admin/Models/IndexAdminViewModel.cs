using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebAlbum.Web.Models.Albums;

namespace WebAlbum.Web.Areas.Admin.Models
{
    public class IndexAdminViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Lockout End Date (Utc)")]
        public DateTime LockoutEndDateUtc { get; set; }

        public ICollection<AlbumViewModel> Albums { get; set; }

    }
}