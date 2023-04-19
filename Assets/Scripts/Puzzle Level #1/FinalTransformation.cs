using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTransformation : MonoBehaviour
{
    // Coroutine responsible for final level transformation
    //For Flooor
    private LaserControl laserControl;
    private Transform floor;
    private Transform tr1;
    private Transform tr2;
    private Transform tr3;

    //For Laser
    public float lineDist;

    //For Door
    public GameObject doorParent;

    private float totalTime = 4f;
    private float firstLerpDuration = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        laserControl = GetComponent<LaserControl>();
        tr1 = laserControl.tr1;
        tr2 = laserControl.tr2;
        tr3 = laserControl.tr3;
        floor = laserControl.floor;
        lineDist = laserControl.lineDist;

        StartCoroutine(LerpTransformations());
    }

    private IEnumerator LerpTransformations()
    {
        // Move from point1 to point2
        float startTime = Time.time;
        float elapsedTime = 0;
        while (elapsedTime < firstLerpDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = elapsedTime / firstLerpDuration;
            laserControl.lineDist = Mathf.Lerp(lineDist, 0, t);
            floor.rotation = Quaternion.Slerp(tr1.rotation, tr2.rotation, t);
            floor.localScale = Vector3.Lerp(tr1.localScale, tr2.localScale, t);
            yield return null;
        }

        // Ensure the object is exactly at point2
        floor.rotation = tr2.rotation;
        floor.localScale = tr2.localScale;

        //Change the laser
        laserControl.lineDist = 0;
        laserControl.line.gameObject.SetActive(false);

        // Move from point2 to point3
        startTime = Time.time;
        elapsedTime = 0;
        float secondLerpDuration = totalTime - firstLerpDuration;
        while (elapsedTime < secondLerpDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = elapsedTime / secondLerpDuration;
            floor.rotation = Quaternion.Slerp(tr2.rotation, tr3.rotation, t);
            floor.localScale = Vector3.Lerp(tr2.localScale, tr3.localScale, t);
            yield return null;
        }

        // Ensure the object is exactly at point3 and laser distance is at 0
        floor.rotation = tr3.rotation;
        floor.localScale = tr3.localScale;

        //Make the door appear
        doorParent.SetActive(true);
    }
}
