using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Rapid.Assessment.Core.Model
{
    public class AssessmentResult : AzureResource
    {
        public string Category { get; set; }
        public string Definition { get; set; }
        public DateTime RunDateTimeUtc { get; set; }
    }
}
