using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bydd365_hsappservice.models
{
    internal class ByodConfig
    {
        public string DBServer { get; set; }
        public string DBName { get; set; }
        public List<string> TableListName { get; set; }
        public string DBUser { get; set; }
        public string DBPass { get; set; }
        public int Timeout { get; set; }
    }
}
