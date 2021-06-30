using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class AppointmentType
    {
        public int appointmentTypeId { get; set; }
        public string appointmentTypeName { get; set; }
        public int? duration { get; set; }
    }
}
