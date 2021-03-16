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

namespace Ros2SocketClient
{
    public enum CommunicatorType { Publisher, Subscriber };
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
                //Encoding.UTF8.GetBytes(communicator.GetType().Name), // did not work, " Publisher`1 "
                Encoding.UTF8.GetBytes(communicator.Port),
                Encoding.UTF8.GetBytes(communicator.CommunicatorType.ToString()),
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
                Encoding.UTF8.GetBytes(communicator.CommunicatorType.ToString()),
                Encoding.UTF8.GetBytes(CommunicatorOperation.Unregister.ToString())
            });

            socket.SendMultipartMessage(msg);
        }


        public void UnregisterAll()
        {
            foreach (var communicator in communicators)
                communicator.Unregister();

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

        CommunicatorType CommunicatorType { get; }

        void Register();
        void Unregister();
    }

    /*
    public class Subscriber<T> : ICommunicator where T : Message, new()
    {
        public readonly string topic;
        protected T message;
        private int port;
        public string MessageType { get { return message.Type; } }
        public string Topic { get { return topic; } }
        public string Port { get { return port.ToString(); } }
    }
    */

    public abstract class Communicator<T> : ICommunicator where T : Message, new()
    {
        protected NetMQSocket socket;
        protected MetaSocket metaSocket;
        protected string url;

        protected Log log;
        protected T message;
        protected CommunicatorType communicatorType;

        protected string topic;
        protected int port;

        public string MessageType { get { return message.Type; } }
        public string Topic { get { return topic; } }
        public string Port { get { return port.ToString(); } }
        public CommunicatorType CommunicatorType { get { return communicatorType; } }

        public void Register()
        {
            metaSocket.isConnected.WaitOne();

            port = socket.BindRandomPort($"tcp://{url}");
            metaSocket.RegisterCommunicator(this);
            log($"Publisher socket bound to port: {port}");
        }
        public void Unregister()
        {
            metaSocket.isConnected.WaitOne();

            metaSocket.UnregisterCommunicator(this);
            socket.Close();
            socket.Dispose();
            log($"Unregistered the publisher socket with port {port}");
        }

    }
    public class Publisher<T> : Communicator<T> where T : Message, new()
    {
        public Publisher(MetaSocket _metaSocket, string _url, string _topic, Log _log)
        {
            communicatorType = CommunicatorType.Publisher;
            log = _log;
            metaSocket = _metaSocket;
            socket = new PublisherSocket();
            url = _url;
            topic = _topic;
            message = new T();

            Register();
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


