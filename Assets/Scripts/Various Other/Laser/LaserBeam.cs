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
        if (Physics.Raycast(ray, out hit, 100))
        {
            CheckHit(hit, dir, laser);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(100));
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
            Vector3 pos = hitInfo.point;
            Vector3 dir = Vector3.Reflect(direction, hitInfo.normal);
            CastRay(pos, direction, laser);
        }
        else if(hitInfo.collider.gameObject.tag == "Player")
        {
            // this is bad but i'm not sure how else to get a reference to the current microgame that's running
            // i tried events but it didn't work out
            if(BossGameManager.Instance != null && BossGameManager.Instance.GetCurSceneIndex() == (int)DataManager.Scenes.LaserLineOfSight)
            {
                LaserLineOfSight.LaserHit();
                laserIndices.Add(hitInfo.point);
                UpdateLaser();
            }
            else if(BossGameManager.Instance == null)
            {
                // make this still work in the case where we're running just this scene
                LaserLineOfSight.LaserHit();
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
