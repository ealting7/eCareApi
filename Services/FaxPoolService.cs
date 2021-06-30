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
        private readonly IcmsContext _databaseContext;

        public FaxPoolService(IcmsContext icmsContext)
        {
            _databaseContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
        }

        public IEnumerable<FaxQueue> GetFaxQueues()
        {
            IEnumerable<FaxQueue> queues = Enumerable.Empty<FaxQueue>();

            var items = _databaseContext.FaxQueues
                            .Where(queue => queue.fax_destination_flag.Equals(true))
                            .OrderByDescending(queue => queue.listorder);

            queues = items.ToList();

            return queues;
        }

        public IEnumerable<Fax> GetFaxPoolFaxes(int id)
        {
            IEnumerable<Fax> faxes = Enumerable.Empty<Fax>();

            //var items = _databaseContext.FaxPoolFaxes
            //                .Where(fx => fx.deleted_flag.Equals(0))
            //                .OrderByDescending(fx => fx.fax_creationtime);

            faxes = (from inbfax in _databaseContext.FaxPoolFaxes
                     join faxqueue in _databaseContext.FaxQueues
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
                     }).OrderByDescending(fax => fax.CreateDate).Take(10).ToList();


            //var items = _databaseContext.FaxPoolFaxes
            //                .GroupJoin(_databaseContext.FaxPoolFaxes, fax => fax.faxqueue_id, queue => queue.id) 
            //                .Where(fax => id > 0 ? fax.faxqueue_id.Equals(id) : fax.faxqueue_id > 0)
            //                .OrderByDescending(fx => fx.fax_creationtime)
            //                .Take(10);


            //faxes = items.ToList();

            return faxes;
        }

        public InboundFax GetFax(int id)
        {
            InboundFax fax = null;

            var faxItem = _databaseContext.FaxPoolFaxes
                            .Where(fx => fx.id.Equals(id));

            fax = faxItem.FirstOrDefault();

            return fax;
        }

        public InboundFax getFaxForPatchAssignTo(int id)
        {
            InboundFax returnedFax = new InboundFax();
            InboundFax fax = null;

            var faxItem = _databaseContext.FaxPoolFaxes
                            .Where(fx => fx.id.Equals(id));

            fax = faxItem.FirstOrDefault();

            if (fax != null)
            {
                returnedFax = fax;
            }

            return returnedFax;
        }


        public InboundFax UpdateAssignTo(int id, Fax updateFax)
        {
            InboundFax returnFax = null;

            return returnFax;
        }
    }
}
