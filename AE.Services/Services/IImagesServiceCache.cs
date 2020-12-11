using System.Collections.Generic;
using AE.Services.Dto;

namespace AE.Services.Services
{
    public interface IImagesServiceCache
    {
        void Set(IEnumerable<PictureDetail> pictures);

        IEnumerable<PictureDetail> Get();
        
        void Clear();
    }
}