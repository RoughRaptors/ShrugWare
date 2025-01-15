using UnityEngine;
using System.Collections.Generic;
using System;

namespace ShrugWare
{
    public class ColoredPortals : Microgame
    {
        [SerializeField]
        GameObject bloodVFX;

        [SerializeField]
        GameObject redPortalInitialObj;

        [SerializeField]
        GameObject greenPortalInitialObj;

        [SerializeField]
        GameObject bluePortalInitialObj;

        [SerializeField]
        GameObject yellowPortalInitialObj;

        [SerializeField]
        List<Vector2> portalEntranceLocations = new List<Vector2>();

        [SerializeField]
        List<Vector2> portalExitLocations = new List<Vector2>();

        [SerializeField]
        GameObject iceExplosionInitialObj;

        [SerializeField]
        GameObject explosionParent; // just for organization

        [SerializeField]
        GameObject portalParent; // just for organization

        [SerializeField]
        GameObject fireExplosionInitialObj;

        [SerializeField]
        GameObject firewallInitialObject;

        [SerializeField]
        AudioClipData portalSound;

        List<GameObject> explosions = new List<GameObject>();
        private const int MAX_EXPLOSIONS = 75;
        private const float EXPLOSION_X_MIN = -100.0f;
        private const float EXPLOSION_X_MAX = 100.0f;
        private const float MIN_EXPLOSION_DURATION = 0.25f;
        private const float MAX_EXPLOSION_DURATION = 0.75f;
        private const float MIN_EXPLOSION_DELAY = 0.0f;
        private const float MAX_EXPLOSION_DELAY = 1.0f;

        private const float EXPLOSION_Y_MIN = 1.0f;
        private const float EXPLOSION_Y_MAX = 20.0f;

        Dictionary<PortalColors, GameObject> portalEntrances = new Dictionary<PortalColors, GameObject>();
        Dictionary<PortalColors, GameObject> portalExits = new Dictionary<PortalColors, GameObject>();

        private bool correctPortalChosen = false;
        private PortalColors correctPortalColor;

        public enum PortalColors
        {
            Red = 0,
            Green,
            Blue,
            Yellow
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerCollider.OnBadCollision += HitAOE;
            PlayerCollider.OnGoodCollision += HitPortal;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerCollider.OnBadCollision -= HitAOE;
            PlayerCollider.OnGoodCollision -= HitPortal;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            microGameTime *= 1.33f;

            int enumLength = Enum.GetNames(typeof(PortalColors)).Length;
            correctPortalColor = (PortalColors)UnityEngine.Random.Range(0, enumLength);

            SpawnEntrancePortals();
            SpawnExitPortals();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            SpawnExplosions();
        }

        protected override bool VictoryCheck()
        {
            return correctPortalChosen;
        }

        private void HitAOE(GameObject aoeObject)
        {
            SetMicrogameEndText(false);
        }

        private void HitPortal(GameObject portalObject)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(portalSound);
            }

            if (portalObject.tag == "Red")
            {
                transform.position = new Vector2(portalExits[PortalColors.Red].transform.position.x, portalExits[PortalColors.Red].transform.position.y - 6.0f);

                if (correctPortalColor == PortalColors.Red)
                {
                    correctPortalChosen = true;
                }
            }
            else if (portalObject.tag == "Green")
            {
                transform.position = new Vector2(portalExits[PortalColors.Green].transform.position.x, portalExits[PortalColors.Green].transform.position.y - 6.0f);

                if (correctPortalColor == PortalColors.Green)
                {
                    correctPortalChosen = true;
                }
            }
            else if (portalObject.tag == "Blue")
            {
                transform.position = new Vector2(portalExits[PortalColors.Blue].transform.position.x, portalExits[PortalColors.Blue].transform.position.y - 6.0f);

                if (correctPortalColor == PortalColors.Blue)
                {
                    correctPortalChosen = true;
                }
            }
            else
            {
                transform.position = new Vector2(portalExits[PortalColors.Yellow].transform.position.x, portalExits[PortalColors.Yellow].transform.position.y - 6.0f);

                if (correctPortalColor == PortalColors.Yellow)
                {
                    correctPortalChosen = true;
                }
            }

