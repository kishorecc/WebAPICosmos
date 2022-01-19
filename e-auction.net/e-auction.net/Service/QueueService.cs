using Azure.Messaging.ServiceBus;
using Azure.Storage.Queues;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace e_auction.net.Service
{
    public static class QueueService
    {
        private static readonly string queueURL = ConfigurationManager.AppSettings["queue"];
        private static readonly string queueKey = ConfigurationManager.AppSettings["Qkey"];
        public static ServiceBusClient client = new ServiceBusClient(queueURL);
        public static ServiceBusSender sender = client.CreateSender("placebids");

        public static async Task InsertMessageAsync(string newMessage)
        {
            // create a batch
            ServiceBusMessage message = new ServiceBusMessage(newMessage);

            await sender.SendMessageAsync(message);
        }

        public static void InsertRabbitMQMessageAsync(string newMessage)
        {
            // create a batch
            string UserName = "user";

            string Password = "tousa0lFCR";

            string HostName = "20.62.177.14";

            var connectionFactory = new RabbitMQ.Client.ConnectionFactory()

            {

                UserName = UserName,

                Password = Password,

                HostName = HostName

            };



            var connection = connectionFactory.CreateConnection();

            var model = connection.CreateModel();



            //model.QueueBind("getbids", "amq.direct", "getbids");

            var properties = model.CreateBasicProperties();

            properties.Persistent = false;


            byte[] messagebuffer = Encoding.Default.GetBytes(newMessage);



            model.BasicPublish("amq.direct", "getbids", properties, messagebuffer);


        }
    }
}
