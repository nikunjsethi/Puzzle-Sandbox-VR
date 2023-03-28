using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class NetworkPlayerInstantiator : MonoBehaviourPunCallbacks
{
    public List<Transform> instantiationPoints;
    // Keep the instantiated player object around
    private GameObject spawnedPlayerPrefab;
    public GameObject[] locations;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("NewPlayer", transform.position, transform.rotation);
        PhotonNetwork.Instantiate("InteractableCube", locations[0].transform.position, locations[0].transform.rotation);
        PhotonNetwork.Instantiate("InteractableCube", locations[1].transform.position, locations[1].transform.rotation);
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
