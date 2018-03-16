using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSArena
{
    public class BuildZone : MonoBehaviour
    {
        public int PlayerID = 0;
        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < this.gameObject.transform.childCount; i++)
            {
                if (PlayerID == 1)
                {
                    this.transform.GetChild(i).GetComponent<BuildSpot>().RotY = 180;
                    this.transform.GetChild(i).GetComponent<BuildSpot>().Team = 1;// change to enums later
                }
                else
                {
                    this.transform.GetChild(i).GetComponent<BuildSpot>().Team = 0;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
