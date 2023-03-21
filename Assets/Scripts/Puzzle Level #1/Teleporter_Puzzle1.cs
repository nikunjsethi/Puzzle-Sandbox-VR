using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Teleporter_Puzzle1 : MonoBehaviour
{
    // Configuration for spawning
    [Header("Spawns")]
    public Transform[] spawns = new Transform[10];
    public List<int> spawnCombo = new List<int>();
    private int playerAmount;

    // private variables used by this script
    private GameManager gameManager;

    /// <summary>
    /// Sets up variables when this script wakes up
    /// </summary>
    private void Awake()
    {
        // as these objects can be loaded after the main scene, they need to find the game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //Find the amount of players
        playerAmount = PhotonNetwork.CountOfPlayers;

        //Find Laser Control Script
        spawns = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LaserControl>().spawnPoints;

        //This sets up the spawn locations
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

                    if (choice1 >= spawns.Length)
                        choice1 -= spawns.Length;

                    if (choice2 <= 0)
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

                int index = Random.Range(0, placesToFill.Count);
                spawnCombo.Add(placesToFill[index]);
            }
        }

        //Send data to spawner

    } // end Awake

    /// <summary>
    /// Teleports the player back to the main hub
    /// </summary>
    /// <param name="other">The object that collided with this teleporter</param>
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Teleporter collision activated");

        if ((other != null) && other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Teleporter collided with player");
            gameManager.TeleportBack();
        }

    } // end OnTriggerEnter
}
