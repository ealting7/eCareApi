using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class InpatientChart
    {

        public int chartId { get; set; }
        public string chartName { get; set; }
        public int? chartOrder { get; set; }
        public string chartTableName { get; set; }
        public int? chartTableId { get; set; }
        public Guid? chartTableGuid { get; set; }
        public string chartType { get; set; }
        public string rationale { get; set; }

        public int sourceId { get; set; }

        public Guid? patientId { get; set; }
        public int? admissionId { get; set; }
        public Guid? usr { get; set; }
        public DateTime dateMeasured { get; set; }

        public int hour { get; set; }
        public int minutes { get; set; }
        public string date { get; set; }

        public int timeInterval { get; set; }

        public string dateHour { get; set; }

        public List<InpatientChartSource> sources { get; set; }

        public List<InpatientChartSource> dailySources { get; set; }

        public Admission inpatientAdmission { get; set; }


        public DateTime? creationDate { get; set; }
        public Guid? creationUser { get; set; }
    }
}
