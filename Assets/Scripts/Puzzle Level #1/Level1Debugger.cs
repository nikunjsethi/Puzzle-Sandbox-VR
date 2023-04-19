using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level1Debugger : MonoBehaviour
{
    public LaserControl laserControl;

    public TMP_Text stateVal;
    public TMP_Text displayClampVal;
    public TMP_Text displayBoolVal;
    public TMP_Text hitValText;
    public TMP_Text requiredHits;
    public TMP_Text reduceAmount;
    public TMP_Text changePerHit;
    private float perHitChange;
    private int hitsToGo;
    private float clampedDisplays;
    private int changingDisplays;
    private int reducingDisplays;
    private int requiredHitsVal;

    private float interval = 0.75f;
    private float lastUpdate;

    // Start is called before the first frame update
    void Start()
    {
        hitsToGo = laserControl.requiredHits - laserControl.hitAmount;
        requiredHitsVal = laserControl.requiredHits;
        reducingDisplays = 0;
        lastUpdate = 0;
        clampedDisplays = 0;
        changingDisplays = 0;
        perHitChange = laserControl.changingDisplays;

        UpdateDisplayVal();
    }

    // Update is called once per frame
    void Update()
    {
        lastUpdate += Time.deltaTime;

        if (lastUpdate >= interval)
        {
            UpdateDisplayVal();
            lastUpdate = 0;
        }

        requiredHitsVal = laserControl.requiredHits;
        hitsToGo = laserControl.requiredHits - laserControl.hitAmount;
        perHitChange = laserControl.changingDisplays;

        string stateValText = laserControl.status.ToString();
        string hitVal = hitsToGo.ToString();
        string clampSum = clampedDisplays.ToString("N2");
        string boolSum = changingDisplays.ToString();
        string reducing = reducingDisplays.ToString();
        string reqHit = requiredHitsVal.ToString();
        string pHit = perHitChange.ToString("N2");

        stateVal.text = stateValText;
        displayClampVal.text = clampSum;
        displayBoolVal.text = boolSum;
        hitValText.text = hitVal;
        reduceAmount.text = reducing;
        requiredHits.text = reqHit;
        changePerHit.text = pHit;
    }

    void UpdateDisplayVal()
    {
        clampedDisplays = 0;
        changingDisplays = 0;
        reducingDisplays = 0;

        foreach (HitDisplays displays in laserControl.displays)
        {
            clampedDisplays += displays.clampVal;

            if (displays.changing)
                changingDisplays++;

            if (displays.reducing)
                reducingDisplays++;
        }
    }
}
