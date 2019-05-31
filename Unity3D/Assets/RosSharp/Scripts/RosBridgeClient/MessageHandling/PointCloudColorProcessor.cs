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
    public class PointCloudColorProcessor : MonoBehaviour
    {
        public PointCloudVisualizer visualizer { get; set; }
        private Image<Bgr, byte> image;
        public Color[][] colors { get; set; }
        public bool[] receivedNewColors { get; set; }

        public void Process(byte[] imageData)
        {
            image = decodeCompressedColorImage(imageData);

            for (int i = 0, y_start = 0, y_end = 0; i < visualizer.numberOfGrids; i++)
            {
                y_end = y_start + visualizer.numberOfRowsPerGrid[i];
                if (y_end > visualizer.imageHeight)
                    y_end = visualizer.imageHeight;
                GetColorsFromImage(i, y_start, y_end, visualizer.numberOfRowsPerGrid[i]);
                y_start = y_end;
                receivedNewColors[i] = true;
            }
        }

        private void GetColorsFromImage(int i, int y_start, int y_end, int numberOfRowsPerGrid)
        {
            for (int y = y_start, j = 0; y <= y_end; y++)
                for (int x = 0; x < visualizer.imageWidth; x++)
                    colors[i][j++] = getRgbColor(image[y, x]);
        }

        private Image<Bgr, byte> decodeCompressedColorImage(byte[] data)
        {
            Mat mat = new Mat();
            CvInvoke.Imdecode(data, LoadImageType.Color, mat);
            return mat.ToImage<Bgr, byte>();
        }

        private Color getRgbColor(Bgr bgr)
        {
            return new Color(((float)bgr.Red) / 255, ((float)bgr.Green) / 255, ((float)bgr.Blue) / 255);
        }

    }
}
