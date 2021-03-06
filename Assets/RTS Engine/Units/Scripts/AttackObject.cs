﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Attack Object script created by Oussama Bouanani, SoumiDelRio.
 * This script is part of the Unity RTS Engine */

namespace RTSEngine
{
	public class AttackObject : MonoBehaviour {
        public bool FollowTarget;
        [HideInInspector]
        public Transform Target;
		[HideInInspector]
		public GameObject Source; //From the attack object was launched.
		[HideInInspector]
		public Vector3 MvtVector; //The attack object
		[HideInInspector]
		public float Speed = 10.0f; //attack object's speed:

		//Attack damage:
		[HideInInspector]
		public Attack.DamageVars[] CustomDamage; //if the target unit/building code is in the list then it will be given the matching damage, if not then the default damage
        public Attack.Damage DefaultUnitDamage;
		[HideInInspector]
		public float DefaultBuildingDamage = 10.0f; //damage points when this unit attacks a building.

		[HideInInspector]
		public bool DamageOnce = true; //do damage once? 
		[HideInInspector]
		public bool DoDamage = true; //do damage at all? 
		[HideInInspector]
		public bool DestroyOnDamage; //destroy on first given damage?

		[HideInInspector]
		public int TargetFactionID; //target faction to attack
		[HideInInspector]
		public int SourceFactionID; //the unit's faction that this object came from:

		[HideInInspector]
		public bool DidDamage = false;
		[HideInInspector]
		public bool AreaDamage = false;

        //Area damage:
        [HideInInspector]
        public Attack.AttackRangesVars[] AttackRanges;

        public List<string> AttackCategoriesList = new List<string>();

        //DoT:
        //[HideInInspector]
        public Attack.DoTVars DoT;

        public EffectObj SpawnEffect; //the spawn effect that is instantied when this object is spawned

        //Attack object life timer:
        [HideInInspector]
		public float DestroyTime;
		[HideInInspector]
		public float DestroyTimer;

        //Attack target effect:
        [HideInInspector]
        public EffectObj AttackEffect;
        [HideInInspector]
        public float AttackEffectTime;

        //Scripts:
        EffectObjPool ObjPool;

		// Use this for initialization
		void Start () {
			DidDamage = false;

			//Settings in order to make OnCollisionEnter work:
			GetComponent<Collider> ().isTrigger = false;
			GetComponent<Rigidbody> ().isKinematic = false;
			GetComponent<Rigidbody> ().useGravity = false;

            ObjPool = EffectObjPool.Instance;
        }

		// Update is called once per frame
		void Update () 
		{
            Vector3 moveDir = MvtVector.normalized;
			//move the attack object towards its target:
            if (Target != null && FollowTarget)
			    moveDir = (Target.position - transform.position).normalized;
            else
                moveDir = MvtVector.normalized;
            transform.position += moveDir * Speed * Time.deltaTime;

			//if we already done damage and this object gets destroyed after damage:
			if (DestroyOnDamage == true && DidDamage == true) {
				//actually hide the object and don't destroy it, hiding it will add it automatically to the pool allowing us to re-use it.
				gameObject.SetActive (false);
			}

			//destroy timer:
			if (DestroyTimer > 0) {
				DestroyTimer -= Time.deltaTime;
			}
			if (DestroyTimer < 0) {
				gameObject.SetActive (false);
			}
		}

        //Show the attack object's spawn effect:
        public void ShowAttackObjEffect()
        {
            if (SpawnEffect != null && ObjPool != null)
            { //if we have a spawn effect object and there's an effect object pooling manager:
              //Get the spawn effect obj
                GameObject SpawnEffectObj = ObjPool.GetEffectObj(EffectObjPool.EffectObjTypes.AttackObjEffect, SpawnEffect);

                //settings for the spawn effect object:
                SpawnEffectObj.SetActive(true); //activate it
                SpawnEffectObj.transform.position = transform.position; //set its position
                SpawnEffectObj.transform.rotation = SpawnEffect.transform.rotation; //its rotation as well

                //and the life time (default):
                SpawnEffectObj.GetComponent<EffectObj>().Timer = SpawnEffectObj.GetComponent<EffectObj>().LifeTime;

            }
        }

        public void ShowAttackObjEffect(GameObject Yo)
        {

        }

        //Attack object collision effect:
        void OnTriggerEnter (Collider other)
		{
			if ((DidDamage == false || DamageOnce == false) && DoDamage == true) { //Make sure that the attack obj either didn't do damage when the attack object is allowed to do damage once or if it can do damage multiple times.
				SelectionObj HitObj = other.gameObject.GetComponent<SelectionObj> ();
				if (HitObj != null) {
					Unit HitUnit = HitObj.MainObj.GetComponent<Unit> ();
					Building HitBuilding = HitObj.MainObj.GetComponent<Building> ();

					//If the damaged object is a unit:
					if (HitUnit) {
						//Check if the unit belongs to the faction that this attack obj is targeted to and if the unit is actually not dead yet:
						if (HitUnit.FactionID == TargetFactionID && HitUnit.Dead == false) {
							if (other != null) {
                                DidDamage = true; //Inform the script that the damage has been done
                                if (AreaDamage)
                                {
                                    AttackManager.Instance.LaunchAreaDamage(transform.position, AttackRanges, SourceFactionID, AttackEffect, AttackEffectTime, AttackCategoriesList, DoT);
                                }
                                else if (DoT.Enabled) //if it's DoT
                                {
                                    //DoT settings:
                                    HitUnit.DoT = DoT;
                                    HitUnit.DoT.Damage = AttackManager.GetDamage(HitUnit.gameObject, DefaultUnitDamage);

                                }
                                else
                                {
                                    //Remove health points from the unit:
                                    HitUnit.AddHealth(-AttackManager.GetDamage(HitUnit.gameObject, DefaultUnitDamage), Source);
                                    //Attack effect: units only currently
                                    AttackManager.Instance.SpawnEffectObj(AttackEffect, other.gameObject, AttackEffectTime, false);
                                }

                                //Spawning the damage effect object:
                                AttackManager.Instance.SpawnEffectObj(HitUnit.DamageEffect, other.gameObject, 0.0f, true);
                            }
                        }
					}
					//If the attack obj hit a building:
					if (HitBuilding) {
						//Check if the building belongs to the faction that this attack obj is targeted to and if the building still has health:
						if (HitBuilding.FactionID == TargetFactionID && HitBuilding.Health >= 0) {
							if (other != null) {
                                DidDamage = true; //Inform the script that the damage has been done

                                if (AreaDamage)
                                {
                                    AttackManager.Instance.LaunchAreaDamage(transform.position, AttackRanges, SourceFactionID, AttackEffect, AttackEffectTime, AttackCategoriesList, DoT);
                                }
                                else
                                {
                                    //Remove health points from the unit:
                                    HitBuilding.AddHealth(-AttackManager.GetDamage(HitBuilding.gameObject, DefaultUnitDamage), Source);
                                }

                                //Spawning the damage effect object:
                                AttackManager.Instance.SpawnEffectObj(HitBuilding.DamageEffect, other.gameObject, 0.0f, true);
                            }
                        }
					}
				}
			}
		}
	}
}