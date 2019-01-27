using System.Collections.Generic;

namespace WebAlbum.Web.Models.Albums
{
    public class AlbumListViewModel
    {
        public IEnumerable<AlbumViewModel> Albums { get; set; }
    }
}