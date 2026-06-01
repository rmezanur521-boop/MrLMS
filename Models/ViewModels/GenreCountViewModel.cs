namespace MrLMS.Models.ViewModels
{
    public class GenreCountViewModel
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }
}