            SetMicrogameEndText(correctPortalChosen);
        }

        private void SpawnEntrancePortals()
        {
            SpawnEntrancePortal(PortalColors.Red);
            SpawnEntrancePortal(PortalColors.Green);
            SpawnEntrancePortal(PortalColors.Blue);
            SpawnEntrancePortal(PortalColors.Yellow);
        }

        private void SpawnExitPortals()
        {
            SpawnExitPortal(PortalColors.Red);
            SpawnExitPortal(PortalColors.Green);
            SpawnExitPortal(PortalColors.Blue);
            SpawnExitPortal(PortalColors.Yellow);
        }

        public void SpawnEntrancePortal(PortalColors portalColor)
        {
            GameObject portalToUse;
            if(portalColor == PortalColors.Red)
            {
                portalToUse = redPortalInitialObj;
            }
            else if (portalColor == PortalColors.Green)
            {
                portalToUse = greenPortalInitialObj;
            }
            else if(portalColor == PortalColors.Blue)
            {
                portalToUse = bluePortalInitialObj;
            }
            else
            {
                portalToUse = yellowPortalInitialObj;
            }
            
            int entrancePosIndex = UnityEngine.Random.Range(0, portalEntranceLocations.Count);
            Vector2 entrancePos = portalEntranceLocations[entrancePosIndex];
            portalEntranceLocations.RemoveAt(entrancePosIndex);

            GameObject entracePortal = Instantiate(portalToUse, entrancePos, Quaternion.identity);
            entracePortal.SetActive(true);
            entracePortal.name = " " + portalColor.ToString() + "Entrance";
            entracePortal.transform.parent = portalParent.transform;

            portalEntrances.Add(portalColor, entracePortal);
        }

        public void SpawnExitPortal(PortalColors portalColor)
        {
            GameObject portalToUse;
            if (portalColor == PortalColors.Red)
            {
                portalToUse = redPortalInitialObj;
            }
            else if (portalColor == PortalColors.Green)
            {
                portalToUse = greenPortalInitialObj;
            }
            else if( portalColor == PortalColors.Blue) 
            {
                portalToUse = bluePortalInitialObj;
            }
            else
            {
                portalToUse = yellowPortalInitialObj;
            }

            int exitPosIndex = UnityEngine.Random.Range(0, portalExitLocations.Count);
            Vector2 exitPos = portalExitLocations[exitPosIndex];
            portalExitLocations.RemoveAt(exitPosIndex);

            GameObject exitPortal = Instantiate(portalToUse, exitPos, Quaternion.identity);
            exitPortal.SetActive(true);
            exitPortal.name = " " + portalColor.ToString() + "Exit";
            exitPortal.GetComponent<BoxCollider2D>().enabled = false;
            exitPortal.layer = 0;
            exitPortal.transform.parent = portalParent.transform;

            // spawn the firewall on all but the correct portal
            if (correctPortalColor != portalColor)
            {
                GameObject fireWall = Instantiate(firewallInitialObject, exitPortal.transform.position, Quaternion.identity);
                fireWall.SetActive(true);
                fireWall.name = portalColor.ToString() + " Firewall";
            }

            portalExits.Add(portalColor, exitPortal);
        }

        private void SpawnExplosions()
        {
            explosions.RemoveAll(x => !x);

            int numExplosionsNeeded = MAX_EXPLOSIONS - explosions.Count;
            for(int j = 0; j < numExplosionsNeeded; ++j)
            {
                GameObject initialObject;
                if(UnityEngine.Random.Range(0, 2) == 0)
                {
                    initialObject = iceExplosionInitialObj;
                }
                else
                {
                    initialObject = fireExplosionInitialObj;
                }

                float xPos = UnityEngine.Random.Range(EXPLOSION_X_MIN, EXPLOSION_X_MAX);
                float yPos = UnityEngine.Random.Range(EXPLOSION_Y_MIN, EXPLOSION_Y_MAX);
                GameObject newExplosion = Instantiate(initialObject, new Vector2(xPos, yPos), Quaternion.identity);
                newExplosion.SetActive(true);
                newExplosion.transform.parent = explosionParent.transform;
                newExplosion.name = "Explosion " + j.ToString();

                float newDuration = UnityEngine.Random.Range(MIN_EXPLOSION_DURATION, MAX_EXPLOSION_DURATION);
                ParticleSystem ps = newExplosion.GetComponent<ParticleSystem>();
                ps.Stop();
                ParticleSystem.MainModule main = ps.main;
                main.duration = newDuration;
                main.startDelay = UnityEngine.Random.Range(MIN_EXPLOSION_DELAY, MAX_EXPLOSION_DELAY);
                ps.Play();

                explosions.Add(newExplosion);
            }
        }
    }
}