using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class MedicationService : IMedication
    {

        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspNetContext;
        private readonly FdbNddfContext _fdbContext;

        public MedicationService(IcmsContext icmsContext, AspNetContext aspNetContext, FdbNddfContext fdbContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
            _fdbContext = fdbContext ?? throw new ArgumentNullException(nameof(fdbContext));
        }

        public List<Medication> medicationSearch(Medication medSearch)
        {
            List<Medication> medicationsFound = null;

            medicationsFound = (
                                    from rndc14 in _fdbContext.rNdc14s

                                    join rGcnseq4 in _fdbContext.rGcnseq4s
                                    on rndc14.GCN_SEQNO equals rGcnseq4.GCN_SEQNO into gcnseq
                                    from rGcnseq4s in gcnseq.DefaultIfEmpty()

                                    join rdosed2 in _fdbContext.rDosed2s
                                    on rGcnseq4s.GCDF equals rdosed2.GCDF into dosed
                                    from rdosed2s in dosed.DefaultIfEmpty()

                                    join rroute3 in _fdbContext.rRouted3s
                                    on rGcnseq4s.GCRT equals rroute3.GCRT into route
                                    from rroute3s in route.DefaultIfEmpty()

                                    where rndc14.BN.Contains(medSearch.medicationName)
                                    orderby rndc14.LN
                                    select new Medication
                                    {
                                        ndc = rndc14.NDC,
                                        sequenceNumber = rndc14.GCN_SEQNO,
                                        medicationName = rndc14.LN,
                                        dose = rdosed2s.GCDF_DESC,
                                        route = rroute3s.GCRT_DESC
                                    }
                               )
                               .Distinct()
                               .ToList();

            List<Medication> returnMedicationSearchResults = null;

            if (medicationsFound != null && medicationsFound.Count < 1000)
            {
                returnMedicationSearchResults = medicationsFound.OrderBy(med => med.medicationName).ToList();
            }

            return returnMedicationSearchResults;
        }


        public List<Medication> updateAdmissionMedications(Medication med)
        {
            List<Medication> admissionMedications = null;

            DateTime creationDate = DateTime.MinValue;

            if (med.createDate.Equals(DateTime.MinValue))
            {
                creationDate = DateTime.Now;
            } 
            else
            {
                creationDate = (DateTime)med.createDate;
            }

            HospitalService hospServ = new HospitalService(_icmsContext, _aspNetContext);

            HospitalInpatientAdmissionOrder order = new HospitalInpatientAdmissionOrder();
            order.hospital_inpatient_admission_id = med.admissionId;
            order.hospital_order_type_id = med.orderTypeId;
            order.order_note = med.note;
            order.creation_date = creationDate;
            order.creation_user_id = (!med.usr.Equals(Guid.Empty)) ? med.usr : Guid.Empty;

            if (hospServ.CreateInpatientAdmissionOrder(order))
            {
                if (createInpatientAdmissionOrderMedication(order, med))
                {

                    admissionMedications = getInpatientAdmissionMedications(med.admissionId);

                    if (admissionMedications != null)
                    {
                        foreach(Medication admissionMed in admissionMedications)
                        {
                            if (!admissionMed.usr.Equals(Guid.Empty))
                            {
                                admissionMed.administeredByName = (
                                                                    from users in _aspNetContext.AspNetUsers
                                                                    where users.UserId.Equals(admissionMed.usr)
                                                                    select users.UserName
                                                                  )
                                                                  .FirstOrDefault();
                            }
                        }
                    }
                }
            }


            List<Medication> returnAdmissionMedications = null;

            if (admissionMedications != null)
            {
                returnAdmissionMedications = admissionMedications.OrderBy(med => med.dateGive).ThenBy(med => med.createDate).ToList();
            }


            return returnAdmissionMedications;
        }


        public List<Medication> reOrderAdmissionMedications(Medication med)
        {
            List<Medication> admissionMedications = null;

            HospitalInpatientAdmissionOrderMedication dbReorderAdmitMed = null;

            dbReorderAdmitMed = (
                    from hospMed in _icmsContext.HospitalInpatientAdmissionOrderMedications
                    where hospMed.hospital_inpatient_admission_order_medication_id.Equals(med.admissionMedicationOrderId)
                    select hospMed
                )
                .FirstOrDefault();

            if (dbReorderAdmitMed != null)
            {

                med.ndc = dbReorderAdmitMed.ndc;
                med.sequenceNumber = dbReorderAdmitMed.gcn_seqno;

                DateTime creationDate = DateTime.MinValue;

                if (med.createDate.Equals(DateTime.MinValue))
                {
                    creationDate = DateTime.Now;
                }
                else
                {
                    creationDate = (DateTime)med.createDate;
                }

                HospitalService hospServ = new HospitalService(_icmsContext, _aspNetContext);

                HospitalInpatientAdmissionOrder order = new HospitalInpatientAdmissionOrder();
                order.hospital_inpatient_admission_id = med.admissionId;
                order.hospital_order_type_id = med.orderTypeId;
                order.order_note = med.note;
                order.creation_date = creationDate;
                order.creation_user_id = (!med.usr.Equals(Guid.Empty)) ? med.usr : Guid.Empty;

                if (hospServ.CreateInpatientAdmissionOrder(order))
                {
                    if (createInpatientAdmissionOrderMedication(order, med))
                    {

                        admissionMedications = getInpatientAdmissionMedications(med.admissionId);

                        if (admissionMedications != null)
                        {
                            foreach (Medication admissionMed in admissionMedications)
                            {
                                if (!admissionMed.usr.Equals(Guid.Empty))
                                {
                                    admissionMed.administeredByName = (
                                                                        from users in _aspNetContext.AspNetUsers
                                                                        where users.UserId.Equals(admissionMed.usr)
                                                                        select users.UserName
                                                                      )
                                                                      .FirstOrDefault();
                                }
                            }
                        }
                    }
                }
            }

            
            List<Medication> returnAdmissionMedications = null;

            if (admissionMedications != null)
            {
                returnAdmissionMedications = admissionMedications.OrderBy(med => med.dateGive).ThenBy(med => med.createDate).ToList();
            }


            return returnAdmissionMedications;
        }


        public List<Medication> removeAdmissionMedications(Medication med)
        {
            List<Medication> returnAdmissionMedications = null;

            List<Medication> admissionMedications = null;

            if (med.admissionId > 0 && med.admissionMedicationOrderId > 0)
            {

                HospitalInpatientAdmissionOrderMedication dbAdmitMed = null;

                dbAdmitMed = (
                        from hospadmitMeds in _icmsContext.HospitalInpatientAdmissionOrderMedications
                        where hospadmitMeds.hospital_inpatient_admission_order_medication_id.Equals(med.admissionMedicationOrderId)
                        select hospadmitMeds
                    )
                    .FirstOrDefault();


                if (dbAdmitMed != null)
                {
                    if (markInpatientAdmissionMedicationNotUsed(med, dbAdmitMed))
                    {
                        markInpatientAdmissionMedicationOrderDeleted(med, dbAdmitMed);

                        admissionMedications = getInpatientAdmissionMedications(med.admissionId);
                    }
                }


                if (admissionMedications != null)
                {
                    returnAdmissionMedications = admissionMedications.OrderBy(med => med.dateGive).ThenBy(med => med.createDate).ToList();
                }
            }


            return returnAdmissionMedications;
        }




        private bool markInpatientAdmissionMedicationNotUsed(Medication med, HospitalInpatientAdmissionOrderMedication admitMed)
        {

            DateTime removeDate = DateTime.MinValue;

            if (med.updateDate.Equals(DateTime.MinValue))
            {
                removeDate = DateTime.Now;
            }
            else
            {
                removeDate = (DateTime)med.updateDate;
            }

            admitMed.deleted_flag = true;
            admitMed.deleted_date = removeDate;
            admitMed.deleted_user_id = med.usr;

            _icmsContext.HospitalInpatientAdmissionOrderMedications.Update(admitMed);

            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                return true;
            }

            return false;

        }

        private bool markInpatientAdmissionMedicationOrderDeleted(Medication med, HospitalInpatientAdmissionOrderMedication admitMed)
        {
            if (admitMed.hospital_inpatient_admission_order_id > 0)
            {

                int orderId = admitMed.hospital_inpatient_admission_order_id;

                if (orderId > 0)
                {

                    HospitalInpatientAdmissionOrder ordr = (
                                                        from hospOrdrs in _icmsContext.HospitalInpatientAdmissionOrders
                                                        where hospOrdrs.hospital_inpatient_admission_order_id.Equals(orderId)
                                                        select hospOrdrs
                                                    )
                                                    .FirstOrDefault();

                    if (ordr != null)
                    {
                        ordr.deleted_flag = true;
                        ordr.deleted_date = med.updateDate;
                        ordr.deleted_user_id = med.usr;

                        _icmsContext.HospitalInpatientAdmissionOrders.Update(ordr);

                        int intResult = _icmsContext.SaveChanges();

                        if (intResult > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private List<Medication> getInpatientAdmissionMedications(int admitId)
        {
            List<Medication> admissionMedications = null;

            HospitalService hospServ = new HospitalService(_icmsContext, _aspNetContext);

            Admission admit = hospServ.getAdmissionMedications(admitId.ToString());

            if (admit != null && admit.medication.Count > 0)
            {
                admissionMedications = new List<Medication>();
                admissionMedications = admit.medication;
            }

            return admissionMedications;
        }


        public bool createInpatientAdmissionOrderMedication(HospitalInpatientAdmissionOrder order, Medication med)
        {

            if (order.hospital_inpatient_admission_order_id > 0 && !string.IsNullOrEmpty(med.ndc) && med.sequenceNumber > 0)
            {

                Medication addMed = (
                                        from rndc14 in _fdbContext.rNdc14s

                                        join rGcnseq4 in _fdbContext.rGcnseq4s
                                        on rndc14.GCN_SEQNO equals rGcnseq4.GCN_SEQNO into gcnseq
                                        from rGcnseq4s in gcnseq.DefaultIfEmpty()

                                        join rdosed2 in _fdbContext.rDosed2s
                                        on rGcnseq4s.GCDF equals rdosed2.GCDF into dosed
                                        from rdosed2s in dosed.DefaultIfEmpty()

                                        join rroute3 in _fdbContext.rRouted3s
                                        on rGcnseq4s.GCRT equals rroute3.GCRT into route
                                        from rroute3s in route.DefaultIfEmpty()

                                        where rndc14.NDC.Equals(med.ndc)
                                        && rndc14.GCN_SEQNO.Equals(med.sequenceNumber)
                                        orderby rndc14.LN
                                        select new Medication
                                        {
                                            ndc = rndc14.NDC,
                                            sequenceNumber = rndc14.GCN_SEQNO,
                                            medicationName = rndc14.LN,
                                            dose = rdosed2s.GCDF_DESC,
                                            route = rroute3s.GCRT_DESC
                                        }
                                  )
                                  .FirstOrDefault();

                if (addMed != null)
                {
                    string timeAdministered = med.createDate.Value.ToShortTimeString();

                    HospitalInpatientAdmissionOrderMedication medicationOrder = new HospitalInpatientAdmissionOrderMedication();

                    medicationOrder.hospital_inpatient_admission_order_id = order.hospital_inpatient_admission_order_id;
                    medicationOrder.medication_name = addMed.medicationName;
                    medicationOrder.ndc = addMed.ndc;
                    medicationOrder.gcn_seqno = addMed.sequenceNumber;
                    medicationOrder.dose = addMed.dose;
                    medicationOrder.administered_date = (!med.createDate.Equals(DateTime.MinValue)) ? med.createDate : DateTime.Now;
                    medicationOrder.time_of_administration = (!string.IsNullOrEmpty(timeAdministered)) ? timeAdministered : DateTime.Now.ToShortTimeString();
                    medicationOrder.administered_user_id = med.usr;
                    medicationOrder.creation_date = (!med.createDate.Equals(DateTime.MinValue)) ? med.createDate : DateTime.Now;
                    medicationOrder.creation_user_id = med.usr;

                    _icmsContext.HospitalInpatientAdmissionOrderMedications.Add(medicationOrder);

                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
