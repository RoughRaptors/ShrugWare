using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    public Material material;
    LaserBeam laserBeam;

    // Update is called once per frame
    void Update()
    {
        Destroy(GameObject.Find("Laser Beam"));
        laserBeam = new LaserBeam(gameObject.transform.position, gameObject.transform.right, material);
    }
}
