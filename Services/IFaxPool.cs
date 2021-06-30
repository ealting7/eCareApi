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

        InboundFax GetFax(int id);

        InboundFax getFaxForPatchAssignTo(int id);

        InboundFax UpdateAssignTo(int id, Fax updateFax);
    }
}
