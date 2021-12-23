using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class AdvancedDirective
    {
        public bool hasMedicalDeclarationStatements { get; set; }
        public bool DoNotResuscitatePatient { get; set; }
        public bool hasPowerOfAttorneyDocument { get; set; }
    }
}
