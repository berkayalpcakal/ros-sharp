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
    public class PointCloudColorSubscriber : UnitySubscriber<MessageTypes.Sensor.CompressedImage>
    {
        public byte[] colorData { get; private set; }
        public int numOfColorReceived = 0;
        public PointCloudColorProcessor colorProcessor;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(MessageTypes.Sensor.CompressedImage colorImage)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            colorData = colorImage.data;
            numOfColorReceived++;
            colorProcessor.Process(colorData);

            Debug.Log("rgb subscriber elapsed time per a frame process: " + stopwatch.ElapsedMilliseconds);
        }

    }

}