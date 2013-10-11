using System;
using WcfServiceTestClient.ServiceReference1;

namespace WcfServiceTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new Service1Client())
            {
                client.ChannelFactory.Endpoint.Behaviors.Add(new SessionTokenBehavior());
                Console.WriteLine(string.Format("Service returned: {0}", client.GetData(1234)));
            }

            Console.ReadLine();
        }
    }
}
