using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IIcu
    {

        public Icu getIcuFormItems();
        public Appointment getHospitalIcuWorkDay(Appointment icu);
        public List<Admission> loadIcuRoomAdmissions(Admission icu);

        public List<Patient> searchPreopPatients(Appointment preop);

    }
}
