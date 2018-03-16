using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.AI;
using RTSEngine;

namespace RTSArena
{
    public class Spawner : NetworkBehaviour
    {
        public string Code; // Unique string ID of the spawner

        public Unit UnitPrefab; // The Unit this spawner will spawn
        public Transform SpawnPosition; // Position the spawner will spawn the unit
        public Transform MovePosition; // Position the units will move to after being spawned
        public Transform Model;
        public LayerMask BuildZone;
        [HideInInspector]
        public bool NewPos = false;
        [HideInInspector]
        public bool Placed = false;
        [HideInInspector]
        public bool CanPlace = false;
        [HideInInspector]
        public BuildSpot CurrentBuildSpot;

        [SyncVar]
        public int FactionID = 0;

        [HideInInspector]
        public GameManager GameMgr;
        [HideInInspector]
        public MFactionManager MFactionMgr;

        private RaycastHit[] hits;
        private Ray ray;
        private void Awake()
        {
            GameMgr = GameManager.Instance;

            //SelectionMgr = GameMgr.SelectionMgr;
        }
        private void Start()
        {
            if (GameManager.MultiplayerGame == true)
            {
                MFactionMgr = GameMgr.Factions[FactionID].MFactionMgr; //Set the multiplayer faction manager
            }
            if (GameManager.PlayerFactionID == 0)
                MovePosition = GameManager.Instance.Team2Zone;
            else
                MovePosition = GameManager.Instance.Team1Zone;
        }

        private void Update()
        {
            Debug.DrawRay(this.transform.position + new Vector3(0, 1, 0), transform.TransformDirection(Vector3.down), Color.red);
            //For the player faction only, because we check if the object is in range or not for other factions inside the NPC building manager: 
            if (FactionID == GameManager.PlayerFactionID)
            {
                if (Placed == false)
                { //If the building isn't placed yet, we'll check if its inside the chosen range from the nearest building center:
                    if (NewPos == true)
                    { //If the building has been moved from its last position.

                        //Fog Of War
                        //uncomment this only if you are using the Fog Of War asset and replace MIN_FOG_STRENGTH with the value you need.
                        /*if (!FogOfWar.GetFogOfWarTeam(FactionID).IsInFog(transform.position, 0.7f))
                        {
                            BuildingPlane.GetComponent<Renderer>().material.color = Color.red; //Show the player that the building can't be placed here.
                            CanPlace = false; //The player can't place the building at this position.
                        }*/

                        //first check if the building is in range of 
                        if (IsSpawnerOverOpenBuildSpot() == false) //Checking if the building is over a built spot (or anything at all for that matter)
                        {
                            Model.GetComponent<Renderer>().material.color = Color.red; //Show the player that the building can't be placed here.
                            CanPlace = false; //The player can't place the building at this position.
                        }
                        else if(IsSpawnerUnderOwnedBase() == false)
                        {
                            Model.GetComponent<Renderer>().material.color = Color.red; //Show the player that the building can't be placed here.
                            CanPlace = false; //The player can't place the building at this position.
                        }
                        else
                        {
                            Model.GetComponent<Renderer>().material.color = Color.green; //Show the player that the building can be placed here.
                            CanPlace = true; //The player can place the building at this position.
                        }
                        NewPos = false;
                    }
                }
            }
        }
        public void StopSpawning()
        {
            CancelInvoke();
        }
        public void SpawnUnit()
        {
            if (GameManager.MultiplayerGame == false)
            {
                UnitPrefab.gameObject.GetComponent<NavMeshAgent>().enabled = false; //disable this component before spawning the unit as it might place the unit in an unwanted position when spawned
                Unit NewUnit = Instantiate(UnitPrefab.gameObject, new Vector3(SpawnPosition.position.x, 1 + UnitPrefab.UnitHeight, SpawnPosition.position.z), UnitPrefab.transform.rotation).GetComponent<Unit>();
                //set the unit faction ID.
                NewUnit.FactionID = GameManager.PlayerFactionID;
                //NewUnit.CreatedBy = this;

                NewUnit.gameObject.GetComponent<NavMeshAgent>().enabled = true; //enable the nav mesh agent component for the newly created unit
            } else
            {
                MFactionMgr.ArenaTryToSpawnUnit(UnitPrefab.Code, new Vector3(SpawnPosition.position.x, 1 + UnitPrefab.UnitHeight, SpawnPosition.position.z), netId, FactionID);
            }
        }
        private bool IsSpawnerOverOpenBuildSpot() // Check if spawner is over an open build spot
        {
            ray = new Ray(this.transform.position + new Vector3(0,1,0), transform.TransformDirection(Vector3.down));
            hits = Physics.RaycastAll(ray, 50);
            if (hits.Length > 0) // Check if there is anything under the spawner
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    int BuildSpotLayer = LayerMask.NameToLayer("BuildSpot");
                    if (hits[i].transform.gameObject.layer == BuildSpotLayer)
                    {
                        if (hits[i].transform.GetComponent<BuildSpot>().IsOpen)
                        { // If we hit a build spot and it's open we return true
                            CurrentBuildSpot = hits[i].transform.GetComponent<BuildSpot>();
                            return true;
                        }
                    }
                }
            }
            if (CurrentBuildSpot != null) CurrentBuildSpot = null;
            return false;
        }

        private bool IsSpawnerUnderOwnedBase()
        {
            if (!GameManager.MultiplayerGame)
                return true;
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position + new Vector3(0, 1, 0), transform.TransformDirection(Vector3.down), out hit,50,BuildZone))
            {
                if (hit.collider.GetComponent<BuildZone>().PlayerID == GameManager.PlayerFactionID)
                    return true;
            }
            return false;
        }

        public void Rotate(float RotY)
        {
            this.transform.Rotate(new Vector3(0, RotY, 0));
        }

        public void SendUnitToAttack(Unit Unit)
        {
            Unit.CheckUnitPath(MovePosition.position, null, GameMgr.MvtStoppingDistance, -1, true); //Move to the goto position:
        }
    }
}