using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class FaxPoolQueue
    {
        public int QueueId { get; set; }

        public string QueueName { get; set; }

        public int? ListOrder { get; set; }

        public int? ParentQueueId { get; set; }
    }
}
