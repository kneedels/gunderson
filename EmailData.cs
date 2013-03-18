using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunderson
{
    public class EmailData
    {
        public string From { get; set; }

        public string Subject { get; set; }

        public DateTime Time { get; set; }

        public string ContentSummary { get; set; }
    }
}
