using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace RosSharp.RosBridgeClient
{
    public class OccupancyGrid : MonoBehaviour
    {
        public Material material;
        private Vector3 map_origin = new Vector3(-1, 0, -1);
        private float cell_resolution = 0.5f;
        private int map_width = 4;
        private int map_height = 4;
        private int numOfVerticesW;
        private int numOfVerticesH;
        private sbyte[] occupancyArray;
        private float[] mappedOccupancy;

        private GameObject grid;
        private Vector3[] vertices;
        private Color[] colors;
        private GameObject[] cells;

        public void UpdateGrid(sbyte[] _data, int _map_width, int _map_height, float _cell_resolution, Vector3 _map_origin)
        {
            Destroy(grid);

            grid = new GameObject("GridMesh");
            grid.transform.parent = gameObject.transform;
            GameObject orijin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orijin.transform.parent = grid.transform;
            orijin.transform.position = _map_origin.Ros2Unity();

            occupancyArray = _data;
            map_width = _map_width;
            map_height = _map_height;
            cell_resolution = _cell_resolution;
            map_origin = _map_origin;
            numOfVerticesH = map_height+ 1;
            numOfVerticesW = map_width + 1;

            cells = new GameObject[map_width * map_height];
            vertices = new Vector3[numOfVerticesW * numOfVerticesH]; 
            colors   = new Color  [numOfVerticesW * numOfVerticesH];

            for (int i = 0; i < numOfVerticesW * numOfVerticesH; i++)
            {
                int i_w = i / numOfVerticesH;
                int i_h = i % numOfVerticesH;

                Vector3 increment = new Vector3(i_h * cell_resolution, i_w * cell_resolution, 0);
                vertices[i] = (map_origin + increment).Ros2Unity();
                colors[i] = OccupancyToColor(20.0f, 0.1f);
            }

            grid.AddComponent<TriangularMeshGrid>().InitializeGrid(map_width, map_height, material, vertices, colors);
            grid.GetComponent<TriangularMeshGrid>().SetColor(colors);
            //grid.GetComponent<TriangularMeshGrid>().ReverseNormals();
        }

        public void UpdateOccupancyColors()
        {
            MapOccupancyToVertexColors();

            for (int i = 0; i < numOfVerticesW * numOfVerticesH; i++)
                colors[i] = OccupancyToColor(mappedOccupancy[i], 1.0f);
            
            grid.GetComponent<TriangularMeshGrid>().SetColor(colors);
        }

        private void MapOccupancyToVertexColors()
        {
            mappedOccupancy = new float[numOfVerticesW * numOfVerticesH];
            for (int i = 0; i < numOfVerticesW * numOfVerticesH; i++)
                mappedOccupancy[i] = 0.0f;

            int v_ll, v_lr, v_ul, v_ur;
            for (int cell_idx = 0; cell_idx < map_width * map_height; cell_idx++)
            {
                float occupancy = occupancyArray[cell_idx];
                int row_idx     = cell_idx / map_height;

                //Map to lower-left vertex
                v_ll = cell_idx + row_idx;
                mappedOccupancy[v_ll] += occupancy / 4.0f;

                //Map to lower-right vertex
                v_lr = (cell_idx + 1) + row_idx;
                mappedOccupancy[v_lr] += occupancy / 4.0f;

                //Map to upper-left vertex
                v_ul = (cell_idx + numOfVerticesH) + row_idx;
                mappedOccupancy[v_ul] += occupancy / 4.0f;

                //Map to upper-right vertex
                v_ur = (cell_idx + numOfVerticesH + 1) + row_idx;
                mappedOccupancy[v_ur] += occupancy / 4.0f;
            }
        }

        private Color OccupancyToColor(float mappedOccupancy, float alpha = 1.0f)
        {
            float color_val = 1 - mappedOccupancy / 100.0f;
            return new Color(color_val, color_val, color_val, alpha);
        }
    }
}
