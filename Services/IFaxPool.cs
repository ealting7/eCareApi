using eCareApi.Entities;
using eCareApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IFaxPool
    {
        IEnumerable<FaxQueue> GetFaxQueues();

        IEnumerable<Fax> GetFaxPoolFaxes(int id);

        public IEnumerable<Fax> getUmFaxPoolFaxes(int id);

        InboundFax GetFax(int id);

        public DocumentForm getFaxToView(int id);

        InboundFax getFaxForPatchAssignTo(int id);

        public List<Fax> updateFaxPoolUm(Fax faxpoolFax);

        public List<Fax> uploadFaxPoolUm(Fax faxpoolFax);

        public List<DocumentForm> uploadFaxPoolCmAttachment(Fax faxpoolFax);

        public List<Fax> removeFaxPoolUm(Fax faxpoolFax);

        InboundFax UpdateAssignTo(int id, Fax updateFax);
    }
}
