using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;


namespace eCareApi.Services
{
    public class StandardService : IStandard
    {
        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspNetContext;

        public StandardService(IcmsContext icmsContext, AspNetContext aspNetContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
        }

        public IEnumerable<State> GetStates()
        {
            IEnumerable<State> states = Enumerable.Empty<State>();

            states = _icmsContext.States
                        .OrderBy(state => state.state_name);

            return states;
        }

        public List<State> GetStatesList()
        {
            List<State> states = null;

            states = (

                    from ste in _icmsContext.States
                    orderby ste.state_name
                    select ste
                )
                .ToList();

            return states;
        }

        public IEnumerable<Tpas> GetTpas()
        {
            IEnumerable<Tpas> tpa = Enumerable.Empty<Tpas>();

            tpa = _icmsContext.Tpas
                        .OrderBy(t => t.tpa_name);

            return tpa;
        }

        public Tpas GetTpa(int id)
        {
            Tpas returnTpa = new Tpas();

            returnTpa = (from tpa in _icmsContext.Tpas
                         where tpa.tpa_id.Equals(id)
                         select tpa
                         ).FirstOrDefault();

            return returnTpa;
        }

        public Email GetTpaEmailBillingOptions(int tpaId)
        {

            Email emailOptions = null;

            emailOptions = (

                    from tpas in _icmsContext.Tpas

                    join tpaPass in _icmsContext.TpaEmailPasswords
                    on tpas.tpa_id equals tpaPass.tpa_id into tpaPassword
                    from tpaPasses in tpaPassword.DefaultIfEmpty()

                    join umEmail in _icmsContext.UmBillingAutoGenerateInvoiceses
                    on tpas.tpa_id equals umEmail.tpa_id into umAutoEmail
                    from umEmails in umAutoEmail.DefaultIfEmpty()

                    where tpas.tpa_id.Equals(tpaId)
                    select new Email
                    {

                        emailAddress = tpas.tpa_email,
                        ccEmailList = umEmails.email_cc_list,
                        attachmentPassword = tpaPasses.password
                    }
                )
                .FirstOrDefault();

            return emailOptions;
        }

        public IcmsUser getAspUser(Guid userId)
        {
            IcmsUser user = null;

            AspNetUsers aspnetUsr = (
                                        from aspusr in _aspNetContext.AspNetUsers
                                        where aspusr.UserId.Equals(userId)
                                        select aspusr
                                     )
                                     .FirstOrDefault();

            if (aspnetUsr != null)
            {
                user = new IcmsUser();

                user.UserId = aspnetUsr.UserId;
                user.FullName = aspnetUsr.UserName;
            }

            return user;
        }

        public IcmsUser getIcmsUser(Guid userId)
        {
            IcmsUser user = null;

            SystemUser icmsUsr = (
                                    from icmsuser in _icmsContext.SystemUsers
                                    where icmsuser.system_user_id.Equals(userId)
                                    select icmsuser
                                    )
                                    .FirstOrDefault();

            if (icmsUsr != null)
            {
                user = new IcmsUser();

                user.UserId = icmsUsr.system_user_id;
                user.FullName = icmsUsr.system_user_first_name + " " + icmsUsr.system_user_last_name;
                user.FirstName = icmsUsr.system_user_first_name;
                user.LastName = icmsUsr.system_user_last_name;
            }

            return user;
        }        

        public List<SimsErRelationship> getRelationships()
        {
            List<SimsErRelationship> relationships = null;

            relationships = (
                                from relation in _icmsContext.SimsErRelationships
                                orderby relation.name
                                select relation
                            )
                            .Distinct()
                            .ToList();

            return relationships;
        }

        public List<HospitalMaritalStatusTypes> getMaritalStatuses()
        {

            List<HospitalMaritalStatusTypes> maritalStatus = null;

            maritalStatus = (

                    from marStat in _icmsContext.HospitalMaritalStatusTypeses
                    orderby marStat.marital_type_name
                    select marStat
                )
                .ToList();

            return maritalStatus;
        }

