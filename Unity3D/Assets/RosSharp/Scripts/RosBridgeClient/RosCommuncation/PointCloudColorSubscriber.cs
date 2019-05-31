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
using System.Collections;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(PointCloudColorProcessor))]
    public class PointCloudColorSubscriber : MonoBehaviour
    {
        public RosConnector rosConnector;
        public string colorTopic;
        public float TimeStep;
        public byte[] colorData { get; private set; }
        public int numOfColorReceived = 0;
        private PointCloudColorProcessor colorProcessor;

        private void Start()
        {
            rosConnector.RosSocket.Subscribe<Messages.Sensor.CompressedImage>(colorTopic, ReceiveColorMessage, (int)(TimeStep * 1000));
            colorProcessor = GetComponent<PointCloudColorProcessor>();
        }

        private void ReceiveColorMessage(Messages.Sensor.CompressedImage colorImage)
        {
            colorData = colorImage.data;
            numOfColorReceived++;
            colorProcessor.Process(colorData);
        }

    }

}