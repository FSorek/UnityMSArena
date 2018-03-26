using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSEngine;

namespace RTSArena
{
    public class SpawnerPlacement : MonoBehaviour
    {
        public static bool IsBuilding = false;

        [SerializeField]
        public Spawner[] AllSpawners;
        public float MinHeight = 0.01f;
        public float MaxHeight = 6.0f;
        public float BuildingYOffset = 0.01f;
        public LayerMask BuildZoneMask; //Terrain layer mask.
        public LayerMask BuildSpotMask; //Build spot layer mask.

        [HideInInspector]
        public Spawner CurrentSpawner;
        private GameManager GameMgr;

        void Awake()
        {
            //find the scripts that we will need:
            GameMgr = GameManager.Instance;
        }
        // Use this for initialization
        void Start()
        {
            GameMgr = GameManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (CurrentSpawner != null) //If we are currently attempting to place a building on the map
            {
                IsBuilding = true; //it means we are informing other scripts that we are placing a building.

                //using a raycheck, we will make the building to place, follow the mouse position and stay on top of the terrain.
                Ray RayCheck = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit[] Hits;
                Hits = Physics.RaycastAll(RayCheck, 1000.0f);

                if (Hits.Length > 0)
                {
                    for (int i = 0; i < Hits.Length; i++)
                    {
                        int TerrainLayer = LayerMask.NameToLayer("BuildZone");
                        if (Hits[i].transform.gameObject.layer == TerrainLayer)
                        {
                            //depending on the height of the terrain, we will place the building on it.
                            Vector3 SpawnerPos = Hits[i].point;
                            //make sure that the building position on the y axis stays inside the min and max height interval:
                            if (SpawnerPos.y < MinHeight)
                            {
                                SpawnerPos.y = MinHeight + BuildingYOffset;
                            }
                            else if (SpawnerPos.y > MaxHeight)
                            {
                                SpawnerPos.y = MaxHeight + BuildingYOffset;
                            }
                            else
                            {
                                SpawnerPos.y += BuildingYOffset;
                            }
                            if (CurrentSpawner.transform.position != SpawnerPos)
                            {
                                CurrentSpawner.NewPos = true; //inform the building's comp that we have moved it so that it checks whether the new position is suitable or not.
                            }
                            CurrentSpawner.transform.position = SpawnerPos; //set the new building's pos.
                        }
                    }
                }
                if (Input.GetMouseButtonDown(1)) //If the player preses the right mouse button.
                {
                    //Abort the building process
                    Destroy(CurrentSpawner.gameObject);
                    CurrentSpawner = null;

                    IsBuilding = false;
                }
                else if (CurrentSpawner.CanPlace == true) //If the player can place the building at its current position:
                {
                    if (Input.GetMouseButton(0)) //If the player preses the left mouse button
                    {
                        //CHECK IF PLAYER HAS ENOUGH GOLD {
                        PlaceSpawner(); //place the building.

                        //if holding and spawning is enabled and the player is holding the right key to do that:
                        /*if (HoldAndSpawn == true && Input.GetKey(HoldAndSpawnKey))
                        {
                            //start placing the same building again
                            StartPlacingBuilding(LastBuildingID);
                        }
                        //Show the tasks again for the builders again:

                        if (SelectionMgr.SelectedUnits.Count > 0 && IsBuilding == false)
                        {
                            SelectionMgr.UIMgr.UpdateUnitTasks();
                        } //}*/
                    }
                }
            }
            else
            {
                if (IsBuilding == true) IsBuilding = false;
            }
        }
        void PlaceSpawner()
        {
            if (GameManager.MultiplayerGame == false)
            { //if it's a single player game.

                //Remove the resources needed to create the building.

                //GameManager.PlayerFactionMgr.AddBuildingToList(CurrentBuilding); //add the building to the faction manager list

                //Activate the player selection collider:
                //CurrentBuilding.PlayerSelection.gameObject.SetActive(true);

                //Set the building's health to 0 so that builders can start adding health to it:
                //CurrentBuilding.Health = 0.0f;

                //CurrentBuilding.BuildingPlane.SetActive(false); //hide the building's plane
                CurrentSpawner.Model.GetComponent<Renderer>().material.color = Color.white;


                //Building is now placed:
                CurrentSpawner.Placed = true;
                /*//custom event:
                if (GameMgr.Events) GameMgr.Events.OnBuildingPlaced(CurrentBuilding);
                CurrentBuilding.ToggleConstructionObj(true); //Show the construction object when the building is placed.
                */
                if (CurrentSpawner.CurrentBuildSpot != null)
                {
                    CurrentSpawner.transform.position = CurrentSpawner.CurrentBuildSpot.transform.position;
                    CurrentSpawner.transform.rotation = CurrentSpawner.CurrentBuildSpot.transform.rotation;
                }
                CurrentSpawner = null;
            }
            else
            { //in case it's a multiplayer game:
              //Debug.Log("Spawn");
              //TakeBuildingResources(CurrentBuilding); //Remove the resources needed to create the building.
              //ask the server to spawn the building for all clients:
                if (CurrentSpawner.CurrentBuildSpot != null)
                    CurrentSpawner.transform.position = CurrentSpawner.CurrentBuildSpot.transform.position;
                float RotY = CurrentSpawner.CurrentBuildSpot.RotY;

                GameMgr.
                    Factions[GameManager.PlayerFactionID].
                    MFactionMgr.
                    TryToSpawnSpawner
                    (CurrentSpawner.Code, CurrentSpawner.transform.position, RotY);
                Destroy(CurrentSpawner.gameObject);

                CurrentSpawner = null;
            }

            IsBuilding = false;
            //Show the tasks panel after placing the building:
            //AudioManager.PlayAudio(GameMgr.GeneralAudioSource.gameObject, PlaceBuildingAudio, false);
        }
        public void StartPlacingBuilding(int SpawnerID)
        {
            //make sure we have enough resources
            if (true) // after implemeting economy/gold
            {
                //Spawn the building for the player to place on the map:
                GameObject SpawnerClone = (GameObject)Instantiate(AllSpawners[SpawnerID].gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                //LastSpawnerID = SpawnerID;
                SpawnerClone.gameObject.GetComponent<Spawner>().FactionID = GameManager.PlayerFactionID;

                //Set the position of the new building:
                RaycastHit Hit;
                Ray RayCheck = Camera.main.ScreenPointToRay(Input.mousePosition);

                //Disable the building's collider when placing it:
                //if(BuildingClone.GetComponent<Collider>()) 

                if (Physics.Raycast(RayCheck, out Hit))
                {
                    Vector3 SpawnerPos = Hit.point;
                    SpawnerPos.y = MinHeight + BuildingYOffset;
                    SpawnerClone.transform.position = SpawnerPos;
                }
                IsBuilding = true;
                CurrentSpawner = SpawnerClone.GetComponent<Spawner>();
            }
            else
            {
                //Inform the player that he can't place this building because he's lacking resources.
                //GameMgr.UIMgr.ShowPlayerMessage ("Not enough resources to launch task!", UIManager.MessageTypes.Error);
            }

        }
    }
}
