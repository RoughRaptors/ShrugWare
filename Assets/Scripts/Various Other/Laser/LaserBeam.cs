// https://www.youtube.com/watch?v=pNE3rfMGEAw&ab_channel=Doc

using ShrugWare;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam
{
    Vector3 pos;
    Vector3 dir;
    GameObject laserObj;
    LineRenderer laserLineRenderer;
    List<Vector3> laserIndices = new List<Vector3>();

    public LaserBeam(Vector3 pos, Vector3 dir, Material material)
    {
        this.pos = pos;
        this.dir = dir;

        this.laserObj = new GameObject();
        this.laserObj.name = "Laser Beam";
        this.laserObj.layer = 7; // enemy attack
        this.laserObj.tag = "Laser";

        this.laserLineRenderer = new LineRenderer();
        this.laserLineRenderer = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        GameObject laserParent = GameObject.Find("Lasers");
        if(laserParent != null)
        {
            this.laserLineRenderer.transform.parent = laserParent.transform;
        }

        this.laserLineRenderer.startWidth = 2.5f;
        this.laserLineRenderer.endWidth = 4.0f;
        this.laserLineRenderer.material = material;
        this.laserLineRenderer.startColor = Color.red;
        this.laserLineRenderer.endColor = Color.red;
        this.laserLineRenderer.sortingOrder = 2;
        this.laserLineRenderer.useWorldSpace = false;

        CastRay(pos, dir, laserLineRenderer);
    }

    void CastRay(Vector3 rayPos, Vector3 rayDir, LineRenderer laser)
    {
        laserIndices.Add(rayPos);

        Ray ray = new Ray(rayPos, rayDir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 125))
        {
            CheckHit(hit, dir, laser);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(125));
            UpdateLaser();
        }
    }

    void UpdateLaser()
    {
        int count = 0;
        laserLineRenderer.positionCount = laserIndices.Count;
        foreach(Vector3 index in laserIndices)
        {
            laserLineRenderer.SetPosition(count++, index);
        }
    }

    void CheckHit(RaycastHit hitInfo, Vector3 direction, LineRenderer laser)
    {
        if (hitInfo.collider.gameObject.tag == "Mirror")
        {
            Vector3 newPos = hitInfo.point;
            Vector3 newDir = Vector3.Reflect(direction, hitInfo.normal);
            CastRay(newPos, newDir, laser);
        }
        else if (hitInfo.collider.gameObject.tag == "Player")
        {
            // we hit the player with a laser, tell it we got hit.
            PlayerCollider.LaserHit(laserObj);
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
        else
        {
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
    }
}
