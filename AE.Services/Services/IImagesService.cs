using System.Threading.Tasks;
using AE.Services.Dto;

namespace AE.Services.Services
{
    public interface IImagesService
    {
        Task<PagedPictures> GetImages(int? page = null);

        Task<PictureDetail> GetImage(string id);

        PictureDetail[] Search(string term);
    }
}