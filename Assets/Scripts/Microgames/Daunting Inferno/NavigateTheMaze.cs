using UnityEngine;

namespace ShrugWare
{
    public class NavigateTheMaze : Microgame
    {
        [SerializeField]
        GameObject mazeObj;

        [SerializeField]
        GameObject allyObj;

        [SerializeField]
        GameObject playerRendererObj;

        bool reachedTank = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            playerRendererObj.SetActive(true);
            mazeObj.SetActive(true);
            allyObj.SetActive(true);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
        }

        protected override bool VictoryCheck()
        {
            return reachedTank;
        }

        private void OnTriggerEnter(Collider other)
        {
            reachedTank = true;
            SetMicrogameEndText(true);
        }
    }
}