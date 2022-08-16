using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class MedicationAdministered
    {

        public int administeredId { get; set; }

        public int admissionMedicationOrderId { get; set; }

        public string medicationName { get; set; }

        public int? adminsteredRouteId { get; set; }

        public DateTime administeredDate { get; set; }
        public string displayAdministeredDate { get; set; }

        public Guid administeredById { get; set; }
        public string administeredByName { get; set; }

        public int? routeOfAdministrationId { get; set; }
        public int? dosageFormsId { get; set; }

        public DateTime creationDate { get; set; }
        public Guid? creationUserId { get; set; }

        public bool? deleted { get; set; }
        public DateTime? deletedDate { get; set; }

    }
}
