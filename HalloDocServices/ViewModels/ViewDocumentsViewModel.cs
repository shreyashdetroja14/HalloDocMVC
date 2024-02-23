﻿using Microsoft.AspNetCore.Http;

namespace HalloDocServices.ViewModels
{
    public class ViewDocumentsViewModel
    {
        public int RequestId { get; set; }

        public IEnumerable<IFormFile>? MultipleFiles { get; set; }

        public List<RequestFileViewModel> FileInfo { get; set; } = null!;
    }
}