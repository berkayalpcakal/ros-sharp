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

            occupancyArray = _data;
            map_width = _map_width;
            map_height = _map_height;
            cell_resolution = _cell_resolution;
            map_origin = _map_origin;
            numOfVerticesW = map_width + 1;
            numOfVerticesH = map_height + 1;

            cells = new GameObject[map_width * map_height];
            vertices = new Vector3[numOfVerticesW * numOfVerticesH]; 
            colors   = new Color  [numOfVerticesW * numOfVerticesH];

            Vector3 position_offset = new Vector3(cell_resolution / 2, cell_resolution / 2, 0).Ros2Unity();

            for (int i = 0; i < numOfVerticesW * numOfVerticesH; i++)
            {
                int i_w = i / numOfVerticesH;
                int i_h = i % numOfVerticesH;

                Vector3 increment = new Vector3(i_h * cell_resolution, i_w * cell_resolution, 0).Ros2Unity();
                vertices[i] = map_origin + increment;
                colors[i] = OccupancyToColor(50.0f);
            }

            grid.AddComponent<TriangularMeshGrid>().InitializeGrid(map_width, map_height, material, vertices, colors);
            grid.GetComponent<TriangularMeshGrid>().SetColor(colors);
        }

        public void UpdateOccupancyColors()
        {
            MapOccupancyToVertexColors();

            for (int i = 0; i < numOfVerticesW * numOfVerticesH; i++)
                colors[i] = OccupancyToColor(mappedOccupancy[i]);
            
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
                int row_idx     = cell_idx / map_width;

                //Map to lower-left vertex
                v_ll = cell_idx + row_idx;
                mappedOccupancy[v_ll] += occupancy / 4.0f;

                //Map to lower-right vertex
                v_lr = (cell_idx + 1) + row_idx;
                mappedOccupancy[v_lr] += occupancy / 4.0f;

                //Map to upper-left vertex
                v_ul = (cell_idx + numOfVerticesW) + row_idx;
                mappedOccupancy[v_ul] += occupancy / 4.0f;

                //Map to upper-right vertex
                v_ur = (cell_idx + numOfVerticesW + 1) + row_idx;
                mappedOccupancy[v_ur] += occupancy / 4.0f;
            }
        }

        private Color OccupancyToColor(float mappedOccupancy)
        {
            float color_val = 1 - mappedOccupancy / 100.0f;
            return new Color(color_val, color_val, color_val);
        }
    }
}
