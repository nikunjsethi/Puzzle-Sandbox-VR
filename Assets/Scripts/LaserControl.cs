using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserControl : MonoBehaviour
{
    public int playerAmount;
    public Transform[] spawnPoints = new Transform[10];

    //Controls
    public Transform laserBowl;
    public Transform laserStart;

    [Header("Rotation")]
    [Range(0, 1f)]
    private float T = 0;
    public float lerpSpeed = 0.3f;
    public float rotSpeed = 10f;
    private float currentRotSpeed;
    private float targetSpeed;
    private float startSpeed;
    private float rotMultiplier;
    private int direction;
    private List<int> directions = new List<int>();
    private int[] possibilities = new int[2];
    private float distanceToCover;
    private Quaternion originalRotation;

    //Line Renderer for Laser
    [Header("Line")]
    private Transform forLineCheck;
    public LineRenderer line;
    public LayerMask layer;
    public int status = 0;
    private bool hitRecorded = false;
    private float lineDist;

    //Display for hits
    private int hitAmount = 0;
    public int requiredHits;
    public int changingDisplays;
    public List<HitDisplays> displays = new List<HitDisplays>();
    private float distances;


    // Start is called before the first frame update
    void Start()
    {
        originalRotation = laserBowl.rotation;
        startSpeed = 0;
        targetSpeed = rotSpeed;

        requiredHits = playerAmount * 3;
        if (requiredHits < 10)
            requiredHits = 10;

        //Get the combination
        possibilities[0] = -1;
        possibilities[1] = 1;

        for (int i = 0; i < requiredHits; i++)
        {
            int dir = Random.Range(0, 2);
            directions.Add(possibilities[dir]);
        }
        //Set Direction
        direction = possibilities[0];

        //Set Line
        lineDist = Mathf.Abs(Vector3.Distance(laserStart.position, forLineCheck.position));
        line.positionCount = 2;
        line.SetPosition(0, laserStart.position);
        line.SetPosition(1, laserStart.position + lineDist * laserStart.right);

        changingDisplays = Mathf.FloorToInt(displays.Count / requiredHits);
    }

    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case 0:
                line.material.color = Color.magenta;

                //Speed Lerp
                T += lerpSpeed * Time.deltaTime;
                T = Mathf.Clamp01(T);

                //Rotation for the laser
                laserBowl.Rotate(0f, 0f, currentRotSpeed * direction * Time.deltaTime);

                break;
            case 1:

                line.material.color = Color.green;

                //Speed Lerp
                T += lerpSpeed * Time.deltaTime;
                T = Mathf.Clamp01(T);

                //Rotation for the laser
                laserBowl.Rotate(0f, 0f, currentRotSpeed * direction * Time.deltaTime);

                break;

            case 2:
                line.material.color = Color.red;

                //Rotation for the laser
                laserBowl.Rotate(0f, 0f, currentRotSpeed * direction * Time.deltaTime);

                //Speed Lerp
                T = Mathf.InverseLerp(0, distanceToCover, laserBowl.rotation.z);
                T = Mathf.Clamp01(T);

                //Rotation for the laser
                laserBowl.Rotate(0f, 0f, currentRotSpeed * direction * Time.deltaTime);


                if (laserBowl.rotation.z <= 1f || laserBowl.rotation.z >= 359f)
                {
                    laserBowl.rotation = originalRotation;
                    T = 0;
                    direction = directions[0];
                    targetSpeed = rotSpeed;
                    hitAmount = 0;
                    status = 0;
                }
                break;

            case 3:

                break;

        }

        //Set direction
        direction = directions[hitAmount];

        //Set Speed
        currentRotSpeed = Mathf.Lerp(startSpeed, targetSpeed, T);

        //Raycast
        Ray ray = new Ray(laserStart.position, laserBowl.forward);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, 100f, layer);

        if (hit)
        {
            Vector3 hitPos = hitInfo.point;
            line.SetPosition(1, hitPos);

            if (status < 2 && !hitRecorded && (hitInfo.collider.CompareTag("Symbol0") || hitInfo.collider.CompareTag("Symbol1")))
            {
                //If we hit the correct side
                if (hitInfo.collider.CompareTag("Symbol" + status))
                {

                    hitRecorded = true;
                    //Add the hit
                    hitAmount++;

                    rotMultiplier = Random.Range(2, 9) * 0.25f;

                    //Speed up the laser
                    T *= 1 / rotMultiplier;
                    targetSpeed *= rotMultiplier;



                    //Change color of display
                    for (int i = (hitAmount - 1) * changingDisplays; i < hitAmount * changingDisplays; i++)
                    {
                        displays[i].changing = true;
                    }


                }
                //If we didnt hit the correct side
                else
                {

                    hitRecorded = true;
                    distances = (laserBowl.rotation.z - originalRotation.z) / hitAmount;
                    direction = -1;
                    distanceToCover = laserBowl.rotation.z;
                    status = 2;

                }
            }
        }
        else
        {
            line.SetPosition(1, laserStart.position + lineDist * laserStart.right);

            if (status < 2 && hitRecorded)
                hitRecorded = false;
        }
    }
}
