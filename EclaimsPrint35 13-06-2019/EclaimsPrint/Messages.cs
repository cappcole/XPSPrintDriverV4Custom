using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EclaimsPrint
{
    class Messages
    {
        public const string FileType = "Allowed file type: xps";
        public const string FileLocked = "File is protected";
        public const string FileInvalid = "File is corrupted";
        public const string FileSize = "Max supported file size: 50MB.";
        public const string FilePages = "Max supported pages: 100";
        public const string NoInternet = "There is no internet connection";
        public const string ConnectionFailure = "We cannot reach the server at this time";
        public const string Error = "Error";

        public const string connectionString = "Connection Failed. Check Your Connection. Our Systems May Be Down. Contact" +
                                    "Contact IAPlus If The Issue Persist ";
        public const string invalidCred =
            "Please Check The Login Information You Have Entered And Try Again.  Contact IAPlus If The Issue Persist.";
        public const string noFile = "file didn't read";
    }


}
