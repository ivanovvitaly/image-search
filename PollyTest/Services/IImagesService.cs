using System.Threading.Tasks;

namespace PollyTest.Services
{
    public interface IImagesService
    {
        Task<PicturesResponse> GetImages(int? page = null);

        Task<PictureDetail> GetImage(string id);

        Task<string> CreateAccessToken();
    }
}