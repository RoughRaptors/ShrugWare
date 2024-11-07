using ShrugWare;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHelper : MonoBehaviour
{
    [SerializeField]
    Microgame game;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.tag == "Collectible" && collision.tag == "Collectible")
        {
            game.PrematureEndGame(false);
        }
    }
}
