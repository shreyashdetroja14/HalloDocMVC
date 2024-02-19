namespace HalloDocMVC.ViewModels
{
    public class RequestFileViewModel
    {
        public int FileId { get; set; }
        public string FileName { get; set; } = null!;

        public string? Uploader { get; set; }

        public DateOnly UploadDate { get; set; }

        public string FilePath { get; set;} = null!;
    }
}
