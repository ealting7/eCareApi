using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Specimen
    {
        public string specimenTypeName { get; set; }
        public int specimenTypeId { get; set; }

        public string specimenVolumeName { get; set; }
        public int specimenVolumeId { get; set; }
    }
}
