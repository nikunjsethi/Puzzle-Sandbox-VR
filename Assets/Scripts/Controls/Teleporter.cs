using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // private variables used by this script
    private GameManager gameManager;

    /// <summary>
    /// Sets up variables when this script wakes up
    /// </summary>
    private void Awake()
    {
        // as these objects can be loaded after the main scene, they need to find the game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    } // end Awake

    /// <summary>
    /// Teleports the player back to the main hub
    /// </summary>
    /// <param name="other">The object that collided with this teleporter</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Teleporter collision activated");

        if ((other != null) && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Teleporter collided with player");
            gameManager.TeleportBack();
        }

    } // end OnTriggerEnter
}
