using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Room
    {
        public int hospitalId { get; set; }
        public string hospitalName { get; set; }
        public int departmentRoomsId { get; set; }
        public int? departmentId { get; set; }
        public string roomName { get; set; }
        public int? roomOccupancy { get; set; }
        public string roomDescription { get; set; }
        public bool? roomAvailable { get; set; }
        public List<Patient> patientsInRoom { get; set; }
    }
}
