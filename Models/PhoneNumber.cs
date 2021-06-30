using eCareApi.Entities;
using System;
using System.Collections.Generic;

namespace eCareApi.Models
{
    public class PhoneNumber
    {
        public Guid? PatientId { get; set; }

        public string? Number { get; set; }

        public string Type { get; set; }
    }
}
