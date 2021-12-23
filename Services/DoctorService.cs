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

        public DoctorService(IcmsContext icmsContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
        }

        public IEnumerable<Doctor> GetDoctors(string first, string last, string state)
        {
            IEnumerable<Doctor> doctors = Enumerable.Empty<Doctor>();

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

                if (!string.IsNullOrWhiteSpace(phoneNumber))
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
    }
}
