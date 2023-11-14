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

        this.laserLineRenderer = new LineRenderer();
        this.laserLineRenderer = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        GameObject laserParent = GameObject.Find("Lasers");
        this.laserLineRenderer.transform.parent = laserParent.transform;
        this.laserLineRenderer.startWidth = 2.5f;
        this.laserLineRenderer.endWidth = 5.0f;
        this.laserLineRenderer.material = material;
        this.laserLineRenderer.startColor = Color.red;
        this.laserLineRenderer.endColor = Color.red;
        this.laserLineRenderer.sortingOrder = 2;

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
        if(hitInfo.collider.gameObject.tag == "Mirror")
        {
            Vector3 newPos = hitInfo.point;
            Vector3 newDir = Vector3.Reflect(direction, hitInfo.normal);
            CastRay(newPos, newDir, laser);
        }
        else if(hitInfo.collider.gameObject.tag == "Player")
        {
            // this is bad but i'm not sure how else to call a function on the current running microgame
            // i tried events but it didn't work out
            if (hitInfo.collider.gameObject.scene.buildIndex == (int)DataManager.Scenes.LaserLineOfSightVertical)
            {
                LaserLineOfSightVertical.LaserHit();
                laserIndices.Add(hitInfo.point);
                UpdateLaser();
            }
            else if (hitInfo.collider.gameObject.scene.buildIndex == (int)DataManager.Scenes.LaserLineOfSightHorizonal)
            {
                LaserLineOfSightHorizontal.LaserHit();
                laserIndices.Add(hitInfo.point);
                UpdateLaser();
            }
            else if (hitInfo.collider.gameObject.scene.buildIndex == (int)DataManager.Scenes.LaserLineOfSightDiagonal)
            {
                LaserLineOfSightDiagonal.LaserHit();
                laserIndices.Add(hitInfo.point);
                UpdateLaser();
            }
        }
        else
        {
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
    }
}
