using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSArena
{
    public class Button_BuildSpawner : MonoBehaviour
    {

        public int spawnerID;

        public void StartBuildingSpawner()
        {

            RTSEngine.GameManager.Instance.spawnerPlacement.StartPlacingBuilding(spawnerID);
        }
    }
}
