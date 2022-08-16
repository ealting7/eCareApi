using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class IcmsUser
    {
        public Guid UserId { get; set; }
        public int caseOwnerId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentName { get; set; }

        public string emailAddress { get; set; }

        public int? reviewTypeItemsId { get; set; }
        public bool? isDefault { get; set; }
        public bool? isDr { get; set; }
        public bool? isThirdParty { get; set; }

        public bool? reviewMd { get; set; }

        public int caseTypeId { get; set; }
        public string caseTypeCode { get; set; }
        public string caseTypeDescr { get; set; }
        public DateTime caseAssignedDate { get; set; }
        public DateTime? caseTerminationDate { get; set; }
    }
}
