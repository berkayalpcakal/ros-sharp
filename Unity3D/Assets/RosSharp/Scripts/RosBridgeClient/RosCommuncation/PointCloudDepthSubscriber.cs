/*
© Siemens AG, 2019
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

using System.Threading;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(PointCloudDepthProcessor))]
    public class PointCloudDepthSubscriber: MonoBehaviour
    {
        public RosConnector rosConnector;
        public string depthTopic;
        public float TimeStep;
        public byte[] depthData { get; private set; }
        public int numOfDepthReceived = 0;
        private PointCloudDepthProcessor depthProcessor;

        private void Start()
        {
            rosConnector.RosSocket.Subscribe<Messages.Sensor.CompressedImage>(depthTopic, ReceiveDepthMessage, (int)(TimeStep * 1000));
            depthProcessor = GetComponent<PointCloudDepthProcessor>();
        }

        private void ReceiveDepthMessage(Messages.Sensor.CompressedImage depthImage)
        {
            depthData = depthImage.data;
            numOfDepthReceived++;
            depthProcessor.Process(depthData);
        }

    }

}