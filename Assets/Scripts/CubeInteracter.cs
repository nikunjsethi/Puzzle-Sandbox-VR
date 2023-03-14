using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class CubeInteracter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Interactable"))
        {
            PhotonNetwork.Destroy(other.gameObject);
            gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
