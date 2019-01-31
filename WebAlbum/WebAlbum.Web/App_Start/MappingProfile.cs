using System;
using System.Linq;
using AutoMapper;
using WebAlbum.DomainModel;
using WebAlbum.Web.Areas.Admin.Models;
using WebAlbum.Web.Models.Account;
using WebAlbum.Web.Models.Albums;
using WebAlbum.Web.Models.Photos;
using WebAlbum.Web.Areas.Users.Models;

namespace WebAlbum.Web.App_Start
{
    /// <inheritdoc />
    /// <summary>
    /// Auto mapper mapping profile.
    /// It's used to map properties from your domain models to your view models and vice versa.
    /// Using the same name for properties in both your domain and view models is highly recommended,
    /// as this will allow for convention-based mapping without any setup.
    /// Sometimes more complex configurations are needed, which can also be configured here.
    /// AutoMapper configuration wiki: https://github.com/AutoMapper/AutoMapper/wiki/Configuration
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, RegisterViewModel>().ReverseMap();
            CreateMap<ApplicationUser, IndexAdminViewModel>();
            CreateMap<ApplicationUser, UserViewModel>();

            CreateMap<Album, NewAlbumViewModel>().ReverseMap();
            CreateMap<Album, AlbumViewModel>().ReverseMap();
            CreateMap<Album, SearchAlbumViewModel>();

            CreateMap<Photo, NewPhotoViewModel>().ReverseMap();
            CreateMap<Photo, PhotoViewModel>()
                .ForMember(dest => dest.Content, o => o.MapFrom(
                    src => "data:image/png;base64," + 
                           Convert.ToBase64String(src.Content.ToArray())));
        }
    }
}
