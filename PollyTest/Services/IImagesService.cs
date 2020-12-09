using System.Threading.Tasks;
using PollyTest.Dto;

namespace PollyTest.Services
{
    public interface IImagesService
    {
        Task<PagedPictures> GetImages(int? page = null);

        Task<PictureDetail> GetImage(string id);

        Task<string> CreateAccessToken();
    }
}