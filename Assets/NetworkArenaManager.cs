using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSEngine;
using UnityEngine.Networking;

namespace RTSArena
{
    public class NetworkArenaManager : NetworkBehaviour
    {
        public MFactionManager[] Factions;
        bool foundNullFaction = false;
        public static float SpawnTimer = 10f;
        [SyncVar] private bool serverNAM = false;
        [SyncVar] private float spawnCountdown = SpawnTimer;


        bool CanSpawn = false;
        NetworkArenaManager serverInstance;
        void Awake()
        {
            Start();
        }   
        void Start()
        {
            CanSpawn = false;
            SearchFactions();
            if (isServer)
            {
                serverInstance = this;
                serverNAM = true;
            }
            else if (isLocalPlayer)
            {
                serverInstance = SearchServerNAM();
            }

        }

        private NetworkArenaManager SearchServerNAM()
        {
            NetworkArenaManager serverNAM = null;
            NetworkArenaManager[] NAMs = GameObject.FindObjectsOfType<NetworkArenaManager>();
            foreach(var nam in NAMs)
            {
                if (nam.serverNAM)
                    serverNAM = nam;
            }
            return serverNAM;
        }

        // Update is called once per frame
        void Update()
        {
            if (CanSpawn)
            {
                foreach (var faction in Factions)
                {
                    if (faction != null)
                        faction.SpawnUnitsOnBattlefield();
                    else foundNullFaction = true;
                }
                CanSpawn = false;
            }
            if (foundNullFaction)
            {
                SearchFactions();
                foundNullFaction = false;
            }
            if (isLocalPlayer)
            {
                if (serverInstance == null)
                    serverInstance = SearchServerNAM();
                else if (serverInstance != null)
                {
                    spawnCountdown = serverInstance.spawnCountdown;
                    if(!CanSpawn)
                        CanSpawn = serverInstance.CanSpawn;
                }
            }
            if (serverNAM)
            {
                if (spawnCountdown > 0)
                    spawnCountdown -= Time.deltaTime;
                else
                {
                    CanSpawn = true;
                    spawnCountdown = SpawnTimer;
                }
            }
        }

        void SearchFactions()
        {
            Factions = new MFactionManager[2];
            for (int i = 0; i < Factions.Length; i++)
            {
                Factions[i] = GameManager.GetTargetFactionConnection(i);
            }
        }
    }
}
