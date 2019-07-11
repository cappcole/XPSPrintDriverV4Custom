using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EclaimsPrint
{
    public class UploadParameters
    {
        public string id { get; set; }
        public string name { get; set; }
        public int pages { get; set; }
        public bool isOptionsSet { get; set; }
        public string optionsLink { get; set; }
    }
}