        public List<PhoneType> getPhoneTypes()
        {

            List<PhoneType> phoneTypes = null;

            phoneTypes = (

                    from phnTypes in _icmsContext.PhoneTypes
                    orderby phnTypes.label
                    select phnTypes
                )
                .ToList();

            return phoneTypes;
        }

        public List<Employer> getDbmsEmployers()
        {

            List<Employer> employers = null;

            employers = (

                    from emply in _icmsContext.Employers
                    where emply.active_flag.Equals(true)
                    orderby emply.employer_name
                    select emply
                )
                .ToList();

            return employers;
        }

        public List<HospitalRace> getHospitalRaces()
        {

            List<HospitalRace> races = null;

            races = (

                    from rces in _icmsContext.HospitalRaces
                    orderby rces.race_name
                    select rces
                )
                .ToList();

            return races;
        }

        public List<Hospital> getHospitals()
        {

            List<Hospital> hospitals = null;

            hospitals = (

                    from hosp in _icmsContext.Hospitals
                    orderby hosp.name
                    select hosp
                )
                .ToList();

            return hospitals;
        }

        public List<Languages> getLanguages()
        {

            List<Languages> languages = null;

            languages = (

                    from langs in _icmsContext.Languageses
                    where (langs.disabled.Equals(false) || !langs.disabled.HasValue)
                    orderby langs.language_name
                    select langs
                )
                .ToList();

            return languages;
        }

        public List<NewlyIdentifiedCmMemberCaseStatus> getCmCaseStatus()
        {

            List<NewlyIdentifiedCmMemberCaseStatus> caseStatus = null;

            caseStatus = (

                    from cseStats in _icmsContext.NewlyIdentifiedCmMemberCaseStatuses
                    where (cseStats.disabled == 0 || !cseStats.disabled.HasValue)
                    orderby cseStats.description
                    select cseStats
                )
                .ToList();

            return caseStatus;
        }

        public List<IcmsUser> getCmCaseOwners()
        {
            List<IcmsUser> caseOwners = null;

            Guid sysRoleId = new Guid("A3B1DAEF-E201-4B0A-B624-46A5E39212EF");

            caseOwners = (
                            from icmsuser in _icmsContext.SystemUsers

                            join usrrole in _icmsContext.SystemUserRoles
                            on icmsuser.system_user_id equals usrrole.system_user_id

                            where usrrole.system_role_id.Equals(sysRoleId)
                            && !icmsuser.user_inactive_flag
                            && icmsuser.data_admin_flag 
                            && icmsuser.security_admin_flag
                            && (icmsuser.discipline_id.Equals(53) || icmsuser.client_services_admin_flag == 1)
                            select new IcmsUser
                            {
                                UserId = icmsuser.system_user_id,
                                FirstName = icmsuser.system_user_first_name,
                                LastName = icmsuser.system_user_last_name,
                                FullName = icmsuser.system_user_first_name + " " + icmsuser.system_user_last_name
                            }
                        )
                        .OrderBy(nurse => nurse.LastName)
                        .ThenBy(nurse => nurse.FirstName)
                        .ToList();

            return caseOwners;
        }

        public List<CaseType> getCmCaseTypes()
        {

            List<CaseType> caseTypes = null;

            caseTypes = (

                    from cseTyps in _icmsContext.CaseTypes
                    where cseTyps.disable_flag.Equals(false)
                    orderby cseTyps.case_type_descr
                    select cseTyps
                )
                .ToList();

            return caseTypes;
        }



        public List<AssessItem> getLoaderTableRows(string tableName, string idColumn, string descriptionColumn)
        {
            List<AssessItem> returnTableRows = null;
            switch (tableName)
            {
                case "HOSPITAL_TEMPERATURE_SITE":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalTemperatureSites
                                        where dbTable.deleted.Equals(false) || dbTable.deleted == null
                                        orderby dbTable.temperature_site
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.temperature_site,
                                            sourceLoaderId = dbTable.hospital_temperature_site_id
                                        }
                                      )
                                      .ToList();
                    break;

