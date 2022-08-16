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


namespace eCareApi.Services
{
    public class ClaimDataService
    {

        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspNetContext;
        private readonly IcmsDataStagingContext _dataStagingContext;

        public ClaimDataService(IcmsContext icmsContext, AspNetContext aspNetContext, IcmsDataStagingContext dataStagingContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
            _dataStagingContext = dataStagingContext ?? throw new ArgumentNullException(nameof(dataStagingContext));
        }


        public List<ClaimDataMine> getDataMinedClaims(ClaimDataMine searchParams)
        {
            List<ClaimDataMine> claims = null;


            List<ClaimDataMine> startClaims = null;
            startClaims = getDownloadedClaims(searchParams);

            List<ClaimDataMine> patientClaims = null;
            patientClaims = getPatientDownloadedClaims(searchParams, startClaims);

            List<ClaimDataMine> removedClaims = null;
            removedClaims = getRemovedClaims(searchParams, patientClaims);


            if (removedClaims != null && removedClaims.Count > 0)
            {
                claims = removedClaims;                
            }


            return claims;
        }

        private List<ClaimDataMine> getDownloadedClaims(ClaimDataMine searchParams)
        {
            List<ClaimDataMine> claims = null;

            try
            {

                bool srchTpa = false;
                bool srchDwnldDate = false;
                DateTime dwnldStartDate = DateTime.MinValue;
                DateTime dwnldEndDate = DateTime.MinValue;

                if (searchParams.tpaId > 0)
                {
                    srchTpa = true;
                }

                if (!searchParams.downloadStartDate.Equals(DateTime.MinValue))
                {
                    srchDwnldDate = true;
                    dwnldStartDate = Convert.ToDateTime(searchParams.downloadStartDate.ToShortDateString() + " 12:00AM");

                    if (!searchParams.downloadEndDate.Equals(DateTime.MinValue))
                    {
                        dwnldEndDate = Convert.ToDateTime(searchParams.downloadEndDate.ToShortDateString() + " 11:58PM");
                    }
                    else
                    {
                        dwnldEndDate = Convert.ToDateTime(searchParams.downloadStartDate.ToShortDateString() + " 11:58PM");
                    }
                }



                if (srchTpa)
                {

                    if (srchDwnldDate &&
                         (!dwnldStartDate.Equals(DateTime.MinValue) && !dwnldEndDate.Equals(DateTime.MinValue)))
                    {
                        claims = (
                                    from dwnldClaims in _dataStagingContext.TpaMedicalClaimses                                    
                                    where dwnldClaims.creation_date > dwnldStartDate && dwnldClaims.creation_date < dwnldEndDate
                                    && dwnldClaims.TPA_ID.Equals(searchParams.tpaId)
                                    select new ClaimDataMine
                                    {
                                        claimId = dwnldClaims.tpa_medical_claims_id,
                                        patientId = dwnldClaims.icms_member_id,                                        
                                        tpaId = dwnldClaims.TPA_ID,
                                        claimantFirstName = dwnldClaims.claimant_first_name,
                                        claimantLastName = dwnldClaims.claimant_last_name,
                                        diag1 = dwnldClaims.DIAG_1,
                                        diag1Desc = dwnldClaims.diag_desc_1,
                                        diag2 = dwnldClaims.DIAG_2,
                                        diag2Desc = dwnldClaims.DX_DESC_2,
                                        diag3 = dwnldClaims.DIAG_3,
                                        diag3Desc = dwnldClaims.DX_DESC_3,
                                        diag4 = dwnldClaims.DIAG_4,
                                        diag4Desc = "",
                                        cpt = dwnldClaims.CPT_CODE,
                                        hcpcs = dwnldClaims.hcpcs_code,
                                        serviceDate = dwnldClaims.service_date,
                                        providerTin = dwnldClaims.provider_tin,
                                        providerName = dwnldClaims.PROVIDER_NAME,
                                        posName = dwnldClaims.pos_name,
                                        paidAmount = dwnldClaims.claim_paid_amount,
                                    }
                                 )
                                 .Distinct()
                                 .ToList();
                    }
                    else
                    {

                        claims = (
                                from dwnldClaims in _dataStagingContext.TpaMedicalClaimses                               
                                where dwnldClaims.TPA_ID.Equals(searchParams.tpaId)
                                select new ClaimDataMine
                                {
                                    claimId = dwnldClaims.tpa_medical_claims_id,
                                    patientId = dwnldClaims.icms_member_id,                                    
                                    tpaId = dwnldClaims.TPA_ID,
                                    claimantFirstName = dwnldClaims.claimant_first_name,
                                    claimantLastName = dwnldClaims.claimant_last_name,
                                    diag1 = dwnldClaims.DIAG_1,
                                    diag1Desc = dwnldClaims.diag_desc_1,
                                    diag2 = dwnldClaims.DIAG_2,
                                    diag2Desc = dwnldClaims.DX_DESC_2,
                                    diag3 = dwnldClaims.DIAG_3,
                                    diag3Desc = dwnldClaims.DX_DESC_3,
                                    diag4 = dwnldClaims.DIAG_4,
                                    diag4Desc = "",
                                    cpt = dwnldClaims.CPT_CODE,
                                    hcpcs = dwnldClaims.hcpcs_code,
                                    serviceDate = dwnldClaims.service_date,
                                    providerTin = dwnldClaims.provider_tin,
                                    providerName = dwnldClaims.PROVIDER_NAME,
                                    posName = dwnldClaims.pos_name,
                                    paidAmount = dwnldClaims.claim_paid_amount,
                                }
                             )
                             .Distinct()
                             .ToList();

                    }


                }
                else if (srchDwnldDate &&
                         (!dwnldStartDate.Equals(DateTime.MinValue) && !dwnldEndDate.Equals(DateTime.MinValue)))
                {

                    claims = (
                                from dwnldClaims in _dataStagingContext.TpaMedicalClaimses                                
                                where dwnldClaims.creation_date > dwnldStartDate && dwnldClaims.creation_date < dwnldEndDate
                                select new ClaimDataMine
                                {
                                    claimId = dwnldClaims.tpa_medical_claims_id,
                                    patientId = dwnldClaims.icms_member_id,                                    
                                    tpaId = dwnldClaims.TPA_ID,
                                    claimantFirstName = dwnldClaims.claimant_first_name,
                                    claimantLastName = dwnldClaims.claimant_last_name,
                                    diag1 = dwnldClaims.DIAG_1,
                                    diag1Desc = dwnldClaims.diag_desc_1,
                                    diag2 = dwnldClaims.DIAG_2,
                                    diag2Desc = dwnldClaims.DX_DESC_2,
                                    diag3 = dwnldClaims.DIAG_3,
                                    diag3Desc = dwnldClaims.DX_DESC_3,
                                    diag4 = dwnldClaims.DIAG_4,
                                    diag4Desc = "",
                                    cpt = dwnldClaims.CPT_CODE,
                                    hcpcs = dwnldClaims.hcpcs_code,
                                    serviceDate = dwnldClaims.service_date,
                                    providerTin = dwnldClaims.provider_tin,
                                    providerName = dwnldClaims.PROVIDER_NAME,
                                    posName = dwnldClaims.pos_name,
                                    paidAmount = dwnldClaims.claim_paid_amount,
                                }
                             )
                             .Distinct()
                             .ToList();

                }


                return claims;
            }
            catch(Exception ex)
            {
                return claims;
            }
        }

