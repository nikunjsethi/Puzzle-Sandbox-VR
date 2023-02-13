using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollider : MonoBehaviour
{
    // for now just put together basic trigger functionality for a key 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            // run the animation for this key to open the door
            Debug.Log("Open the door!");
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            // run the animation to close the door if the key is removed
            Debug.Log("Close the door!");
        }

    }
}
