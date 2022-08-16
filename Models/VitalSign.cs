using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class VitalSign
    {
        public int vitalSignId { get; set; }

        public DateTime dateTaken { get; set; }
        public string displayDateTaken { get; set; }


        public decimal? temperature { get; set; }
        public bool isFarenheit { get; set; }
        public bool isRectal { get; set; }
        public bool? alertHighTemperature { get; set; }
        public bool? alertLowTemperature { get; set; }

        public int? temperatureSiteId { get; set; }

        public int? pulseRate { get; set; }
        public bool? alertHighPulseRate { get; set; }
        public bool? alertLowPulseRate{ get; set; }

        public int? respirationRate { get; set; }
        public bool? alertHighRespirationRate { get; set; }
        public bool? alertLowRespirationRate { get; set; }

        public int? systolicBloodPressure { get; set; }
        public int? diastolicBloodPressure { get; set; }
        public bool? alertHighBloodPressure { get; set; }
        public bool? alertLowBloodPressure { get; set; }

    }
}
