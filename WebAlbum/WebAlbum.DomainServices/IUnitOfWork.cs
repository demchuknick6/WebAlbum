using System;
using System.Threading.Tasks;

namespace WebAlbum.DomainServices
{
    public interface IUnitOfWork : IDisposable
    {
        int Save();
        Task<int> SaveAsync();
    }
}
