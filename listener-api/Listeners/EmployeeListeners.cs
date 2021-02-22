using listener_api.Interfaces;
using Steeltoe.Messaging.RabbitMQ.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace listener_api.Listeners
{
    public class EmployeeListeners
    {
        public const string RECEIVE_AND_CONVERT_QUEUE = "slack_comunication_queue";
        private readonly ISlackApi islackApi;

        public EmployeeListeners(ISlackApi islackApi)
        {
            this.islackApi = islackApi;
        }

        [RabbitListener(RECEIVE_AND_CONVERT_QUEUE)]
        public void ListenForAMessage(string bodymsg)
        {
            var msg = new MsgSlack() { Text = bodymsg };
            islackApi.SendWebhook(msg);
        }

    }
}
