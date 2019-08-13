using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class MapProcessor : MonoBehaviour
    {
        public OccupancyGrid occupancyGrid;

        private MessageTypes.Nav.MapMetaData mapMetaData;
        private sbyte[] data;
        private bool isNewMsgReceived = false;

        public void Update()
        {
            if(isNewMsgReceived)
            {
                UpdateOccupancyGrid();
                isNewMsgReceived = false;
            }
        }

        public void SetData(MessageTypes.Nav.MapMetaData _mapMetaData, sbyte[] _data)
        {
            data = _data;
            mapMetaData = _mapMetaData;
            isNewMsgReceived = true;
        }

        private void UpdateOccupancyGrid()
        {
                occupancyGrid.UpdateGrid( data, (int)mapMetaData.height, (int)mapMetaData.width, mapMetaData.resolution,
                    new Vector3((float)mapMetaData.origin.position.x, (float)mapMetaData.origin.position.y, (float)mapMetaData.origin.position.z));

            occupancyGrid.UpdateOccupancyColors();
        }
    }
}