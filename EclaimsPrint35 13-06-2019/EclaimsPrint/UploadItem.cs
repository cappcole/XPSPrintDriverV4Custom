using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EclaimsPrint
{
    public class UploadItem
    {
        public string FilePath { get; set; }
        public int Pages { get; set; }
        public string SubNumber { get; set; }
        public string Status { get; set; }
        public bool reUpload { get; set; }

        public UploadItem()
        {
            Pages = 0;

            try
            {
                Status = UploadStatus.Waiting;
            }
            catch (Exception)
            {
                Status = UploadStatus.Waiting;
            }

        }
    }



    public static class UploadStatus
    {
        public const string Waiting = "Waiting";
        public const string Uploading = "Uploading";
        public const string Uploaded = "Uploaded";
        public const string Failed = "Failed";
    }


}
