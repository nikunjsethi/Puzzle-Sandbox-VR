using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using System.Security.Cryptography.X509Certificates;

public class FallMechanic : MonoBehaviour
{
    private GameManager gameManager;
    private Vector3 spawnPoint;
    private Quaternion spawnRotation;
    private NetworkManager networkManager;
    private XROrigin xrOrigin;


    // Start is called before the first frame update
    void Start()
    {
        
        // as these objects can be loaded after the main scene, they need to find the game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        int playerTeleportPos = networkManager.getPlayerIDZeroBased();

        // Find the start location and tranport the player there
        GameObject startLocation = GameObject.Find("StartLocation" + playerTeleportPos);

        
        if (startLocation == null)
        {
            Debug.Log("No start location found for player with ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            spawnPoint = startLocation.transform.position;
            spawnRotation = startLocation.transform.rotation;
        }

        xrOrigin = FindObjectOfType<XROrigin>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            xrOrigin.transform.position = spawnPoint;
            xrOrigin.transform.rotation = spawnRotation;
        }
    }
}
