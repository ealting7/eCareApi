using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;


using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;
using Syncfusion.XlsIO;

namespace eCareApi.Services
{
    public class BillingService : IBilling
    {
        private readonly IcmsContext _icmsContext;
        private readonly IcmsDataStagingContext _icmsDataStagingContext;
        private readonly AspNetContext _aspnetContext;
        private readonly IConverter _converter;

        public BillingService(IcmsContext icmsContext, IcmsDataStagingContext icmsDataStagingContext, AspNetContext aspnetContext, IConverter converter)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _icmsDataStagingContext = icmsDataStagingContext ?? throw new ArgumentNullException(nameof(icmsDataStagingContext));
            _aspnetContext = aspnetContext ?? throw new ArgumentNullException(nameof(aspnetContext));
            _converter = converter;
        }


        public IEnumerable<Bill> GetDbmsCurrentBilling()
        {
            List<Bill> allBills = new List<Bill>();

            try
            {
                IEnumerable<BillingCreateRefreshDates> billDates = (
                                                        from billCrtRefDtes in _icmsContext.BillingCreateRefreshDates
                                                        where billCrtRefDtes.most_recent_used_date == 1
                                                        && billCrtRefDtes.update_type == "Generate"
                                                        && billCrtRefDtes.bill_type == "LCM"
                                                        orderby billCrtRefDtes.creation_date descending
                                                        select new BillingCreateRefreshDates
                                                        {
                                                            start_date = billCrtRefDtes.start_date,
                                                            end_date = billCrtRefDtes.end_date
                                                        }
                                                      ).Take(1);


                if (billDates.Count().Equals(1))
                {
                    DateTime startDate = DateTime.MinValue;
                    DateTime endDate = DateTime.MinValue;

                    foreach (BillingCreateRefreshDates dte in billDates)
                    {
                        startDate = (DateTime)dte.start_date;
                        endDate = (DateTime)dte.end_date;
                    }

                    var billMemIds = (
                                        from bill in _icmsDataStagingContext.LcmBillingWorktables
                                        where (bill.record_date >= startDate && bill.record_date <= endDate)
                                        select bill.member_id
                                   )
                                   .Distinct()
                                   .ToList();

                    List<Patient> validMems = (
                                        from members in _icmsContext.Patients

                                        join member_enroll in _icmsContext.MemberEnrollments
                                        on members.member_id equals member_enroll.member_id

                                        orderby members.member_last_name, members.member_first_name

                                        where billMemIds.Contains(members.member_id)
                                        select new Patient
                                        {
                                            PatientId = members.member_id,
                                            firstName = members.member_first_name,
                                            lastName = members.member_last_name,
                                            middleName = members.member_middle_name,
                                            ssn = members.member_ssn,
                                            dateOfBirth = members.member_birth,
                                            dateOfBirthDisplay = (members.member_birth.Value != null) ? members.member_birth.Value.ToShortDateString() : "N/A",
                                            gender = members.gender_code,
                                            emailAddress = members.member_email,
                                            isSandsShpg = members.is_sands_shpg,
                                            isWishard = members.is_wishard,
                                            claimsEnrollmentId = member_enroll.claims_enrollment_id,
                                            egpMemberId = member_enroll.egp_member_id,
                                            effectiveDate = member_enroll.member_effective_date,
                                            disenrollDate = member_enroll.member_disenroll_date,
                                            disenrollReasonId = member_enroll.disenroll_reason_id
                                        }
                                   )
                                   .ToList();

                    var validMemIds = validMems.Where(mem => mem.isSandsShpg.Equals(null) && mem.isWishard.Equals(null))
                                               .OrderBy(mem => mem.lastName).ThenBy(mem => mem.firstName)
                                               .Select(mem => mem.PatientId).ToList();


                    var lcmCodes = (
                        from billcodes in _icmsContext.LcmBillingCodes
                        select billcodes
                        )
                        .Distinct()
                        .OrderBy(cd => cd.billing_code)
                        .ToList();


                    //orderby finalbills.employer, finalbills.patient 

                    List<Bill> finalBills = (
                                        from finalbills in _icmsDataStagingContext.LcmBillingWorktables
                                        where validMemIds.Contains((Guid)finalbills.member_id)

                                        select new Bill
                                        {
                                            billId = finalbills.lcm_record_id,
                                            lcmInvoiceNumber = finalbills.LCM_Invoice_Number,
                                            memberId = finalbills.member_id,
                                            employer = finalbills.employer,
                                            employerBillingRate = (decimal)finalbills.lcm_rate,
                                            patientFullName = finalbills.patient,
                                            recordDate = finalbills.record_date,
                                            billCode = finalbills.time_code,
                                            billMinutes = finalbills.time_length,
                                            billDisabled = finalbills.disable_flag,
                                            billSentDate = finalbills.sent_date
                                        }
                            )
                            .ToList();


                    if (finalBills != null)
                    {
                        List<Bill> orderBills = new List<Bill>();

                        foreach (Bill bill in finalBills)
                        {

                            foreach (LcmBillingCodes code in lcmCodes)
                            {
                                if (bill.billCode.Equals(code.billing_id))
                                {
                                    bill.billCodeDescription = code.billing_description;
                                    break;
                                }
                            }


                            foreach (Patient pat in validMems)
                            {
                                if (bill.memberId.Equals(pat.PatientId))
                                {
                                    bill.billingPatient = new Patient();
                                    bill.billingPatient = pat;
                                }
                            }


                            bill.billingStartDate = startDate;
                            bill.billingEndDate = endDate;


                            orderBills.Add(bill);
                        }

                        if (orderBills != null && orderBills.Count > 0)
                        {
                            allBills = orderBills.OrderBy(bill => bill.billingPatient.lastName).ThenBy(bill => bill.billingPatient.firstName).ToList();
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return allBills;
        }

        public List<Bill> getDbmsCurrentInvoice()
        {
            List<Bill> allBills = new List<Bill>();

            try
            {
                IEnumerable<BillingCreateRefreshDates> billDates = (
                                                        from billCrtRefDtes in _icmsContext.BillingCreateRefreshDates
                                                        where billCrtRefDtes.most_recent_used_date == 1
                                                        && billCrtRefDtes.update_type == "Generate"
                                                        && billCrtRefDtes.bill_type == "LCM"
                                                        orderby billCrtRefDtes.creation_date descending
                                                        select new BillingCreateRefreshDates
                                                        {
                                                            start_date = billCrtRefDtes.start_date,
                                                            end_date = billCrtRefDtes.end_date
                                                        }
                                                      ).Take(1);


                if (billDates.Count().Equals(1))
                {

                    DateTime startDate = DateTime.MinValue;
                    DateTime endDate = DateTime.MinValue;

                    foreach (BillingCreateRefreshDates dte in billDates)
                    {
                        startDate = (DateTime)dte.start_date;
                        endDate = (DateTime)dte.end_date;
                    }

                    //***UNCOMMENT**
                    var billMemIds = (
                                        from bill in _icmsDataStagingContext.LcmBillingWorktables
                                        where (bill.record_date >= startDate && bill.record_date <= endDate)
                                        && (bill.disable_flag.Value.Equals(false) || !bill.disable_flag.HasValue)
                                        && !bill.sent_date.HasValue
                                        select bill.member_id
                                   )
                                   .Distinct()
                                   .ToList();


                    //var billMemIds = (
                    //                    from bill in _icmsDataStagingContext.BillingBackups
                    //                    where (bill.record_date >= startDate && bill.record_date <= endDate)
                    //                    && (bill.disable_flag.Value.Equals(false) || !bill.disable_flag.HasValue)
                    //                    && bill.sent_date.HasValue
                    //                    select bill.member_id
                    //               )
                    //               .Distinct()
                    //               .ToList();
                    //***UNCOMMENT**


                    List<Patient> validMems = (
                                        from members in _icmsContext.Patients

                                        join member_enroll in _icmsContext.MemberEnrollments
                                        on members.member_id equals member_enroll.member_id

                                        join emplAddr in _icmsContext.EmployerAddresses
                                        on member_enroll.employer_id equals emplAddr.employer_id into emplAddrs
                                        from employerAddrs in emplAddrs.DefaultIfEmpty()

                                        orderby members.member_last_name, members.member_first_name

                                        where billMemIds.Contains(members.member_id)
                                        select new Patient
                                        {
                                            PatientId = members.member_id,
                                            firstName = members.member_first_name,
                                            lastName = members.member_last_name,
                                            middleName = members.member_middle_name,
                                            FullName = members.member_first_name + " " + members.member_last_name,
                                            ssn = members.member_ssn,
                                            dateOfBirth = members.member_birth,
                                            dateOfBirthDisplay = (members.member_birth.Value != null) ? members.member_birth.Value.ToShortDateString() : "N/A",
                                            gender = members.gender_code,
                                            emailAddress = members.member_email,
                                            isSandsShpg = members.is_sands_shpg,
                                            isWishard = members.is_wishard,
                                            claimsEnrollmentId = member_enroll.claims_enrollment_id,
                                            egpMemberId = (member_enroll.egp_member_id != null) ? member_enroll.egp_member_id : member_enroll.claims_enrollment_id,
                                            effectiveDate = member_enroll.member_effective_date,
                                            disenrollDate = member_enroll.member_disenroll_date,
                                            disenrollReasonId = member_enroll.disenroll_reason_id,
                                            employerId = member_enroll.employer_id,
                                            employerAddress = employerAddrs.address_line_one + ((employerAddrs.address_line_two != null) ? " " + employerAddrs.address_line_two : ""),
                                            employerCityStateZip = employerAddrs.city + ", " + employerAddrs.state_abbrev + " " + employerAddrs.zip_code,
                                        }
                                   )
                                   .ToList();

                    var validMemIds = validMems.Where(mem => mem.isSandsShpg.Equals(null) && mem.isWishard.Equals(null))
                                               .OrderBy(mem => mem.lastName).ThenBy(mem => mem.firstName)
                                               .Select(mem => mem.PatientId).ToList();


                    var lcmCodes = (
                        from billcodes in _icmsContext.LcmBillingCodes
                        select billcodes
                        )
                        .Distinct()
                        .OrderBy(cd => cd.billing_code)
                        .ToList();


                    //orderby finalbills.employer, finalbills.patient 


                    //***UNCOMMENT**
                    List<Bill> finalBills = (
                                        from finalbills in _icmsDataStagingContext.LcmBillingWorktables
                                        where validMemIds.Contains((Guid)finalbills.member_id)
                                        && (finalbills.disable_flag.Value.Equals(false) || !finalbills.disable_flag.HasValue)
                                        && !finalbills.sent_date.HasValue

                                        select new Bill
                                        {
                                            billId = finalbills.lcm_record_id,
                                            lcmInvoiceNumber = finalbills.LCM_Invoice_Number,
                                            memberId = finalbills.member_id,
                                            employer = finalbills.employer,
                                            employerBillingRate = (decimal)finalbills.lcm_rate,
                                            patientFullName = finalbills.patient,
                                            recordDate = finalbills.record_date,
                                            billCode = finalbills.time_code,
                                            billMinutes = finalbills.time_length,
                                            billDisabled = finalbills.disable_flag,
                                            billSentDate = finalbills.sent_date,
                                            billNote = finalbills.notes
                                        }
                            )
                            .OrderBy(bill => bill.memberId)
                            .ThenBy(bill => bill.billCode)
                            .ToList();

                    //List<Bill> finalBills = (
                    //                            from finalbills in _icmsDataStagingContext.BillingBackups
                    //                            where validMemIds.Contains((Guid)finalbills.member_id)
                    //                            && (finalbills.disable_flag.Value.Equals(false) || !finalbills.disable_flag.HasValue)
                    //                            && finalbills.sent_date.HasValue
                    //                            select new Bill
                    //                            {
                    //                                billId = (Guid)finalbills.lcm_record_id,
                    //                                lcmInvoiceNumber = finalbills.LCM_Invoice_Number,
                    //                                memberId = finalbills.member_id,
                    //                                employer = finalbills.employer,
                    //                                employerBillingRate = (decimal)finalbills.lcm_rate,
                    //                                patientFullName = finalbills.patient,
                    //                                recordDate = finalbills.record_date,
                    //                                billCode = finalbills.time_code,
                    //                                billMinutes = finalbills.time_length,
                    //                                billDisabled = finalbills.disable_flag,
                    //                                billSentDate = finalbills.sent_date,
                    //                                billNote = finalbills.notes
                    //                            }
                    //                )
                    //                .OrderBy(bill => bill.memberId)
                    //                .ThenBy(bill => bill.billCode)
                    //                .ToList();
                    //***UNCOMMENT**



                    if (finalBills != null)
                    {

                        List<Bill> orderBills = new List<Bill>();

                        foreach (Bill bill in finalBills)
                        {

                            foreach (LcmBillingCodes code in lcmCodes)
                            {
                                if (bill.billCode.Equals(code.billing_id))
                                {
                                    bill.billCodeDescription = code.billing_description;
                                    break;
                                }
                            }


                            foreach (Patient pat in validMems)
                            {
                                if (bill.memberId.Equals(pat.PatientId))
                                {
                                    bill.billingPatient = new Patient();
                                    bill.billingPatient = pat;
                                }
                            }


                            bill.billingStartDate = startDate;
                            bill.billingEndDate = endDate;


                            orderBills.Add(bill);
                        }

                        if (orderBills != null && orderBills.Count > 0)
                        {
                            allBills = orderBills.OrderBy(bill => bill.billingPatient.lastName).ThenBy(bill => bill.billingPatient.firstName).ToList();
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return allBills;
        }

        public List<Bill> saveBilling(List<Bill> bills)
        {
            List<Bill> returnBills = null;
            List<Bill> tempBills = new List<Bill>();


            foreach (Bill bill in bills)
            {
                LcmBillingWorktable billInDb = (from lcmwwrktbl in _icmsDataStagingContext.LcmBillingWorktables
                                                where lcmwwrktbl.lcm_record_id.Equals(bill.billId)
                                                select lcmwwrktbl)
                                                .FirstOrDefault();

                if (billInDb != null)
                {
                    billInDb.time_code = bill.billCode;
                    billInDb.time_length = bill.billMinutes;
                    billInDb.disable_flag = bill.billDisabled;

                    _icmsDataStagingContext.LcmBillingWorktables.Update(billInDb);

                    tempBills.Add(bill);
                }
            }


            int result = _icmsDataStagingContext.SaveChanges();


            if (result > 0 && tempBills.Count > 0)
            {
                returnBills = new List<Bill>();
                returnBills = tempBills;
            }


            return returnBills;
        }

        public Bill GetBill(string id)
        {
            Bill returnBill = new Bill();

            try
            {
                Guid lcmRecordId = Guid.Empty;

                if (Guid.TryParse(id, out lcmRecordId))
                {
                    returnBill = (
                            from finalbills in _icmsDataStagingContext.LcmBillingWorktables
                            where finalbills.lcm_record_id.Equals(lcmRecordId)

                            select new Bill
                            {
                                billId = finalbills.lcm_record_id,
                                memberId = finalbills.member_id,
                                employer = finalbills.employer,
                                employerBillingRate = (decimal)finalbills.lcm_rate,
                                patientFullName = finalbills.patient,
                                recordDate = finalbills.record_date,
                                billCode = finalbills.time_code,
                                billMinutes = finalbills.time_length,
                                billDisabled = finalbills.disable_flag,
                                billSentDate = finalbills.sent_date
                            }
                            ).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

            }

            return returnBill;
        }


        public List<BillingCodes> GetDbmsBillingCodes()
        {
            List<BillingCodes> billingCodes = null;


            billingCodes = (
                                from billcodes in _icmsContext.LcmBillingCodes
                                orderby billcodes.billing_code
                                select new BillingCodes
                                {
                                    billingCodeId = billcodes.billing_id,
                                    billingCode = billcodes.billing_code,
                                    billingDescription = billcodes.billing_description,
                                    displayCodeDescription = billcodes.billing_code + ' ' + billcodes.billing_description
                                }
                            )
                            .Distinct()
                            .ToList();




            return billingCodes;
        }

        public List<BillUpdateReason> getBillingUpdateReasons()
        {

            List<BillUpdateReason> reasons = null;

            reasons = (

                    from billReasons in _icmsContext.LcmBilllingCodesUpdateReasons
                    orderby billReasons.reason
                    select new BillUpdateReason
                    {
                        reasonsId = billReasons.lcm_billing_codes_update_reason_id,
                        reason = billReasons.reason
                    }
                )
                .ToList();

            return reasons;
        }


        public Note GetDbmsCurrentBillingNote(string id)
        {
            List<Note> billingNote = new List<Note>();


            Guid lcmRecordId = Guid.Empty;

            if (Guid.TryParse(id, out lcmRecordId))
            {

                Bill bill = new Bill();

                bill = (
                            from bills in _icmsDataStagingContext.LcmBillingWorktables
                            where bills.lcm_record_id.Equals(lcmRecordId)
                            select new Bill
                            {
                                memberId = bills.member_id,
                                recordDate = bills.record_date
                            }
                        ).SingleOrDefault();


                if (bill != null)
                {
                    Guid memberId = (Guid)bill.memberId;
                    DateTime recordDate = (DateTime)bill.recordDate;


                    billingNote = (
                                    from note in _icmsContext.MemberNotes
                                    where note.member_id.Equals(memberId)
                                    && note.record_date.Equals(recordDate)
                                    select new Note
                                    {
                                        noteText = note.evaluation_text,
                                        noteSequenceNumber = note.record_seq_num
                                    }
                                  )
                                  .OrderBy(seq => seq.noteSequenceNumber)
                                  .ToList();
                }
            }


            Note returnNote = new Note();


            foreach (var nte in billingNote)
            {
                returnNote.noteText += nte.noteText;
            }

            return returnNote;
        }


        public Note GetBackupBillingNote(string id)
        {
            Note returnNote = new Note();


            List<Note> billingNote = new List<Note>();


            int billingBackupId = 0;

            if (int.TryParse(id, out billingBackupId) && billingBackupId > 0)
            {

                Bill bill = new Bill();

                bill = (
                            from bkups in _icmsDataStagingContext.BillingBackups
                            where bkups.billing_backup_id.Equals(billingBackupId)
                            select new Bill
                            {
                                memberId = bkups.member_id,
                                recordDate = bkups.record_date
                            }
                        ).SingleOrDefault();


                if (bill != null)
                {
                    Guid memberId = (Guid)bill.memberId;
                    DateTime recordDate = (DateTime)bill.recordDate;
                    bill.billingBackupId = billingBackupId;


                    billingNote = (
                                    from note in _icmsContext.MemberNotes
                                    where note.member_id.Equals(memberId)
                                    && note.record_date.Equals(recordDate)
                                    select new Note
                                    {
                                        noteText = note.evaluation_text,
                                        noteSequenceNumber = note.record_seq_num
                                    }
                                  )
                                  .OrderBy(seq => seq.noteSequenceNumber)
                                  .ToList();


                    if (billingNote.Count > 0)
                    {
                        foreach (var nte in billingNote)
                        {
                            returnNote.noteText += nte.noteText;
                        }
                    }
                    else
                    {

                        returnNote.noteText = getMergedMemberNoteForMemberInBackup(bill);
                    }
                }
            }



            return returnNote;
        }


        public IEnumerable<Bill> GetDbmsRefreshBills(Bill refreshDates)
        {
            List<Bill> allBills = new List<Bill>();

            DateTime startDate = (DateTime)refreshDates.billingStartDate;
            DateTime endDate = (DateTime)refreshDates.billingEndDate;

            if (!startDate.Equals(DateTime.MinValue) && !endDate.Equals(DateTime.MinValue))
            {
                List<Note> billableNotes = (
                    from memNotes in _icmsContext.MemberNotes

                    join mem in _icmsContext.Patients
                    on memNotes.member_id equals mem.member_id into mem
                    from membr in mem.DefaultIfEmpty()

                    where memNotes.record_date >= startDate
                    && memNotes.record_date <= endDate
                    && !memNotes.note_billed.HasValue
                    orderby membr.member_last_name, membr.member_first_name, memNotes.record_date
                    select new Note
                    {
                        memberId = memNotes.member_id,
                        firstName = membr.member_first_name,
                        lastName = membr.member_last_name,
                        recordDate = memNotes.record_date,
                        billingId = memNotes.billing_id,
                        billingMinutes = memNotes.RN_notes
                    }
                    ).Distinct()
                    .OrderByDescending(nte => nte.recordDate)
                    .OrderBy(nte => nte.firstName)
                    .OrderBy(nte => nte.lastName)
                    .ToList();


                if (billableNotes.Count > 0)
                {
                    PatientService patSer = new PatientService(_icmsContext, _aspnetContext);


                    foreach (Note nte in billableNotes)
                    {
                        if (!nte.memberId.Equals(Guid.Empty) && !nte.recordDate.Equals(DateTime.MinValue))
                        {
                            if (!NoteIsInBilling(nte))
                            {
                                Patient member = patSer.GetDbmsMember(nte.memberId.ToString());
                                Note billablNte = GetMemberNoteForBillingTextField(nte);

                                if (!member.PatientId.Equals(Guid.Empty) && !string.IsNullOrEmpty(billablNte.noteText))
                                {
                                    Bill addNoteToBilling = new Bill();
                                    addNoteToBilling.memberId = nte.memberId;
                                    addNoteToBilling.patientFullName = member.firstName + " " + member.lastName;

                                    addNoteToBilling.recordDate = nte.recordDate;
                                    addNoteToBilling.billCode = nte.billingId;
                                    addNoteToBilling.billMinutes = nte.billingMinutes;

                                    addNoteToBilling.billNote = billablNte.noteText;
                                    addNoteToBilling.caseOwner = billablNte.caseOwnerName;

                                    addNoteToBilling.employer = member.employerName;
                                    addNoteToBilling.tpa = member.tpaName;

                                    allBills.Add(addNoteToBilling);
                                }
                            }
                        }
                    }
                }


                //foreach(Bill billablenot in allBills)
                //{
                //System.Diagnostics.Debug.WriteLine("Bill MemberId: " + billablenot.memberId + " Bill record_date: " + billablenot.recordDate);                
                //}
            }


            return allBills;
        }

        public IEnumerable<Bill> AddRefreshBillsToCurrentBilling(List<Bill> bills)
        {
            List<Bill> returnBills = new List<Bill>();

            try
            {
                foreach (Bill bill in bills)
                {
                    if (!VerifyBillNotInBilling(bill))
                    {
                        Bill addedBill = AddLcmBillingWorktableItem(bill);


                        if (!addedBill.billId.Equals(Guid.Empty))
                        {
                            returnBills.Add(addedBill);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return returnBills;
        }

        public Bill AddRefreshDates(Bill dates)
        {
            Bill returnBills = new Bill();

            try
            {
                List<BillingCreateRefreshDates> currentDate = (
                                          from bilcrtrefdte in _icmsContext.BillingCreateRefreshDates
                                          where bilcrtrefdte.most_recent_used_date == 1
                                          && bilcrtrefdte.bill_type.Equals("LCM")
                                          && bilcrtrefdte.update_type.Equals("Generate")
                                          orderby bilcrtrefdte.creation_date descending
                                          select bilcrtrefdte
                                          ).ToList();

                int result = 0;

                if (currentDate != null)
                {
                    foreach (BillingCreateRefreshDates dte in currentDate)
                    {
                        dte.most_recent_used_date = 0;
                        dte.update_date = DateTime.Now;
                        dte.update_date_userid = dates.userId;

                        _icmsContext.BillingCreateRefreshDates.Update(dte);

                        result = _icmsContext.SaveChanges();
                    }
                }
                else
                {
                    result = 1;
                }

                if (result > 0)
                {
                    result = 0;

                    BillingCreateRefreshDates refreshDates = new BillingCreateRefreshDates();
                    refreshDates.start_date = dates.billingStartDate;
                    refreshDates.end_date = dates.billingEndDate;
                    refreshDates.bill_type = "LCM";
                    refreshDates.most_recent_used_date = 1;
                    refreshDates.system_user_id = dates.userId;
                    refreshDates.update_type = "Generate";

                    _icmsContext.BillingCreateRefreshDates.Add(refreshDates);

                    result = _icmsContext.SaveChanges();


                    if (result > 0)
                    {
                        returnBills.billingStartDate = dates.billingStartDate;
                        returnBills.billingEndDate = dates.billingEndDate;
                    }
                }


                return returnBills;
            }
            catch (Exception ex)
            {
                return returnBills;
            }
        }

        public Bill getLastBillingRefreshDates()
        {
            Bill returnBills = new Bill();

            try
            {
                BillingCreateRefreshDates currentDate = (
                                          from bilcrtrefdte in _icmsContext.BillingCreateRefreshDates
                                          where bilcrtrefdte.most_recent_used_date == 1
                                          && bilcrtrefdte.bill_type.Equals("LCM")
                                          && bilcrtrefdte.update_type.Equals("Generate")
                                          orderby bilcrtrefdte.creation_date descending
                                          select bilcrtrefdte
                                          ).FirstOrDefault();

                if (currentDate != null)
                {
                    returnBills.billingStartDate = currentDate.start_date;
                    returnBills.billingEndDate = currentDate.end_date;
                }


                return returnBills;
            }
            catch (Exception ex)
            {
                return returnBills;
            }
        }

        public Bill createBillInvoicePdf(string directory, List<Bill> bills)
        {
            Bill invoiceReturned = new Bill();

            try
            {
                if (bills.Count > 0)
                {

                    Bill invoice1 = createBillInvoice1Pdf(directory, bills);

                    if (invoice1 != null)
                    {

                        invoiceReturned.invoicePdf = invoice1.invoicePdf;
                        invoiceReturned.invoiceBase64 = invoice1.invoiceBase64;
                        invoiceReturned.invoiceContentType = invoice1.invoiceContentType;
                        invoiceReturned.invoiceFileName = invoice1.invoiceFileName;
                        invoiceReturned.invoiceFileName = invoice1.invoiceFileName;
                    }

                    Bill invoice2 = createBillInvoice2Pdf(directory, bills);

                    if (invoice2 != null)
                    {

                        invoiceReturned.invoice2Pdf = invoice2.invoice2Pdf;
                        invoiceReturned.invoice2Base64 = invoice2.invoice2Base64;
                        invoiceReturned.invoice2ContentType = invoice2.invoice2ContentType;
                        invoiceReturned.invoice2FileName = invoice2.invoice2FileName;
                        invoiceReturned.invoice2Html = invoice2.invoice2Html;
                    }


                    //StringBuilder html = new StringBuilder();


                    //html.Append(createBillInvoicePdf_Initialize(directory, bills));

                    //html.Append(createBillInvoicePdf_Items(bills));

                    //html.Append(createBillInvoicePdf_Finalize(bills));

                    //invoiceReturned.invoiceFileUrlLocation = "";

                    //FileContentResult pdfBill = generatePdfToFile(html.ToString(), "Billing Invoice - LCM");

                    //generate PDF in (a) server's folder
                    //string fileUrlLocation = generatePdfOnEdrive(html.ToString());

                    //if (!string.IsNullOrEmpty(fileUrlLocation))
                    //{
                    //    invoiceReturned.invoiceFileUrlLocation = fileUrlLocation;
                    //}

                }
            }
            catch (Exception ex)
            {

            }

            return invoiceReturned;
        }

        private Bill createBillInvoice1Pdf(string directory, List<Bill> bills)
        {

            Bill invoiceReturned = null;

            StringBuilder html = new StringBuilder();

            html.Append(createBillInvoicePdf_Initialize(directory, bills));

            html.Append(createBillInvoicePdf_Items(bills));

            html.Append(createBillInvoicePdf_Finalize(bills));

            FileContentResult pdfBill = generatePdfToFile(html.ToString(), "Billing Invoice - LCM", "dbms_invoice.pdf");

            if (pdfBill != null)
            {

                invoiceReturned = new Bill();

                invoiceReturned.invoicePdf = pdfBill.FileContents;
                invoiceReturned.invoiceBase64 = Convert.ToBase64String(pdfBill.FileContents);
                invoiceReturned.invoiceContentType = pdfBill.ContentType;
                invoiceReturned.invoiceFileName = pdfBill.FileDownloadName;
                invoiceReturned.invoiceFileUrlLocation = "";
            }


            return invoiceReturned;
        }

        private Bill createBillInvoice2Pdf(string directory, List<Bill> bills)
        {

            Bill invoiceReturned = null;

            StringBuilder html = new StringBuilder();

            html.Append(createBillInvoice2Pdf_Initialize());

            foreach (Bill bill in bills)
            {

                html.Append(createBillInvoice2Pdf_HeaderEmployer(directory, bill));

                html.Append(createBillInvoice2Pdf_ServiceDetails(bill));

                html.Append(createBillInvoice2Pdf_Bills(bill, bills));

                html.Append(createBillInvoice2Pdf_Footer());

                html.Append(createBillInvoice2Pdf_Notes(bill, bills));
            }

            html.Append(createBillInvoice2Pdf_Finalize());

            //DELETE WHEN DONE
            //invoiceReturned = new Bill();
            //invoiceReturned.invoice2Html = html.ToString();
            //DELETE WHEN DONE


            //UNCOMMENT OUT
            FileContentResult pdfBill = generatePdfToFile(html.ToString(), "Billing Invoice w/ Notes- LCM", "dbms_invoice2.pdf");


            if (pdfBill != null)
            {

                invoiceReturned = new Bill();

                invoiceReturned.invoice2Pdf = pdfBill.FileContents;
                invoiceReturned.invoice2Base64 = Convert.ToBase64String(pdfBill.FileContents);
                invoiceReturned.invoice2ContentType = pdfBill.ContentType;
                invoiceReturned.invoice2FileName = pdfBill.FileDownloadName;
            }
            //UNCOMMENT OUT


            return invoiceReturned;
        }


        private string createBillInvoice2Pdf_Initialize()
        {

            string start = @"
            <html>
                <head>
                    <style>
                        body {
                            font-family: Arial, sans-serif, 'Trebuchet MS', 'Lucida Sans Unicode', 'Lucida Grande', 'Lucida Sans';
                            font-size: 16px;
                            color: black;
                        }
                    </style>
                </head>
                <body>";

            return start;
        }

        private string createBillInvoice2Pdf_HeaderEmployer(string directory, Bill patBills)
        {

            string header = @"
                <div id='header'>

                    <table style='width: 100%; margin-bottom: 10px;'>
                        <tr>
                            <td>
                                <img src='" + directory + "\\dbms_logo.png' alt='DBMS Logo' style='width: 100px; height: 50px; opacity: .1;' />" +
                                "<span style='font-size: 12px'>107 Crosspoint Blvd Suite 250 Indianapolis IN 46256 - (800) 728-0327</span>" +
                            "</td>" +
                            "<td style='text-align: right;'>" +
                                "<span style='font-size: 14px'>Invoice #:</span>" + patBills.billingPatient.claimsEnrollmentId +
                            "</td>" +
                        "</tr> " +
                        "<tr>" +
                            "<td colspan='2' style='height: 15px; background-color: #193866; border-radius: 2px; box-shadow: 4px 5px 10px #6e6e6e;'></td>" +
                        "</tr>" +
                    "</table> ";



            string employer = @"
                <table style='width: 100%; margin-bottom: 20px; font-size: 14px'>" +
                    "<tr>" +
                        "<td>" + patBills.employer +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + patBills.billingPatient.employerAddress +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" + patBills.billingPatient.employerCityStateZip +
                        "</td>" +
                    "</tr>" +
                "</tabel>" +

            "</div> ";

            string html = header + employer;

            return html;
        }


        private string createBillInvoice2Pdf_ServiceDetails(Bill patBills)
        {

            string serviceDetail = @"
                <table style='font-size: 14px; margin-bottom: 20px;'>" +
                    "<tr>" +
                        "<td><span><b>Service Detail:</b></span>" +
                        "</td>" +
                        "<td>" +
                            "<span>LCM For " + patBills.billingPatient.FullName + " " + patBills.billingPatient.claimsEnrollmentId + "</span>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td><span><b>Client:</b></span>" +
                        "</td>" +
                        "<td>" +
                            "<span>" + patBills.employer + "</span>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td><span><b>Invoice Period:</b></span>" +
                        "</td>" +
                        "<td>" +
                            "<span>" + patBills.billingStartDate.Value.ToShortDateString() + " - " + patBills.billingEndDate.Value.ToShortDateString() + "</span>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td><span><b>Due Date:</b></span>" +
                        "</td>" +
                        "<td>" +
                            "<span>" + DateTime.Now.ToShortDateString() + "</span>" +
                        "</td>" +
                    "</tr>" +
                "</table>";

            return serviceDetail;
        }

        private string createBillInvoice2Pdf_Bills(Bill patBills, List<Bill> billList)
        {

            string html = "";

            string billTableHeader = @"
                <table style='border-collapse: collapse; width: 100%; font-size: 14px; margin-bottom: 30px;'>" +
                    "<tr style='border-bottom: 1px solid black;'>" +
                        "<td><span><b>Code</b></span>" +
                        "</td>" +
                        "<td><span><b>Description</b></span>" +
                        "</td>" +
                        "<td><span><b>Time</b></span>" +
                        "</td>" +
                        "<td><span><b>Rate</b></span>" +
                        "</td>" +
                        "<td><span><b>Amount</b></span>" +
                        "</td>" +
                    "</tr>";


            string billTableDetails = "";
            decimal billTotal = 0;
            double billTotalMinutes = 0;

            foreach (Bill bill in billList)
            {

                if (bill.billingPatient.PatientId.Equals(patBills.billingPatient.PatientId))
                {

                    decimal billAmount = (Convert.ToDecimal(bill.billMinutes) / 60) * (decimal)bill.employerBillingRate;
                    string displayAmount = Math.Round(billAmount, 2).ToString("#,###,###.00");
                    billTotal += billAmount;
                    billTotalMinutes += (double)bill.billMinutes;

                    billTableDetails += @"
                        <tr style='font-size: 12px;'>" +
                            "<td>" + bill.billCode +
                            "</td>" +
                            "<td>" + bill.billCodeDescription +
                            "</td>" +
                            "<td>" + bill.billMinutes +
                            "</td>" +
                            "<td>" + Math.Round((decimal)bill.employerBillingRate, 2).ToString("#,###,###.00") +
                            "</td>" +
                            "<td>" + displayAmount +
                            "</td>" +
                        "</tr>";
                }
            }

            string billTableFooter = @"
                    <tr style='border-bottom: 1px solid black;'>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td><b>" + billTotalMinutes + "</b></td>" +
                        "<td></td>" +
                        "<td><b>" + Math.Round((decimal)billTotal, 2).ToString("#,###,###.00") + "</b></td>" +
                    "</tr>" +
                "</table>";


            html = billTableHeader + billTableDetails + billTableFooter;

            return html;
        }

        private string createBillInvoice2Pdf_Footer()
        {

            string html = "";

            string details = @"
                <table style='width: 100%; font-size: 12px; margin-bottom: 10px;'>" +
                    "<tr>" +
                        "<td>Please remit payment to:" +
                        "</td>" +
                        "<td>DBMS Health Services, LLC" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                        "</td>" +
                        "<td>Attn: Account Receivable" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                        "</td>" +
                        "<td>107 Crosspoint Blvd Suite 250" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                        "</td>" +
                        "<td>Indianapolis IN 46256" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                        "</td>" +
                        "<td>Tax ID #: 35-1916286" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                        "</td>" +
                        "<td>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td colspan='2' style='text-align: center;'>Questions call Billing Department @ (317) 582-1200 ext. 223 - Email: accounting@dbmshealth.com" +
                        "</td>" +
                    "</tr>" +
                "</table>";


            string footer = @"
                <table style='width: 100%; font-size: 12px; margin-bottom: 40px'>" +
                    "<tr>" +
                        "<td>" +
                            "DBMS Health has, in good faith, included all appropriate services and applicable charges on this invoice. We respectfully" +
                            "request that you contact us immediately if there are any questions or concerns in order to resolve such matters timely." +
                            "A late fee will apply if payment is not received by due date." +
                        "</td>" +
                    "</tr>" +
                "</table><div style='page-break-after: always;'></div>";


            html = details + footer;

            return html;
        }

        private string createBillInvoice2Pdf_Notes(Bill patBills, List<Bill> billList)
        {

            string html = "";

            string header = createBillInvoice2Pdf_NotesHeader(patBills);
            string notes = createBillInvoice2Pdf_NotesDetails(patBills, billList);

            html = header + notes;

            return html;
        }

        private string createBillInvoice2Pdf_NotesHeader(Bill patBills)
        {

            string html = "";

            html = @"
                <table style='width: 100%; margin-bottom: 20px;'>" +
                    "<tr>" +
                        "<td style='text-align: right;'>" +
                            "<span><b>Invoice Date:</b>" + DateTime.Now.ToShortDateString() + "</span>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td style='text-align: right;'>" +
                            "<span><b>Invoice #:</b>" + patBills.billingPatient.claimsEnrollmentId + "</span>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td style='text-align: center;'>" +
                            "<span><b>LCM NOTES FOR THE MONTH OF " + DateTime.Now.ToString("MMMM").ToUpper() + "</b></span> " +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                            "<span><b>Patient:</b>" + patBills.billingPatient.FullName + "</span> " +
                        "</td>" +
                    "</tr>" +
                "</table>";

            return html;
        }

        private string createBillInvoice2Pdf_NotesDetails(Bill patBills, List<Bill> billList)
        {

            string html = "";

            string noteTableHeader = @"
                <table style='border-collapse: collapse; width: 100%; font-size: 14px; margin-bottom: 30px;'>" +
                    "<tr style='border-bottom: 1px solid black;'>" +
                        "<td style='text-align: left;'><span><b>Date</b></span>" +
                        "</td>" +
                        "<td style='text-align: center;'><span><b>Code</b></span>" +
                        "</td>" +
                        "<td><span><b>Minutes</b></span>" +
                        "</td>" +
                        "<td><span><b>Notes</b></span>" +
                        "</td>" +
                    "</tr>";


            string noteTableDetail = "";

            foreach (Bill bill in billList)
            {

                if (bill.billingPatient.PatientId.Equals(patBills.billingPatient.PatientId))
                {

                    noteTableDetail += @"
                        <tr>" +
                            "<td style='text-align: left;'>" +
                                "<span style='font-size: 12px;'>" + bill.recordDate.Value.ToShortDateString() + "</span>" +
                            "</td>" +
                            "<td style='text-align: center;'>" +
                                "<span style='font-size: 12px;'>" + bill.billCode + "</span> " +
                            "</td>" +
                            "<td>" +
                                "<span style='font-size: 12px;'>" + bill.billMinutes + "</span> " +
                            "</td>" +
                            "<td>" +
                                "<span style='font-size: 12px;'>" + bill.billNote + "</span> " +
                            "</td>" +
                        "<tr>" +
                        "<tr style='border-bottom: 1px solid black;'>" +
                            "<td colspan='4'>" +
                            "</td>" +
                        "<tr>" +
                    "</table>";
                }
            }

            string noteTableFooter = @"<div style='page-break-after: always;'></div>";

            html = noteTableHeader + noteTableDetail + noteTableFooter;

            return html;
        }



        private string createBillInvoice2Pdf_Finalize()
        {

            string end = @" </body></html>";

            return end;
        }


        public Bill getBillingPeriods(string type, string span)
        {
            Bill returnPeriods = new Bill();

            returnPeriods.backupPeriods = new List<BillingBackup>();

            try
            {

                List<BillingBackup> backups = null;
                int months = 0;

                if (!string.IsNullOrEmpty(span) && !span.Equals("na"))
                {
                    months = int.Parse(span);
                }

                if (months > 0)
                {

                    DateTime endDte = DateTime.Now;
                    DateTime strDte = endDte.AddMonths(-months);

                    backups = (

                                from billbckups in _icmsDataStagingContext.BillingBackups
                                where billbckups.billing_type.Equals("LCM")
                                && (billbckups.creation_date >= strDte && billbckups.creation_date <= endDte)
                                select new BillingBackup
                                {
                                    backup_period_id = billbckups.backup_period_id,
                                    creation_date = billbckups.creation_date
                                }

                              ).Distinct()
                              .OrderByDescending(creDte => creDte.creation_date)
                              .ToList();

                }
                else
                {

                    backups = (

                                from billbckups in _icmsDataStagingContext.BillingBackups
                                where billbckups.billing_type.Equals("LCM")
                                select new BillingBackup
                                {
                                    backup_period_id = billbckups.backup_period_id,
                                    creation_date = billbckups.creation_date
                                }

                              ).Distinct()
                              .OrderByDescending(creDte => creDte.creation_date)
                              .ToList();
                }

                if (backups != null)
                {
                    foreach (BillingBackup period in backups)
                    {
                        returnPeriods.backupPeriods.Add(period);
                    }
                }


                return returnPeriods;
            }
            catch (Exception ex)
            {
                return returnPeriods;
            }
        }

        public IEnumerable<Bill> getBillsFromBillingPeriod(string date)
        {
            List<Bill> arBillsReturned = new List<Bill>();
            List<Bill> billsWithPayments = null;
            List<Bill> billsWithBalance = null;

            try
            {
                DateTime createDate = DateTime.MinValue;

                if (DateTime.TryParse(date, out createDate))
                {
                    List<BillingBackup> backupbills = (from bilbkup in _icmsDataStagingContext.BillingBackups
                                                       where bilbkup.creation_date.Equals(createDate)
                                                       select bilbkup
                                                        ).OrderBy(mem => mem.member_id)
                                                        .ToList();


                    if (backupbills != null && backupbills.Count > 0)
                    {
                        Guid memberIdCheck = Guid.Empty;
                        decimal memberBillAmountTotal = 0;
                        bool addBill = false;
                        Bill outsideBill = new Bill();

                        foreach (BillingBackup bill in backupbills)
                        {

                            if (!memberIdCheck.Equals(bill.member_id))
                            {

                                if (addBill)
                                {
                                    if (!outsideBill.memberId.Equals(Guid.Empty))
                                    {
                                        outsideBill.billTotalAmount = memberBillAmountTotal;
                                        arBillsReturned.Add(outsideBill);
                                    }
                                }

                                Bill arBill = new Bill();
                                memberBillAmountTotal = 0;

                                arBill.memberId = bill.member_id;
                                arBill.patientFullName = bill.patient;
                                arBill.billingPatient = getBillingPatient((Guid)bill.member_id);
                                arBill.employer = bill.employer;
                                arBill.lcmInvoiceNumber = bill.LCM_Invoice_Number;

                                outsideBill = arBill;

                                decimal employerBillingRate = ((bill.lcm_rate.HasValue) ? (decimal)bill.lcm_rate.Value : 0);
                                decimal billMinutes = ((bill.time_length.HasValue) ? (decimal)bill.time_length.Value : 0);

                                memberBillAmountTotal += (billMinutes / 60) * employerBillingRate;

                                memberIdCheck = (Guid)bill.member_id;
                                addBill = true;

                            }
                            else
                            {

                                decimal employerBillingRate = ((bill.lcm_rate.HasValue) ? (decimal)bill.lcm_rate.Value : 0);
                                decimal billMinutes = ((bill.time_length.HasValue) ? (decimal)bill.time_length.Value : 0);

                                memberBillAmountTotal += (billMinutes / 60) * employerBillingRate;

                            }
                        }


                        if (addBill)
                        {
                            if (!outsideBill.billId.Equals(Guid.Empty))
                            {
                                outsideBill.billTotalAmount = memberBillAmountTotal;

                                arBillsReturned.Add(outsideBill);
                            }
                        }


                        billsWithPayments = loadBillsTotalPayments(arBillsReturned);
                        billsWithBalance = loadBillsRemainingBalance(billsWithPayments);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            List<Bill> sortedList = billsWithBalance.OrderBy(last => last.billingPatient.lastName).ThenBy(first => first.billingPatient.firstName).ToList();  //billsWithBalance.OrderBy(emp => emp.employer).ThenBy(pat => pat.patientFullName).ToList();

            return sortedList;
        }

        public IEnumerable<Bill> getBillsFromBillingBackup(string lcmInvoiceNumber, string id)
        {
            List<Bill> returnedArBills = new List<Bill>();

            try
            {
                Guid memberId = Guid.Empty;


                if (!string.IsNullOrEmpty(lcmInvoiceNumber) && Guid.TryParse(id, out memberId))
                {
                    List<BillingBackup> backupbills = (from bilbkup in _icmsDataStagingContext.BillingBackups
                                                       where bilbkup.LCM_Invoice_Number.Equals(lcmInvoiceNumber)
                                                       && bilbkup.member_id.Equals(memberId)
                                                       select bilbkup
                                                        ).OrderBy(recDte => recDte.record_date)
                                                        .ToList();


                    if (backupbills != null && backupbills.Count > 0)
                    {

                        foreach (BillingBackup bill in backupbills)
                        {
                            Bill addArBill = new Bill();
                            addArBill.billingBackupId = bill.billing_backup_id;
                            addArBill.billId = (Guid)bill.lcm_record_id;
                            addArBill.lcmInvoiceNumber = bill.LCM_Invoice_Number;
                            addArBill.billingBackupPeriodId = bill.backup_period_id;
                            addArBill.memberId = bill.member_id;
                            addArBill.patientFullName = bill.patient;
                            addArBill.recordDate = bill.record_date;
                            addArBill.billCode = bill.time_code;

                            addArBill.billCodeDescription = getCodeDescription((int)bill.time_code);

                            addArBill.billMinutes = bill.time_length;
                            addArBill.employerBillingRate = (decimal)bill.lcm_rate;
                            addArBill.billSentDate = bill.sent_date;

                            returnedArBills.Add(addArBill);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return returnedArBills;
        }

        public Bill getBillPaymentHistory(string lcmInvoiceNumber, string id)
        {
            Bill paymentsReturned = new Bill();

            try
            {
                Guid memberId = Guid.Empty;


                if (!string.IsNullOrEmpty(lcmInvoiceNumber) && Guid.TryParse(id, out memberId))
                {
                    List<AccountsReceivablePayments> billPayments = (from arpayments in _icmsContext.AccountsReceivablePayments
                                                                     where arpayments.invoice_id.Equals(lcmInvoiceNumber)
                                                                     && arpayments.member_id.Equals(memberId)
                                                                     select arpayments
                                                        ).OrderBy(crDte => crDte.creation_date)
                                                        .ToList();


                    if (billPayments != null && billPayments.Count > 0)
                    {
                        paymentsReturned.payments = new List<Payment>();

                        decimal billTotal = getBillBackupTotalAmountUsingInvoiceNumber(lcmInvoiceNumber, memberId);
                        decimal billBalance = billTotal;


                        foreach (AccountsReceivablePayments paid in billPayments)
                        {
                            billBalance = billBalance - (decimal)paid.payment_amount;

                            Payment pay = new Payment();
                            pay.paymentId = paid.accounts_receivable_payment_id;
                            pay.invoiceId = paid.invoice_id;
                            pay.billTotal = billTotal;
                            pay.paymentDate = paid.payment_date;
                            pay.paymentDateDisplay = (pay.paymentDate != null && !pay.paymentDate.Equals(DateTime.MinValue)) ?
                                pay.paymentDate.Value.ToShortDateString() : "N/A";
                            pay.paymentAmount = paid.payment_amount;
                            pay.balanceAfterPayment = billBalance;
                            pay.paymentUserId = paid.payment_user_id;

                            if (pay.paymentUserId != null)
                            {
                                pay.paymentUserName = getUserName((Guid)pay.paymentUserId);
                            }

                            paymentsReturned.payments.Add(pay);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return paymentsReturned;
        }

        public decimal getBillTotalAmount(string lcmInvoiceNumber, string id)
        {
            decimal billTotal = 0;

            try
            {
                Guid memberId = Guid.Empty;


                if (!string.IsNullOrEmpty(lcmInvoiceNumber) && Guid.TryParse(id, out memberId))
                {
                    decimal total = getBillBackupTotalAmountUsingInvoiceNumber(lcmInvoiceNumber, memberId);
                    billTotal = total;
                }
            }
            catch (Exception ex)
            {

            }

            return billTotal;
        }

        public Payment getArPayment(string id)
        {
            Payment paymentReturned = null;

            int paymentId = 0;

            if (int.TryParse(id, out paymentId))
            {
                AccountsReceivablePayments pymnts = (from pymts in _icmsContext.AccountsReceivablePayments
                                                     where pymts.accounts_receivable_payment_id.Equals(paymentId)
                                                     select pymts)
                                                        .FirstOrDefault();

                if (pymnts != null)
                {
                    paymentReturned = new Payment();
                    paymentReturned.paymentId = pymnts.accounts_receivable_payment_id;
                    paymentReturned.invoiceId = pymnts.invoice_id;
                    paymentReturned.patientId = pymnts.member_id;
                    paymentReturned.paymentAmount = pymnts.payment_amount;
                    paymentReturned.paymentDate = pymnts.payment_date;
                }
            }


            return paymentReturned;
        }

        public List<Bill> getArStatusBills(Bill billPeriods)
        {

            List<Bill> returnedArBills = new List<Bill>();

            try
            {

                if (billPeriods.backupPeriods != null && billPeriods.backupPeriods.Count > 0)
                {

                    List<BillingBackup> backupbills = new List<BillingBackup>();

                    foreach (BillingBackup period in billPeriods.backupPeriods)
                    {

                        backupbills = (
                                        from bilbkup in _icmsDataStagingContext.BillingBackups
                                        where bilbkup.backup_period_id.Equals(period.backup_period_id)
                                        select bilbkup
                                        )
                                        .OrderBy(recDte => recDte.record_date)
                                        .ToList();

                    }


                    if (backupbills != null && backupbills.Count > 0)
                    {

                        foreach (BillingBackup bill in backupbills)
                        {
                            Bill addArBill = new Bill();
                            addArBill.billingBackupId = bill.billing_backup_id;
                            addArBill.billId = (Guid)bill.lcm_record_id;
                            addArBill.lcmInvoiceNumber = bill.LCM_Invoice_Number;
                            addArBill.billingBackupPeriodId = bill.backup_period_id;
                            addArBill.memberId = bill.member_id;
                            addArBill.patientFullName = bill.patient;
                            addArBill.employer = bill.employer;
                            addArBill.recordDate = bill.record_date;
                            addArBill.billCode = bill.time_code;

                            addArBill.billCodeDescription = getCodeDescription((int)bill.time_code);

                            addArBill.billMinutes = bill.time_length;
                            addArBill.employerBillingRate = (decimal)bill.lcm_rate;
                            addArBill.billSentDate = bill.sent_date;

                            returnedArBills.Add(addArBill);
                        }

                    }

                }

            }
            catch (Exception ex)
            {

            }

            return returnedArBills;
        }



        public List<Payment> makeArPayment(Payment pay)
        {

            List<Payment> paymentsReturned = null;

            int result = 0;

            AccountsReceivablePayments payment = new AccountsReceivablePayments();
            payment.member_id = pay.patientId;
            payment.invoice_id = pay.invoiceId;
            payment.bill_type = pay.billType;
            payment.bill_total = pay.billTotal;
            payment.check_number = pay.checkNumber;
            payment.payment_date = pay.paymentDate;
            payment.payment_user_id = pay.paymentUserId;
            payment.payment_amount = pay.paymentAmount;
            payment.creation_date = DateTime.Now;

            _icmsContext.AccountsReceivablePayments.Add(payment);

            result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                List<AccountsReceivablePayments> pymts = (from pays in _icmsContext.AccountsReceivablePayments
                                                          select pays)
                                                            .ToList();

                if (pymts.Count > 0)
                {
                    paymentsReturned = new List<Payment>();


                    foreach (AccountsReceivablePayments pymt in pymts)
                    {
                        Payment paymentToReturn = new Payment();
                        paymentToReturn.paymentId = pymt.accounts_receivable_payment_id;
                        paymentToReturn.patientId = pymt.member_id;
                        paymentToReturn.invoiceId = pymt.invoice_id;
                        paymentToReturn.checkNumber = pymt.check_number;
                        paymentToReturn.paymentDate = pymt.payment_date;


                        paymentsReturned.Add(paymentToReturn);
                    }
                }
            }


            return paymentsReturned;
        }

        public bool deleteArPayment(Payment pay)
        {
            bool removed = false;

            AccountsReceivablePayments pymnt = (from accts in _icmsContext.AccountsReceivablePayments
                                                where accts.accounts_receivable_payment_id.Equals(pay.paymentId)
                                                select accts)
                                                .FirstOrDefault();

            if (pymnt != null)
            {
                _icmsContext.AccountsReceivablePayments.Remove(pymnt);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    removed = true;
                }
            }


            return removed;
        }

        public void UpdateUploadedArPaymentsForBillingBackup(Guid usrId, List<BillingBackup> billsInSystem, List<UnpaidReport> paymentsFromReport)
        {

            Guid lcmrdid = Guid.Empty;
            Guid memid = Guid.Empty;


            //GET DISTINCT MEMBERID'S
            List<Guid> memberIdsInBilling = billsInSystem.Select(mem => (Guid)mem.member_id).Distinct().ToList();

            if (memberIdsInBilling != null && memberIdsInBilling.Count > 0)
            {

                foreach (Guid memberIdInBillList in memberIdsInBilling)
                {

                    //GET ALL BILLS IN THE BILL LIST ASSOCIATED WITH THE CURRENT MEMBER
                    List<string> memberInvoicesInBilling = billsInSystem.Where(memId => memId.member_id.Equals(memberIdInBillList)).Select(inv => inv.LCM_Invoice_Number).Distinct().ToList();

                    if (memberInvoicesInBilling != null && memberInvoicesInBilling.Count > 0)
                    {
                        decimal memberBillTotal = 0;
                        decimal memberPaymentTotal = 0;

                        foreach (string invoice in memberInvoicesInBilling)
                        {

                            //TOTAL THE MEMBER BILLS
                            memberBillTotal += getBillBackupTotalAmountUsingInvoiceNumber(invoice, memberIdInBillList);
                            //TOTAL MEMBER PAYMENTS 
                            memberPaymentTotal += getBillTotalPaymentUsingInvoiceNumber(invoice, memberIdInBillList);
                        }

                        if (memberBillTotal > 0)
                        {
                            //TOTAL MEMBER ACCOUNT BALANCE
                            decimal memberAccountBalance = memberBillTotal - memberPaymentTotal;

                            //MEMBER'S ACCOUNT BALANCE...IF THERE IS A BALANCE REMAINING...APPLY THE PAYMENTS FOUND IN THE DOWNLOADED REPORT TO THE BALANCE
                            if (memberAccountBalance > 0)
                            {

                                //GET THE "CLIENT SPECIFIC" ID OF THE MEMBER FROM BILL (WILL BE USED TO LOOK UP DOWNLOADED PAYMENTS)
                                string participantId = billsInSystem.Where(mem => mem.member_id.Equals(memberIdInBillList)).Select(part => part.memberid).Distinct().FirstOrDefault();

                                if (participantId != null)
                                {
                                    //REMOVE TRAILING ZEROS...****COUNT THE NUMBER OF CHARACTERS BEFORE AND AFTER THE "REMOVE"
                                    if (participantId.EndsWith("00") && participantId.Length > 9)
                                    {
                                        int doublezero = participantId.LastIndexOf("00");
                                        participantId = participantId.Substring(0, doublezero);

                                    }

                                    //TOTAL PAYMENTS IN THE DOWNLOADED REPORT FOR THE MEMBER (PAYMENT MUST MATCH THE "CLIENT SPECIFIC" ID...PULLED FROM THE MEMBER BILL ABOVE)
                                    decimal downloadedMemberPaymentBalance = getUploadedReportPayments(participantId, paymentsFromReport);

                                    //MEMBER DOWNLOADED PAYMENTS...IF THERE ARE ANY DOWNLOADED PAYMENTS...APPLY THOSE DOWNLOAED PAYMENTS TO ANY, AND ALL,
                                    //MEMBER BILLS (STARTING WITH THE OLDEST MEMBER BILL AND MOVING TOWARDS THE NEWEST BILL)
                                    if (downloadedMemberPaymentBalance > 0)
                                    {
                                        ApplyArPayments(usrId, billsInSystem, memberIdInBillList, memberAccountBalance, downloadedMemberPaymentBalance);
                                    }

                                }

                            }
                        }

                    }

                }

            }

        }


        public decimal getUploadedReportPayments(string participantId, List<UnpaidReport> payments)
        {

            decimal returnPayments = 0;

            foreach (UnpaidReport paid in payments)
            {

                string participant = paid.participant;

                if (participant.Equals(participantId))
                {
                    string total = paid.total;

                    if (!string.IsNullOrEmpty(total))
                    {
                        decimal paidAmount = decimal.Parse(total);

                        if (paidAmount > 0)
                        {
                            returnPayments += paidAmount;
                        }
                    }
                }

            }

            return returnPayments;

        }

        public void ApplyArPayments(Guid usrId, List<BillingBackup> billsInSystem, Guid memberId, decimal memberAccountBalance, decimal downloadedMemberPaymentBalance)
        {

            //GET MEMBER BILLS
            List<BillingBackup> memberBillsInSystem = billsInSystem.Where(mem => mem.member_id.Equals(memberId)).OrderBy(crdte => crdte.creation_date).ToList();

            if (memberBillsInSystem != null && memberBillsInSystem.Count > 0)
            {
                foreach (BillingBackup oldestMemberBill in memberBillsInSystem)
                {

                    //IF THE DOWNLOADED PAYMENT BALANCE IS (STILL) GREATER THAN ZERO, USE PAYMENT BALANCE TOWARDS MEMBER ACCOUNT BALANCE (OLDEST BILL FIRST)
                    if (downloadedMemberPaymentBalance > 0)
                    {

                        //GET OLDEST BILL BALANCE
                        decimal oldestBillTotal = getBillBackupTotalAmountUsingInvoiceNumber(oldestMemberBill.LCM_Invoice_Number, (Guid)oldestMemberBill.member_id);
                        //GET OLDEST BILL PAYMENT BALANCE
                        decimal oldestBillPaymentTotal = getBillTotalPaymentUsingInvoiceNumber(oldestMemberBill.LCM_Invoice_Number, (Guid)oldestMemberBill.member_id);


                        if (oldestBillTotal > 0)
                        {
                            //OLDEST BILL BALANCE
                            decimal oldestBillBalance = oldestBillTotal - oldestBillPaymentTotal;


                            if (oldestBillBalance > 0)
                            {
                                decimal paymentTowardsOldestBillBalance = 0;


                                if (downloadedMemberPaymentBalance >= oldestBillBalance)
                                {
                                    //IF THE DOWNLOADED MEMBER PAYMENTS ARE GREATER THAN (OR EQUAL TO) THE OLDEST BILL BALANCE...USE THE OLDEST BILL BALANCE AS
                                    //THE PAYMENT AMOUNT (WILL BE ENTERED IN THE SYSTEM AS A PAYMENT)
                                    paymentTowardsOldestBillBalance = oldestBillBalance;
                                }
                                else
                                {
                                    //IF THE DOWNLOADED MEMBER PAYMENTS ARE LESS THAN THE OLDEST BILL BALANCE...APPLY THE REMAINING AMOUNT OF THE
                                    //DOWNLOADED MEMBER PAYMENTS TO BE USED AS THE PAYMENT AMOUNT (WILL BE ENTERED IN THE SYSTEM AS A PAYMENT)
                                    paymentTowardsOldestBillBalance = downloadedMemberPaymentBalance;
                                }


                                int result = 0;
                                DateTime now = DateTime.Now;

                                //ENTER THE PAYMENT IN THE SYSTEM
                                AccountsReceivablePayments payment = new AccountsReceivablePayments();
                                payment.member_id = oldestMemberBill.member_id;
                                payment.invoice_id = oldestMemberBill.LCM_Invoice_Number;
                                payment.bill_type = "LCM";
                                payment.bill_total = oldestBillTotal;
                                payment.check_number = "";
                                payment.payment_date = now;
                                payment.payment_user_id = usrId;
                                payment.payment_amount = paymentTowardsOldestBillBalance;
                                payment.creation_date = now;
                                payment.comment = "Payment applied via unpaid report download on " + now.ToShortDateString();

                                _icmsContext.AccountsReceivablePayments.Add(payment);

                                result = _icmsContext.SaveChanges();


                                if (result > 0)
                                {
                                    //SUBTRACT THE PAYMENT AMOUNT (JUST ENTERED IN THE SYSTEM) FROM THE DOWNLOADED MEMBER PAYMENTS AMOUNT...
                                    //THE DOWNLOAED MEMBER PAYMENTS AMOUNT WILL BE VERIFIED AT THE BEGINING OF THE FUNCTION
                                    downloadedMemberPaymentBalance -= paymentTowardsOldestBillBalance;
                                }

                            }
                        }

                    }

                }

            }

        }


        public UnpaidReport runUnpaidReport(IFormCollection fileData)
        {
            UnpaidReport returnedReport = null;

            try
            {
                if (fileData.Files.Count.Equals(1) && fileData.Files[0].Name.Equals("unpaidXls") &&
                    fileData.Files[0].Length > 0 && fileData.Files[0].ContentType.Equals("application/vnd.ms-excel"))
                {
                    using (Stream fileStream = fileData.Files[0].OpenReadStream())
                    {

                        if (fileData.TryGetValue("tpa", out var tpaid) && int.TryParse(tpaid, out int intTpaId))
                        {

                            List<BillingBackup> bills = getBillingBackup_200_DaysOld(intTpaId);
                            List<UnpaidReport> payments = getUploadedReportPayments(fileStream, bills);

                            StandardService standServ = new StandardService(_icmsContext, _aspnetContext);
                            Tpas tpa = standServ.GetTpa(intTpaId);

                            if (payments.Count > 0)
                            {

                                if (bills != null && bills.Count > 0)
                                {

                                    Guid guidUserId = Guid.Empty;

                                    if (fileData.TryGetValue("usr", out var usrId))
                                    {
                                        Guid.TryParse(usrId, out guidUserId);
                                    }

                                    UpdateUploadedArPaymentsForBillingBackup(guidUserId, bills, payments);

                                }


                                int reportId = getNextRptId();


                                if (reportId > 0)
                                {
                                    payments.OrderBy(pay => pay.status);

                                    verifyUploadedReport(reportId, ref payments, bills);

                                    returnedReport = generateUnpaidBillsReport(payments, tpa);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return returnedReport;
        }

        public ArStatusReport runArStatusReport(List<Bill> bills)
        {

            ArStatusReport returnedReport = null;

            try
            {

                if (bills != null && bills.Count > 0)
                {

                    List<Bill> reportBills = new List<Bill>();

                    foreach (Bill bill in bills)
                    {

                        var exists = reportBills.Where(addbill => addbill.lcmInvoiceNumber.Contains(bill.lcmInvoiceNumber)).Distinct().FirstOrDefault();

                        if (exists == null)
                        {

                            bill.billTotalAmount = getBillBackupTotalAmountUsingInvoiceNumber(bill.lcmInvoiceNumber, (Guid)bill.memberId);
                            bill.billTotalPaymentAmount = getBillTotalPaymentUsingInvoiceNumber(bill.lcmInvoiceNumber, (Guid)bill.memberId);

                            if (bill.billTotalAmount.ToString().All(char.IsNumber) && bill.billTotalPaymentAmount.ToString().All(char.IsNumber))
                            {
                                bill.billRemainingBalance = bill.billTotalAmount - bill.billTotalPaymentAmount;
                            }
                            else
                            {
                                bill.billRemainingBalance = 0;
                            }

                            Bill addArBill = new Bill();
                            addArBill.lcmInvoiceNumber = bill.lcmInvoiceNumber;
                            addArBill.billingBackupPeriodId = bill.billingBackupPeriodId;
                            addArBill.memberId = bill.memberId;
                            addArBill.patientFullName = bill.patientFullName;
                            addArBill.employer = bill.employer;
                            addArBill.billTotalAmount = bill.billTotalAmount;
                            addArBill.billTotalPaymentAmount = bill.billTotalPaymentAmount;
                            addArBill.billRemainingBalance = bill.billRemainingBalance;

                            reportBills.Add(addArBill);

                        }
                        else
                        {
                            Bill newBill = new Bill();
                        }
                    }


                    returnedReport = generateArStatusReport(reportBills);




                }
            }
            catch (Exception ex)
            {

            }

            return returnedReport;
        }




        private bool NoteIsInBilling(Note note)
        {
            bool inBilling = true;

            //System.Diagnostics.Debug.WriteLine("member ID: "" + nte.member_id.ToString());

            var noteInBillingTable = (
                from billNotes in _icmsDataStagingContext.LcmBillingWorktables
                where billNotes.member_id.Equals(note.memberId)
                && !billNotes.record_date.Equals(note.recordDate)
                select billNotes
                ).ToList();


            inBilling = false;

            return inBilling;
        }

        private Note GetMemberNoteForBillingTextField(Note note)
        {
            Note returnNote = new Note();


            var notes = (
                from memNotes in _icmsContext.MemberNotes
                where memNotes.member_id.Equals(note.memberId)
                && memNotes.record_date.Equals(note.recordDate)
                orderby memNotes.record_seq_num
                select memNotes
                ).ToList();


            if (notes.Count > 0)
            {
                StringBuilder concatNote = new StringBuilder();


                foreach (MemberNotes nte in notes)
                {
                    concatNote.Append(nte.evaluation_text);

                    if (!returnNote.caseOwnerId.Equals(Guid.Empty) && !nte.user_updated.Equals(Guid.Empty))
                    {
                        Note caseownerNote = GetMemberNoteForBillingCaseOwner(nte);

                        returnNote.caseOwnerId = caseownerNote.caseOwnerId;
                        returnNote.caseOwnerName = caseownerNote.caseOwnerName;
                    }
                }

                returnNote.noteText = concatNote.ToString();
            }


            return returnNote;
        }

        private Note GetMemberNoteForBillingCaseOwner(MemberNotes note)
        {
            Note returnNote = new Note();

            IcmsUser caseowner = (
                from memnt in _icmsContext.MemberNotes

                join casown in _icmsContext.CaseOwners
                on memnt.user_updated equals casown.system_user_id into casown
                from caseOwnr in casown.DefaultIfEmpty()

                join sysusr in _icmsContext.SystemUsers
                on caseOwnr.system_user_id equals sysusr.system_user_id into sysusr
                from systemusr in sysusr.DefaultIfEmpty()

                where memnt.user_updated.Equals(note.user_updated)

                select new IcmsUser
                {
                    UserId = (Guid)memnt.user_updated,
                    FullName = systemusr.system_user_first_name + " " + systemusr.system_user_last_name
                }
            ).FirstOrDefault();


            if (!caseowner.UserId.Equals(Guid.Empty))
            {
                returnNote.caseOwnerId = caseowner.UserId;
                returnNote.caseOwnerName = caseowner.FullName;
            }

            return returnNote;
        }

        private bool VerifyBillNotInBilling(Bill bill)
        {
            bool isInBilling = false;


            var lcmRecordId = (
                from lcmBills in _icmsDataStagingContext.LcmBillingWorktables
                where lcmBills.member_id.Equals(bill.memberId)
                select lcmBills.lcm_record_id
                )
                .Distinct()
                .FirstOrDefault();

            if (!lcmRecordId.Equals(Guid.Empty))
            {
                isInBilling = true;
            }


            return isInBilling;
        }

        private Bill AddLcmBillingWorktableItem(Bill bill)
        {

            Bill returnBillAdded = new Bill();
            returnBillAdded.billId = Guid.Empty;

            try
            {
                DateTime dteNow = DateTime.Now;
                PatientService patServ = new PatientService(_icmsContext, _aspnetContext);
                Patient pat = patServ.GetDbmsMember(bill.memberId.ToString());


                if (pat.PatientId.Equals(bill.memberId))
                {
                    Note memberNote = GetPatientMemberNotes((Guid)bill.memberId, (DateTime)bill.recordDate);


                    if (memberNote.memberId.Equals(bill.memberId))
                    {
                        DateTime memBirth = DateTime.MinValue;
                        memBirth = (DateTime)pat.dateOfBirth;

                        LcmBillingWorktable lcmWrkTbl = new LcmBillingWorktable();
                        lcmWrkTbl.member_id = bill.memberId;
                        lcmWrkTbl.record_date = bill.recordDate;

                        lcmWrkTbl.print_lcm = false;
                        lcmWrkTbl.date_updated = dteNow;
                        lcmWrkTbl.comments = "";

                        lcmWrkTbl.patient = pat.firstName + " " + pat.lastName;
                        lcmWrkTbl.dob = memBirth.ToShortDateString();
                        lcmWrkTbl.employer = pat.employerName;
                        lcmWrkTbl.tpa = pat.tpaName;
                        lcmWrkTbl.case_manager = memberNote.caseOwnerName;
                        lcmWrkTbl.notes = memberNote.noteText;
                        lcmWrkTbl.time_code = memberNote.billingId;
                        lcmWrkTbl.time_length = memberNote.billingMinutes;

                        lcmWrkTbl.lcm_rate = (Double)pat.employerLcmRate;
                        lcmWrkTbl.user_updated = bill.userId;


                        _icmsDataStagingContext.LcmBillingWorktables.Add(lcmWrkTbl);
                        int result = _icmsDataStagingContext.SaveChanges();

                        if (result > 0)
                        {
                            if (!lcmWrkTbl.lcm_record_id.Equals(Guid.Empty))
                            {
                                returnBillAdded.billId = lcmWrkTbl.lcm_record_id;
                                returnBillAdded.memberId = lcmWrkTbl.member_id;
                                returnBillAdded.recordDate = lcmWrkTbl.record_date;
                            }
                        }
                    }
                }

                return returnBillAdded;
            }
            catch (Exception ex)
            {
                return returnBillAdded;
            }

        }

        private Note GetPatientMemberNotes(Guid memberId, DateTime recordDate)
        {
            List<Note> memberNote = new List<Note>();


            memberNote = (
                            from note in _icmsContext.MemberNotes

                            join sysusr in _icmsContext.SystemUsers
                            on note.user_updated equals sysusr.system_user_id into sysusr
                            from systemusr in sysusr.DefaultIfEmpty()

                            where note.member_id.Equals(memberId)
                            && note.record_date.Equals(recordDate)
                            select new Note
                            {
                                noteText = note.evaluation_text,
                                noteSequenceNumber = note.record_seq_num,
                                billingId = note.billing_id,
                                billingMinutes = note.RN_notes,
                                caseOwnerId = note.user_updated,
                                caseOwnerName = systemusr.system_user_first_name + " " + systemusr.system_user_last_name
                            }
                            )
                            .OrderBy(seq => seq.noteSequenceNumber)
                            .ToList();


            Note returnNote = new Note();


            if (memberNote.Count > 0)
            {
                foreach (Note nte in memberNote)
                {
                    returnNote.noteText += nte.noteText;


                    if (returnNote.billingId == null || returnNote.billingId.Equals(0))
                    {
                        returnNote.billingId = nte.billingId;
                    }

                    if (returnNote.billingMinutes == null || returnNote.billingMinutes.Equals(0))
                    {
                        returnNote.billingMinutes = nte.billingMinutes;
                    }

                    if (returnNote.caseOwnerId == null || returnNote.caseOwnerId.Equals(Guid.Empty))
                    {
                        returnNote.caseOwnerId = nte.caseOwnerId;
                        returnNote.caseOwnerName = nte.caseOwnerName;
                    }
                }


                returnNote.memberId = memberId;
                returnNote.recordDate = recordDate;
            }


            return returnNote;
        }

        private string createBillInvoicePdf_Initialize(string directory, List<Bill> bills)
        {


            string html = @"<html>
                            <head>
                            <style>
                                .logo {
                                    width: 95%;
                                    padding-top: 5px;
                                }
                                .logo-holder {
                                    vertical-align: bottom;
                                    background: rgba(25, 56, 102, .8);
                                    width: 100px;
                                    height: 50px;
                                }
                                .logo-image {
                                    width: 100px;
                                    height: 50px; 
                                    opacity: .1;
                                }
                                .logo-address {
                                    text-align: right;
                                    vertical-align: bottom;
                                }
                                .logo-address-text {
                                    text-align: right;
                                    font-size: .8em;
                                    color: #193866; 
                                }
                                .bill-title {
                                    background-color: #193866;
                                    color: white;
                                    font-family: copperplate, georgia, garamond, verdana;
                                    font-size: 2em;
                                    text-align: center;
                                    border-radius: 2px;
                                    box-shadow: 4px 5px 10px #6e6e6e;
                                }
                                .bill-dates {
                                    width: 95%;    
                                    font-size: .8em;
                                    vertical-align: top;
                                }
                                .bill-dates-due {
                                    text-align: right;
                                }
                                .patientsBills {
                                    width: 95%;
                                    border-radius: 2px;
                                    box-shadow: 4px 5px 10px #6e6e6e;
                                }
                                .employer {
                                    background-color: #17a600;
                                    color: white;
                                    font-family: tahoma, trebuchet ms, helvetica;
                                    font-size: .9em;
                                    font-variant: small-caps;
                                    font-weight: bold;
                                    border: 1px solid #17a600;
                                    border-radius: 2px;
                                    padding-left: 5px;
                                    padding-right: 5px;
                                    padding-top: 2px;
                                    padding-bottom: 2px;
                                }
                                .patient {
                                    background-color: #EDF406;
                                    font-family: arial, times new roman, courier;
                                    font-size: 1.2em;
                                    font-style: italic;
                                    border-radius: 2px;
                                    box-shadow: 4px 5px 10px #6e6e6e;
                                    padding-left: 5px;
                                    padding-top: 2px;
                                    padding-bottom: 2px;
                                }
                                .alt-id {
                                    background-color: #EDF406;
                                    font-family: arial, times new roman, courier;
                                    font-size: .8em;
                                    font-style: italic;
                                }
                                .invoicenum {
                                    color: #990606;
                                    font-family: tahoma, trebuchet ms, helvetica;
                                    font-size: .8em;
                                    font-variant: small-caps;
                                    border: 1px solid #990606;
                                    border-radius: 2px;
                                    box-shadow: 4px 5px 10px #6e6e6e;
                                    padding-left: 10px;
                                    padding-right: 10px;
                                    padding-top: 2px;
                                    padding-bottom: 2px;
                                }
                                .pat-tot-header {
                                    font-family: tahoma, trebuchet ms, helvetica;
                                    color: #204E91;
                                    font-size: .8em;
                                    font-weight: bold;
                                } 
                                .pat-tot {
                                    background-color: #193866;
                                    color: white;
                                    font-size: 1em;
                                    font-weight: bold;
                                    border-radius: 2px;     
                                    box-shadow: 4px 5px 10px #6e6e6e;
                                }
                                .pat-tot.center {
                                    text-align: center;
                                }
                                .bill-item-header {
                                    font-family: tahoma, trebuchet ms, helvetica;
                                    color: #204E91;
                                    font-size: .8em;
                                }                
                                .bill-item-header.left {
                                    text-align: left;
                                }
                                .bill-item {
                                    font-family: tahoma, trebuchet ms, helvetica;
                                    font-size: .7em;
                                    font-style: italic;
                                    color: #023278;
                                }
                                .bill-item.center {
                                    text-align: center;
                                }
                                .bill-tot-header {
                                    font-family: tahoma, trebuchet ms, helvetica;
                                    background-color: #193866;
                                    color: white;
                                    font-size: .9em;
                                    font-weight: bold;
                                    padding-left: 5px;
                                }
                                .bill-tot-header.right {
                                    text-align: right;
                                    padding-right: 60px;
                                }
                                .bill-total {
                                    width: 95%;
                                    background-color: #193866;
                                    color: white;
                                    border-radius: 2px;
                                    box-shadow: 4px 5px 10px #6e6e6e;                                        
                                }
                                .pat-separate {
                                    background-color: black;
                                    color: black;
                                    height: 3px;
                                }
                                .bill-item-separate {
                                    background-color: #204E91;
                                    color: #204E91;
                                    height: .9px;
                                }
                                .bill-tot-separate {
                                    background-color: #193866;
                                    color: black;
                                    height: 2px;
                                }
                            </style>
                            </head>
                            <body>

                            <div>
                                <table class='logo'>
                                <tr>
                                    <td class='logo-holder'>
                                    <img src='" + directory + "/dbms_logo.png' alt='DBMS Logo' class='logo-image'>" +
                                        "</td> " +
                                        "<td class='logo-address'> " +
                                        "<span class='logo-address-text'>5975 Castle Creek Pky N. Dr. Indianapolis, IN 46250 - (800) 728-0327</span> " +
                                        "</td> " +
                                    "</tr> " +
                                    "<tr> " +
                                        "<td colspan='2'> " +
                                        "<div class='invoice-header'>" +
                                            "<h1 class='bill-title'>Billing Invoice - LCM</h1>" +
                                        "</div>" +
                                        "</td>" +
                                    "</tr>" +
                                    "</table>" +
                                "</div>" +


                                "<table class='bill-dates'>" +
                                    "<tr>" +
                                    "<td><b>Invoice Period:</b> " + bills[0].billingStartDate.Value.ToShortDateString() + " - " + bills[0].billingEndDate.Value.ToShortDateString() + "</td>" +
                                    "<td class='bill-dates-due'><b>Due Date:</b> " + DateTime.Now.ToShortDateString() + "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "<br>";

            return html;
        }

        private string createBillInvoicePdf_Items(List<Bill> bills)
        {
            try
            {

                StringBuilder html = new StringBuilder();
                Guid patientIdCheck = Guid.Empty;


                //table to hold the bills
                html.Append("<table id='tblBills' width='95%'>");


                foreach (Bill bill in bills)
                {

                    //get a member's ID
                    Guid patientid = (Guid)bill.memberId;

                    if (!patientid.Equals(Guid.Empty))
                    {
                        if (!patientIdCheck.Equals(patientid))
                        {
                            string patientFullName = bill.patientFullName;

                            string altId = (bill.billingPatient.egpMemberId != null && !string.IsNullOrEmpty(bill.billingPatient.egpMemberId)) ?
                                bill.billingPatient.egpMemberId : bill.billingPatient.claimsEnrollmentId;


                            if (altId != null && !string.IsNullOrEmpty(altId))
                            {
                                if (altId.Contains("YWSHP"))
                                {
                                    altId = altId.Substring(altId.IndexOf("YWSHP") + 5);
                                }

                                if (altId.Length > 9)
                                {
                                    if (altId.LastIndexOf("00").Equals(altId.Length - 2))
                                    {
                                        altId = altId.Substring(0, altId.Length - 2);
                                    }
                                }
                            }



                            //If InStr(Right(strReturningId, 2), "00") Then
                            //    strReturningId = Left(strReturningId, Len(strReturningId) - 2)
                            //End If


                            string employer = bill.employer;
                            string invoiceNumber = bill.lcmInvoiceNumber;

                            html.Append("<table id='tblPatientBills' class='patientsBills'>");

                            html.Append(" <tr>");
                            html.Append("   <td>");
                            html.Append("     <table id=tblPat_" + patientid.ToString() + " width='95%'>");
                            html.Append("       <tr>");
                            html.Append("         <td>");
                            html.Append($@"<span class='patient'>{patientFullName}</ span > ");
                            html.Append($@"<span class='alt-id'>{altId}</span>  ");
                            html.Append($@"<span class='employer'>{employer}</span> ");
                            //html.Append("");
                            //html.Append("");
                            html.Append("         </td>");
                            html.Append("         <td>");
                            html.Append("             <span class='invoicenum'>" + invoiceNumber + "</span>");
                            html.Append("         </td>");
                            html.Append("       </tr>");
                            html.Append("     </table>");
                            html.Append("     <hr class='pat-separate'>");

                            html.Append(createInvoiceBillableLineItem(bills, bill));

                            html.Append("     <br>");
                            html.Append("   </td>");
                            html.Append(" </tr>");

                            html.Append("</table>");

                            html.Append("     <br>");

                            patientIdCheck = patientid;
                        }
                    }
                }


                html.Append("</table>");


                return html.ToString();
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        private string createInvoiceBillableLineItem(List<Bill> checkBills, Bill bill)
        {
            try
            {


                StringBuilder tblRow = new StringBuilder();
                StringBuilder tblStart = new StringBuilder();
                StringBuilder tblEnd = new StringBuilder();
                StringBuilder tblBills = new StringBuilder();
                decimal totalAmount = 0;

                if (checkBills.Count > 0)
                {

                    tblStart.Append("     <table id=tblPatBills_" + bill.memberId.ToString() + " width='95%'>");
                    tblStart.Append("       <tr><th class='bill-item-header left'>Code - Description</th>");
                    tblStart.Append("       <th class='bill-item-header'>Mins</th>");
                    tblStart.Append("       <th class='bill-item-header'>Rate</th>");
                    tblStart.Append("       <th class='bill-item-header'>Amount</th></tr>");
                    tblStart.Append("       <tbody>");

                    tblEnd.Append("       </table>");


                    tblRow.Append("<tr><td colspan='4'><hr class='bill-item-separate'></td></tr>");

                    decimal billCodeAmount = 0;
                    int nextBillCode = 0;
                    Guid lastBillId = Guid.Empty;
                    int lastBillCode = 0;
                    string lastBillDescription = "";
                    double lastMinutes = 0;
                    decimal lastEmployerRate = 0;

                    if (bill.memberId.Equals(new Guid("00454b31-3496-4974-9a94-9fc0d71d41cc")))
                    {
                        Debug.Print("here");
                    }

                    foreach (Bill chkBill in checkBills)
                    {

                        if (chkBill.memberId.Equals(bill.memberId))
                        {

                            if (nextBillCode.Equals(0))
                            {
                                nextBillCode = ((chkBill.billCode.HasValue) ? (int)chkBill.billCode : 0);
                            }
                            else
                            {

                                int currentBillCode = ((chkBill.billCode.HasValue) ? (int)chkBill.billCode : 0);

                                if (nextBillCode != currentBillCode)
                                {

                                    tblRow.Append(createInvoiceBillableHtml(chkBill.billId, ((chkBill.billCode.HasValue) ? (int)chkBill.billCode : 0), 
                                                                            chkBill.billCodeDescription,
                                                                            ((chkBill.billMinutes.HasValue) ? (double)chkBill.billMinutes : 0), 
                                                                            ((chkBill.employerBillingRate.HasValue) ?(decimal)chkBill.employerBillingRate : 0),
                                                                            billCodeAmount));

                                    nextBillCode = ((chkBill.billCode.HasValue) ? (int)chkBill.billCode : 0);
                                    billCodeAmount = 0;
                                }
                            }

                            lastBillId = chkBill.billId;
                            lastBillCode = ((chkBill.billCode.HasValue) ? (int)chkBill.billCode : 0);
                            lastBillDescription = chkBill.billCodeDescription;
                            lastMinutes = ((chkBill.billMinutes.HasValue) ? (double)chkBill.billMinutes : 0);
                            lastEmployerRate = ((chkBill.employerBillingRate.HasValue) ? (decimal)chkBill.employerBillingRate : 0);

                            if (chkBill.billMinutes > 0 && chkBill.employerBillingRate > 0)
                            {
                                decimal billMinutes = ((chkBill.billMinutes.HasValue) ? Convert.ToDecimal((double)chkBill.billMinutes) : 0);
                                decimal employerRate = ((chkBill.employerBillingRate.HasValue) ? (decimal)chkBill.employerBillingRate : 0);

                                billCodeAmount += (billMinutes / 60 * employerRate);
                            }

                            totalAmount += billCodeAmount;

                        }
                    }

                    if (billCodeAmount > 0)
                    {
                        tblRow.Append(createInvoiceBillableHtml(lastBillId, lastBillCode, lastBillDescription, lastMinutes, lastEmployerRate, billCodeAmount));
                    }

                    tblRow.Append("       <tr>");
                    tblRow.Append("         <td colspan='4'><hr class='bill-item-separate'></td>");
                    tblRow.Append("       </tr>");
                    tblRow.Append("       <tr>");
                    tblRow.Append("         <td class='pat-tot-header'>Total</td>");
                    tblRow.Append("         <td></td>");
                    tblRow.Append("         <td></td>");
                    tblRow.Append("         <td class='pat-tot center'>" + "$" + Math.Round(totalAmount, 2).ToString("#.00") + "</td>");
                    tblRow.Append("       </tr>");

                    tblStart.Append("       </tbody>");

                    tblBills.Append(tblStart.ToString() + tblRow.ToString() + tblEnd.ToString());

                }


                return tblBills.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private string createInvoiceBillableHtml(Guid billId, int billCode, string billDescription, double billMinutes, 
                                                 decimal employerRate, decimal billAmount)
        {

            try
            {

                string html = "";

                html = "       <tr id=rowBill_" + billId + "'>";
                html += "         <td class='bill-item'>" + billCode + " - " + billDescription + "</td>";
                html += "         <td class='bill-item center'>" + billMinutes + "</td>";
                html += "         <td class='bill-item center'>" + employerRate + "</td>";
                html += "         <td class='bill-item center'>" + "$" + Math.Round(billAmount, 2).ToString("#.00") + "</td>";
                html += "       </tr>";

                return html;
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        private string createBillInvoicePdf_Finalize(List<Bill> bills)
        {

            decimal totalAmount = 0;

            StringBuilder tbl = new StringBuilder();

            tbl.Append("    <br>");
            tbl.Append("    <table id='tblFinalBillAmount' class='bill-total' width='95%'>");
            tbl.Append("       <tr>");
            tbl.Append("         <td class='bill-tot-header'>Invoice Total</td>");
            tbl.Append("         <td></td>");
            tbl.Append("         <td></td>");

            foreach (Bill chkBill in bills)
            {
                decimal billMinutes = ((chkBill.billMinutes.HasValue) ? Convert.ToDecimal((double)chkBill.billMinutes) : 0);
                decimal employerRate = ((chkBill.employerBillingRate.HasValue) ? (decimal)chkBill.employerBillingRate : 0);

                decimal billAmount = (billMinutes / 60 * employerRate);
                totalAmount += billAmount;
            }

            tbl.Append("         <td class='bill-tot-header right'>" + "$" + Math.Round(totalAmount, 2).ToString("#,###,###.00") + "</td>");
            tbl.Append("       </tr>");
            tbl.Append("    </table>");
            tbl.Append("  </body>");
            tbl.Append("</html>");


            return tbl.ToString();
        }

        private string createArStatusReportPdf_Initialize()
        {
            StringBuilder html = new StringBuilder();

            DateTime nowDate = DateTime.Now;

            html.Append("<html>");
            html.Append("  <head>");
            html.Append("    <style>");
            html.Append(createArStatusReportPdf_Styles());
            html.Append("    </style>");
            html.Append("  </head>");
            html.Append("  <body>");


            return html.ToString();
        }

        private string createArStatusReportPdf_Styles()
        {
            string css = @"
    .header {
        text-align: center;
        width: 100%;
    }

    .header-txt {
        font-size: 25px;
        font-weight: bold;
    }

    .billing-period-container {
        width: 98%;
        border: 1px solid black;
        border-radius: 4px;
        box-shadow: 4px 5px 10px #6e6e6e;
        margin-bottom: 20px;
        padding: 10px;
    }

    .billing-period-txt-container {
        width: 100%;
        text-align: center;
        margin-bottom: 10px;
    }

    .billing-period-txt {
        font-size: 20px;
        font-weight: 300;
    }

    .employer-container {
        border: 1px solid blue;
        border-radius: 4px;
        box-shadow: 4px 5px 10px #6e6e6e;
        margin: 10px;
    }

    .employer-name-container {
        width: 100%;
        text-align: center;
        background-color: green;
    }

    .employer-total-name-container {
        width: 100%;
        text-align: center;
        background-color: green;
    }

        .employer-total-name-container.small {
            width: 99%;
            background-color: #f7f4b7;
            border: 1px solid green;
        }

    .employer-txt {
        font-size: 15px;
        font-weight: 600;
        color: yellow;
    }

    .employer-total-txt {
                font-size: 15px;
                font-weight: 600;
                color: yellow;
    }

        .employer-total-txt.small {
            font-size: 11px;
            color: green;
        }

    .bills-container {
        width: 100%;
        text-align: center;
    }

    .bills-table {
        margin-top: 15px;
        border-collapse: collapse;
        width: 100%;
    }

    .bills-table thead tr.underline {
        border-bottom: 1px solid #0600b0;
    }

        .bills-table th {
            text-align: center;
            font-size: 16px;
            font-weight: bold;
            color: #0600b0;
        }

            .bills-table th.left {
                text-align: left;
            }

            .bills-table th.right {
                text-align: right;
            }


        .bills-table td {
            text-align: center;
            font-size: 14px;
            font-weight: 400;
        }

            .bills-table td.left {
                text-align: left;
            }

            .bills-table td.right {
                text-align: right;
            }

    .bills-total-container {
        display: table;
        table-layout: fixed;
        width:100%;
    }

    .bills-total-row {
        display: table-row;
        width: 100%;
    }

    .bills-total-employer-row {
        display: table-row;
        width: 100%;
        background-color: green;
        text-align: center;
    }

    .bills-total-employer-cell {        
        display: table-cell;
        padding: 10px;
        margin: 5px;
        width: 100%;
    }


    .bills-total-cell{        
        display: table-cell;
        padding: 10px;
        margin: 5px;
        width: 30%;
    }

    .total-bill-txt {
        color: blue;
    }

    .total-paid-txt {
        color: green;
    }
                
    .total-bal-txt {
        color: red;
    }

";

            return css;
        }

        private string createArStatusReportPdf_Items(List<Bill> bills)
        {
            StringBuilder html = new StringBuilder();

            decimal periodBillsTotal = 0;
            decimal periodPaymentTotal = 0;

            html.Append("    <div id='arStatusReportContext' class='billing-content'>");

            html.Append("      <div id='header' class='header'>");
            html.Append("        <span class='header-txt'>Accounts Receivable Status</span>");
            html.Append("      </div>");

            IEnumerable<string> backPeriodId = bills.Select(bill => bill.billingBackupPeriodId).Distinct();

            //GO THROUGH EACH BILLING PERIOD
            foreach (string period in backPeriodId)
            {

                IEnumerable<string> employers = bills.Where(bill => bill.billingBackupPeriodId.Equals(period)).Select(bill => bill.employer).Distinct();


                html.Append("      <div id='billingPeriodContainer' class='billing-period-container'>");

                html.Append("        <div id='billingPeriodText' class='billing-period-txt-container'>");
                html.Append("          <span class='billing-period-txt'>" + period + "</span>");
                html.Append("        </div>");

                int employerCount = 0;
                int patientCount = 0;

                //THEN GO THROUGH EACH EMPLOYER
                foreach (string employer in employers)
                {

                    employerCount++;

                    if (employers != null)
                    {

                        if (employerCount.Equals(6))
                        {
                            html.Append("<div style='page-break-after: always;'></div>");
                            html.Append("        <div id='employerContainer' class='employer-container' style='margin-top: 10px;'>");
                        }
                        else
                        {
                            html.Append("        <div id='employerContainer' class='employer-container'>");
                        }

                        html.Append("          <div id='employerNameContainer' class='employer-name-container'>");
                        html.Append("            <span class='employer-txt'>" + employer + "</span>");
                        html.Append("          </div>");


                        decimal employerBillsTotal = 0;
                        decimal employerPaymentTotal = 0;
                        decimal employerBalance = 0;


                        IEnumerable<Bill> employerPeriodBills = bills.Where(bill => bill.billingBackupPeriodId.Equals(period) && bill.employer.Equals(employer))
                                                                        .Distinct()
                                                                        .ToList();

                        if (employerPeriodBills != null)
                        {

                            html.Append("          <div id='billsContainer' class='bills-container'>");
                            html.Append("            <table id='tblBills' class='bills-table'>");
                            html.Append("              <thead>");
                            html.Append("                <tr class='underline'>");
                            html.Append("                  <th class='left'>NAME</th>");
                            html.Append("                  <th>INVOICE</th>");
                            html.Append("                  <th>BILL</th>");
                            html.Append("                  <th>PAID</th>");
                            html.Append("                  <th>BALANCE</th>");
                            html.Append("                </tr>");
                            html.Append("              </thead>");
                            html.Append("              <tbody>");


                            foreach (Bill employerBill in employerPeriodBills)
                            {

                                patientCount++;

                                html.Append("                <tr>");
                                html.Append("                  <td class='left'>" + employerBill.patientFullName + "</td>");
                                html.Append("                  <td>" + employerBill.lcmInvoiceNumber + "</td>");
                                html.Append("                  <td><span class='total-bill-txt'>$" + employerBill.billTotalAmount.Value.ToString("###,###,##0.00") + "</td>");
                                html.Append("                  <td><span class='total-paid-txt'>$" + employerBill.billTotalPaymentAmount.Value.ToString("###,###,##0.00") + "</td>");
                                html.Append("                  <td><span class='total-bal-txt'>$" + employerBill.billRemainingBalance.Value.ToString("###,###,##0.00") + "</td>");
                                html.Append("                </tr>");

                                employerBillsTotal += (decimal)employerBill.billTotalAmount;
                                employerPaymentTotal += (decimal)employerBill.billTotalPaymentAmount;

                                periodBillsTotal += (decimal)employerBill.billTotalAmount;
                                periodPaymentTotal += (decimal)employerBill.billTotalPaymentAmount;

                            }

                            html.Append("              </tbody>");
                            html.Append("            </table>");
                            // id='billsContainer'
                            html.Append("          </div>");

                            employerBalance = employerBillsTotal - employerPaymentTotal;

                            html.Append("          <div id='employerNameContainer' class='employer-total-name-container small' style='margin-left: 3px;'>");
                            html.Append("            <span class='employer-total-txt small'>" + employer + " TOTALS</span>");
                            html.Append("          </div>");

                            html.Append("          <div id='billsTotalContainer' class='bills-total-container'>");
                            html.Append("            <div id='billsTotalRow' class='bills-total-row'>");
                            html.Append("              <div class='bills-total-cell'>");
                            html.Append("                <label>BILL</label>&nbsp;<span class='total-bill-txt'>$" + employerBillsTotal.ToString("###,###,##0.00") + "</span>");
                            html.Append("              </div>");

                            html.Append("              <div class='bills-total-cell'>");
                            html.Append("                <label>PAID</label>&nbsp;<span class='total-paid-txt'>$" + employerPaymentTotal.ToString("###,###,##0.00") + "</span>");
                            html.Append("              </div>");

                            html.Append("              <div class='bills-total-cell'>");
                            html.Append("                <label>BALANCE</label>&nbsp;<span class='total-bal-txt'>$" + employerBalance.ToString("###,###,##0.00") + "</span>");
                            html.Append("              </div>");
                            html.Append("            </div>");
                            html.Append("          </div>");

                            // id='billsContainer'
                            html.Append("        </div>");

                        }
                    }
                }

                html.Append("<div style='page-break-after: always;'></div>");
                html.Append(" <div class='bills-total-container'>");
                html.Append("  <div class='bills-total-row'>");
                html.Append("   <div class='bills-total-cell'>");
                html.Append("    <span>LCM TOTALS: </span>");
                html.Append("   </div>");
                html.Append("   <div class='bills-total-cell'>");
                html.Append("    <span>" + periodBillsTotal.ToString("###,###,##0.00") + "</span>");
                html.Append("   </div>");
                html.Append("  </div>");
                html.Append("  <div class='bills-total-row'>");
                html.Append("   <div class='bills-total-cell'>");
                html.Append("    <span>LCM PAYMENTS: </span>");
                html.Append("   </div>");
                html.Append("   <div class='bills-total-cell'>");
                html.Append("    <span>" + periodPaymentTotal.ToString("###,###,##0.00") + "</span>");
                html.Append("   </div>");
                html.Append("  </div>");
                html.Append("  <div class='bills-total-row'>");
                html.Append("   <div class='bills-total-cell'>");
                html.Append("    <span>LCM BALANCE: </span>");
                html.Append("   </div>");
                html.Append("   <div class='bills-total-cell'>");
                html.Append("    <span>" + (periodBillsTotal - periodPaymentTotal).ToString("###,###,##0.00") + "</span>");
                html.Append("   </div>");
                html.Append("  </div>");
                html.Append(" </div>");


                // id='billingPeriodContainer'
                html.Append("      </div>");

            }

            // id='arStatusReportContext'
            html.Append("    </div");


            return html.ToString();
        }

        private string createArStatusReportPdf_Finalize()
        {

            StringBuilder html = new StringBuilder();

            html.Append("  </body>");
            html.Append("</html>");

            return html.ToString();
        }

        private string createUnpaidReportPdf_Initialize(List<UnpaidReport> payments, Tpas tpa)
        {
            StringBuilder html = new StringBuilder();

            DateTime nowDate = DateTime.Now;

            html.Append("<html>");
            html.Append("  <head>");
            html.Append("    <style>");
            html.Append(createUnpaidReportPdf_Styles());
            html.Append("    </style>");
            html.Append("  </head>");
            html.Append("  <body>");

            html.Append("  <div class='report-header'>");
            html.Append("    <span class='report-header-txt'>LCM INVOICE STATUS REPORT</span>");
            html.Append("  </div>");

            html.Append("  <div class='download-for-title'>");
            html.Append("    <span class='downloaded-for-title-txt'>Downloaded for " + tpa.tpa_name + "</span>");
            html.Append("  </div>");

            html.Append("  <div class='print-date'>");
            html.Append("    <span class='print-date-txt'>Print Date " + nowDate.ToShortDateString() + "</span>");
            html.Append("  </div>");


            return html.ToString();
        }

        private string createUnpaidReportPdf_Styles()
        {
            string css = @"

    .report-header {
        text-align: center;
        width: 100%;
    }

    .report-header-txt {
        font-size: 25px;
        font-weight: bold;
    }

    .download-for-title {
        text-align: center;
        width: 100%;
        margin-bottom: 15px;
    }
         
    .downloaded-for-title-txt {
        font-size: 18px;
        font-weight: 300;
    }

    .print-date {
        text-align: right;
        width: 100%;
        margin-bottom: 10px;
    }

    .print-date-txt {
        font-size: 16px;
        font-weight: 300;
    }

    .payment-status-container {
        width: 98%;
        border: 1px solid black;
        border-radius: 2px;
        box-shadow: 4px 5px 10px #6e6e6e;
        margin-bottom: 20px;
        padding: 10px;
    }

    .yellow-bar {
        background-color: yellow;
        height: 20px;
        width: 100%;
        text-align: center;
    }

    .yellow-bar-txt {
        font-size: 14px;
        font-weight: 400;
    }

    .payment-list {
        width: 100%;
        text-align: center;
    }

    .payment-tbl {
        margin-top: 15px;
        border-collapse: collapse;
        width: 100%;
    }

    .collapse-head {    
            visibility: hidden;
    }

    .payment-tbl th {
        text-align: center;
        font-size: 16px;
        font-weight: bold;
        color: blue;
    }

    .payment-tbl thead tr.underline {
        border-bottom: 1px solid blue;
    }

    .hidden {
        color: white;
    }

    .payment-tbl th.left {
        text-align: left;
    }

    .payment-tbl th.right {
        text-align: right;
    }

    .payment-tbl td {
        text-align: center;
        font-size: 14px;
        font-weight: 400;
    }

    .payment-tbl td.left {
        text-align: left;
    }

    .payment-tbl td.right {
        text-align: right;
    }

    .blue-divider {
        border-bottom: 1px solid blue;
        width: 100%;
    }


    .totals {
        width: 100%;
        display:flex;
        flex-direction: row;
        margin-bottom: 15px;
    }

    .totals-lbl {
        flex: auto;
    }

    .totals-label-txt {
        font-size: 14px;
    }

    .totals-amount {
        flex: auto;
    }

    .totals-amount-txt {
        color: red;
        font-size: 14px;
    }
";

            return css;
        }

        private string createUnpaidReportPdf_Items(List<UnpaidReport> payments)
        {
            StringBuilder html = new StringBuilder();

            IEnumerable<string> status = payments.Select(p => p.status).Distinct();

            foreach (string payStatus in status)
            {

                decimal statusChargeTotal = 0;
                decimal statusTotal = 0;


                html.Append("  <div class='payment-status-container'>");

                html.Append("    <div class='yellow-bar'>");
                html.Append("      <span class='yellow-bar-txt'>");
                html.Append(payStatus);
                html.Append("      </span>");
                html.Append("    </div>");

                html.Append("    <div id='payment-list-" + payStatus + "' class='payment-list'>");
                html.Append("      <table id='payment-tbl-" + payStatus + "' class='payment-tbl'>");
                html.Append("        <thead>");
                html.Append("          <tr class='underline'>");
                html.Append("            <th class='left'>Last</th>");
                html.Append("            <th class='left'>First</th>");
                html.Append("            <th>ID</th>");
                html.Append("            <th>Invoice Number</th>");
                html.Append("            <th class='right'>Charge</th>");
                html.Append("            <th class='right'>Total</th>");
                html.Append("            <th>Service Date</th>");
                html.Append("            <th>Paid Date</th>");
                html.Append("          </tr>");
                html.Append("        </thead>");
                html.Append("        <tbody>");


                IEnumerable<UnpaidReport> statusPayments = payments.Select(p => p)
                                                                    .Where(p => p.status.Equals(payStatus))
                                                                    .OrderBy(p => p.claimantLastName)
                                                                    .ThenBy(p => p.claimantFirstName);

                int lngLastName = 0;
                int intFirstName = 0;
                int intId = 0;
                int intInvoiceNumber = 0;
                string strLastName = "";
                string strFirstName = "";
                string strId = "";
                string strInvoiceNumber = "";


                if (statusPayments.Count() > 0)
                {
                    decimal chargeAmount = 0;
                    decimal totalAmount = 0;


                    foreach (UnpaidReport statusPay in statusPayments)
                    {

                        if (statusPay.invoiceNumber == null)
                        {
                            statusPay.invoiceNumber = "";
                        }

                        string serviceDate = (!string.IsNullOrEmpty(statusPay.serviceDate))
                                                    ?
                                                    DateTime.Parse(statusPay.serviceDate).ToShortDateString()
                                                    :
                                                    "N/A";

                        string paidDate = (!string.IsNullOrEmpty(statusPay.paidDate))
                                                    ?
                                                    DateTime.Parse(statusPay.paidDate).ToShortDateString()
                                                    :
                                                    "N/A";

                        decimal charge = (!string.IsNullOrEmpty(statusPay.charge))
                                            ?
                                            decimal.Parse(statusPay.charge)
                                            :
                                            0;

                        decimal total = (!string.IsNullOrEmpty(statusPay.total))
                                            ?
                                            decimal.Parse(statusPay.total)
                                            :
                                            0;
                        if (statusPay.claimantLastName.Length > lngLastName)
                        {
                            lngLastName = statusPay.claimantLastName.Length;
                            strLastName = statusPay.claimantLastName;
                        }

                        if (statusPay.claimantFirstName.Length > intFirstName)
                        {
                            intFirstName = statusPay.claimantFirstName.Length;
                            strFirstName = statusPay.claimantFirstName;
                        }

                        if (statusPay.participant.Length > intId)
                        {
                            intId = statusPay.participant.Length;
                            strId = statusPay.participant;
                        }

                        if (statusPay.invoiceNumber.Length > intInvoiceNumber)
                        {
                            intInvoiceNumber = statusPay.invoiceNumber.Length;
                            strInvoiceNumber = statusPay.invoiceNumber;
                        }

                        html.Append("      <tr>");
                        html.Append("        <td class='left'>" + statusPay.claimantLastName + "</td>");
                        html.Append("        <td class='left'>" + statusPay.claimantFirstName + "</td>");
                        html.Append("        <td>" + statusPay.participant + "</td>");
                        html.Append("        <td>" + statusPay.invoiceNumber + "</td>");
                        html.Append("        <td class='right'>$" + charge.ToString("###,###,###.00") + "</td>");
                        html.Append("        <td class='right'>$" + total.ToString("###,###,###.00") + "</td>");
                        html.Append("        <td>" + serviceDate + "</td>");
                        html.Append("        <td>" + paidDate + "</td>");
                        html.Append("      </tr>");

                        if (decimal.TryParse(statusPay.charge, out chargeAmount))
                        {
                            statusChargeTotal += chargeAmount;
                        }

                        if (decimal.TryParse(statusPay.total, out totalAmount))
                        {
                            statusTotal += totalAmount;
                        }


                    }
                }

                html.Append("        </tbody>");
                html.Append("      </table>");
                html.Append("    </div>");

                html.Append("    <div class='blue-divider'></div>");

                html.Append("    <div id='payment-list-" + payStatus + "-total' class='payment-list'>");
                html.Append("      <table id='payment-tbl-" + payStatus + "-total' class='payment-tbl'>");
                html.Append("          <thead>");
                html.Append("            <tr>");
                html.Append("              <th class='collapse-head'><span class='hidden'>Last</span></th>");
                html.Append("              <th class='collapse-head'><span class='hidden'>First</span></th>");
                html.Append("              <th class='collapse-head'><span class='hidden'>ID</span></th>");
                html.Append("              <th class='collapse-head'><span class='hidden'>Invoice Number</span></th>");
                html.Append("              <th class='collapse-head'><span class='hidden'>Charge</span></th>");
                html.Append("              <th class='collapse-head'><span class='hidden'>Total</span></th>");
                html.Append("              <th class='collapse-head'><span class='hidden'>Service Date</span></th>");
                html.Append("              <th class='collapse-head'><span class='hidden'>Paid Date</span></th>");
                html.Append("            </tr>");
                html.Append("        </thead>");
                html.Append("        <tbody>");
                html.Append("          <tr>");
                html.Append("            <td class='left'><span class='hidden'>" + strLastName + "</span></td>");
                html.Append("            <td class='left'><span class='hidden'>" + strFirstName + "</span></td>");
                html.Append("            <td><span class='hidden'>" + strId + "</span></td>");
                html.Append("            <td><span class='hidden'>" + strInvoiceNumber + "</span.</td>");
                html.Append("            <td class='right'><span class='totals-amount-txt'>$" + statusChargeTotal.ToString("###,###,###.00") + "</span></td>");
                html.Append("            <td class='right'><span class='totals-amount-txt'>$" + statusTotal.ToString("###,###,###.00") + "</span></td>");
                html.Append("            <td></td>");
                html.Append("            <td></td>");
                html.Append("          </tr>");
                html.Append("        </tbody>");
                html.Append("       </table>");
                html.Append("     </div>");


                //html.Append("     <div class='totals'>");
                //html.Append("       <div class='totals-lbl'>");
                //html.Append("         <span class='totals-label-txt'>Totals</span>");
                //html.Append("       </div>");
                //html.Append("       <div class='totals-amount'>");
                //html.Append("         <span class='totals-amount-txt'>$" + statusChargeTotal.ToString() + "</span>");
                //html.Append("         <span class='totals-amount-txt'>$" + statusTotal.ToString() + "</span>");
                //html.Append("       </div>");
                //html.Append("     </div>");
                html.Append("   </div>");

            }

            return html.ToString();
        }

        private string createUnpaidReportPdf_Finalize(List<UnpaidReport> payments)
        {

            StringBuilder html = new StringBuilder();





            html.Append("  </body>");
            html.Append("</html>");

            return html.ToString();
        }

        private string generatePdfOnEdrive(string html)
        {
            string fileLocation = "";

            try
            {

                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "Billing Invoice - LCM",
                    Out = @"E:\DbmsInvoice\dbms_invoice.pdf"
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "--CONFIDENTIAL--" }
                };

                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };


                _converter.Convert(pdf);


                fileLocation = @"E:\DbmsInvoice\dbms_invoice.pdf";


                return fileLocation;
            }
            catch (Exception ex)
            {
                return fileLocation;
            }
        }

        private FileContentResult generatePdfToFile(string html, string docTitle, string fileName)
        {
            try
            {

                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = docTitle,
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "--CONFIDENTIAL--" }
                };

                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };


                var file = _converter.Convert(pdf);

                FileContentResult invoiceFileContentResult = new FileContentResult(file, "application/pdf");
                invoiceFileContentResult.FileDownloadName = fileName;


                return invoiceFileContentResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private decimal getBillBackupTotalAmountUsingInvoiceNumber(string invoiceNumber, Guid memberId)
        {
            decimal returnBillAmount = 0;


            List<BillingBackup> backupbills = (
                    from billbkup in _icmsDataStagingContext.BillingBackups
                    where billbkup.LCM_Invoice_Number.Equals(invoiceNumber)
                    && billbkup.member_id.Equals(memberId)
                    select billbkup
                ).ToList();


            if (backupbills.Count > 0)
            {
                foreach (BillingBackup bill in backupbills)
                {
                    if (bill.time_length > 0 && bill.lcm_rate > 0)
                    {
                        double billAmount = ((double)bill.time_length / 60) * (double)bill.lcm_rate;

                        returnBillAmount += (decimal)billAmount;
                    }
                }
            }


            return returnBillAmount;
        }

        private decimal getBillTotalPaymentUsingInvoiceNumber(string invoiceNumber, Guid memberId)
        {
            decimal returnPaymentAmount = 0;


            List<AccountsReceivablePayments> payments = (
                                                            from arpay in _icmsContext.AccountsReceivablePayments
                                                            where arpay.invoice_id.Equals(invoiceNumber)
                                                            && arpay.member_id.Equals(memberId)
                                                            select arpay
                                                        )
                                                        .ToList();


            if (payments.Count > 0)
            {
                foreach (AccountsReceivablePayments pay in payments)
                {
                    if (pay.payment_amount > 0)
                    {
                        returnPaymentAmount += (decimal)pay.payment_amount;
                    }
                }
            }


            return returnPaymentAmount;
        }

        private string getUserName(Guid userid)
        {
            string returnUserName = "";

            SystemUser usr = (from sysusr in _icmsContext.SystemUsers
                              where sysusr.system_user_id.Equals(userid)
                              select sysusr
                                ).FirstOrDefault();

            if (usr != null)
            {
                returnUserName = usr.system_user_first_name + " " + usr.system_user_last_name;
            }
            else
            {
                returnUserName = getAspNetUsername(userid);
            }


            return returnUserName;
        }

        private string getAspNetUsername(Guid userid)
        {
            string returnUserName = "";

            AspNetUsers aspmemusr = (from aspmem in _aspnetContext.AspNetMemberships

                                     join aspusr in _aspnetContext.AspNetUsers
                                     on aspmem.UserId equals aspusr.UserId into aspnetUsr
                                     from aspnetusr in aspnetUsr.DefaultIfEmpty()

                                     where aspmem.UserId.Equals(userid)
                                     select aspnetusr
                                ).FirstOrDefault();


            if (aspmemusr != null)
            {
                returnUserName = aspmemusr.UserName;
            }

            return returnUserName;
        }

        private List<Bill> loadBillsTotalPayments(List<Bill> bills)
        {
            List<Bill> returnedList = new List<Bill>();


            foreach (Bill bill in bills)
            {
                List<AccountsReceivablePayments> payments = (from pay in _icmsContext.AccountsReceivablePayments
                                                             where pay.member_id.Equals(bill.memberId)
                                                             && pay.invoice_id.Equals(bill.lcmInvoiceNumber)
                                                             select pay
                                                                ).ToList();

                if (payments.Count > 0)
                {
                    decimal billTotal = 0;


                    foreach (AccountsReceivablePayments paid in payments)
                    {
                        billTotal += (decimal)paid.payment_amount;
                    }


                    bill.billTotalPaymentAmount = billTotal;
                }


                returnedList.Add(bill);

            }


            return returnedList;
        }

        private List<Bill> loadBillsRemainingBalance(List<Bill> bills)

        {
            List<Bill> returnedList = new List<Bill>();


            foreach (Bill bill in bills)
            {
                bill.billRemainingBalance = bill.billTotalAmount - bill.billTotalPaymentAmount;

                if (bill.billRemainingBalance > 0)
                {
                    Debug.WriteLine("bill.billRemainingBalance: " + bill.billRemainingBalance);
                }



                returnedList.Add(bill);
            }


            return returnedList;
        }

        private string getCodeDescription(int codeId)
        {
            string descriptionReturned = "";

            LcmBillingCodes code = (from lcmcodes in _icmsContext.LcmBillingCodes
                                    where lcmcodes.billing_id.Equals(codeId)
                                    select lcmcodes)
                                    .FirstOrDefault();

            if (code != null)
            {
                descriptionReturned = code.billing_description;
            }


            return descriptionReturned;
        }

        private string getMergedMemberNoteForMemberInBackup(Bill note)
        {
            string noteReturned = "";

            Guid memberId = (Guid)note.memberId;


            MergedMemberReference mergeMember = (from mermemref in _icmsContext.MergedMemberReferences
                                                 where mermemref.incorrect_member_id.Equals(memberId)
                                                 select mermemref).FirstOrDefault();

            if (mergeMember != null && (mergeMember.correct_member_id != null && !mergeMember.correct_member_id.Equals(Guid.Empty)))
            {
                List<Note> billingNote = new List<Note>();
                DateTime recordDate = (DateTime)note.recordDate;

                billingNote = (
                                from notes in _icmsContext.MemberNotes
                                where notes.member_id.Equals(mergeMember.correct_member_id)
                                && notes.record_date.Equals(recordDate)
                                select new Note
                                {
                                    noteText = notes.evaluation_text,
                                    noteSequenceNumber = notes.record_seq_num
                                }
                                )
                                .OrderBy(seq => seq.noteSequenceNumber)
                                .ToList();


                if (billingNote.Count > 0)
                {
                    foreach (var nte in billingNote)
                    {
                        noteReturned += nte.noteText;
                    }
                }
            }


            return noteReturned;
        }

        private List<BillingBackup> getBillingBackup_200_DaysOld(int tpaId)
        {
            List<BillingBackup> bills = new List<BillingBackup>();

            string tpaName = (from tpas in _icmsContext.Tpas
                              where tpas.tpa_id.Equals(tpaId)
                              select tpas.tpa_name
                                )
                                .FirstOrDefault();

            if (!string.IsNullOrEmpty(tpaName))
            {
                DateTime now = DateTime.Now;
                DateTime daysOld_120 = now.AddDays(-200);


                bills = (from bilbkup in _icmsDataStagingContext.BillingBackups
                         where bilbkup.tpa.Equals(tpaName)
                         && (bilbkup.creation_date >= daysOld_120 && bilbkup.creation_date <= now)
                         select bilbkup
                            )
                            .ToList();

            }


            return bills;
        }

        private List<UnpaidReport> getUploadedReportPayments(Stream fileStream, List<BillingBackup> bills)
        {
            List<UnpaidReport> payments = new List<UnpaidReport>();


            try
            {
                if (bills.Count > 0)
                {
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;
                        application.DefaultVersion = ExcelVersion.Excel2016;
                        application.UseFastRecordParsing = true;

                        fileStream.Position = 0;
                        IWorkbook workBook = excelEngine.Excel.Workbooks.Open(fileStream);

                        if (workBook != null)
                        {
                            if (workBook.ActiveSheet.Rows.Count() > 1)
                            {
                                IWorksheet workSheet = workBook.ActiveSheet;
                                IRange[] rows = workSheet.Rows;
                                IRange[] ranges = workSheet.Cells;
                                int unpdId = 1;


                                foreach (IRange row in rows)
                                {

                                    if (row.AddressLocal != "A1:O1")
                                    {
                                        UnpaidReport paymentItem = new UnpaidReport();


                                        foreach (IRange cell in row.Cells)
                                        {
                                            if (cell.Value2 != null && !string.IsNullOrEmpty(cell.Value2.ToString()))
                                            {
                                                AssignUnpaidReportItemToPayment(cell, ref paymentItem);
                                            }
                                        }


                                        if (!string.IsNullOrEmpty(paymentItem.participant))
                                        {
                                            //if (IsInSystem(ref paymentItem))
                                            //{
                                            paymentItem.unpaidId = unpdId;
                                            payments.Add(paymentItem);

                                            unpdId += 1;
                                            //}
                                        }

                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return payments;

        }

        private bool IsInSystem(ref UnpaidReport paymentItem)
        {
            Member mem = null;

            try
            {
                string participant = paymentItem.participant;

                mem = (
                                from mems in _icmsContext.Patients

                                join memEnroll in _icmsContext.MemberEnrollments
                                on mems.member_id equals memEnroll.member_id into memEnroll
                                from memEnrollment in memEnroll.DefaultIfEmpty()

                                where memEnrollment.claims_enrollment_id.Equals(participant)
                                select mems
                                )
                                .FirstOrDefault();


                if (mem != null && !mem.member_id.Equals(Guid.Empty))
                {
                    paymentItem.memberId = mem.member_id;
                    return true;
                }
                else
                {

                    string first = paymentItem.claimantFirstName;
                    string last = paymentItem.claimantLastName;

                    mem = (
                            from mems in _icmsContext.Patients

                            join memEnroll in _icmsContext.MemberEnrollments
                            on mems.member_id equals memEnroll.member_id into memEnroll
                            from memEnrollment in memEnroll.DefaultIfEmpty()

                            where mems.member_first_name.Equals(first)
                            && mems.member_last_name.Equals(last)
                            select mems
                            )
                            .FirstOrDefault();

                    if (mem != null && !mem.member_id.Equals(Guid.Empty))
                    {
                        paymentItem.memberId = mem.member_id;
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        private void AssignUnpaidReportItemToPayment(IRange cell, ref UnpaidReport paymentItem)
        {
            string cellValue = cell.Value2.ToString();


            if (cell.AddressLocal.Equals($"A{cell.Row}"))
            {
                paymentItem.groupNum = cellValue;
            }
            else if (cell.AddressLocal.Equals($"B{cell.Row}"))
            {
                paymentItem.participant = cellValue;
            }
            else if (cell.AddressLocal.Equals($"C{cell.Row}"))
            {
                paymentItem.dept = cellValue;
            }
            else if (cell.AddressLocal.Equals($"D{cell.Row}"))
            {
                paymentItem.claimantLastName = cellValue;
            }
            else if (cell.AddressLocal.Equals($"E{cell.Row}"))
            {
                paymentItem.claimantFirstName = cellValue;
            }
            else if (cell.AddressLocal.Equals($"F{cell.Row}"))
            {
                paymentItem.claim = cellValue;
            }
            else if (cell.AddressLocal.Equals($"G{cell.Row}"))
            {
                paymentItem.procedure = cellValue;
            }
            else if (cell.AddressLocal.Equals($"H{cell.Row}"))
            {
                paymentItem.provLOC = cellValue;
            }
            else if (cell.AddressLocal.Equals($"I{cell.Row}"))
            {
                paymentItem.status = cellValue;
            }
            else if (cell.AddressLocal.Equals($"J{cell.Row}"))
            {
                paymentItem.charge = cellValue;
            }
            else if (cell.AddressLocal.Equals($"K{cell.Row}"))
            {
                paymentItem.inelig = cellValue;
            }
            else if (cell.AddressLocal.Equals($"L{cell.Row}"))
            {
                paymentItem.total = cellValue;
            }
            else if (cell.AddressLocal.Equals($"M{cell.Row}"))
            {
                paymentItem.serviceDate = cellValue;
            }
            else if (cell.AddressLocal.Equals($"N{cell.Row}"))
            {
                paymentItem.paidDate = cellValue;
            }
            else if (cell.AddressLocal.Equals($"O{cell.Row}"))
            {
                paymentItem.complete = cellValue;
            }

        }

        private int getNextRptId()
        {
            int reportId = 0;

            RptNextUniqueId rptId = (
                                        from nxtrptid in _icmsContext.RptNextUniqueIds
                                        select nxtrptid
                                    )
                                    .FirstOrDefault();

            int tmpRptId = (int)rptId.nxt_uniqueID;

            if (tmpRptId > 0)
            {
                rptId.nxt_uniqueID = tmpRptId + 1;

                _icmsContext.RptNextUniqueIds.Update(rptId);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {
                    reportId = tmpRptId;
                }
            }


            return reportId;
        }

        private void verifyUploadedReport(int reportId, ref List<UnpaidReport> payments, List<BillingBackup> bills)
        {

            foreach (UnpaidReport money in payments)
            {

                money.reportId = reportId;


                if (money.status.Equals("8"))
                {
                    foreach (BillingBackup bill in bills)
                    {
                        if (money.memberId.Equals(bill.member_id))
                        {
                            money.invoiceNumber = bill.LCM_Invoice_Number;
                        }
                    }
                }
            }

        }


        private ArStatusReport generateArStatusReport(List<Bill> bills)
        {

            ArStatusReport returnReport = new ArStatusReport();


            StringBuilder html = new StringBuilder();

            html.Append(createArStatusReportPdf_Initialize());

            html.Append(createArStatusReportPdf_Items(bills));

            html.Append(createArStatusReportPdf_Finalize());

            FileContentResult pdfArStatusRpt = generatePdfToFile(html.ToString(), "Ar Status Report", "arStatusRpt.pdf");


            returnReport.reportFileUrlLocation = "";
            returnReport.reportPdf = pdfArStatusRpt.FileContents;
            returnReport.reportBase64 = Convert.ToBase64String(pdfArStatusRpt.FileContents);
            returnReport.reportContentType = pdfArStatusRpt.ContentType;
            returnReport.reportFileName = pdfArStatusRpt.FileDownloadName;


            return returnReport;
        }

        private UnpaidReport generateUnpaidBillsReport(List<UnpaidReport> payments, Tpas tpa)
        {

            UnpaidReport returnReport = new UnpaidReport();


            StringBuilder html = new StringBuilder();

            html.Append(createUnpaidReportPdf_Initialize(payments, tpa));

            html.Append(createUnpaidReportPdf_Items(payments));

            html.Append(createUnpaidReportPdf_Finalize(payments));

            FileContentResult pdfUnpaidRpt = generatePdfToFile(html.ToString(), "Unpaid Report", "unpaidRpt.pdf");


            returnReport.reportFileUrlLocation = "";
            returnReport.reportPdf = pdfUnpaidRpt.FileContents;
            returnReport.reportBase64 = Convert.ToBase64String(pdfUnpaidRpt.FileContents);
            returnReport.reportContentType = pdfUnpaidRpt.ContentType;
            returnReport.reportFileName = pdfUnpaidRpt.FileDownloadName;


            return returnReport;
        }

        private Patient getBillingPatient(Guid memberId)
        {
            Patient returnPatient = new Patient();

            PatientService patServ = new PatientService(_icmsContext, _aspnetContext);
            returnPatient = patServ.GetPatientsUsingId(memberId.ToString());

            return returnPatient;
        }

    }

}
 
