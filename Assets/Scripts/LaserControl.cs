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
    public LineRenderer line;
    public LayerMask layer;
    public int status = 0;
    private bool hitRecorded = false;

    //Display for hits
    private int hitAmount = 0;
    public int requiredHits;
    public int changingDisplays;
    public List<HitDisplays> displays = new List<HitDisplays>();
    private float distances;

    [Header("Spawns")]
    public Transform[] spawns = new Transform[10];
    private List<int> spawnCombo = new List<int>();


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

        for (int j = 0; j < playerAmount; j++)
        {
            if (j == 0)
            {
                int index = Random.Range(0, spawns.Length);
                spawnCombo.Add(index);
            }
            else if (j > 0 && j < 5)
            {
                List<int> placesToChoose = new List<int>();

                for (int y = 0; y < spawnCombo.Count; y++)
                {
                    int choice1 = spawnCombo[y] + 2;
                    int choice2 = spawnCombo[y] - 2;

                    if (choice1 > spawns.Length)
                        choice1 -= spawns.Length;

                    if (choice2 < 0)
                        choice2 += spawns.Length;

                    if (!spawnCombo.Contains(choice1))
                        placesToChoose.Add(choice1);
                    if (!spawnCombo.Contains(choice2))
                        placesToChoose.Add(choice2);
                }

                int index = Random.Range(0, placesToChoose.Count);
                spawnCombo.Add(placesToChoose[index]);
            }
            else
            {
                List<int> placesToFill = new List<int>();

                for (int y = 0; y < spawns.Length; y++)
                {
                    if (!spawnCombo.Contains(y))
                        placesToFill.Add(y);
                    else
                        continue;
                }
                    

                int l = playerAmount - spawnCombo.Count;

                for (int i = 0; i < l; i++)
                {
                    int index = Random.Range(0, placesToFill.Count);

                    spawnCombo.Add(placesToFill[index]);
                    placesToFill.Remove(placesToFill[index]);
                }        
            }
        }

        //Send data to spawner

        //Set Line
        float distance = 
        line.positionCount = 2;
        line.SetPosition(0, laserStart.position);
        line.SetPosition(1, laserStart.position + 100f * laserStart.right);

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

        //If hit is detected and the Laser is not returning to the original spot
        if (hit && status >= 0 && status < 2 && !hitRecorded)
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
            else if (!hitInfo.collider.CompareTag("Symbol" + status))
            {

                hitRecorded = true;
                distances = (laserBowl.rotation.z - originalRotation.z) / hitAmount;
                direction = -1;
                distanceToCover = laserBowl.rotation.z;
                status = 2;

            }
        }
        else if (!hit && status >= 0 && status < 2 && hitRecorded)
        {
            hitRecorded = false;
        }

    }

}
