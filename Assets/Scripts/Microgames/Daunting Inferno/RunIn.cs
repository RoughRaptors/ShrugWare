using UnityEngine;

namespace ShrugWare
{
    public class RunIn : Microgame
    {
        bool insideRoom = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += SetInsideRoom;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= SetInsideRoom;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
        }

        protected override bool VictoryCheck()
        {
            return insideRoom;
        }
        
        private void SetInsideRoom(GameObject obj)
        {
            insideRoom = true;
        }
    }
}