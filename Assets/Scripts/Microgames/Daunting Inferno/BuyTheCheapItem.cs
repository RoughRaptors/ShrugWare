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

        [SerializeField]
        GameObject merchantsAndItemsObj;

        [SerializeField]
        GameObject signsObj;

        [SerializeField]
        AudioClipData buySound;

        private bool merchant1Correct = false;
        private bool merchant2Correct = false;
        private bool merchant3Correct = false;
        private bool wonGame = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerCollider.OnGoodCollision += PurchasedItem;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerCollider.OnGoodCollision -= PurchasedItem;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            AssignItemSprites();
            GeneratePrices();

            merchantsAndItemsObj.SetActive(true);
            merchant1Text.gameObject.SetActive(true);
            merchant2Text.gameObject.SetActive(true);
            merchant3Text.gameObject.SetActive(true);
            signsObj.SetActive(true);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
        }

        protected override bool VictoryCheck()
        {
            return wonGame;
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
            // weaponObjs.Remove(weaponObjs[0]); this generates an error, the line isn't needed
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
                wonGame = true;
                SetMicrogameEndText(true);
            }
            else
            {
                SetMicrogameEndText(false);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(buySound);
            }
        }
    }
}