using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;

public class NetworkPlayerInstantiator : MonoBehaviourPunCallbacks
{
    // public variables updated by other scripts
    public List<Transform> instantiationPoints;
    public GameObject[] locations;

    // Keep the instantiated player object around
    private NetworkManager networkManager;
    private PhotonView pv;
    private GameObject spawnedPlayerPrefab;


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            //playerCount = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
            networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // get the user id from the network manager for this user and spawn them
        Int64 userId = Convert.ToInt64(networkManager.m_userId);
        object[] objects = new object[1] { userId };
        //spawnedPlayerPrefab = PhotonNetwork.Instantiate("NewPlayer", transform.position, transform.rotation);

        // enable the avatar entity on the main XR rig so we can grab the data now
        //networkManager.avatarEntity.SetActive(true);
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("NewPlayer", transform.position, transform.rotation, 0, objects);

        // testing the id information for the photon view of this object after it is created
        PhotonView spawnedPhotonView = spawnedPlayerPrefab.GetComponent<PhotonView>();
        //Debug.Log("Owner: " + spawnedPhotonView.Owner + ", View ID: " + spawnedPhotonView.ViewID + ", Scene View ID:" + spawnedPhotonView.sceneViewId);
        //Debug.Log("Actor ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
    } 

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
