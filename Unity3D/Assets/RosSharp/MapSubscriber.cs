using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector), typeof(MapProcessor))]
    public class MapSubscriber : Subscriber<Messages.Navigation.OccupancyGrid>
    {
        private RosConnector rosConnector;
        private MapProcessor mapProcessor;

        protected override void Start()
        {
            base.Start();
            rosConnector = GetComponent<RosConnector>();
            mapProcessor = GetComponent<MapProcessor>();
        }

        protected override void ReceiveMessage(Messages.Navigation.OccupancyGrid message)
        {
            mapProcessor.SetData(message.info, message.data);
        }

    }
}