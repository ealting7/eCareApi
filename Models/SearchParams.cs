using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class SearchParams
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string stateAbbrev { get; set; }

        public string facilityName { get; set; }

        public Guid usr { get; set; }
    }
}
