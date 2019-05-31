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

namespace RosSharp.RosBridgeClient
{

    public class TriangularMeshGrid : MonoBehaviour
    {
        private int width;
        private int height;
        private Material material;

        private int numberOfRows;
        private int numberOfColumns;

        private PointCloudVisualizer visualizer;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private Vector3[] vertices;
        private int[] triangles;
        private float maxEdgeLength;

        private void Awake()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshFilter.mesh = new Mesh { name = "TriangularMeshGrid" };
            visualizer = gameObject.GetComponentInParent<PointCloudVisualizer>();
            maxEdgeLength = visualizer.maxEdgeLength;
        }

        public void InitializeGrid(int _numberOfRows, int _numberOfColumns, Material _material, Vector3[] _vertices, Color[] _colors)
        {
            numberOfRows = _numberOfRows;
            numberOfColumns = _numberOfColumns;
            width = numberOfColumns + 1;
            height = numberOfRows + 1;
            material = _material;

            vertices = new Vector3[width * height];
            triangles = new int[numberOfRows * numberOfColumns * 6];

            for (int ti = 0, vi = 0, y = 0; y < numberOfRows; y++, vi++)
                for (int x = 0; x < numberOfColumns; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + numberOfColumns + 1;
                    triangles[ti + 2] = vi + 1;

                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + numberOfColumns + 1;
                    triangles[ti + 5] = vi + numberOfColumns + 2;
                }

            meshFilter.mesh.vertices = _vertices;
            meshFilter.mesh.colors = _colors;
            meshFilter.mesh.triangles = triangles;
            meshRenderer.material = material;
        }

        public void SetColor(Color[] _colors)
        {
            meshFilter.mesh.colors = _colors;
        }

        public void SetVertices(Vector3[] _vertices)
        {

            for (int ti = 0, vi = 0, y = 0; y < numberOfRows; y++, vi++)
                for (int x = 0; x < numberOfColumns; x++, ti += 6, vi++)
                {
                    Vector3 p1 = _vertices[vi];
                    Vector3 p2 = _vertices[vi + numberOfColumns + 1];
                    Vector3 p3 = _vertices[vi + 1];
                    Vector3 p4 = _vertices[vi + numberOfColumns + 2];

                    if (Vector3.Distance(p1, p2) > maxEdgeLength || Vector3.Distance(p2, p3) > maxEdgeLength || Vector3.Distance(p2, p3) > maxEdgeLength)
                    {
                        triangles[ti] = 0;
                        triangles[ti + 1] = 0;
                        triangles[ti + 2] = 0;
                    }
                    else
                    {
                        triangles[ti] = vi;
                        triangles[ti + 1] = vi + numberOfColumns + 1;
                        triangles[ti + 2] = vi + 1;
                    }

                    if (Vector3.Distance(p2, p3) > maxEdgeLength || Vector3.Distance(p2, p4) > maxEdgeLength || Vector3.Distance(p3, p4) > maxEdgeLength)
                    {
                        triangles[ti + 3] = 0;
                        triangles[ti + 4] = 0;
                        triangles[ti + 5] = 0;
                    }
                    else
                    {
                        triangles[ti + 3] = vi + 1;
                        triangles[ti + 4] = vi + numberOfColumns + 1;
                        triangles[ti + 5] = vi + numberOfColumns + 2;
                    }
                }
            meshFilter.mesh.triangles = triangles;
            meshFilter.mesh.vertices = _vertices;

        }

    }
}