        private List<ClaimDataMine> getPatientDownloadedClaims(ClaimDataMine searchParams, List<ClaimDataMine> claims)
        {
            List<ClaimDataMine> returnClaims = null;

            try
            {

                //returnClaims = (claims.Join(clm)


                //claims = (
                //            from dwnldClaims in _dataStagingContext.TpaMedicalClaimses

                //            join patient in _icmsContext.Patients
                //            on dwnldClaims.icms_member_id equals patient.member_id into pats
                //            from patients in pats.DefaultIfEmpty()

                //            join patientEnroll in _icmsContext.MemberEnrollments
                //            on patients.member_id equals patientEnroll.member_id into patEnrolls
                //            from patientEnrolls in patEnrolls.DefaultIfEmpty()

                //            join emply in _icmsContext.Employers
                //            on patientEnrolls.employer_id equals emply.employer_id into emplys
                //            from employers in emplys.DefaultIfEmpty()

                //            join tpaEmply in _icmsContext.TpaEmployers
                //            on employers.employer_id equals tpaEmply.employer_id into tpaEmplys
                //            from tpaEmpoyers in tpaEmplys.DefaultIfEmpty()

                //            join tpas in _icmsContext.Tpas
                //            on tpaEmpoyers.tpa_id equals tpas.tpa_id into tpaes
                //            from TpasDb in tpaes.DefaultIfEmpty()

                //            where dwnldClaims.creation_date > dwnldStartDate && dwnldClaims.creation_date < dwnldEndDate
                //            && dwnldClaims.TPA_ID.Equals(searchParams.tpaId)
                //            select new ClaimDataMine
                //            {
                //                claimId = dwnldClaims.tpa_medical_claims_id,
                //                patientId = dwnldClaims.icms_member_id,
                //                patientSsn = patients.member_ssn,
                //                patientBirth = patients.member_birth,
                //                inLcm = patients.member_in_lcm,
                //                inDm = patients.member_in_dm,
                //                employerName = employers.employer_name,
                //                tpaName = TpasDb.tpa_name,
                //                tpaId = dwnldClaims.TPA_ID,
                //                claimantFirstName = dwnldClaims.claimant_first_name,
                //                claimantLastName = dwnldClaims.claimant_last_name,
                //                diag1 = dwnldClaims.DIAG_1,
                //                diag1Desc = dwnldClaims.diag_desc_1,
                //                diag2 = dwnldClaims.DIAG_2,
                //                diag2Desc = dwnldClaims.DX_DESC_2,
                //                diag3 = dwnldClaims.DIAG_3,
                //                diag3Desc = dwnldClaims.DX_DESC_3,
                //                diag4 = dwnldClaims.DIAG_4,
                //                diag4Desc = "",
                //                cpt = dwnldClaims.CPT_CODE,
                //                hcpcs = dwnldClaims.hcpcs_code,
                //                serviceDate = dwnldClaims.service_date,
                //                providerTin = dwnldClaims.provider_tin,
                //                providerName = dwnldClaims.PROVIDER_NAME,
                //                posName = dwnldClaims.pos_name,
                //                paidAmount = dwnldClaims.claim_paid_amount,
                //            }
                //            )
                //            .Distinct()
                //            .ToList();
                    


                return returnClaims;
            }
            catch (Exception ex)
            {
                return returnClaims;
            }
        }

        private List<ClaimDataMine> getRemovedClaims(ClaimDataMine searchParams, List<ClaimDataMine> claims)
        {
            List<ClaimDataMine> returnClaims = null;

            if (claims != null && claims.Count > 0)
            {
                if (searchParams.removeLcmPatients)
                {
                    returnClaims = claims.Where(clm =>clm.inLcm.Value.Equals(false)).ToList();
                }

                if (searchParams.removeLcmTriggers)
                {

                }

                if (searchParams.removeInactivePatients)
                {

                }

                if (searchParams.removeOlderThanPatients && searchParams.removeOlderThanAge > 0)
                {

                }
            }

            return returnClaims;
        }

    }
}
