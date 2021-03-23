/*
© Siemens AG, 2021
Author: Berkay Alp Cakal (berkay_alp.cakal.ct@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Ros2SocketClient
{
    public delegate void SubscriptionCallback<T>(T t) where T : Message;

    public enum CommunicatorOperation { Register, Unregister };

    public class MetaSocket
    {
        private PairSocket socket;
        private List<ICommunicator> communicators = new List<ICommunicator>();
        public ManualResetEvent isConnected = new ManualResetEvent(false);
        public Log log;

        public MetaSocket(string _host, string _port, Log _log)
        {
            log = _log;

            AsyncIO.ForceDotNet.Force();
            socket = new PairSocket();
            socket.Bind($"tcp://{_host}:{_port}");
            isConnected.Set();

            log($"MetaSocket bound to:    tcp://{_host}:{_port}");
        }

        public void RegisterCommunicator(ICommunicator communicator)
        {
            communicators.Add(communicator);
                        
            NetMQMessage msg = new NetMQMessage(new List<byte[]> {
                Encoding.UTF8.GetBytes(communicator.Port),
                Encoding.UTF8.GetBytes(communicator.Type),
                Encoding.UTF8.GetBytes(CommunicatorOperation.Register.ToString()),
                Encoding.UTF8.GetBytes(communicator.Topic),
                Encoding.UTF8.GetBytes(communicator.MessageType)
            });

            socket.SendMultipartMessage(msg);
        }

        public void UnregisterCommunicator(ICommunicator communicator)
        {
            communicators.Remove(communicator);

            NetMQMessage msg = new NetMQMessage(new List<byte[]> {
                Encoding.UTF8.GetBytes(communicator.Port),
                Encoding.UTF8.GetBytes(communicator.Type),
                Encoding.UTF8.GetBytes(CommunicatorOperation.Unregister.ToString())
            });

            socket.SendMultipartMessage(msg);
        }


        public void UnregisterAll()
        {
            foreach (var communicator in communicators.ToList())
                communicator.UnregisterSocket();

            socket.Close();
            socket.Dispose();
            NetMQConfig.Cleanup();

            isConnected.Reset();
            log("MetaSocket unregistered all");
        }
    }
    public interface ICommunicator
    {
        string MessageType { get; }
        string Topic { get; }
        string Port { get; }
        string Type { get; }

        void RegisterSocket();
        void UnregisterSocket();
    }

    public abstract class Communicator<T> : ICommunicator where T : Message, new()
    {
        protected NetMQPoller poller;
        protected NetMQSocket socket;
        protected MetaSocket metaSocket;
        protected string url;

        protected Log log;
        protected T message;

        protected string topic;
        protected int port;

        public string MessageType { get { return message.Type; } }
        public string Topic { get { return topic; } }
        public string Port { get { return port.ToString(); } }

        public string Type { get { return GetType().Name.Split('`')[0]; } }
        
        public void RegisterSocket()
        {
            metaSocket.isConnected.WaitOne();

            port = socket.BindRandomPort($"tcp://{url}");
            metaSocket.RegisterCommunicator(this);
            log($"Communicator socket bound to port: {port}");
        }
        public void UnregisterSocket()
        {
            metaSocket.isConnected.WaitOne();
            metaSocket.UnregisterCommunicator(this);

            if (poller != null)
            {
                if (poller.IsRunning)
                    poller.Stop();
                if(!poller.IsDisposed)
                    poller.Dispose();
            }

            socket.Close();
            socket.Dispose();
            log($"Unregistered the communicator socket with port {port}");
        }
    }


    public class Subscriber<T> : Communicator<T> where T : Message, new()
    {
        public Subscriber(MetaSocket _metaSocket, SubscriptionCallback<T> _subscriptionHandler, string _url, string _topic, Log _log)
        {
            log = _log;
            metaSocket = _metaSocket;
            url = _url;
            topic = _topic;
            message = new T();

            SubscriberSocket subscriberSocket = new SubscriberSocket();
            socket = subscriberSocket;
            RegisterSocket();

            subscriberSocket.SubscribeToAnyTopic();
            subscriberSocket.ReceiveReady += (sender, args) =>
            {
                OnReceiveMessage(_subscriptionHandler, args.Socket.ReceiveFrameBytes());
            };

            poller = new NetMQPoller();
            poller.Add(subscriberSocket);
            poller.RunAsync();
        }

        public void OnReceiveMessage(SubscriptionCallback<T> SubscriptionCallback, byte[] encoded_msg)
        {
            //message.Deserialize(encoded_msg);
            SubscriptionCallback(message);
        }
    }


    public class Publisher<T> : Communicator<T> where T : Message, new()
    {
        public Publisher(MetaSocket _metaSocket, string _url, string _topic, Log _log)
        {
            //communicatorType = CommunicatorType.Publisher;
            log = _log;
            metaSocket = _metaSocket;
            socket = new PublisherSocket();
            url = _url;
            topic = _topic;
            message = new T();

            RegisterSocket();
        }

        public void Publish(T _message)
        {
            message = _message;
            var netMQmsg = new NetMQMessage(expectedFrameCount: 1);
            netMQmsg.Append(message.Serialize());
            socket.SendMultipartMessage(netMQmsg);
        }
    }
}


