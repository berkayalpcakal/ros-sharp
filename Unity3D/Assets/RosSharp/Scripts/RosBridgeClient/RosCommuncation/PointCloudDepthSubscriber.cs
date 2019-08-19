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
    public class PointCloudDepthSubscriber: Subscriber<MessageTypes.Sensor.CompressedImage>
    {
        public byte[] depthData { get; private set; }
        public int numOfDepthReceived = 0;
        public PointCloudDepthProcessor depthProcessor;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(MessageTypes.Sensor.CompressedImage depthImage)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            depthData = depthImage.data;
            numOfDepthReceived++;
            depthProcessor.Process(depthData);

            Debug.Log("depth subscriber elapsed time per a frame process: " + stopwatch.ElapsedMilliseconds);

        }

    }

}