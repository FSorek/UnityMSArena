using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSArena
{
    public class BuildSpot : MonoBehaviour {

        //[HideInInspector]
        public bool IsOpen = true;
        public int Team = 1;
        public float RotY = 0;
        // Use this for initialization
        void Start() {

        }

        private void Awake()
        {
            IsOpen = true;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
