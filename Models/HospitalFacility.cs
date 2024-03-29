﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class HospitalFacility
    {
        public int? hospitalId { get; set; }
        public string? hospitalName { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string? city { get; set; }
        public string? stateAbbrev { get; set; }
        public string? zip { get; set; }
        public string? hospitalSpecialty { get; set; }
        public string npi { get; set; }

        public string referralNumber { get; set; }
        public Guid memberId { get; set; }

        public Guid usr { get; set; }
        public DateTime creationDate { get; set; }
    }
}
