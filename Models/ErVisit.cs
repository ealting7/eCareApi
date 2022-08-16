using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class ErVisit
    {
        public int erId { get; set; }
        public Guid? patientId { get; set; }
        public Patient? erPatient { get; set; }
        public DateTime? checkInDate { get; set; }
        public string displayCheckInDate { get; set; }
        public DateTime? checkOutDdate { get; set; }
        public string displayCheckOutDate { get; set; }

        public string visitReason { get; set; }
        public bool? ambulanceUsed { get; set; }

        public int? roomId { get; set; }
        public string roomName { get; set; }
        public string roomPrefix { get; set; }

        public int? erStatusId { get; set; }
        public string erStatus { get; set; }
    }
}
