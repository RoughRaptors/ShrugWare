using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ShrugWare
{
    public class PickUpAdds : Microgame
    {
        [SerializeField]
        GameObject playerObject;

        [SerializeField]
        GameObject bloodVFX;

        [SerializeField]
        List<GameObject> spawnPositions = new List<GameObject>();

        private List<GameObject> healerObjs = new List<GameObject>();
        private List<GameObject> addObjs = new List<GameObject>();
        private List<GameObject> aggroedAdds = new List<GameObject>();

        private const float SAFE_DISTANCE = 12.5f;
        private const int NUM_ADDS = 3;
        private int numAddsAggroed = 0;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            int chosenSpawnIndex = UnityEngine.Random.Range(0, spawnPositions.Count);
            GameObject spawnPosObj = spawnPositions[chosenSpawnIndex];
            spawnPosObj.SetActive(true);

            for(int i = 0; i < spawnPosObj. transform.childCount; ++i)
            {
                GameObject go = spawnPosObj.transform.GetChild(i).gameObject;
                if (go.layer == LayerMask.NameToLayer("Enemy Body"))
                {
                    addObjs.Add(go);
                }
                else if (go.layer == LayerMask.NameToLayer("Friendly Collider"))
                {
                    healerObjs.Add(go);
                }
            }
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += EnemyCollide;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= EnemyCollide;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            MoveAggroedAdds();
        }

        protected override bool VictoryCheck()
        {
            bool success = true;
            if(numAddsAggroed != NUM_ADDS)
            {
                success = false;

                // if we're x distance away from the other adds, we're safe
                foreach (GameObject healerObj in healerObjs)
                {
                    if (IsHealerNearAdd(healerObj))
                    {
                        GameObject bloodObj = Instantiate(bloodVFX, healerObj.transform.position, Quaternion.identity);
                        bloodObj.SetActive(true);
                        healerObj.SetActive(false);
                    }
                }
            }

            return success;
        }

        private void EnemyCollide(GameObject collideGO)
        {
            if(aggroedAdds.Contains(collideGO))
            {
                return;
            }

            ++numAddsAggroed;
            if (!aggroedAdds.Contains(collideGO))
            {
                aggroedAdds.Add(collideGO);
            }

            if(numAddsAggroed == NUM_ADDS)
            {
                SetMicrogameEndText(true);
            }
        }

        private void MoveAggroedAdds()
        {
            foreach(GameObject aggroedAdd in aggroedAdds)
            {
                aggroedAdd.transform.position = Vector3.MoveTowards(aggroedAdd.transform.position, playerObject.transform.position, 30.0f * Time.deltaTime);
                
                if(playerObject.transform.position.x > aggroedAdd.transform.position.x)
                {
                    aggroedAdd.transform.rotation = new Quaternion(0, 180, 0, 0);
                }
                else
                {
                    aggroedAdd.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
        }

        // on end, to determine which healers aren't safe, get their distance to each other object 
        private bool IsHealerNearAdd(GameObject healerObj)
        {
            float minDistFromAdd = float.MaxValue;
            foreach (GameObject addObj in addObjs)
            {
                float dist = Vector3.Distance(healerObj.transform.position, addObj.transform.position);
                if (dist < minDistFromAdd)
                {
                    minDistFromAdd = dist;
                }
            }

            if(minDistFromAdd < SAFE_DISTANCE)
            {
                return true;
            }

            return false;
        }
    }
}