                case "HOSPITAL_PULSE_RHYTHM":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalPulseRhythms
                                        where dbTable.deleted.Equals(false) || dbTable.deleted == null
                                        orderby dbTable.pulse_rhythm
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.pulse_rhythm,
                                            sourceLoaderId = dbTable.hospital_pulse_rhythm_id
                                        }
                                      )
                                      .ToList();
                    break;
                    
                case "HOSPITAL_FI02_LEVELS":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalFi02Levelses
                                        orderby dbTable.hospital_fi02_levels_id
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.liters_per_minute,
                                            sourceLoaderId = dbTable.hospital_fi02_levels_id
                                        }
                                      )
                                      .ToList();
                    break;

                case "STATE":
                    returnTableRows = (
                                        from dbTable in _icmsContext.States
                                        orderby dbTable.state_name
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.state_name,
                                            sourceLoaderTextId = dbTable.state_abbrev
                                        }
                                      )
                                      .ToList();
                    break;

                case "SIMS_ER_RELATIONSHIP":
                    returnTableRows = (
                                        from dbTable in _icmsContext.SimsErRelationships
                                        orderby dbTable.name
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.name,
                                            sourceLoaderId = dbTable.sims_er_relationship_id
                                        }
                                      )
                                      .Distinct()
                                      .OrderBy(dbsourc => dbsourc.sourceLoaderDescription)
                                      .ToList();
                    break;

                case "HOSPITAL_ADMISSION_SOURCE":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalAdmissionSources
                                        orderby dbTable.admission_source_name
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.admission_source_name,
                                            sourceLoaderId = dbTable.hosptial_admission_source_id
                                        }
                                      )                                      
                                      .ToList();
                    break;

                case "HOSPITAL_INTUBATION_METHOD":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalIntubationMethods
                                        orderby dbTable.intubation_method
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.intubation_method,
                                            sourceLoaderId = dbTable.hosptial_intubation_method_id
                                        }
                                      )
                                      .ToList();
                    break;

                case "HOSPITAL_INTUBATION_CORMACK_GRADE":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalIntubationCormackGrades
                                        orderby dbTable.intubation_grade
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.intubation_grade,
                                            sourceLoaderId = dbTable.hosptial_intubation_cormack_grade_id
                                        }
                                      )
                                      .ToList();

                    break;

                case "HOSPITAL_ENDOTRACHEAL_TUBE_TYPE":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalEndotrachealTubeTypes
                                        orderby dbTable.tube_type
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.tube_type,
                                            sourceLoaderId = dbTable.hosptial_endotracheal_tube_type_id
                                        }
                                      )
                                      .ToList();
                    break;

                case "HOSPITAL_SQUEEZE_PUSH_STRENGTH":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalSqueezePushStrengths
                                        orderby dbTable.squeeze_push_strength
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.squeeze_push_strength,
                                            sourceLoaderId = dbTable.hospital_squeeze_push_strength_id
                                        }
                                      )
                                      .ToList();
                    break;

                case "HOSPITAL_TRACHEAL_SUCTION_METHOD":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalTrachealSuctionMethods
                                        orderby dbTable.suction_method
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.suction_method,
                                            sourceLoaderId = dbTable.hosptial_tracheal_suction_method_id
                                        }
                                      )
                                      .ToList();
                    break;

                case "HOSPITAL_VENTILATION_TYPE":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalVentilationTypes
                                        orderby dbTable.ventilation_type
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.ventilation_type,
                                            sourceLoaderId = dbTable.hosptial_ventilation_type_id
                                        }
                                      )
                                      .ToList();
                    break;

                case "HOSPITAL_VENTILATION_MODE":
                    returnTableRows = (
                                        from dbTable in _icmsContext.HospitalVentilationModes
                                        orderby dbTable.ventilation_mode
                                        select new AssessItem
                                        {
                                            sourceLoaderDescription = dbTable.ventilation_mode,
                                            sourceLoaderId = dbTable.hosptial_ventilation_mode_id
                                        }
                                      )
                                      .ToList();
                    break;
            }


            return returnTableRows;
        }



        public Email emailBillingInvoice(Email invoice)
        {

            Email emailInvoice = null;

            if (!string.IsNullOrEmpty(invoice.emailAddress) && invoice.emailAttachment != null)
            {

                EmailsOutbound newEmail = new EmailsOutbound();

                string path = Directory.GetCurrentDirectory();

                if (!string.IsNullOrEmpty(path))
                {

                    byte[] attachmentFile = null;
                    string attachmentFileName = "";

                    path += "\\email\\billing\\";
                    string fileName = "billing invoice.zip";

                    ZipFile zip = createEmailAttachmentZipBilling(path, fileName, invoice);


                    if (zip != null)
                    {
                        attachmentFile = File.ReadAllBytes(path + fileName);
                        attachmentFileName = fileName;
                    }
                    else
                    {
                        attachmentFile = invoice.emailAttachment;
                        attachmentFileName = invoice.emailAttachmentName;
                    }

                    newEmail.creation_date = DateTime.Now;
                    newEmail.user_id = invoice.usr;
                    newEmail.email_address = invoice.emailAddress;
                    newEmail.email_cc_list = invoice.ccEmailList;
                    newEmail.email_message = "Good Day, " + Environment.NewLine + Environment.NewLine +
                                             "The attached file is billing related information from DBMS Health, your Medical Management Company. Should you " +
                                             "have any questions, please do NOT respond to this email.Please send all questions to " +
                                             "Accounting2@dbmshealth.com" + Environment.NewLine + Environment.NewLine +
                                             "Thank you" + Environment.NewLine + Environment.NewLine +
                                             "Contact Clinical Director at:" + Environment.NewLine +
                                             "T: (317) 582 - 1200" + Environment.NewLine +
                                             "www.dbmshealth.com" + Environment.NewLine + Environment.NewLine +
                                             "CONFIDENTIALITY NOTICE: The documents accompanying this facsimile transmission contain confidential " +
                                             "information.The information intended only for the use of the individual(s) or entity named above. If you " +
                                             "are not the intended recipient, you are notified any reliance on the contents of this facsimile information " +
                                             "is not permissible.If you have received this facsimile in error, please immediately notify us by telephone " +
                                             "at the number above to arrange for the return of the original documents. Thank you.";

                    newEmail.email_subject = invoice.emailSubject;
                    newEmail.email_to = invoice.emailAddress;
                    newEmail.email_type_id = 24;
                    newEmail.file_blob = attachmentFile;
                    newEmail.file_identifier = attachmentFileName;
                    newEmail.zip_file_name = (zip != null) ? attachmentFileName : null;
                    newEmail.zip_file_password = (zip != null) ? invoice.attachmentPassword : null;

                    _icmsContext.EmailsOutbounds.Add(newEmail);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {

                        emailInvoice = new Email();
                        emailInvoice.emailId = newEmail.email_outbound_id;

                        if (!string.IsNullOrEmpty(newEmail.zip_file_name))
                        {
                            emailInvoice.emailAttachmentName = newEmail.zip_file_name;
                        }
                        else
                        {
                            emailInvoice.emailAttachmentName = newEmail.file_identifier;
                        }
                    }
                }
            }

            return emailInvoice;
        }

        private ZipFile createEmailAttachmentZipBilling(string path, string zipFileName, Email invoice)
        {

            try
            {

                if (File.Exists(path + zipFileName))
                {                
                    File.Delete(path + zipFileName);
                }

                if (File.Exists(path + invoice.emailAttachmentName))
                {                
                    File.Delete(path + invoice.emailAttachmentName);
                }

                if (File.Exists(path + invoice.emailAttachmentName2))
                {
                    File.Delete(path + invoice.emailAttachmentName2);
                }


                if (!string.IsNullOrEmpty(invoice.emailAttachmentName) && invoice.emailAttachment.Length > 0)
                {
                    //create invoice file from byte[]
                    File.WriteAllBytes(path + invoice.emailAttachmentName, invoice.emailAttachment);
                }

                if (!string.IsNullOrEmpty(invoice.emailAttachmentName2) && invoice.emailAttachment2.Length > 0)
                {
                    //create invoice file from byte[]
                    File.WriteAllBytes(path + invoice.emailAttachmentName2, invoice.emailAttachment2);
                }


                if (File.Exists(path + invoice.emailAttachmentName) || File.Exists(path + invoice.emailAttachmentName2))
                {

                    using (ZipFile newZip = new ZipFile())
                    {

                        if (!string.IsNullOrEmpty(invoice.attachmentPassword))
                        {
                            newZip.Password = invoice.attachmentPassword;
                        }

                        if (File.Exists(path + invoice.emailAttachmentName))
                        {
                            newZip.AddFile(path + invoice.emailAttachmentName, "");
                        }

                        if (File.Exists(path + invoice.emailAttachmentName2))
                        {
                            newZip.AddFile(path + invoice.emailAttachmentName2, "");
                        }

                        newZip.Save(path + zipFileName);

                        if (!string.IsNullOrEmpty(invoice.emailAttachmentName))
                        {
                            File.Delete(path + invoice.emailAttachmentName);
                        }

                        if (!string.IsNullOrEmpty(invoice.emailAttachmentName2))
                        {
                            File.Delete(path + invoice.emailAttachmentName2);
                        }

                        return newZip;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public HospitalFacility addFacility(HospitalFacility facility)
        {

            HospitalFacility newFacility = null;

            if (!string.IsNullOrEmpty(facility.hospitalName))
            {

                if (!facilityExists(facility.hospitalName))
                {

                    int deptId = addDepartment(facility.hospitalName);

                    if (deptId > 0)
                    {

                        newFacility = new HospitalFacility();
                        newFacility.hospitalId = deptId;
                        newFacility.hospitalName = facility.hospitalName;

                        int addrId = addDepartmentAddress(facility, deptId);

                        if (addrId > 0)
                        {

                            newFacility.address1 = facility.address1;
                            newFacility.city = facility.city;
                            newFacility.stateAbbrev = facility.stateAbbrev;
                            newFacility.zip = facility.zip;
                        }
                    }
                }
            }

            return newFacility;
        }

        private bool facilityExists(string facilityName)
        {

            rDepartment dbDept = (

                    from depts in _icmsContext.rDepartments
                    where depts.label.Equals(facilityName)
                    select depts
                )
                .FirstOrDefault();

            if (dbDept != null && dbDept.id > 0)
            {
                return true;
            }

            return false;
        }

        private int addDepartment(string facilityName)
        {

            rDepartment newDepartment = new rDepartment();
            newDepartment.label = facilityName;

            _icmsContext.rDepartments.Add(newDepartment);
            int result = _icmsContext.SaveChanges();

            if (result > 0 && newDepartment.id > 0)
            {
                return newDepartment.id;
            }

            return 0;
        }

        private int addDepartmentAddress(HospitalFacility facility, int facilityId)
        {

            if (!string.IsNullOrEmpty(facility.address1) || !string.IsNullOrEmpty(facility.city) ||
                !string.IsNullOrEmpty(facility.stateAbbrev) || !string.IsNullOrEmpty(facility.zip))
            {

                FacilityAddress newAddress = new FacilityAddress();
                newAddress.id = facilityId;

                if (!string.IsNullOrEmpty(facility.address1))
                {

                    if (facility.address1.Length > 50)
                    {

                        newAddress.address_line_one = facility.address1.Substring(0, 50);
                        newAddress.address_line_two = facility.address1.Substring(50, 50);
                    }
                    else
                    {
                        newAddress.address_line_one = facility.address1;
                    }
                }

                newAddress.city = (!string.IsNullOrEmpty(facility.city)) ? facility.city : "";
                newAddress.state_abbrev = (!string.IsNullOrEmpty(facility.stateAbbrev)) ? facility.stateAbbrev : "";
                newAddress.zip_code = (!string.IsNullOrEmpty(facility.zip)) ? facility.zip : "";


                _icmsContext.FacilityAddresses.Add(newAddress);
                int resultAddr = _icmsContext.SaveChanges();

                if (resultAddr > 0 && newAddress.facility_address_id > 0)
                {

                    return newAddress.facility_address_id;
                }
            }

            return 0;
        }

    }
}
