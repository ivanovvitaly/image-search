namespace AE.Services.Dto
{
    public class PagedPictures
    {
        public Picture[] Pictures { get; set; }
        
        public int Page { get; set; }
        
        public int PageCount { get; set; }

        public bool HasMore { get; set; }
    }
}