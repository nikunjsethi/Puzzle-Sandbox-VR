using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json.Linq;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class LaserControl : MonoBehaviour
{
    private int playerAmount;
    public Transform[] spawnPoints = new Transform[10];
    private PuzzleMusicControl musicControl;

    //Controls
    public Transform laserBowl;
    public Transform laserStart;

    [Header("Rotation")]
    [Range(0, 1f)]
    public float rotSpeed = 30f;
    private float currentRotSpeed;
    private float rotMultiplier;
    private int direction;
    private List<int> directions = new List<int>();
    private int[] possibilities = { 1, -1 };
    private float distanceToCover;
    private Quaternion originalRotation;

    //Line Renderer for Laser
    [Header("Line")]
    public LineRenderer line;
    public Renderer torusRender;
    public int status = 4;
    private bool hitRecorded = false;
    private float lineDist;
    private Vector3 dirVec;
    private float resetTime = 2f;
    private float currentResetTime = 0;

    [Header("Line Colors")]
    //Set to private as we're grabbing the colors from HitDisplay function or setting them later in the code
    private Color color1;
    private Color color2;
    private Color mainColor;
    // Fail color - currently at black
    public Color defColor = Color.black;


    //Display for hits
    [Header("Hit Display")]
    private int hitAmount = 0;
    public int requiredHits;
    private int minHits = 10;
    public float changingDisplays;
    private float displayChangeVal;
    public List<HitDisplays> displays = new List<HitDisplays>();
    private float distances;

    [Header("Winning Transforms")]
    public Transform floor;
    public Transform tr1;
    public Transform tr2;
    public Transform tr3;
    private float fRotT = 0;
    public float rotTime = 2.5f;
    private float curRotTime = 0;
    private float finalT = 0f;
    public float totalTime = 4f; //Time to complete the transition
    private float currentTime = 0;
    public AnimationCurve curve;
    public AnimationCurve curve1;
    private float doorTotalTime = 1.5f;
    private float doorCurTime = 0f;
    public Renderer doorMiddle;
    public Renderer doorFrame;
    private Color32 doorCol;
    private Color32 midCol;

    //Checking to see if the panel belongs to a spawn point
    bool IsSpawn(GameObject toCheck)
    {
        string goName = toCheck.name;
        char lastChar = goName[goName.Length - 1];
        int digit = int.Parse(lastChar.ToString());

        return digit < playerAmount;
    }

    bool RayCheck(Collider collider)
    {
        if (!collider.CompareTag("Symbol0") && !collider.CompareTag("Symbol1"))
            return false;

        if (!IsSpawn(collider.gameObject) || status >= 2 || hitRecorded)
            return false;

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerAmount = PhotonNetwork.CountOfPlayers;
        musicControl = GetComponent<PuzzleMusicControl>();

        originalRotation = laserBowl.rotation;
        currentRotSpeed = rotSpeed;

        requiredHits = Mathf.Max(playerAmount * 3, minHits);

        //Get the combination

        for (int i = 0; i < requiredHits; i++)
        {
            int dir = Random.Range(0, 2);
            directions.Add(possibilities[dir]);
        }

        //Set Direction
        direction = directions[0];

        Transform lineTR = line.transform;
        //Set Line Distance and positions
        lineDist = 30f;
        dirVec = laserBowl.forward;
        line.positionCount = 2;
        line.SetPosition(0, line.transform.position);
        line.SetPosition(1, line.transform.position + lineDist * dirVec);

        //Check how many displays we'll be changing
        //Proportionally
        changingDisplays = displays.Count / requiredHits;
        displayChangeVal = 0;

        //Set Laser colors
        color1 = displays[0].defColor;
        color2 = displays[0].color2;

        //Set floor value
        floor.transform.localScale = tr1.localScale;
        floor.transform.rotation = tr1.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        dirVec = laserBowl.forward;

        switch (status)
        {
            case 0:
                currentResetTime = 0;
                mainColor = color1;

                curRotTime = 0;

                //Rotation for the laser
                laserBowl.Rotate(0f, currentRotSpeed * direction * Time.deltaTime, 0f);

                break;
            case 1:
                currentResetTime = 0;
                mainColor = color2;

                curRotTime = 0;

                //Rotation for the laser
                laserBowl.Rotate(0f, currentRotSpeed * direction * Time.deltaTime, 0f);

                break;

            case 2:

                mainColor = defColor;

                //Start Rotating back the laser
                curRotTime += Time.deltaTime;

                fRotT = Mathf.Clamp01(curRotTime / rotTime);

                Quaternion lasRot = laserBowl.rotation;

                Quaternion finLasRot = Quaternion.Slerp(lasRot, originalRotation, curve1.Evaluate(fRotT));

                //Set the Laser bowl rotation
                laserBowl.rotation = finLasRot;

                if (fRotT == 1)
                {
                    direction = directions[0];
                    currentRotSpeed = rotSpeed;
                    hitAmount = 0;
                    if (directions[0] == 1)
                        status = 0;
                    else
                        status = 1;
                }
                break;

            case 3:

                //Start Rotating back the laser
                curRotTime += Time.deltaTime;

                fRotT = Mathf.Clamp01(curRotTime / rotTime);

                lasRot = laserBowl.rotation;

                finLasRot = Quaternion.Slerp(lasRot, originalRotation, curve1.Evaluate(fRotT));

                //Set the Laser bowl rotation
                laserBowl.rotation = finLasRot;

                if (fRotT == 1)
                {
                    doorCurTime += Time.deltaTime;

                    float doorT = Mathf.Clamp01(doorCurTime / doorTotalTime);

                    float startAlpha1 = 0f;
                    float endAlpha1 = 255f;

                    float alpha1 = Mathf.Lerp(startAlpha1, endAlpha1, curve1.Evaluate(doorT));

                    midCol = doorMiddle.material.GetColor("_Color");
                    midCol.a = (byte)alpha1;

                    if (doorT >= 1f / 3f)
                    {
                        float doorT2 = Mathf.Clamp01((doorT - 1f / 3f) / (2f / 3f));
                        float alpha2 = Mathf.Lerp(0f, 255f, curve.Evaluate(doorT2));

                        doorCol = doorFrame.material.GetColor("_Color");
                        doorCol.a = (byte)alpha2;

                        if (doorT == 1f)
                        {
                            //Set the rotation and scale values of the floor
                            Vector3 rot1 = tr1.eulerAngles;
                            Vector3 rot2 = tr2.eulerAngles;
                            Vector3 rot3 = tr3.eulerAngles;

                            Vector3 sc1 = tr1.localScale;
                            Vector3 sc2 = tr2.localScale;
                            Vector3 sc3 = tr3.localScale;

                            //Proceed with the progress
                            currentTime += Time.deltaTime;

                            //First transition takes 75% of the total time
                            finalT = Mathf.Clamp01(currentTime / totalTime);
                            float firstFinalT = finalT / 0.75f;

                            Vector3 rotVal = Vector3.Lerp(rot1, rot2, curve.Evaluate(firstFinalT));
                            Vector3 scVal = Vector3.Lerp(sc1, sc2, curve.Evaluate(firstFinalT));

                            //Launch the second transition
                            if (finalT >= 0.75f)
                            {
                                // adjust progress to start from 0 at 0.75
                                float secondFinalT = (finalT - 0.75f) / 0.25f;

                                Vector3 rotVal2 = Vector3.Lerp(rot2, rot3, curve.Evaluate(secondFinalT));
                                Vector3 scVal2 = Vector3.Lerp(sc2, sc3, curve.Evaluate(secondFinalT));

                                // use the next values for the rest of the time
                                rotVal = rotVal2;
                                scVal = scVal2;

                                Vector3 lastLinePos = Vector3.Lerp(line.GetPosition(1), line.GetPosition(0), secondFinalT);

                                line.SetPosition(1, lastLinePos);
                            }
                            

                            //Set the size of the floor
                            floor.eulerAngles = rotVal;
                            floor.localScale = scVal;

                            
                        }
                    }
                    
                }

                //Change colors of the doors
                doorFrame.material.SetColor("_Color", doorCol);
                doorMiddle.material.SetColor("_Color", midCol);

                break;

            case 4:
                if (musicControl.introFinished)
                {
                    if (direction == 1)
                        status = 0;
                    else
                        status = 1;
                }
                    
                break;
        }

        //Set direction
        direction = directions[hitAmount];

        if (status < 2)
        {
            if (direction == 1)
                status = 0;
            else
                status = 1;
        }

        //Line Color
        line.material.SetColor("_Color", mainColor);
        torusRender.material.SetColor("Color", mainColor);

        if (hitAmount >= requiredHits)
            status = 3;

        //Raycast
        Ray ray = new Ray(laserStart.position, dirVec);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, 100f);

        if (hit)
        {
            Vector3 hitPos = hitInfo.point;
            line.SetPosition(1, hitPos);

            if (!RayCheck(hitInfo.collider))
            {
                return;
            }

            //If we hit the correct side
            if (hitInfo.collider.CompareTag("Symbol" + status))
            {
                SymbolBehavior receiver = hitInfo.collider.GetComponentInParent<SymbolBehavior>();
                receiver.failAmount = 0;

                //Add the hit
                hitAmount++;
                rotMultiplier = 1.15f;

                //Speed up the laser
                currentRotSpeed *= rotMultiplier;

                //Change color of display
                displayChangeVal += changingDisplays;

                for (int i = (int)((hitAmount - 1) * changingDisplays); i < displayChangeVal; i++){
                
                    displays[i].changing = true;
                }

                hitRecorded = true;

            }
            //If we didnt hit the correct side
            else
            {
                SymbolBehavior receiver = hitInfo.collider.GetComponentInParent<SymbolBehavior>();
                receiver.failAmount++;

                distances = (laserBowl.rotation.y - originalRotation.y) / hitAmount;
                distanceToCover = laserBowl.rotation.y;

                displayChangeVal = 0;
                hitRecorded = true;
                status = 2;
            }
        }
        else
        {
            line.SetPosition(1, line.transform.position + lineDist * dirVec);

            if (hitRecorded)
                hitRecorded = false;
        }
    }
}
