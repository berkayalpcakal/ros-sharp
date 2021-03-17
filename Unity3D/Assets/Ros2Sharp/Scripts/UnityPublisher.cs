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

using UnityEngine;

namespace Ros2SocketClient
{
    [RequireComponent(typeof(Ros2Connector))]
    public abstract class UnityPublisher<T> : MonoBehaviour where T : Message, new()
    {
        public string Topic;
        protected Ros2Connector rosConnector;
        protected Publisher<T> publisher;

        protected virtual void Start()
        {
            rosConnector = GetComponent<Ros2Connector>();
            RegisterPublisher();
        }

        protected void OnEnable()
        {
            if (rosConnector != null)
            {
                RegisterPublisher();
                Debug.Log("Publisher Enabled");
            }
        }

        protected void OnDisable()
        {
            if(!rosConnector.isUnregisteredAll)
            {
                Debug.Log("Publisher Disabled");
                UnregisterPublisher();
            }
        }

        protected void RegisterPublisher()
        {
            publisher = new Publisher<T>(rosConnector.metaSocket, rosConnector.host, Topic, new Log(x => Debug.Log(x)));
        }

        protected void UnregisterPublisher()
        {
            publisher.UnregisterSocket();
            publisher = null;
        }

        protected void Publish(T message)
        {
            publisher.Publish(message);
        }


    }

}

