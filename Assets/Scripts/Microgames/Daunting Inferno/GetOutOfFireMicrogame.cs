using UnityEngine;

namespace ShrugWare
{
    public class GetOutOfFireMicrogame : Microgame
    {
        private bool inFire = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodExit += FireEscape;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodExit -= FireEscape;
        }

        protected override bool VictoryCheck() => !inFire;

        // once they're out, we don't care if they go back in
        private void FireEscape(GameObject fireObject)
        {
            inFire = false;
            SetMicrogameEndText(true);
        }
    }
}