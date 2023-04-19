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
    private Quaternion originalRotation;

    //Line Renderer for Laser
    [Header("Line")]
    public LineRenderer line;
    public Renderer torusRender;
    public int status = 4;
    private bool hitRecorded = false;
    public float lineDist;
    public Vector3 dirVec;

    [Header("Line Colors")]
    //Set to private as we're grabbing the colors from HitDisplay function or setting them later in the code
    private Color color1;
    private Color color2;
    public Color mainColor;
    // Fail color - currently at black
    public Color defColor = Color.black;


    //Display for hits
    [Header("Hit Display")]
    public int hitAmount;
    public int requiredHits;
    private int minHits = 8;
    public float changingDisplays;
    private float displayChangeVal;
    public List<HitDisplays> displays = new List<HitDisplays>();

    [Header("Winning Transforms")]
    public FinalTransformation finalTransformation;
    public Transform floor;
    public Transform tr1;
    public Transform tr2;
    public Transform tr3;
    private float fRotT = 0;
    public float rotTime = 2.5f;
    private float curRotTime = 0;
    public AnimationCurve curve1;
    public Renderer doorMiddle;
    public Renderer doorFrame;
    

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

        hitAmount = 0;
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
        changingDisplays = (float)displays.Count / (float)requiredHits;
        displayChangeVal = 0;

        //Set Laser colors and index values
        color1 = displays[0].defColor;
        color2 = displays[0].color2;

        for (int i = 0; i < displays.Count; i++)
        {
            displays[i].listIndex = i;
        }

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

                mainColor = color1;

                curRotTime = 0;

                //Rotation for the laser
                laserBowl.Rotate(0f, currentRotSpeed * direction * Time.deltaTime, 0f);

                break;
            case 1:

                mainColor = color2;

                curRotTime = 0;

                //Rotation for the laser
                laserBowl.Rotate(0f, currentRotSpeed * direction * Time.deltaTime, 0f);

                break;

            case >= 2 and < 4:

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

                    if (status == 2 && !displays[0].reducing)
                    {
                        hitAmount = 0;
                        if (directions[0] == 1)
                            status = 0;
                        else
                            status = 1;
                    }

                    if (status == 3)
                        finalTransformation.enabled = true;

                }
                
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

                for (int i = 0; i < displayChangeVal; i++)
                {
                    if (displays[i].clampVal == 1f)
                        continue;

                    float fraction;

                    if (i + 1 > displayChangeVal)
                    {
                        fraction = displayChangeVal - i;
                    }
                    else
                    {
                        fraction = 1f;
                    }

                    displays[i].clampVal = fraction;
                    displays[i].changing = true;
                }

                if (hitAmount == requiredHits)
                    status = 3;

                hitRecorded = true;

            }
            //If we didnt hit the correct side
            else
            {
                SymbolBehavior receiver = hitInfo.collider.GetComponentInParent<SymbolBehavior>();
                receiver.failAmount++;

                for (int i = 0; i < displays.Count; i++)
                {
                    if (displays[i].changing == true)
                        displays[i].reducing = true;
                }

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
