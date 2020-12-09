namespace PollyTest.Dto
{
    public class PagedPictures
    {
        public Picture[] Pictures { get; set; }
        
        public int Page { get; set; }
        
        public int PageCount { get; set; }

        public bool HasMorePictures { get; set; }
    }
}