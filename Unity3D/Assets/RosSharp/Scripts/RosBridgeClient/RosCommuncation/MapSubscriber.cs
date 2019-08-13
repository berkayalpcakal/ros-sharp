using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector), typeof(MapProcessor))]
    public class MapSubscriber : Subscriber<MessageTypes.Nav.OccupancyGrid>
    {
        private MapProcessor mapProcessor;

        protected override void Start()
        {
            base.Start();
            mapProcessor = GetComponent<MapProcessor>();
        }

        protected override void ReceiveMessage(MessageTypes.Nav.OccupancyGrid message)
        {
            mapProcessor.SetData(message.info, message.data);
        }

    }
}