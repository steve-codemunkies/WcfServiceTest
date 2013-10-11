using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace WcfServiceTestClient
{
    public class SessionTokenInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var correlationId = Guid.NewGuid();
            var messageData = ConsumeMessage(request);

            var header = new MessageHeader<string>("hello everybody! hello doctor nick!");
            //var header = new MessageHeader<string>("hello everybody! hello doctor steve!");
            var untyped = header.GetUntypedHeader("UkashSession", "http://ukashapi.ukash.com");
            messageData.NewMessage.Headers.Add(untyped);

            request = messageData.NewMessage;

            return correlationId;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        /// <summary>
        /// Consumes a <see cref="Message"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Message"/> needs to be considered as read-once only. This means that
        /// the consumer needs to recreate the message after consuming it.
        /// 
        /// This method helps with that by reading the contents of the message to a string, and
        /// recreating the incoming message.
        /// </remarks>
        /// <param name="message">The <see cref="Message"/> to be consumed.</param>
        /// <returns>The contents of the message as a <see cref="string"/></returns>
        private MessageData ConsumeMessage(Message message)
        {
            string messageString;

            var ms = GetMessageAsMemoryStream(message);

            return new MessageData()
                {
                    MessageString = GetMessageAsString(ms),
                    NewMessage = CreateReplacementMessage(message, ms)
                };
        }

        private class MessageData
        {
            public string MessageString { get; set; }
            public Message NewMessage { get; set; }
        }

        private static MemoryStream GetMessageAsMemoryStream(Message message)
        {
            var ms = new MemoryStream();
            var writer = XmlWriter.Create(ms);
            message.WriteMessage(writer);
            writer.Flush();
            return ms;
        }

        private static string GetMessageAsString(MemoryStream ms)
        {
            ms.Position = 0;
            var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        private static Message CreateReplacementMessage(Message message, MemoryStream ms)
        {
            ms.Position = 0;
            var reader = XmlReader.Create(ms);
            var newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            return newMessage;
        }
    }
}