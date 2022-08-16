using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Doctor
    {
        public Guid pcpId { get; set; }

        public string? fullName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? practiceName { get; set; }
        public string? npi { get; set; }
        public string? emailAddress { get; set; }

        public int? specialtyId { get; set; }
        public string? specialtyDesc { get; set; }

        public int providerAddressId { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string street { get; set; }
        public string? city { get; set; }
        public string? stateAbbrev { get; set; }
        public string? zip { get; set; }


        public int providerPhoneId { get; set; }
        public int providerContatctId { get; set; }
        public string? phoneNumber { get; set; }


        public Guid usr { get; set; }
        public DateTime creationDate { get; set; }

    }
}
