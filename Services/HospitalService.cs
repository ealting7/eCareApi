using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class HospitalService : IHospital
    {
        private readonly IcmsContext _icmsContext;

        public HospitalService(IcmsContext icmsContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
        }

        public IEnumerable<Hospital> GetCollectionFacilities()
        {
            IEnumerable<Hospital> facilities = Enumerable.Empty<Hospital>();

            facilities = (from hosp in _icmsContext.Hospitals
                            where hosp.specimen_collection_equipped.Equals(true)
                            && (hosp.deleted.Equals(0) || hosp.deleted == null)
                            orderby hosp.name
                            select hosp)
                           .Take(50)
                           .ToList();

            return facilities;
        }

        public IEnumerable<HospitalFacility> GetLaboratoryFacilities()
        {
            IEnumerable<HospitalFacility> facilities = Enumerable.Empty<HospitalFacility>();

            facilities = (from hosp in _icmsContext.Hospitals
                          join hospSpec in _icmsContext.HospitalSpecialtys
                          on hosp.hospital_specialty_id equals hospSpec.hospital_specialty_id
                          where hospSpec.hospital_specialty_id.Equals(2) //Laboratory
                          && (hosp.deleted.Equals(0) || hosp.deleted == null)
                          orderby hosp.name
                          select new HospitalFacility{
                              hospitalId = hosp.hospital_id,
                              hospitalName = hosp.name,
                              hospitalSpecialty = hospSpec.specialty_descr,
                              address1 = hosp.address1,
                              address2 = hosp.address2,
                              city = hosp.city,
                              zip = hosp.zip,
                              stateAbbrev = hosp.state_abbrev
                          })
                           .Take(50)
                           .ToList();

            return facilities;
        }


        public Hospital GetHospitalUsingId(int id)
        {
            Hospital facility = new Hospital();

            facility = (from hosp in _icmsContext.Hospitals
                        where hosp.hospital_id.Equals(id)
                        select hosp).FirstOrDefault();                        

            return facility;
        }


        public bool CreateInpatientAdmission(HospitalInpatientAdmission admit)
        {
            bool admitCreated = false;


            if (!admit.member_id.Equals(Guid.Empty))
            {
                if (!string.IsNullOrEmpty(admit.registration_number))
                {
                    admit.registration_number = generateRegistrationNumber();
                }


                _icmsContext.HospitalInpatientAdmissions.Add(admit);

                admitCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }


            return admitCreated;
        }

        public bool CreateInpatientAdmissionOrder(HospitalInpatientAdmissionOrder order)
        {
            bool orderCreated = false;


            if (order.hospital_inpatient_admission_id > 0)
            {
                _icmsContext.HospitalInpatientAdmissionOrders.Add(order);

                orderCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }


            return orderCreated;
        }

        public bool CreateInpatientAdmissionOrderLab(HospitalInpatientAdmissionOrderLab labOrder)
        {
            bool labCreated = false;


            if (labOrder.hospital_inpatient_admission_order_id > 0)
            {
                _icmsContext.HospitalInpatientAdmissionOrderLabs.Add(labOrder);

                labCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }


            return labCreated;
        }

        public bool CreateInpatientAdmissionOrderDiagnosis(List<HospitalInpatientAdmissionOrderDiagnosis> diags)
        {
            bool diagsCreated = false;


            foreach (HospitalInpatientAdmissionOrderDiagnosis icd10 in diags)
            {
                bool added = false;


                if (icd10.hospital_inpatient_admission_id > 0 &&
                    icd10.hospital_inpatient_admission_order_id > 0 &&
                    icd10.diagnosis_codes_10_id > 0)
                {
                    _icmsContext.HospitalInpatientAdmissionDiagnoses.Add(icd10);

                    added = (_icmsContext.SaveChanges() > 0) ? true : false;
                }


                if (added && !diagsCreated)
                {
                    diagsCreated = true;
                }
            }


            return diagsCreated;
        }

        public bool CreateHospitalAppointmentSchedule(HospitalAppointmentSchedule appt)
        {
            bool apptCreated = false;

            if (!appt.member_id.Equals(Guid.Empty))
            {
                _icmsContext.HospitalAppointmentSchedules.Add(appt);

                apptCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }


            return apptCreated;
        }



        private string generateRegistrationNumber()
        {
            string registrationNumber = "";

            NextAdmissionId nextAdmissionId = (from admitId in _icmsContext.NextAdmissionIds
                                                 select admitId).FirstOrDefault();

            if (nextAdmissionId.next_admission_id > 0)
            {
                int nextAdmitId = (int)nextAdmissionId.next_admission_id;

                nextAdmissionId.next_admission_id = nextAdmitId + 1;

                _icmsContext.NextAdmissionIds.Attach(nextAdmissionId);
                _icmsContext.Entry(nextAdmissionId).State = EntityState.Modified;
                int updated = _icmsContext.SaveChanges();


                if (updated > 0)
                {
                    registrationNumber = "ADM" + nextAdmitId.ToString();
                }
            }


            return registrationNumber;
        }
    }
}
