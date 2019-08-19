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
    public class PointCloudVisualizer : MonoBehaviour
    {
        public int imageWidth;
        public int imageHeight;
        public int numberOfGrids;
        public float maxEdgeLength;
        public Material material;
        private PointCloudColorProcessor colorProcessor;
        private PointCloudDepthProcessor depthProcessor;

        private int numberOfRows;
        private int numberOfColumns;
        private GameObject[] grids;
        private Vector3[][] vertices;
        private Vector3[][] normals;
        private Color[][] colors;
        public int[] numberOfRowsPerGrid{ get; private set; }
        public bool isGenerated { get; private set; }
        public int meshIdx { get; private set; }

        private void Start()
        {
            meshIdx = 0;
            colorProcessor  = GetComponent<PointCloudColorProcessor>();
            depthProcessor  = GetComponent<PointCloudDepthProcessor>();
            numberOfRows    = imageHeight - 1;
            numberOfColumns = imageWidth  - 1;

            colorProcessor.visualizer = this;
            colorProcessor.colors = new Color[numberOfGrids][];
            colorProcessor.receivedNewColors = new bool[numberOfGrids];

            depthProcessor.visualizer = this;
            depthProcessor.coordinates = new Vector3[numberOfGrids][];
            depthProcessor.receivedNewDepths = new bool[numberOfGrids];

            CreateGrids();
            GameObject.Find("PointCloudVisualizer").transform.parent = GameObject.Find("tool0").transform;
        }

        private void Update()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            if (colorProcessor.receivedNewColors[meshIdx])
                grids[meshIdx].GetComponent<TriangularMeshGrid>().SetColor(colorProcessor.colors[meshIdx]);
            colorProcessor.receivedNewColors[meshIdx] = false;

            if (depthProcessor.receivedNewDepths[meshIdx])
                grids[meshIdx].GetComponent<TriangularMeshGrid>().SetVertices(depthProcessor.coordinates[meshIdx]);
            depthProcessor.receivedNewDepths[meshIdx] = false;

            meshIdx = (meshIdx + 1) % numberOfGrids;

            Debug.Log("visualizer elapsed time per a grid update: " + stopwatch.ElapsedMilliseconds);
        }

        private void CreateGrids()
        {
            isGenerated = false;

            grids = new GameObject[numberOfGrids];
            vertices = new Vector3[numberOfGrids][];
            normals = new Vector3[numberOfGrids][];
            colors = new Color[numberOfGrids][];

            numberOfRowsPerGrid = new int[numberOfGrids];
            int chunkSize = (int)(numberOfRows / numberOfGrids) + 1;

            int rowsCovered = 0;
            for (int i = 0, y_start = 0, y_end = 0; i < numberOfGrids; i++)
            {
                numberOfRowsPerGrid[i] = chunkSize;
                rowsCovered += chunkSize;
                if (rowsCovered > numberOfRows)
                    numberOfRowsPerGrid[i] -= rowsCovered - numberOfRows;

                colorProcessor.colors[i]      = new Color  [imageWidth * (numberOfRowsPerGrid[i] + 1)];
                depthProcessor.coordinates[i] = new Vector3[imageWidth * (numberOfRowsPerGrid[i] + 1)];

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

    }
}