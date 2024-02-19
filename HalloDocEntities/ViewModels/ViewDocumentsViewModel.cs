using Microsoft.AspNetCore.Http;

namespace HalloDocEntities.ViewModels
{
    public class ViewDocumentsViewModel
    {
        public int RequestId { get; set; }

        public IEnumerable<IFormFile>? MultipleFiles { get; set; }

        public List<RequestFileViewModel> FileInfo { get; set; } = null!;
    }
}
