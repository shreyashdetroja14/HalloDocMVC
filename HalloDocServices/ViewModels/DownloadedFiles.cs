﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels
{
    public class DownloadedFile
    {
        public int RequestId { get; set; }

        public int FileId { get; set; }

        public byte[] Data { get; set; } = new byte[0];

        public string Filename { get; set; } = null!;
    }
}
