using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class AdvancedDirective
    {
        public long advanceDirectiveId { get; set; }
        public bool hasMedicalDeclarationStatements { get; set; }
        public bool DoNotResuscitatePatient { get; set; }
        public bool hasPowerOfAttorneyDocument { get; set; }
    }
}
