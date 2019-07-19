using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class LaserScanMap : MonoBehaviour
    {
        //public float registerInterval = 5.0f;
        public bool register = false;
        private GameObject laserScanSphereMap;

        public void Start()
        {
            laserScanSphereMap = new GameObject("Laser Scan Sphere Map");
            laserScanSphereMap.transform.parent = null;

            //StartCoroutine("DoCheck");
        }

        public void Update()
        {
            if(register)
            {
                RegisterLaserScanSpheres();
                register = !register;
            }
        }

        //public IEnumerator DoCheck()
        //{
        //    for (; ; )
        //    {
        //        RegisterLaserScanSpheres();
        //        yield return new WaitForSeconds(registerInterval);
        //    }
        //}

        private void RegisterLaserScanSpheres()
        {
            if (GameObject.Find("laserScanSpheres") != null)
            {
                GameObject laserScanElement = Instantiate(GameObject.Find("laserScanSpheres"), laserScanSphereMap.transform);
            }
        }
    }
}

