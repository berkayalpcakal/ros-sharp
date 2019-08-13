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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Threading;

namespace RosSharp.RosBridgeClient
{
    public class CustomPointCloudVisualizer : MonoBehaviour
    {
        public GameObject[] grids { get; private set; }
        private Vector3[][] vertices;
        private Vector3[][] normals;
        private Color[][] colors;
        public bool isGenerated { get; private set; }
        private int meshIdx = 0;

        public void CreateGrids(int numberOfGrids, int[] numberOfRowsPerGrid, int imageWidth, int imageHeight, int numberOfColumns, Material material)
        {
            isGenerated = false;

            grids = new GameObject[numberOfGrids];
            vertices = new Vector3[numberOfGrids][];
            normals = new Vector3[numberOfGrids][];
            colors = new Color[numberOfGrids][];

            for (int i = 0, y_start = 0, y_end = 0; i < numberOfGrids; i++)
            {
                vertices[i] = new Vector3[imageWidth * (numberOfRowsPerGrid[i] + 1)];
                normals[i]  = new Vector3[imageWidth * (numberOfRowsPerGrid[i] + 1)];
                colors[i]   = new Color  [imageWidth * (numberOfRowsPerGrid[i] + 1)];

                y_end = y_start + numberOfRowsPerGrid[i];
                for (int y = y_start, j = 0; y <= y_end; y++)
                    for (int x = 0; x < imageWidth; x++, j++)
                        vertices[i][j] = transform.TransformPoint(new Vector3(x - imageWidth / 2, y - imageHeight / 2) / 1000);
                y_start = y_end - 1;

                grids[i] = new GameObject("Grid " + i);
                grids[i].transform.parent = gameObject.transform;
                grids[i].AddComponent<TriangularMeshGrid>().InitializeGrid(numberOfRowsPerGrid[i], numberOfColumns, material, vertices[i], colors[i]);
            }
            isGenerated = true;
        }

        public void UpdateMesh()
        {

        }
    }
}