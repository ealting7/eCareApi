using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class DoctorService : IDoctor
    {
        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspnetContext;
        private readonly IcmsDataStagingContext _datastagingContext;

        public DoctorService(IcmsContext icmsContext, AspNetContext aspnetContext, IcmsDataStagingContext datastagingContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspnetContext = aspnetContext ?? throw new ArgumentNullException(nameof(aspnetContext));
            _datastagingContext = datastagingContext ?? throw new ArgumentNullException(nameof(datastagingContext));
        }

        public IEnumerable<Doctor> GetDoctors(string first, string last, string state)
        {
            IEnumerable<Doctor> doctors = Enumerable.Empty<Doctor>();

            if (!string.IsNullOrEmpty(state))
            {

                doctors = (from dr in _icmsContext.Pcps
                           join drAddr in _icmsContext.PcpAddresses
                           on dr.pcp_id equals drAddr.pcp_id
                           join drSpecial in _icmsContext.PcpSpecialtys
                           on dr.pcp_id equals drSpecial.pcp_id
                           join Spec in _icmsContext.Specialtys
                           on drSpecial.specialty_id equals Spec.specialty_id
                           join drAddrConts in _icmsContext.PcpAddressContacts
                           on drAddr.provider_address_id equals drAddrConts.provider_address_id
                           join drConts in _icmsContext.PcpContacts
                           on drAddrConts.provider_contact_id equals drConts.provider_contact_id
                           join drContPhones in _icmsContext.PcpContactPhones
                           on drConts.provider_contact_id equals drContPhones.provider_contact_id
                           join drPhone in _icmsContext.PcpPhoneNumbers
                           on drContPhones.provider_phone_id equals drPhone.provider_phone_id
                           where dr.provider_first_name.StartsWith(first)
                           && dr.provider_last_name.StartsWith(last)
                           && drAddr.state_abbrev.Equals(state)
                           && (dr.disable_flag.Equals(false) || !dr.disable_flag.HasValue)
                           orderby dr.provider_last_name, dr.provider_first_name, dr.npi, drAddr.state_abbrev, drAddr.city
                           select new Doctor
                           {
                               providerAddressId = drAddr.provider_address_id,
                               pcpId = dr.pcp_id,
                               firstName = dr.provider_first_name,
                               lastName = dr.provider_last_name,
                               practiceName = dr.provider_group_name,
                               npi = (!string.IsNullOrEmpty(dr.npi)) ? dr.npi : (!string.IsNullOrWhiteSpace(dr.billing_npi)) ? dr.billing_npi : "",
                               specialtyDesc = Spec.specialty_desc,
                               emailAddress = dr.email_address,
                               phoneNumber = drPhone.phone_number,
                               address1 = drAddr.address_line1,
                               address2 = drAddr.address_line2,
                               city = drAddr.city,
                               stateAbbrev = drAddr.state_abbrev,
                               zip = drAddr.zip_code
                           })
                           .Take(50)
                           .ToList();
            }
            else
            {

                doctors = (from dr in _icmsContext.Pcps
                           join drAddr in _icmsContext.PcpAddresses
                           on dr.pcp_id equals drAddr.pcp_id
                           join drSpecial in _icmsContext.PcpSpecialtys
                           on dr.pcp_id equals drSpecial.pcp_id
                           join Spec in _icmsContext.Specialtys
                           on drSpecial.specialty_id equals Spec.specialty_id
                           join drAddrConts in _icmsContext.PcpAddressContacts
                           on drAddr.provider_address_id equals drAddrConts.provider_address_id
                           join drConts in _icmsContext.PcpContacts
                           on drAddrConts.provider_contact_id equals drConts.provider_contact_id
                           join drContPhones in _icmsContext.PcpContactPhones
                           on drConts.provider_contact_id equals drContPhones.provider_contact_id
                           join drPhone in _icmsContext.PcpPhoneNumbers
                           on drContPhones.provider_phone_id equals drPhone.provider_phone_id
                           where dr.provider_first_name.StartsWith(first)
                           && dr.provider_last_name.StartsWith(last)
                           orderby dr.provider_last_name, dr.provider_first_name, dr.npi, drAddr.state_abbrev, drAddr.city
                           select new Doctor
                           {
                               providerAddressId = drAddr.provider_address_id,
                               pcpId = dr.pcp_id,
                               firstName = dr.provider_first_name,
                               lastName = dr.provider_last_name,
                               practiceName = dr.provider_group_name,
                               npi = (!string.IsNullOrEmpty(dr.npi)) ? dr.npi : (!string.IsNullOrWhiteSpace(dr.billing_npi)) ? dr.billing_npi : "",
                               specialtyDesc = Spec.specialty_desc,
                               emailAddress = dr.email_address,
                               phoneNumber = drPhone.phone_number,
                               address1 = drAddr.address_line1,
                               address2 = drAddr.address_line2,
                               city = drAddr.city,
                               stateAbbrev = drAddr.state_abbrev,
                               zip = drAddr.zip_code
                           })
                                           .Take(100)
                                           .ToList();
            }

            return doctors;
        }

        public Doctor GetDoctorUsingProvAddrId(string id, string phoneNumber)
        {
            Doctor returnDoctor = new Doctor();
            int providerAddressId = 0;

            if (int.TryParse(id, out providerAddressId) && 
                providerAddressId > 0)
            {
                var doctor = (from drAddr in _icmsContext.PcpAddresses
                          join dr in _icmsContext.Pcps
                          on drAddr.pcp_id equals dr.pcp_id
                          join drSpecial in _icmsContext.PcpSpecialtys
                          on dr.pcp_id equals drSpecial.pcp_id
                          join Spec in _icmsContext.Specialtys
                          on drSpecial.specialty_id equals Spec.specialty_id
                          join drAddrConts in _icmsContext.PcpAddressContacts
                          on drAddr.provider_address_id equals drAddrConts.provider_address_id
                          join drConts in _icmsContext.PcpContacts
                          on drAddrConts.provider_contact_id equals drConts.provider_contact_id
                          join drContPhones in _icmsContext.PcpContactPhones
                          on drConts.provider_contact_id equals drContPhones.provider_contact_id
                          join drPhone in _icmsContext.PcpPhoneNumbers
                          on drContPhones.provider_phone_id equals drPhone.provider_phone_id
                          where drAddr.provider_address_id.Equals(providerAddressId)                          
                          select new Doctor
                          {
                              pcpId = dr.pcp_id,
                              firstName = (!string.IsNullOrWhiteSpace(dr.provider_first_name)) ? dr.provider_first_name : "",
                              lastName = (!string.IsNullOrWhiteSpace(dr.provider_last_name)) ? dr.provider_last_name : "",
                              practiceName = (!string.IsNullOrWhiteSpace(dr.provider_group_name)) ? dr.provider_group_name : "",
                              npi = (!string.IsNullOrEmpty(dr.npi)) ? dr.npi : (!string.IsNullOrWhiteSpace(dr.billing_npi)) ? dr.billing_npi : "",
                              specialtyDesc = (!string.IsNullOrWhiteSpace(Spec.specialty_desc)) ? Spec.specialty_desc : "",
                              emailAddress = (!string.IsNullOrWhiteSpace(dr.email_address)) ? dr.email_address : "",
                              phoneNumber = (!string.IsNullOrWhiteSpace(drPhone.phone_number)) ? drPhone.phone_number : "",
                              address1 = (!string.IsNullOrWhiteSpace(drAddr.address_line1)) ? drAddr.address_line1 : "",
                              address2 = (!string.IsNullOrWhiteSpace(drAddr.address_line2)) ? drAddr.address_line2 : "",
                              city = (!string.IsNullOrWhiteSpace(drAddr.city)) ? drAddr.city : "",
                              stateAbbrev = (!string.IsNullOrWhiteSpace(drAddr.state_abbrev)) ? drAddr.state_abbrev : "",
                              zip = (!string.IsNullOrWhiteSpace(drAddr.zip_code)) ? drAddr.zip_code : ""
                          }).ToList();                

                if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != "na")
                {
                    returnDoctor = doctor.Where(dr => dr.phoneNumber.Equals(phoneNumber)).FirstOrDefault();
                }
                else
                {
                    returnDoctor = doctor.Where(dr => dr.phoneNumber.Equals(DBNull.Value) || dr.phoneNumber.Equals("")).FirstOrDefault();
                }
            }

            return returnDoctor;
        }

        public Doctor GetProviderUsingPcpId(string id)
        {
            Doctor returnDoctor = new Doctor();

            Guid pcpId = Guid.Empty;

            if (Guid.TryParse(id, out pcpId))
            {
                var doctor = (from dr in _icmsContext.Pcps

                              join drSpecial in _icmsContext.PcpSpecialtys
                              on dr.pcp_id equals drSpecial.pcp_id into provSpec
                              from providerSpecialty in provSpec.DefaultIfEmpty()

                              join Spec in _icmsContext.Specialtys
                              on providerSpecialty.specialty_id equals Spec.specialty_id into specials
                              from specialty in specials.DefaultIfEmpty()

                              where dr.pcp_id.Equals(pcpId)
                              select new Doctor
                              {
                                  pcpId = dr.pcp_id,
                                  firstName = (!string.IsNullOrWhiteSpace(dr.provider_first_name)) ? dr.provider_first_name : "",
                                  lastName = (!string.IsNullOrWhiteSpace(dr.provider_last_name)) ? dr.provider_last_name : "",
                                  practiceName = (!string.IsNullOrWhiteSpace(dr.provider_group_name)) ? dr.provider_group_name : "",
                                  npi = (!string.IsNullOrEmpty(dr.npi)) ? dr.npi : (!string.IsNullOrWhiteSpace(dr.billing_npi)) ? dr.billing_npi : "",
                                  specialtyDesc = (!string.IsNullOrWhiteSpace(specialty.specialty_desc)) ? specialty.specialty_desc : "",
                                  emailAddress = (!string.IsNullOrWhiteSpace(dr.email_address)) ? dr.email_address : "",
                              }).ToList();


                if (doctor != null)
                {
                    foreach(Doctor doc in doctor)
                    {
                        returnDoctor.pcpId = doc.pcpId;
                        returnDoctor.firstName = doc.firstName;
                        returnDoctor.lastName = doc.lastName;
                        returnDoctor.fullName = doc.firstName + " " + doc.lastName;
                        returnDoctor.practiceName = doc.practiceName;
                        returnDoctor.npi = doc.npi;
                        returnDoctor.specialtyDesc = doc.specialtyDesc;
                        returnDoctor.emailAddress = doc.emailAddress;
                    }
                }

            }

            return returnDoctor;
        }

        public List<Doctor> GetAllProviderAddresses(string id)
        {
            List<Doctor> returnProvAddresses = new List<Doctor>();
            Guid pcpId = Guid.Empty;

            if (Guid.TryParse(id, out pcpId))
            {
                var addresses = (from drAddr in _icmsContext.PcpAddresses
                              join dr in _icmsContext.Pcps
                              on drAddr.pcp_id equals dr.pcp_id
                              join drSpecial in _icmsContext.PcpSpecialtys
                              on dr.pcp_id equals drSpecial.pcp_id
                              join Spec in _icmsContext.Specialtys
                              on drSpecial.specialty_id equals Spec.specialty_id
                              join drAddrConts in _icmsContext.PcpAddressContacts
                              on drAddr.provider_address_id equals drAddrConts.provider_address_id
                              where drAddr.pcp_id.Equals(pcpId)
                              select new Doctor
                              {
                                  providerAddressId = drAddr.provider_address_id,
                                  pcpId = dr.pcp_id,
                                  firstName = (!string.IsNullOrWhiteSpace(dr.provider_first_name)) ? dr.provider_first_name : "",
                                  lastName = (!string.IsNullOrWhiteSpace(dr.provider_last_name)) ? dr.provider_last_name : "",
                                  practiceName = (!string.IsNullOrWhiteSpace(dr.provider_group_name)) ? dr.provider_group_name : "",
                                  npi = (!string.IsNullOrEmpty(dr.npi)) ? dr.npi : (!string.IsNullOrWhiteSpace(dr.billing_npi)) ? dr.billing_npi : "",
                                  specialtyDesc = (!string.IsNullOrWhiteSpace(Spec.specialty_desc)) ? Spec.specialty_desc : "",
                                  emailAddress = (!string.IsNullOrWhiteSpace(dr.email_address)) ? dr.email_address : "",
                                  address1 = (!string.IsNullOrWhiteSpace(drAddr.address_line1)) ? drAddr.address_line1 : "",
                                  address2 = (!string.IsNullOrWhiteSpace(drAddr.address_line2)) ? drAddr.address_line2 : "",
                                  city = (!string.IsNullOrWhiteSpace(drAddr.city)) ? drAddr.city : "",
                                  stateAbbrev = (!string.IsNullOrWhiteSpace(drAddr.state_abbrev)) ? drAddr.state_abbrev : "",
                                  zip = (!string.IsNullOrWhiteSpace(drAddr.zip_code)) ? drAddr.zip_code : ""
                              })
                              .ToList();


                if (addresses != null)
                {
                    foreach (Doctor addr in addresses)
                    {
                        Doctor addAddress = new Doctor();

                        addAddress.pcpId = addr.pcpId;
                        addAddress.firstName = addr.firstName;
                        addAddress.lastName = addr.lastName;
                        addAddress.fullName = addr.firstName + " " + addr.lastName;
                        addAddress.practiceName = addr.practiceName;
                        addAddress.npi = addr.npi;
                        addAddress.specialtyDesc = addr.specialtyDesc;
                        addAddress.emailAddress = addr.emailAddress;
                        addAddress.address1 = addr.address1;
                        addAddress.address2 = addr.address2;
                        addAddress.street = addr.address1 + addr.address2;
                        addAddress.city = addr.city;
                        addAddress.stateAbbrev = addr.stateAbbrev;
                        addAddress.zip = addr.zip;

                        returnProvAddresses.Add(addAddress);
                    }
                }

            }


            return returnProvAddresses;
        }

        public Doctor getProvider(string id)
        {
            Doctor returnProvider = null;
            Guid pcpId = Guid.Empty;

            if (Guid.TryParse(id, out pcpId))
            {
                returnProvider = (from dr in _icmsContext.Pcps
                                 where dr.pcp_id.Equals(pcpId)
                                 select new Doctor
                                 {
                                     pcpId = dr.pcp_id,
                                     firstName = (!string.IsNullOrWhiteSpace(dr.provider_first_name)) ? dr.provider_first_name : "",
                                     lastName = (!string.IsNullOrWhiteSpace(dr.provider_last_name)) ? dr.provider_last_name : "",
                                     practiceName = (!string.IsNullOrWhiteSpace(dr.provider_group_name)) ? dr.provider_group_name : "",
                                     npi = (!string.IsNullOrEmpty(dr.npi)) ? dr.npi : (!string.IsNullOrWhiteSpace(dr.billing_npi)) ? dr.billing_npi : "",
                                     emailAddress = (!string.IsNullOrWhiteSpace(dr.email_address)) ? dr.email_address : "",
                                 })
                                 .Distinct()                                 
                                 .FirstOrDefault();


                if (returnProvider != null)
                {

                    Doctor mdAddress = GetProviderNewestAddress(pcpId);

                    if (mdAddress != null)
                    {

                        returnProvider.providerAddressId = mdAddress.providerAddressId;
                        returnProvider.address1 = mdAddress.address1;
                        returnProvider.address2 = mdAddress.address2;
                        returnProvider.city = mdAddress.city;
                        returnProvider.stateAbbrev = mdAddress.stateAbbrev;
                        returnProvider.zip = mdAddress.zip;
                    }

                    Doctor mdPhone = getProviderNewestPhone(pcpId);

                    if (mdPhone != null)
                    {
                        returnProvider.phoneNumber = mdPhone.phoneNumber;
                        returnProvider.providerPhoneId = mdPhone.providerPhoneId;
                        returnProvider.providerContatctId = mdPhone.providerContatctId;
                    }

                    Doctor mdSpecialty = getProviderSpecialty(pcpId);

                    if (mdSpecialty != null)
                    {
                        returnProvider.specialtyId = mdSpecialty.specialtyId;
                        returnProvider.specialtyDesc = mdSpecialty.specialtyDesc;
                    }
                }
            }


            return returnProvider;
        }

        private Doctor GetProviderNewestAddress(Guid pcpId)
        {
            Doctor address = null;

            if (!pcpId.Equals(Guid.Empty))
            {
                address = (
                    
                    from drAddr in _icmsContext.PcpAddresses
                    join dr in _icmsContext.Pcps
                    on drAddr.pcp_id equals dr.pcp_id                    
                    where drAddr.pcp_id.Equals(pcpId)
                    orderby drAddr.provider_address_id descending
                    select new Doctor
                    {
                        providerAddressId = drAddr.provider_address_id,
                        address1 = (!string.IsNullOrWhiteSpace(drAddr.address_line1)) ? drAddr.address_line1 : "",
                        address2 = (!string.IsNullOrWhiteSpace(drAddr.address_line2)) ? drAddr.address_line2 : "",
                        city = (!string.IsNullOrWhiteSpace(drAddr.city)) ? drAddr.city : "",
                        stateAbbrev = (!string.IsNullOrWhiteSpace(drAddr.state_abbrev)) ? drAddr.state_abbrev : "",
                        zip = (!string.IsNullOrWhiteSpace(drAddr.zip_code)) ? drAddr.zip_code : ""
                    }
                )
                .Take(1)
                .FirstOrDefault();                
            }

            return address;
        }

        private Doctor GetProviderAddressUsingProvAddrId(int provAddrId)
        {
            Doctor address = null;

            if (provAddrId > 0)
            {
                address = (

                    from drAddr in _icmsContext.PcpAddresses
                    join dr in _icmsContext.Pcps
                    on drAddr.pcp_id equals dr.pcp_id
                    where drAddr.provider_address_id.Equals(provAddrId)
                    select new Doctor
                    {
                        providerAddressId = drAddr.provider_address_id,
                        pcpId = (drAddr.pcp_id.HasValue) ? (Guid)drAddr.pcp_id : Guid.Empty,
                        address1 = (!string.IsNullOrWhiteSpace(drAddr.address_line1)) ? drAddr.address_line1 : "",
                        address2 = (!string.IsNullOrWhiteSpace(drAddr.address_line2)) ? drAddr.address_line2 : "",
                        city = (!string.IsNullOrWhiteSpace(drAddr.city)) ? drAddr.city : "",
                        stateAbbrev = (!string.IsNullOrWhiteSpace(drAddr.state_abbrev)) ? drAddr.state_abbrev : "",
                        zip = (!string.IsNullOrWhiteSpace(drAddr.zip_code)) ? drAddr.zip_code : ""
                    }
                )
                .FirstOrDefault();
            }

            return address;
        }

        private int getProviderAddressId(Guid pcpId)
        {

            int providerAddressId = 0;

            providerAddressId = (

                    from provAddr in _icmsContext.PcpAddresses
                    where provAddr.pcp_id.Equals(pcpId)
                    select provAddr.provider_address_id
                )
                .FirstOrDefault();

            if (providerAddressId.Equals(0))
            {

                Doctor addAddr = new Doctor();
                addAddr.pcpId = pcpId;

                Doctor newAddr = addNewProviderAddress(addAddr);

                if (newAddr != null)
                {
                    providerAddressId = newAddr.providerAddressId;
                }
            }

            return providerAddressId;
        }

        private int getProviderContactId(int providerAddressId, Doctor provider)
        {

            int providerContactId = 0;

            providerContactId = (

                    from provContact in _icmsContext.PcpAddressContacts
                    where provContact.provider_address_id.Equals(providerAddressId)
                    select provContact.provider_contact_id
                )
                .FirstOrDefault();

            if (providerContactId.Equals(0))
            {

                Doctor addContact = new Doctor();
                addContact.providerAddressId = providerAddressId;
                addContact.firstName = provider.firstName;
                addContact.lastName = provider.lastName;

                Doctor newContact = addNewProviderContact(addContact);

                if (newContact != null)
                {
                    providerContactId = newContact.providerContatctId;
                }
            }

            return providerContactId;
        }

        private int getProviderPhoneId(int providerContactId, string phoneNumber)
        {

            int providerPhoneId = 0;


            ProviderPhone newPhone = new ProviderPhone();
            newPhone.phone_number = phoneNumber;
            newPhone.creation_date = DateTime.Now;

            _icmsContext.PcpPhoneNumbers.Add(newPhone);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                providerPhoneId = newPhone.provider_phone_id;

                if (providerPhoneId > 0)
                {

                    Doctor addProvPhone = new Doctor();
                    addProvPhone.providerPhoneId = providerPhoneId;
                    addProvPhone.providerContatctId = providerContactId;

                    Doctor newProvPhone = addProviderContactPhone(addProvPhone);

                    if (newProvPhone != null)
                    {
                        providerPhoneId = newProvPhone.providerPhoneId;
                    }
                }
            }

            return providerPhoneId;
        }

        private Doctor getProviderNewestPhone(Guid pcpId)

        {
            Doctor mdPhone = null;

            if (!pcpId.Equals(Guid.Empty))
            {
                mdPhone = (

                    from drAddr in _icmsContext.PcpAddresses
                    join dr in _icmsContext.Pcps
                    on drAddr.pcp_id equals dr.pcp_id

                    join drAddrConts in _icmsContext.PcpAddressContacts
                    on drAddr.provider_address_id equals drAddrConts.provider_address_id

                    join provContact in _icmsContext.PcpContacts
                    on drAddrConts.provider_contact_id equals provContact.provider_contact_id

                    join provContactPhone in _icmsContext.PcpContactPhones
                    on provContact.provider_contact_id equals provContactPhone.provider_contact_id

                    join provPhone in _icmsContext.PcpPhoneNumbers
                    on provContactPhone.provider_phone_id equals provPhone.provider_phone_id

                    where drAddr.pcp_id.Equals(pcpId)
                    orderby drAddr.provider_address_id descending
                    select new Doctor
                    {
                        providerPhoneId = provPhone.provider_phone_id,
                        providerContatctId = provContactPhone.provider_contact_id,
                        phoneNumber = provPhone.phone_number
                    }
                )
                .Take(1)
                .FirstOrDefault();                

            }

            return mdPhone;
        }

        private Doctor GetProviderPhoneUsingProvPhnId(int provPhnId)
        {
            Doctor mdPhone = null;

            if (provPhnId > 0)
            {
                mdPhone = (

                    from drAddr in _icmsContext.PcpAddresses
                    join dr in _icmsContext.Pcps
                    on drAddr.pcp_id equals dr.pcp_id

                    join drAddrConts in _icmsContext.PcpAddressContacts
                    on drAddr.provider_address_id equals drAddrConts.provider_address_id

                    join provContact in _icmsContext.PcpContacts
                    on drAddrConts.provider_contact_id equals provContact.provider_contact_id

                    join provContactPhone in _icmsContext.PcpContactPhones
                    on provContact.provider_contact_id equals provContactPhone.provider_contact_id

                    join provPhone in _icmsContext.PcpPhoneNumbers
                    on provContactPhone.provider_phone_id equals provPhone.provider_phone_id

                    where provPhone.provider_phone_id.Equals(provPhnId)
                    select new Doctor
                    {
                        providerPhoneId = provPhone.provider_phone_id,
                        providerContatctId = provContactPhone.provider_contact_id,
                        phoneNumber = provPhone.phone_number
                    }
                )
                .FirstOrDefault();

            }

            return mdPhone;
        }

        private Doctor getProviderSpecialty(Guid pcpId)

        {
            Doctor specialty = null;

            if (!pcpId.Equals(Guid.Empty))
            {
                specialty = (

                    from dr in _icmsContext.Pcps

                    join drSpecial in _icmsContext.PcpSpecialtys
                    on dr.pcp_id equals drSpecial.pcp_id

                    join Spec in _icmsContext.Specialtys
                    on drSpecial.specialty_id equals Spec.specialty_id

                    where drSpecial.pcp_id.Equals(pcpId)
                    select new Doctor
                    {
                        specialtyId = drSpecial.specialty_id,
                        specialtyDesc = (!string.IsNullOrWhiteSpace(Spec.specialty_desc)) ? Spec.specialty_desc : ""
                    }
                )
                .FirstOrDefault();
                
            }

            return specialty;
        }



        public List<Utilization> getProviderMedicalReviews(string provId)
        {

            List<Utilization> medicalReviews = null;

            Guid userId = Guid.Empty;

            if (Guid.TryParse(provId, out userId))
            {
            
                Guid icmsSysUserid = getUserIcmsSystemUserIdReference(userId);

                if (!icmsSysUserid.Equals(Guid.Empty))
                {

                    medicalReviews = (

                            from mdRev in _icmsContext.MdReviewRequests

                            join pat in _icmsContext.Patients
                            on mdRev.member_id equals pat.member_id

                            join referral in _icmsContext.MemberReferrals
                            on mdRev.referral_number equals referral.referral_number

                            join su in _icmsContext.SystemUsers
                            on mdRev.entered_by_system_user_id equals su.system_user_id into sysUsrs
                            from enteredByUsr in sysUsrs.DefaultIfEmpty()

                            where mdRev.assigned_to_system_user_id.Equals(icmsSysUserid)
                            && mdRev.completed.Equals(false)

                            orderby mdRev.creation_date descending

                            select new Utilization
                            {                                
                                referralNumber = mdRev.referral_number,
                                authNumber = referral.auth_number,
                                memberId = mdRev.member_id,
                                patient = new Patient
                                {
                                    firstName = pat.member_first_name,
                                    lastName = pat.member_last_name,
                                    FullName = pat.member_first_name + " " + pat.member_last_name
                                },
                                mdReviewRequest = new MedicalReview
                                {
                                    mdReviewRequestId = mdRev.md_review_request_id,
                                    taskId = mdRev.task_id,
                                    taskNote = mdRev.task_note,
                                    startActionDate = (mdRev.start_action_date.HasValue) ? mdRev.start_action_date : null,
                                    endActionDate = (mdRev.end_action_date.HasValue) ? mdRev.end_action_date : null,
                                    creationDate = (mdRev.creation_date.HasValue) ? mdRev.creation_date : null,
                                    displayCreationDate = (mdRev.creation_date.HasValue) ? mdRev.creation_date.Value.ToShortDateString() : "",
                                    enteredByUsername = enteredByUsr.system_user_first_name + " " + enteredByUsr.system_user_last_name
                                },
                            }
                        )
                        .ToList();
                }
            }


            return medicalReviews;
        }

        public Utilization getProviderMedicalReview(string provId, int medRevReqId)
        {

            Utilization medicalReview = null;

            Guid userId = Guid.Empty;

            if (Guid.TryParse(provId, out userId))
            {

                Guid icmsSysUserid = getUserIcmsSystemUserIdReference(userId);

                if (!icmsSysUserid.Equals(Guid.Empty))
                {

                    medicalReview = (

                            from mdRev in _icmsContext.MdReviewRequests

                            join pat in _icmsContext.Patients
                            on mdRev.member_id equals pat.member_id

                            join referral in _icmsContext.MemberReferrals
                            on mdRev.referral_number equals referral.referral_number

                            join su in _icmsContext.SystemUsers
                            on mdRev.entered_by_system_user_id equals su.system_user_id into sysUsrs
                            from enteredByUsr in sysUsrs.DefaultIfEmpty()

                            where mdRev.md_review_request_id.Equals(medRevReqId)
                            && mdRev.assigned_to_system_user_id.Equals(icmsSysUserid)
                            && mdRev.completed.Equals(false)

                            orderby mdRev.creation_date descending

                            select new Utilization
                            {
                                referralNumber = mdRev.referral_number,
                                authNumber = referral.auth_number,
                                memberId = mdRev.member_id,
                                patient = new Patient
                                {
                                    firstName = pat.member_first_name,
                                    lastName = pat.member_last_name,
                                    FullName = pat.member_first_name + " " + pat.member_last_name
                                },
                                mdReviewRequest = new MedicalReview
                                {
                                    mdReviewRequestId = mdRev.md_review_request_id,
                                    taskId = mdRev.task_id,
                                    taskNote = mdRev.task_note,
                                    requestNote = mdRev.md_review_appeal_note,
                                    startActionDate = (mdRev.start_action_date.HasValue) ? mdRev.start_action_date : null,
                                    displayStartActionDate = (mdRev.start_action_date.HasValue) ? mdRev.start_action_date.Value.ToShortDateString() : "",
                                    endActionDate = (mdRev.end_action_date.HasValue) ? mdRev.end_action_date : null,
                                    displayEndActionDate = (mdRev.end_action_date.HasValue) ? mdRev.end_action_date.Value.ToShortDateString() : "",
                                    creationDate = (mdRev.creation_date.HasValue) ? mdRev.creation_date : null,
                                    displayCreationDate = (mdRev.creation_date.HasValue) ? mdRev.creation_date.Value.ToShortDateString() : "",
                                    enteredByUserId = (mdRev.entered_by_system_user_id.HasValue) ? mdRev.entered_by_system_user_id.Value : null,
                                    enteredByUsername = (enteredByUsr.system_user_id != null) ? 
                                                        enteredByUsr.system_user_first_name + " " + enteredByUsr.system_user_last_name: null,
                                    enteredByEmail = (enteredByUsr.system_user_id != null) ? enteredByUsr.email_address : null,
                                },
                            }
                        )
                        .FirstOrDefault();
                }
            }


            if (medicalReview != null)
            {

                if (medicalReview.mdReviewRequest != null && 
                    string.IsNullOrEmpty(medicalReview.mdReviewRequest.enteredByUsername) &&
                    !medicalReview.mdReviewRequest.enteredByUserId.Equals(Guid.Empty))
                {

                    IcmsUserService usrServ = new IcmsUserService(_icmsContext, _aspnetContext);
                    IcmsUser usr = usrServ.getIcmsUser(medicalReview.mdReviewRequest.enteredByUserId.ToString());

                    if (usr != null)
                    {
                        medicalReview.mdReviewRequest.enteredByUserId = usr.UserId;
                        medicalReview.mdReviewRequest.enteredByUsername = usr.FullName;
                        medicalReview.mdReviewRequest.enteredByEmail = usr.emailAddress;
                    }
                }


                UtilizationManagementService utilServ = new UtilizationManagementService(_icmsContext, _aspnetContext, _datastagingContext);

                medicalReview.requests = utilServ.getMedicalReviewRequests(medRevReqId, medicalReview.referralNumber);

                medicalReview.diagnosisCodes = utilServ.GetReferralCodes("ICD10", medicalReview.referralNumber, (Guid)medicalReview.memberId);
                medicalReview.cptCodes = utilServ.GetReferralCodes("CPT", medicalReview.referralNumber, (Guid)medicalReview.memberId);
                medicalReview.hcpcsCodes = utilServ.GetReferralCodes("HCPCS", medicalReview.referralNumber, (Guid)medicalReview.memberId);

                medicalReview.notes = utilServ.getReferralNotes(medicalReview.referralNumber, (Guid)medicalReview.memberId);

                medicalReview.faxes = utilServ.getReferralFaxes(medicalReview.referralNumber, (Guid)medicalReview.memberId);
            }

            return medicalReview;
        }

        private Guid getUserIcmsSystemUserIdReference(Guid userId)
        {

            Guid icmsSysUsrId = Guid.Empty;

            icmsSysUsrId = (

                    from icmref in _aspnetContext.IcmsUserReferences
                    where icmref.UserId.Equals(userId)
                    select icmref.icms_system_user_id.Value
                )
                .FirstOrDefault();

            return icmsSysUsrId;
        }


        public Doctor addProviderNew(Doctor mdDoctor)
        {

            Doctor providerReturn = null;

            Doctor newProvider = insertNewProvider(mdDoctor);

            if (newProvider != null)
            {

                providerReturn = new Doctor();
                providerReturn.pcpId = newProvider.pcpId;
                providerReturn.lastName = newProvider.lastName;
                providerReturn.firstName = newProvider.firstName;

                mdDoctor.pcpId = newProvider.pcpId;

                if (!string.IsNullOrEmpty(mdDoctor.address1) || !string.IsNullOrEmpty(mdDoctor.city) ||
                    !string.IsNullOrEmpty(mdDoctor.stateAbbrev) || !string.IsNullOrEmpty(mdDoctor.zip)) 
                {

                    Doctor address = addNewProviderAddress(mdDoctor);

                    if (address != null)
                    {

                        providerReturn.providerAddressId = address.providerAddressId;
                        providerReturn.address1 = address.address1;
                        providerReturn.address2 = address.address2;
                        providerReturn.city = address.city;
                        providerReturn.stateAbbrev = address.stateAbbrev;
                        providerReturn.zip = address.zip;
                    }
                }

                if (mdDoctor.specialtyId > 0)
                {

                    Doctor specialty = addNewProviderSpecialty(mdDoctor);

                    if (specialty != null)
                    {
                        providerReturn.specialtyId = specialty.specialtyId;
                    }
                }

                if (!string.IsNullOrEmpty(mdDoctor.phoneNumber))
                {

                    Doctor phone = addNewProviderPhone(mdDoctor);

                    if (phone != null)
                    {
                        providerReturn.providerPhoneId = phone.providerPhoneId;
                        providerReturn.phoneNumber = phone.phoneNumber;
                    }
                }
            }

            return providerReturn;
        }

        private Doctor insertNewProvider(Doctor mdDoctor)
        {

            Doctor newProvider = null;

            List<PcpTable> pcps = (

                                from prov in _icmsContext.Pcps
                                where prov.provider_first_name.Equals(mdDoctor.firstName)
                                && prov.provider_last_name.Equals(mdDoctor.lastName)
                                select prov
                            )
                            .ToList();

            if (pcps != null && pcps.Count > 0)
            {

                if (!string.IsNullOrEmpty(mdDoctor.address1))
                {

                    bool uniqueProvider = true;

                    foreach (PcpTable pcp in pcps)
                    {

                        Doctor duplicateProvider = getProvider(pcp.pcp_id.ToString());

                        if (mdDoctor.address1.Substring(0, 50).Equals(duplicateProvider.address1) &&
                            mdDoctor.city.Equals(duplicateProvider.city) &&
                            mdDoctor.stateAbbrev.Equals(duplicateProvider.stateAbbrev))
                        {
                            uniqueProvider = false;
                            break;
                        }
                    }

                    if (uniqueProvider)
                    {
                        newProvider = addProviderBasic(mdDoctor);
                    }
                }
            }
            else
            {
                newProvider = addProviderBasic(mdDoctor);
            }

            return newProvider;
        }

        private Doctor addProviderBasic(Doctor mdDoctor)
        {

            Doctor providerReturn = null;

            PcpTable addProvider = new PcpTable();
            addProvider.provider_last_name = mdDoctor.lastName;
            addProvider.provider_first_name = mdDoctor.firstName;
            addProvider.disable_flag = false;

            _icmsContext.Pcps.Add(addProvider);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                providerReturn = getProvider(addProvider.pcp_id.ToString());
            }

            return providerReturn;
        }

        public Doctor updateProviderNpi(Doctor mdDoctor)
        {

            Doctor providerReturn = null;

            if (!mdDoctor.pcpId.Equals(Guid.Empty))
            {

                PcpTable dbDoctor = (

                        from pcp in _icmsContext.Pcps
                        where pcp.pcp_id.Equals(mdDoctor.pcpId)
                        select pcp
                    )
                    .FirstOrDefault();

                if (dbDoctor != null)
                {

                    dbDoctor.npi = mdDoctor.npi;

                    _icmsContext.Pcps.Update(dbDoctor);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        providerReturn = getProvider(mdDoctor.pcpId.ToString());
                    }
                }
            }

            return providerReturn;
        }


        public Doctor updateProviderAddress(Doctor mdDoctor)
        {

            Doctor providerReturn = null;

            if (!mdDoctor.pcpId.Equals(Guid.Empty))
            {

                Doctor dbMdAddress = null;

                if (mdDoctor.providerAddressId > 0)
                {
                    dbMdAddress = GetProviderAddressUsingProvAddrId(mdDoctor.providerAddressId);
                } 
                else
                {
                    dbMdAddress = GetProviderNewestAddress(mdDoctor.pcpId);
                }

                if (dbMdAddress != null)
                {

                    dbMdAddress.address1 = (mdDoctor.address1.Length > 50) ? mdDoctor.address1.Substring(0, 50) : mdDoctor.address1;
                    dbMdAddress.address2= (mdDoctor.address1.Length > 50) ? mdDoctor.address1.Substring(50, 50) : "";
                    dbMdAddress.city = mdDoctor.city;
                    dbMdAddress.stateAbbrev = mdDoctor.stateAbbrev;
                    dbMdAddress.zip = mdDoctor.zip;

                    providerReturn = saveProviderAddress(dbMdAddress);
                } 
                else
                {
                    providerReturn = addNewProviderAddress(mdDoctor);
                }
            }

            return providerReturn;
        }
        public Doctor addNewProviderAddress(Doctor mdDoctor)
        {
            
            Doctor providerReturn = null;

            if (!mdDoctor.pcpId.Equals(Guid.Empty))
            {

                ProviderAddress newAddress = new ProviderAddress();
                newAddress.pcp_id = mdDoctor.pcpId;
                newAddress.address_line1 = (mdDoctor.address1.Length > 50) ? mdDoctor.address1.Substring(0, 50) : mdDoctor.address1;
                newAddress.address_line2 = (mdDoctor.address1.Length > 50) ? mdDoctor.address1.Substring(50, 50) : "";
                newAddress.city = mdDoctor.city;
                newAddress.state_abbrev = mdDoctor.stateAbbrev;
                newAddress.zip_code = mdDoctor.zip;
                newAddress.address_type_id = 2; //business type

                _icmsContext.PcpAddresses.Add(newAddress);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    providerReturn = GetProviderAddressUsingProvAddrId(newAddress.provider_address_id);
                }
            }

            return providerReturn;
        }
        public Doctor saveProviderAddress(Doctor mdAddress)
        {

            Doctor providerReturn = null;

            if (mdAddress.providerAddressId > 0)
            {

                ProviderAddress provAddr = (

                        from provaddr in _icmsContext.PcpAddresses
                        where provaddr.provider_address_id.Equals(mdAddress.providerAddressId)
                        select provaddr
                    )
                    .FirstOrDefault();

                if (provAddr != null)
                {

                    provAddr.address_line1 = mdAddress.address1;
                    provAddr.address_line2 = mdAddress.address2;
                    provAddr.city = mdAddress.city;
                    provAddr.state_abbrev = mdAddress.stateAbbrev;
                    provAddr.zip_code = mdAddress.zip;

                    _icmsContext.PcpAddresses.Update(provAddr);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        providerReturn = GetProviderAddressUsingProvAddrId(mdAddress.providerAddressId);
                    }
                }
            }

            return providerReturn;
        }


        public Doctor updateProviderPhone(Doctor mdDoctor)
        {
            
            Doctor providerReturn = null;

            if (!mdDoctor.pcpId.Equals(Guid.Empty))
            {

                Doctor dbMdPhone = null;

                if (mdDoctor.providerPhoneId > 0)
                {
                    dbMdPhone = GetProviderPhoneUsingProvPhnId(mdDoctor.providerPhoneId);
                }
                else
                {
                    dbMdPhone = getProviderNewestPhone(mdDoctor.pcpId);
                }

                if (dbMdPhone != null)
                {

                    dbMdPhone.phoneNumber = mdDoctor.phoneNumber;

                    providerReturn = saveProviderPhone(dbMdPhone);
                }
                else
                {
                    providerReturn = addNewProviderPhone(mdDoctor);
                }
            }

            return providerReturn;
        }
        public Doctor addNewProviderPhone(Doctor mdPhone)
        {

            Doctor providerReturn = null;

            if (!mdPhone.pcpId.Equals(Guid.Empty))
            {

                int providerAddressId = getProviderAddressId(mdPhone.pcpId);

                if (providerAddressId > 0)
                {

                    int providerContactId = getProviderContactId(providerAddressId, mdPhone);

                    if (providerContactId > 0)
                    {

                        int providerPhoneId = getProviderPhoneId(providerContactId, mdPhone.phoneNumber);

                        if (providerPhoneId > 0)
                        {
                            providerReturn = GetProviderPhoneUsingProvPhnId(providerPhoneId);
                        }
                    }
                }
            }

            return providerReturn;
        }
        public Doctor saveProviderPhone(Doctor mdPhone)
        {

            Doctor providerReturn = null;

            if (mdPhone.providerPhoneId > 0)
            {

                ProviderPhone provPhn = (

                        from provaddr in _icmsContext.PcpPhoneNumbers
                        where provaddr.provider_phone_id.Equals(mdPhone.providerPhoneId)
                        select provaddr
                    )
                    .FirstOrDefault();

                if (provPhn != null)
                {

                    provPhn.phone_number = mdPhone.phoneNumber;

                    _icmsContext.PcpPhoneNumbers.Update(provPhn);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        providerReturn = GetProviderPhoneUsingProvPhnId(mdPhone.providerPhoneId);
                    }
                }
            }

            return providerReturn;
        }



        public Doctor updateProviderSpecialty(Doctor mdDoctor)
        {

            Doctor providerReturn = null;

            if (!mdDoctor.pcpId.Equals(Guid.Empty))
            {

                Doctor dbMdSpecialty = null;

                if (!mdDoctor.pcpId.Equals(Guid.Empty))
                {
                    dbMdSpecialty = getProviderSpecialty(mdDoctor.pcpId);
                }

                if (dbMdSpecialty != null)
                {

                    dbMdSpecialty.pcpId = mdDoctor.pcpId;
                    dbMdSpecialty.specialtyId = mdDoctor.specialtyId;

                    providerReturn = saveProviderSpecialty(dbMdSpecialty);
                }
                else
                {
                    providerReturn = addNewProviderSpecialty(mdDoctor);
                }
            }

            return providerReturn;
        }
        private Doctor saveProviderSpecialty(Doctor mdSpecialty)
        {

            Doctor providerReturn = null;

            if (!mdSpecialty.pcpId.Equals(Guid.Empty))
            {

                ProviderSpecialtys provSpec = (

                        from provspecial in _icmsContext.PcpSpecialtys
                        where provspecial.pcp_id.Equals(mdSpecialty.pcpId)
                        select provspecial
                    )
                    .FirstOrDefault();

                if (provSpec != null)
                {

                    provSpec.specialty_id = (int)mdSpecialty.specialtyId;

                    _icmsContext.PcpSpecialtys.Update(provSpec);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        providerReturn = getProviderSpecialty(mdSpecialty.pcpId);
                    }
                }
            }

            return providerReturn;
        }
        private Doctor addNewProviderSpecialty(Doctor mdSpecialty)
        {

            Doctor providerReturn = null;

            if (!mdSpecialty.pcpId.Equals(Guid.Empty))
            {

                ProviderSpecialtys newSpecialty = new ProviderSpecialtys();
                newSpecialty.pcp_id = mdSpecialty.pcpId;
                newSpecialty.specialty_id = (int)mdSpecialty.specialtyId;

                _icmsContext.PcpSpecialtys.Add(newSpecialty);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    providerReturn = getProviderSpecialty(mdSpecialty.pcpId);
                }
            }

            return providerReturn;
        }



        private Doctor addNewProviderContact(Doctor mdContact)
        {

            Doctor providerReturn = null;

            ProviderContact newProviderContact = new ProviderContact();

            newProviderContact.contact_name = mdContact.firstName + " " + mdContact.lastName;
            newProviderContact.contact_title = "Provider";

            _icmsContext.PcpContacts.Add(newProviderContact);
            int resultContact = _icmsContext.SaveChanges();

            if (resultContact > 0)
            {

                ProviderAddressContact newContact = new ProviderAddressContact();

                newContact.contact_type_id = 1;
                newContact.provider_address_id = mdContact.providerAddressId;
                newContact.provider_contact_id = newProviderContact.provider_contact_id;

                _icmsContext.PcpAddressContacts.Add(newContact);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    providerReturn = new Doctor();
                    providerReturn.providerContatctId = newContact.provider_contact_id;
                }
            }

            return providerReturn;
        }
        private Doctor addProviderContactPhone(Doctor mdPhone)
        {

            Doctor providerReturn = null;

            ProviderContactPhone newContact = new ProviderContactPhone();

            newContact.phone_type_id = 1;
            newContact.provider_phone_id = mdPhone.providerPhoneId;
            newContact.provider_contact_id = mdPhone.providerContatctId;

            _icmsContext.PcpContactPhones.Add(newContact);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                providerReturn = new Doctor();
                providerReturn.providerPhoneId = newContact.provider_phone_id;
            }

            return providerReturn;
        }



        public Utilization addMedicalReviewQuestion(MedicalReview medReview)
        {
            
            Utilization utilization = null;

            if (medReview.mdReviewRequestId > 0)
            {

                MdReviewRequest medRequest = (

                        from medRev in _icmsContext.MdReviewRequests
                        where medRev.md_review_request_id.Equals(medReview.mdReviewRequestId)
                        select medRev
                    )
                    .FirstOrDefault();

                if (medRequest != null)
                {

                    List<MedicalReview> reviewQuestions = insertMedicalReviewQuestion(medReview);

                    if (reviewQuestions != null)
                    {

                        utilization = new Utilization();
                        utilization.referralNumber = medReview.referralNumber;
                        utilization.memberId = medReview.memberId;

                        utilization.mdReviewRequest = new MedicalReview();
                        utilization.mdReviewRequest.mdReviewRequestId = medReview.mdReviewRequestId;
                        utilization.mdReviewRequest.questions = reviewQuestions;

                        UtilizationManagementService utilServ = new UtilizationManagementService(_icmsContext, _aspnetContext, _datastagingContext);

                        utilization.requests = utilServ.getReferralRequests(medReview.referralNumber, (Guid)medReview.memberId);
                    }
                }
            }

            return utilization;
        }
        private List<MedicalReview> insertMedicalReviewQuestion(MedicalReview medReview)
        {

            List<MedicalReview> questions = null;

            if (medReview.mdReviewRequestId > 0)
            {

                UtilizationManagementService utilServ = new UtilizationManagementService(_icmsContext, _aspnetContext, _datastagingContext);

                UtilizationRequest newReviewQuestion = setReviewQuestion(medReview);
                Utilization requests = utilServ.addUtilizationsUmRequest(newReviewQuestion);

                if (requests.requests != null)
                {
                    questions = getReviewQuestions(medReview);
                }
            }

            return questions;
        }
        private UtilizationRequest setReviewQuestion(MedicalReview medReview)
        {

            UtilizationRequest addQuestion = new UtilizationRequest();

            addQuestion.reviewRequestId = medReview.mdReviewRequestId;
            addQuestion.assignedToUserId = medReview.assignedToUserId;
            addQuestion.createDate = medReview.creationDate;
            addQuestion.usr = medReview.usr;
            addQuestion.requestNote = medReview.questionNote;
            addQuestion.referralNumber = medReview.referralNumber;
            addQuestion.memberId = (Guid)medReview.memberId;

            if (!string.IsNullOrEmpty(medReview.assignedToEmail))
            {
                addQuestion.emailAddress = medReview.assignedToEmail;
            }

            return addQuestion;

        }
        private List<MedicalReview> getReviewQuestions(MedicalReview medReview)
        {

            List<MedicalReview> questions = null;

            questions = (

                    from mdQuest in _icmsContext.MdReviewQuestions
                    where mdQuest.md_review_request_id.Equals(medReview.mdReviewRequestId)
                    select new MedicalReview
                    {
                        mdReviewQuestionId = mdQuest.md_review_question_id,
                        referralNumber = mdQuest.referral_number,
                        memberId = mdQuest.member_id,
                        questionNote = mdQuest.md_question_note,
                        answerNote = mdQuest.md_answer_note
                    }
                )
                .ToList();

            return questions;
        }




        public Utilization addMedicalReviewDetermination(MedicalReview medReview)
        {

            Utilization utilization = null;

            if (medReview.mdReviewRequestId > 0)
            {

                List<MedicalReview> reviewDeterminations = insertMedicalReviewDetermination(medReview);

                if (reviewDeterminations != null)
                {

                    utilization = new Utilization();
                    utilization.referralNumber = medReview.referralNumber;
                    utilization.memberId = medReview.memberId;

                    utilization.mdReviewRequest = new MedicalReview();
                    utilization.mdReviewRequest.mdReviewRequestId = medReview.mdReviewRequestId;
                    utilization.mdReviewRequest.determinations = reviewDeterminations;
                }
            }

            return utilization;
        }

        private List<MedicalReview> insertMedicalReviewDetermination(MedicalReview medReview)
        {

            List<MedicalReview> determinations = null;

            if (medReview.mdReviewRequestId > 0)
            {

                UtilizationManagementService utilServ = new UtilizationManagementService(_icmsContext, _aspnetContext, _datastagingContext);

                UtilizationRequest newDetermination = setReviewDetermination(medReview);
                Utilization requests = utilServ.addUtilizationsUmRequestDetermination(newDetermination);

                if (requests != null && requests.requests != null)
                {
                    determinations = getReviewDeterminations(medReview);
                }
            }

            return determinations;
        }

        private UtilizationRequest setReviewDetermination(MedicalReview medReview)
        {

            UtilizationRequest addDetermination = new UtilizationRequest();

            addDetermination.reviewRequestId = medReview.mdReviewRequestId;
            addDetermination.assignedToUserId = medReview.assignedToUserId;
            addDetermination.decisionId = medReview.decisionId;
            addDetermination.createDate = medReview.creationDate;
            addDetermination.usr = medReview.usr;
            addDetermination.requestNote = medReview.determinationNote;
            addDetermination.referralNumber = medReview.referralNumber;
            addDetermination.memberId = (Guid)medReview.memberId;

            if (!string.IsNullOrEmpty(medReview.assignedToEmail))
            {
                addDetermination.emailAddress = medReview.assignedToEmail;
            }

            return addDetermination;

        }

        private List<MedicalReview> getReviewDeterminations(MedicalReview medReview)
        {

            List<MedicalReview> determinations = null;

            determinations = (

                    from mdQuest in _icmsContext.MdReviewDeterminations
                    where mdQuest.md_review_request_id.Equals(medReview.mdReviewRequestId)
                    select new MedicalReview
                    {
                        mdReviewDeterminationId = mdQuest.md_review_determination_id,
                        referralNumber = mdQuest.referral_number,
                        memberId = (!string.IsNullOrEmpty(mdQuest.member_id)) ? new Guid(mdQuest.member_id) : null,
                        determinationNote = mdQuest.md_review_determination_note,
                    }
                )
                .ToList();

            return determinations;
        }



        public bool disableProvider(Doctor mddoctor)
        {

            if (!mddoctor.pcpId.Equals(Guid.Empty))
            {

                PcpTable dbProvider = (

                        from pcp in _icmsContext.Pcps
                        where pcp.pcp_id.Equals(mddoctor.pcpId)
                        select pcp
                    )
                    .FirstOrDefault();

                if (dbProvider != null)
                {
                    
                    dbProvider.disable_flag = true;

                    if (!mddoctor.creationDate.Equals(DateTime.MinValue)) dbProvider.date_updated = mddoctor.creationDate;
                    if (!mddoctor.usr.Equals(Guid.Empty)) dbProvider.user_updated = mddoctor.usr;

                    _icmsContext.Pcps.Update(dbProvider);
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
