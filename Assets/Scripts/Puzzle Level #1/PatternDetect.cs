using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PatternDetect : MonoBehaviour
{
    private int playerAmount;

    private List<int> pattern = new List<int>();
    
    [SerializeField] public List<SymbolBehavior> symbols = new List<SymbolBehavior>();
    public HitDisplays[] displays;

    private List<int> displaysChanged = new List<int>();
    private List<int> startDisp = new List<int>();
    private int lastDisplay = 0;
    private List<bool> done = new List<bool>();

    public GameObject teleporter;
    public GameObject floor;
    public GameObject door;

    // Start is called before the first frame update
    void Start()
    {
        displays = FindObjectsOfType<HitDisplays>();
        playerAmount = PhotonNetwork.CountOfPlayers;

        int totalDisplays = displays.Length;
        int displayPerPerson = Mathf.FloorToInt(displays.Length / playerAmount);

        for (int i = 0; i < playerAmount; i++)
        {
            int number = Random.Range(0, 2);
            pattern.Add(number);

            done[i] = false;

            if (i < playerAmount - 1)
            {
                startDisp.Add(i * displayPerPerson);
                displaysChanged.Add(displayPerPerson);
                totalDisplays -= displayPerPerson;
                continue;
            }

            displaysChanged.Add(totalDisplays);
            startDisp.Add(i * displayPerPerson);


        }

        floor.SetActive(false);
        door.SetActive(false);
        teleporter.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < playerAmount; i++)
        {
            if (symbols[i].activeSide == pattern[i])
            {
                for (int x = startDisp[i]; x < startDisp[i] + displaysChanged[i]; x++)
                {
                    displays[i].changing = true;
                }

                done[i] = true;
            }
            else
            {
                for (int x = startDisp[i]; x < startDisp[i] + displaysChanged[i]; x++)
                {
                    displays[i].changing = false;
                }

                done[i] = false;
            }

        }


        foreach (bool value in done)
        {
            if (!value)
                continue;

            teleporter.SetActive(true);
            floor.SetActive(true);
            door.SetActive(true);
        }
    }
}
