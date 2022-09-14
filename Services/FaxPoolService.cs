using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class FaxPoolService : IFaxPool
    {
        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspNetContext;
        private readonly IcmsDataStagingContext _dataStagingContext;

        public FaxPoolService(IcmsContext icmsContext, AspNetContext aspNetContext, IcmsDataStagingContext dataStagingContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
            _dataStagingContext = dataStagingContext ?? throw new ArgumentNullException(nameof(dataStagingContext));
        }

        public IEnumerable<FaxQueue> GetFaxQueues()
        {
            IEnumerable<FaxQueue> queues = Enumerable.Empty<FaxQueue>();

            var items = _icmsContext.FaxQueues
                            .Where(queue => queue.fax_destination_flag.Equals(true))
                            .OrderByDescending(queue => queue.listorder);

            queues = items.ToList();

            return queues;
        }

        public IEnumerable<Fax> GetFaxPoolFaxes(int id)
        {
            IEnumerable<Fax> faxes = Enumerable.Empty<Fax>();

            faxes = (from inbfax in _icmsContext.rInboundFaxes
                     join faxqueue in _icmsContext.FaxQueues
                     on inbfax.faxqueue_id equals faxqueue.id into fxplque
                     from faxpoolqueue in fxplque.DefaultIfEmpty()
                     where id > 0 ? inbfax.faxqueue_id.Equals(id) : (inbfax.faxqueue_id.Equals(null) || inbfax.faxqueue_id.Equals(0))
                     select new Fax
                     {
                         FaxId = inbfax.id,
                         CreateDate = inbfax.fax_creationtime,
                         AssignedToUserId = inbfax.assigned_to_user_id,
                         FaxImage = inbfax.fax_image,
                         FaxQueueId = inbfax.faxqueue_id,
                         FaxQueueName = faxpoolqueue.queue_name
                     })
                     .OrderByDescending(fax => fax.CreateDate)
                     .Take(10)
                     .ToList();


            return faxes;
        }

        public IEnumerable<Fax> getUmFaxPoolFaxes(int id)
        {
            IEnumerable<Fax> faxes = Enumerable.Empty<Fax>();

            faxes = (

                from inbfax in _icmsContext.rInboundFaxes

                join faxqueue in _icmsContext.FaxQueues
                on inbfax.faxqueue_id equals faxqueue.id into fxplque

                from faxpoolqueue in fxplque.DefaultIfEmpty()
                where inbfax.faxqueue_id.Equals(id)
                && inbfax.deleted_flag.Equals(false)
                && !inbfax.member_id.HasValue
                && (inbfax.referral_number.Length.Equals(0) || inbfax.referral_number == null)
                && !inbfax.assigned_by_user_id.HasValue
                select new Fax
                    {
                        FaxId = inbfax.id,
                        CreateDate = inbfax.fax_creationtime,
                        AssignedToUserId = inbfax.assigned_to_user_id,
                        FaxQueueId = inbfax.faxqueue_id,
                        FaxQueueName = faxpoolqueue.queue_name,
                        MemberId = inbfax.member_id,
                        ReferralNumber = inbfax.referral_number,
                        deleted = inbfax.deleted_flag
                    }
                )
                .OrderByDescending(fax => fax.CreateDate)
                .Take(25)
                .ToList();


            return faxes;
        }

        public InboundFax GetFax(int id)
        {
            InboundFax fax = null;

            var faxItem = _icmsContext.rInboundFaxes
                            .Where(fx => fx.id.Equals(id));

            fax = faxItem.FirstOrDefault();

            return fax;
        }

        public DocumentForm getFaxToView(int id)
        {

            DocumentForm attachment = null;

            if (id > 0)
            {

                attachment = (

                    from faxpool in _icmsContext.rInboundFaxes
                    where faxpool.id.Equals(id)
                    select new DocumentForm
                    {
                        documentId = faxpool.id,
                        documentFileName = (!string.IsNullOrEmpty(faxpool.email_filename)) ? faxpool.email_filename : "faxpool.pdf",
                        creationDate = faxpool.fax_creationtime,
                        displayCreationDate = (faxpool.fax_creationtime.HasValue) ?
                            faxpool.fax_creationtime.Value.ToShortDateString() + " " + faxpool.fax_creationtime.Value.ToShortTimeString() :
                            "",
                        documentImage = faxpool.fax_image,
                        documentBase64 = Convert.ToBase64String(faxpool.fax_image)
                    }
                )
                .FirstOrDefault();


                if (attachment != null && !string.IsNullOrEmpty(attachment.documentFileName))
                {
                    switch (Path.GetExtension(attachment.documentFileName))
                    {
                        case ".gif":
                            attachment.documentContentType = "image/gif";
                            break;
                        case ".jpg":
                            attachment.documentContentType = "image/jpeg";
                            break;
                        case ".bmp":
                            attachment.documentContentType = "image/x-windows-bmp";
                            break;
                        case ".png":
                            attachment.documentContentType = "image/png";
                            break;
                        case ".csv":
                            attachment.documentContentType = "text/csv";
                            break;
                        case ".pdf":
                            attachment.documentContentType = "application/pdf";
                            break;
                        case ".txt":
                            attachment.documentContentType = "text/plain";
                            break;
                        case ".xls":
                            attachment.documentContentType = "application/vnd.ms-excel";
                            break;
                        case ".xlsx":
                            attachment.documentContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            break;
                        case ".doc":
                            attachment.documentContentType = "application/msword";
                            break;
                        case ".docx":
                            attachment.documentContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            break;
                        default:
                            attachment.documentContentType = "application/pdf";
                            break;
                    }
                }
            }


            return attachment;
        }

        public InboundFax getFaxForPatchAssignTo(int id)
        {
            InboundFax returnedFax = new InboundFax();
            InboundFax fax = null;

            var faxItem = _icmsContext.rInboundFaxes
                            .Where(fx => fx.id.Equals(id));

            fax = faxItem.FirstOrDefault();

            if (fax != null)
            {
                returnedFax = fax;
            }

            return returnedFax;
        }


        public List<Fax> updateFaxPoolUm(Fax faxpoolFax)
        {

            List<Fax> referralFaxes = null;

            if (faxpoolFax.FaxId > 0)
            {

                InboundFax dbFaxPoolFax = (
                        from rInbFx in _icmsContext.rInboundFaxes
                        where rInbFx.id.Equals(faxpoolFax.FaxId)
                        select rInbFx
                    )
                    .FirstOrDefault();

                if (dbFaxPoolFax != null && 
                    !dbFaxPoolFax.member_id.HasValue && 
                    (dbFaxPoolFax.referral_number.Length.Equals(0) || dbFaxPoolFax.referral_number == null))
                {

                    if (!faxpoolFax.MemberId.Equals(Guid.Empty) && !string.IsNullOrEmpty(faxpoolFax.ReferralNumber))
                    {
                        dbFaxPoolFax.member_id = faxpoolFax.MemberId;
                        dbFaxPoolFax.referral_number = faxpoolFax.ReferralNumber;

                        _icmsContext.rInboundFaxes.Update(dbFaxPoolFax);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {

                            UtilizationManagementService utilServ = new UtilizationManagementService(_icmsContext, _aspNetContext, _dataStagingContext);
                            referralFaxes = utilServ.getReferralFaxes(faxpoolFax.ReferralNumber, (Guid)faxpoolFax.MemberId);
                        }
                    }
                }
            }

            return referralFaxes;
        }


        public List<Fax> uploadFaxPoolUm(Fax faxpoolFax)
        {

            List<Fax> referralFaxes = null;

            try
            {

                if (faxpoolFax != null &&
                    faxpoolFax.MemberId.HasValue &&
                    !string.IsNullOrEmpty(faxpoolFax.ReferralNumber) &&
                    (faxpoolFax.FaxImage != null && faxpoolFax.FaxImage.Length > 0))
                {

                    InboundFax dbFaxPoolFax = new InboundFax();
                    dbFaxPoolFax.deleted_flag = false;
                    dbFaxPoolFax.fax_creationtime = DateTime.Now;
                    dbFaxPoolFax.fax_image = faxpoolFax.FaxImage;
                    dbFaxPoolFax.faxqueue_id = 16;
                    dbFaxPoolFax.referral_number = faxpoolFax.ReferralNumber;
                    dbFaxPoolFax.member_id = faxpoolFax.MemberId;
                    dbFaxPoolFax.email_filename = (!string.IsNullOrEmpty(faxpoolFax.faxName)) ? faxpoolFax.faxName : "";
                    dbFaxPoolFax.fax_remoteid = "SIMS Upload";
                    dbFaxPoolFax.fax_type_id = 1;
                    dbFaxPoolFax.ready_flag = true;
                    dbFaxPoolFax.priority_level = 2;

                    _icmsContext.rInboundFaxes.Add(dbFaxPoolFax);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {

                        UtilizationManagementService utilServ = new UtilizationManagementService(_icmsContext, _aspNetContext, _dataStagingContext);
                        referralFaxes = utilServ.getReferralFaxes(faxpoolFax.ReferralNumber, (Guid)faxpoolFax.MemberId);
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return referralFaxes;
        }

        public List<DocumentForm> uploadFaxPoolCmAttachment(Fax faxpoolFax)
        {

            List<DocumentForm> cmAttachments = null;

            try
            {

                if (!faxpoolFax.MemberId.Equals(Guid.Empty))
                {

                    InboundFax dbFax = GetFax(faxpoolFax.FaxId);

                    if (dbFax != null)
                    {

                        //create a new cm attachment
                        MemberNotesAttachment newCmAttachment = new MemberNotesAttachment();

                        newCmAttachment.creation_user_id = faxpoolFax.AssignedToUserId;
                        newCmAttachment.file_blob = dbFax.fax_image;
                        newCmAttachment.file_identifier = "Patient Fax Pool Document";
                        newCmAttachment.member_id = (Guid)faxpoolFax.MemberId;
                        newCmAttachment.record_date = (DateTime)faxpoolFax.CreateDate;

                        _icmsContext.MemberNotesAttachments.Add(newCmAttachment);
                        
                        if (_icmsContext.SaveChanges() > 0)
                        {

                            //update the fax in the fax pool
                            dbFax.member_id = (Guid)faxpoolFax.MemberId;
                            dbFax.completed_by_user_id = faxpoolFax.AssignedToUserId;
                            dbFax.completed_date = (DateTime)faxpoolFax.CreateDate;

                            _icmsContext.rInboundFaxes.Update(dbFax);
                            _icmsContext.SaveChanges();


                            NoteService noteServ = new NoteService(_icmsContext, _aspNetContext, _dataStagingContext);
                            cmAttachments = noteServ.getPatientCmAttachments(faxpoolFax.MemberId.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return cmAttachments;
        }


        public List<Fax> removeFaxPoolUm(Fax faxpoolFax)
        {

            List<Fax> referralFaxes = null;

            if (faxpoolFax.FaxId > 0 && !string.IsNullOrEmpty(faxpoolFax.ReferralNumber))
            {

                InboundFax dbFaxPoolFax = (
                        from rInbFx in _icmsContext.rInboundFaxes
                        where rInbFx.id.Equals(faxpoolFax.FaxId)
                        && rInbFx.referral_number.Equals(faxpoolFax.ReferralNumber)
                        select rInbFx
                    )
                    .FirstOrDefault();

                if (dbFaxPoolFax != null)
                {

                    if (!faxpoolFax.MemberId.Equals(Guid.Empty) && !string.IsNullOrEmpty(faxpoolFax.ReferralNumber))
                    {
                        dbFaxPoolFax.member_id = null;
                        dbFaxPoolFax.referral_number = null;
                        dbFaxPoolFax.faxqueue_id = 16;

                        _icmsContext.rInboundFaxes.Update(dbFaxPoolFax);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {

                            UtilizationManagementService utilServ = new UtilizationManagementService(_icmsContext, _aspNetContext, _dataStagingContext);
                            referralFaxes = utilServ.getReferralFaxes(faxpoolFax.ReferralNumber, (Guid)faxpoolFax.MemberId);
                        }
                    }
                }
            }

            return referralFaxes;
        }


        public InboundFax UpdateAssignTo(int id, Fax updateFax)
        {
            InboundFax returnFax = null;

            return returnFax;
        }
    }
}
