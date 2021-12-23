using eCareApi.Entities;
using eCareApi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;


namespace eCareApi.Services
{
    public interface IBilling
    {
        IEnumerable<Bill> GetDbmsCurrentBilling();

        List<Bill> saveBilling(List<Bill> bills);

        Bill GetBill(string id);

        List<BillingCodes> GetDbmsBillingCodes();

        Note GetDbmsCurrentBillingNote(string id);

        Note GetBackupBillingNote(string id); 

        IEnumerable<Bill> GetDbmsRefreshBills(Bill refreshDates);

        IEnumerable<Bill> AddRefreshBillsToCurrentBilling(List<Bill> bills);

        Bill getLastBillingRefreshDates();

        Bill AddRefreshDates(Bill dates);

        Bill createBillInvoicePdf(string directory, List<Bill> bills);

        Bill getBillingPeriods(string type, string span);

        IEnumerable<Bill> getBillsFromBillingPeriod(string date);

        List<Bill> getArStatusBills(Bill billPeriods);

        IEnumerable<Bill> getBillsFromBillingBackup(string lcmInvoiceNumber, string id);

        Bill getBillPaymentHistory(string lcmInvoiceNumber, string id);

        decimal getBillTotalAmount(string lcmInvoiceNumber, string id);

        Payment getArPayment(string id);

        List<Payment> makeArPayment(Payment pay);

        bool deleteArPayment(Payment pay);

        UnpaidReport runUnpaidReport(IFormCollection fileData);

        ArStatusReport runArStatusReport(List<Bill> bill);
    }
}
