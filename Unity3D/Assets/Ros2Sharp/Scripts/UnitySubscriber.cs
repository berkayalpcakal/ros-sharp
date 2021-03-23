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

using System.Collections;
using UnityEngine;

namespace Ros2SocketClient
{
    [RequireComponent(typeof(Ros2Connector))]
    public abstract class UnitySubscriber<T> : MonoBehaviour where T : Message, new()
    {
        public string Topic;
        protected Ros2Connector rosConnector;
        protected Subscriber<T> subscriber;

        protected virtual void Start()
        {
            rosConnector = GetComponent<Ros2Connector>();
            RegisterSubscriber();
        }

        //protected void OnEnable()
        //{
        //    Debug.Log("Subscriber Enabled");
        //    if (rosConnector != null)
        //    {
        //        RegisterSubscriber();
        //    }
        //}

        //protected void OnDisable()
        //{
        //    Debug.Log("Subscriber Disabled");
        //    UnregisterSubscriber();
        //}


        protected void RegisterSubscriber()
        {
            StartCoroutine("WaitForRos2Connector");
            subscriber = new Subscriber<T>(rosConnector.metaSocket, SubscriptionCallback, rosConnector.host, Topic, new Log(x => Debug.Log(x)));
        }

        //protected void UnregisterSubscriber()
        //{
        //    if (subscriber != null)
        //    {
        //        subscriber.UnregisterSocket();
        //        subscriber = null;
        //    }
        //}

        IEnumerable WaitForRos2Connector()
        {
            yield return new WaitUntil(() => { return GetComponent<Ros2Connector>() != null; });
        }

        protected abstract void SubscriptionCallback(T message);

    }

}

