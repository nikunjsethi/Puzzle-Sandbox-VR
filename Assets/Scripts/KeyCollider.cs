using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollider : MonoBehaviour
{
    public Door_anim MainDoor_R;
    public Door_anim MainDoor_L;

    //public Animation doorOpen;
    //[SerializeField] Animation doorClose;
    // for now just put together basic trigger functionality for a key 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            // run the animation for this key to open the door
            Debug.Log("Open the door!");
            MainDoor_R.Open();
            MainDoor_L.Open();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            // run the animation to close the door if the key is removed
            Debug.Log("Close the door!");
            MainDoor_R.Close();
            MainDoor_L.Close();
        }

    }
}
