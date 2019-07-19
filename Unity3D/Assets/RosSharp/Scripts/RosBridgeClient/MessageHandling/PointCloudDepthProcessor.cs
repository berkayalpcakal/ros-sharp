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
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace RosSharp.RosBridgeClient
{
    public class PointCloudDepthProcessor : MonoBehaviour
    {
        public PointCloudVisualizer visualizer { get; set; }
        private Image<Gray, short> image;
        private float focal = 0.0017f;
        public bool[] receivedNewDepths { get; set; }
        public Vector3[][] coordinates { get; set; }

        public void Process(byte[] imageData)
        {
            image = decodeCompressedGrayImage(imageData);

            for (int i = 0, y_start = 0, y_end = 0; i < visualizer.numberOfGrids; i++)
            {
                y_end = y_start + visualizer.numberOfRowsPerGrid[i];
                if (y_end > visualizer.imageHeight)
                    y_end = visualizer.imageHeight;
                GetDepthsFromImage(i, y_start, y_end, visualizer.numberOfRowsPerGrid[i]);
                y_start = y_end;
                receivedNewDepths[i] = true;
            }
            
        }

        private void GetDepthsFromImage(int i, int y_start, int y_end, int numberOfRowsPerGrid)
        {        
            for (int y = y_start, j = 0; y <= y_end; y++)
                for (int x = 0; x < visualizer.imageWidth; x++)
                    coordinates[i][j++] = get3DPoint(x, y, image.Data[y, x, 0]).Ros2Unity();
        }

        private Image<Gray, short> decodeCompressedGrayImage(byte[] data)
        {
            Mat mat = new Mat();
            CvInvoke.Imdecode(data, LoadImageType.AnyDepth, mat);
            return mat.ToImage<Gray, short>();
        }

        private Vector3 get3DPoint(int x, int y, int z)
        {
            float depth = ((float)z) / 1000;
            return new Vector3( (x - visualizer.imageWidth / 2) * focal * depth, (y - visualizer.imageHeight / 2) * focal * depth, depth);
        }

    }

}