using ShrugWare;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHelper : MonoBehaviour
{
    [SerializeField]
    Microgame microgame;

    [SerializeField]
    Minigame minigame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(microgame != null)
        {
            if (this.tag == "Collectible" && collision.tag == "Collectible" && !microgame.gameOver)
            {
                microgame.PrematureEndGame(false);
            }
        }
        else if(minigame != null)
        {
            if (this.tag == "Collectible" && collision.tag == "Collectible" && !minigame.gameOver)
            {
                minigame.PrematureEndGame(false);
            }
        }
    }
}
