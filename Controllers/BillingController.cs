using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace eCareApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly IBilling _billingInterface;
        private readonly IHostingEnvironment _hostingEnvironment;

        public BillingController(IBilling billingInterface, IHostingEnvironment hostingEnvironment)
        {
            _billingInterface = billingInterface ?? throw new ArgumentNullException(nameof(billingInterface));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }


        [HttpGet("dbms/bills/current")]
        public IActionResult GetDbmsCurrentBilling()
        {
            var bills = _billingInterface.GetDbmsCurrentBilling();

            if (bills == null || bills.Count().Equals(0))
            {
                return NoContent();
            }

            var returnBills = new List<Bill>();

            foreach (var bill in bills)
            {
                returnBills.Add(new Bill
                {
                    billId = bill.billId,
                    lcmInvoiceNumber = bill.lcmInvoiceNumber,
                    billingPatient = bill.billingPatient,
                    memberId = bill.memberId,
                    patientFullName = bill.patientFullName,
                    employer = bill.employer,
                    employerBillingRate = bill.employerBillingRate,
                    recordDate = bill.recordDate,
                    billCode = bill.billCode,
                    billCodeDescription = bill.billCodeDescription,
                    billMinutes = bill.billMinutes,
                    billDisabled = bill.billDisabled,
                    billSentDate = (bill.billSentDate.Equals(DateTime.MinValue)) ? null : bill.billSentDate,
                    billingStartDate = bill.billingStartDate,
                    billingEndDate = bill.billingEndDate
                }); 
            }


            return Ok(returnBills);
        }

        
        [HttpPost("dbms/bills/current/save")]
        public IActionResult saveDbmsCurrentBilliong(List<Bill> bills)
        {

            var returnPayments = _billingInterface.saveBilling(bills);

            if (returnPayments == null)
            {
                return NoContent();
            }

            return Ok(returnPayments);
        }

        [HttpGet("dbms/bills/current/{id}")]
        public IActionResult GetBill(string id)
        {
            var bill = _billingInterface.GetBill(id);

            if (bill == null)
            {
                return NoContent();
            }

            return Ok(bill);
        }


        [HttpGet("dbms/billing/codes")]
        public IActionResult GetDbmsBillingCodes()
        {
            var codes = _billingInterface.GetDbmsBillingCodes();

            if (codes == null)
            {
                return NoContent();
            }

            var returnBillingCodes = new List<BillingCodes>();

            foreach (var code in codes)
            {
                returnBillingCodes.Add(new BillingCodes
                {
                    billingCodeId = code.billingCodeId,
                    billingCode = code.billingCode,
                    billingDescription = code.billingDescription,
                    displayCodeDescription = code.displayCodeDescription
                });
            }


            return Ok(returnBillingCodes);
        }


        [HttpGet("dbms/bills/notes/{id}")]
        public IActionResult GetDbmsCurrentBillingNote(string id)
        {
            var note = _billingInterface.GetDbmsCurrentBillingNote(id);

            if (note == null)
            {
                return NoContent();
            }

            return Ok(note);
        }

        
        [HttpGet("dbms/bills/backupnotes/{id}")]
        public IActionResult GetBackupNote(string id)
        {
            var note = _billingInterface.GetBackupBillingNote(id);

            if (note == null)
            {
                return NoContent();
            }

            return Ok(note);
        }


        [HttpPost("dbms/bills/refresh")]
        public IActionResult GetDbmsRefreshBills(Bill refreshDates)
        {
            var bills = _billingInterface.GetDbmsRefreshBills(refreshDates);

            if (bills == null)
            {
                return NoContent();
            }

            return Ok(bills);
        }




        [HttpPost("dbms/bills/refresh/add")]
        public IActionResult AddRefreshBillsToCurrentBilling(List<Bill> bills)
        {
            var returnBills = _billingInterface.AddRefreshBillsToCurrentBilling(bills);

            if (returnBills == null)
            {
                return NoContent();
            }

            return Ok(returnBills);
        }


        [HttpPost("dbms/bills/refreshdates")]
        public IActionResult AddRefreshDates(Bill dates)
        {
            var returnDates = _billingInterface.AddRefreshDates(dates);

            if (returnDates == null)
            {
                return NoContent();
            }

            return Ok(returnDates);
        }



        [HttpGet("dbms/bills/get/refreshdates")]
        public IActionResult getLastBillingRefreshDates()
        {
            var returnDates = _billingInterface.getLastBillingRefreshDates();

            if (returnDates == null)
            {
                return NoContent();
            }

            return Ok(returnDates);
        }


        [HttpPost("dbms/bills/invoice/create")]
        public IActionResult createBillInvoicePdf(List<Bill> bills)
        {
            string projectRootPath = Path.Combine(_hostingEnvironment.WebRootPath, "images\\billing");


            var returnBillFile = _billingInterface.createBillInvoicePdf(projectRootPath, bills);

            if (returnBillFile == null)
            {
                return NoContent();
            }

            return Ok(returnBillFile);

            //return File(returnBillFile.invoicePdf, returnBillFile.invoiceContentType);
        }


        [HttpGet("dbms/bills/get/billingperiods/{type}/{span}")]
        public IActionResult getBillingPeriods(string type, string span)
        {
            var returnDates = _billingInterface.getBillingPeriods(type, span);

            if (returnDates == null)
            {
                return NoContent();
            }

            return Ok(returnDates);
        }


        [HttpGet("dbms/bills/ar/lcm/{date}")]
        public IActionResult getBillsFromBillingPeriod(string date)
        {
            var bills = _billingInterface.getBillsFromBillingPeriod(date);

            if (bills == null || bills.Count().Equals(0))
            {
                return NoContent();
            }

            var returnBills = new List<Bill>();

            foreach (var bill in bills)
            {
                returnBills.Add(new Bill
                {
                    memberId = bill.memberId,
                    patientFullName = bill.patientFullName,
                    employer = bill.employer,
                    lcmInvoiceNumber = bill.lcmInvoiceNumber,
                    billTotalAmount = bill.billTotalAmount,
                    billTotalPaymentAmount = bill.billTotalPaymentAmount,
                    billRemainingBalance = bill.billRemainingBalance
                }); ;
            }


            return Ok(returnBills);
        }

        
        [HttpPost("dbms/bills/ar/lcm/status/report")]
        public IActionResult getArStatusBills(Bill billPeriods)
        {

            var retBills = _billingInterface.getArStatusBills(billPeriods);

            if (retBills == null)
            {
                return NoContent();
            }

            return Ok(retBills);
        }

        [HttpPost("dbms/bills/ar/lcm/status/report/display")]
        public IActionResult runArStatusReport(List<Bill> bill)
        {

            var retRpt = _billingInterface.runArStatusReport(bill);

            if (retRpt == null)
            {
                return NoContent();
            }

            return Ok(retRpt);
        }
        


        [HttpGet("dbms/bills/ar/lcm/bill/{lcmInvoiceNumber}/{id}")]
        public IActionResult getBillFromBillingBackup(string lcmInvoiceNumber, string id)
        {
            var bills = _billingInterface.getBillsFromBillingBackup(lcmInvoiceNumber, id);

            if (bills == null || bills.Count().Equals(0))
            {
                return NoContent();
            }

            return Ok(bills);
        }


        [HttpGet("dbms/bills/ar/lcm/payment/{lcmInvoiceNumber}/{id}")]
        public IActionResult getBillPaymentHistory(string lcmInvoiceNumber, string id)
        {
            var payment = _billingInterface.getBillPaymentHistory(lcmInvoiceNumber, id);

            if (payment == null)
            {
                return NoContent();
            }

            return Ok(payment);
        }

        [HttpGet("dbms/bills/ar/lcm/total/{lcmInvoiceNumber}/{id}")]
        public IActionResult getBillTotalAmount(string lcmInvoiceNumber, string id)
        {
            var billtotal = _billingInterface.getBillTotalAmount(lcmInvoiceNumber, id);

            if (billtotal <= 0)
            {
                return NoContent();
            }

            return Ok(billtotal);
        }


        
        [HttpGet("dbms/bills/ar/lcm/payment/{id}")]
        public IActionResult getArPayment(string id)
        {
            var payment = _billingInterface.getArPayment(id);

            if (payment == null)
            {
                return NoContent();
            }

            return Ok(payment);
        }




        [HttpPost("dbms/bills/ar/lcm/payment")]
        public IActionResult makeArPayment(Payment pay)
        {

            var returnPayments = _billingInterface.makeArPayment(pay);

            if (returnPayments == null)
            {
                return NoContent();
            }

            return Ok(returnPayments);
        }

        
        [HttpPost("dbms/bills/ar/lcm/payment/delete")]
        public IActionResult deleteArPayment(Payment pay)
        {
            var removed = _billingInterface.deleteArPayment(pay);
            return Ok(removed);
        }




        [HttpPost("dbms/bills/unpaid/report")]
        public IActionResult runUnpaidReport([FromForm]IFormCollection fileData)
        {

            var unpaidRpt = _billingInterface.runUnpaidReport(fileData);

            if (unpaidRpt == null)
            {
                return NoContent();
            }

            return Ok(unpaidRpt);

        }
        


    }
}
