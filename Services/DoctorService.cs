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
    }
}
