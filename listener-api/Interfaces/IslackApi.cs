using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace listener_api.Interfaces
{
    public interface ISlackApi
    {
        [Post("/services/T01NZEPF420/B01NCH3RW79/OTB4WN9PaB7smNEyYWR5PpS0")]
        Task SendWebhook([Body] MsgSlack msg );
    }

    public class MsgSlack
    {
        public string Text { get; set; }
    }
}
