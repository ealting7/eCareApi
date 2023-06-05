using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface ICaseManagement
    {

        public List<IcmsUser> getCmCaseOwners();

        public List<Case> getCaseOwnerCases(string caseOwnerId);

        public Case getCmPatient(string patId);

        public List<Patient> getCmPatientSearch(Patient srchPatients);

        public Case getCmPatientReports(string patId);

        public Case getCmPatientComphrehensiveReport(string patId, int compId);

        public List<Bill> getCmPatientBills(string patId);

        public Bill getCmPatientBill(string patId, string billId);

        public List<Bill> getCmPatientHistoricalBills(string patId);

        public Note getComprehensiveReportHistoricalNote(Note lcmNote);

        public Note getComprehensiveReportNote(string noteType, string followupId);


        public List<Case> flipCmCaseReports(Case report);
        public Case removeCmCaseReportReferral(Case report);
        public Case addCmCaseReportReferral(Case report);
        public Case removeCmCaseReportFacility(LcmReports report);
        public Case addCmCaseReportFacility(LcmReports report);
        public Case removeCmCaseReportCode(LcmReports report);
        public Case addCmCaseReportCode(LcmReports report);
        public Case updateCmCaseReportOriginal(LcmReports report);
        public Case updateCmCaseReportUpdated(LcmReports report);
        public Case updateCmCaseReportAdditional(LcmReports report);

        public Case updateCmCaseReportNotes(LcmReports report);

        public Case createCmCaseReportComprehensive(LcmReports report);
    }
}
