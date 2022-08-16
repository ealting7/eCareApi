using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
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
