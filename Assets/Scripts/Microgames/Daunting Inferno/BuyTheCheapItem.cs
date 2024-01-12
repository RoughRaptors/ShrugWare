using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace ShrugWare
{
    // to make this a bit more interesting, randomize the spawn of the three items, as well as their prices
    // determine the correct item to buy from these prices
    public class BuyTheCheapItem : Microgame
    {
        [SerializeField]
        TextMeshProUGUI merchant1Text;

        [SerializeField]
        TextMeshProUGUI merchant2Text;

        [SerializeField]
        TextMeshProUGUI merchant3Text;

        [SerializeField]
        Sprite swordSprite;

        [SerializeField]
        Sprite axeSprite;

        [SerializeField]
        Sprite staffSprite;

        [SerializeField]
        List<GameObject> weaponObjs;

        private bool merchant1Correct = false;
        private bool merchant2Correct = false;
        private bool merchant3Correct = false;

        new private void Start()
        {
            base.Start();

            AssignItemSprites();
            GeneratePrices();
        }

        private void OnEnable()
        {
            PlayerCollider.OnGoodCollision += PurchasedItem;
        }

        private void OnDisable()
        {
            PlayerCollider.OnGoodCollision -= PurchasedItem;
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
            return false;
        }

        private void AssignItemSprites()
        {
            // sword
            int rand = Random.Range(0, weaponObjs.Count);
            weaponObjs[rand].GetComponent<SpriteRenderer>().sprite = swordSprite;
            weaponObjs.Remove(weaponObjs[rand]);

            // axe
            rand = Random.Range(0, weaponObjs.Count);
            weaponObjs[rand].GetComponent<SpriteRenderer>().sprite = axeSprite;
            weaponObjs.Remove(weaponObjs[rand]);

            // staff. only 1 left
            weaponObjs[0].GetComponent<SpriteRenderer>().sprite = staffSprite;
            weaponObjs.Remove(weaponObjs[0]);
        }

        private void GeneratePrices()
        {
            float price1 = Random.Range(0, 1000);
            float price2 = Random.Range(0, 1000);
            float price3 = Random.Range(0, 1000);
            merchant1Text.text = price1.ToString();
            merchant2Text.text = price2.ToString();
            merchant3Text.text = price3.ToString();

            if(price1 < price2 && price1 < price3)
            {
                merchant1Correct = true;
            }
            else if(price2 < price1 && price2 < price3)
            {
                merchant2Correct = true;
            }
            else
            {
                merchant3Correct = true;
            }
        }

        private void PurchasedItem(GameObject gameObj)
        {
            if(gameObj.name == "Merchant 1 Body" && merchant1Correct
                || (gameObj.name == "Merchant 2 Body" && merchant2Correct)
                || (gameObj.name == "Merchant 3 Body" && merchant3Correct))
            {
                SetMicrogameEndText(true);
            }
            else
            {
                SetMicrogameEndText(false);
            }
        }
    }
}