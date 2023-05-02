using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR;
using Photon.Pun;
//using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
//using TMPro;
using System;
using Unity.VisualScripting;

public class NetworkPlayer : MonoBehaviour
{
    //public Transform Head;
    //public Transform LeftHand;
    //public Transform RightHand;
    private PhotonView pv;

    //private Transform headRig;
    //private Transform leftHandRig;
    //private Transform rightHandRig;

    public List<GameObject> instantiationPoint;                                 //the points where the players will instantiate around the table
    public List<GameObject> cubeDisable;

   // public TextMeshProUGUI playerCount;
    public NetworkManager networkManager;
    public NetworkAvatar networkAvatarScript;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            //playerCount = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
            networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        }

        NetworkAvatar networkAvatarScript = gameObject.GetComponentInChildren<NetworkAvatar>();
        //networkAvatarScript.ConfigureAvatarEntity(pv);
        //networkAvatarScript.LoadUserFromPhotonView();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            instantiationPoint = new List<GameObject>();
            instantiationPoint.AddRange(GameObject.FindGameObjectsWithTag("StartingPoints"));
            cubeDisable = new List<GameObject>();
            cubeDisable.AddRange(GameObject.FindGameObjectsWithTag("Hide"));
            XROrigin origin = FindObjectOfType<XROrigin>();
            //headRig = origin.transform.Find("Camera Offset/Main Camera");
            //leftHandRig = origin.transform.Find("Camera Offset/LeftHand Controller");
            //rightHandRig = origin.transform.Find("Camera Offset/RightHand Controller");
            
            int playerCount = PhotonNetwork.CountOfPlayers;
            Debug.Log("Player count is : " + playerCount);

            // teleport the player(s) to their spot in the base hub
            int playerTeleportPos = networkManager.getPlayerIDZeroBased();

            origin.transform.position = instantiationPoint[playerTeleportPos].transform.position;
            origin.transform.rotation = instantiationPoint[playerTeleportPos].transform.rotation;
        }
        int newCount = PhotonNetwork.CountOfPlayers;
        pv.RPC("PlayerStats", RpcTarget.AllBuffered, newCount);
        pv.RPC("RPC_Array_Update", RpcTarget.AllBuffered);
        Debug.Log("RPC getting called : " + newCount);
       
    }

    [PunRPC]
    void PlayerStats(int number)
    {
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            number = PhotonNetwork.CountOfPlayers;
            //playerCount.text = number.ToString();
        }
    }

    [PunRPC]
    void RPC_Array_Update()
    {
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            int randomNumber = UnityEngine.Random.Range(0, cubeDisable.Count);
            Debug.Log("Randon value : " + randomNumber);
            PhotonNetwork.Destroy(cubeDisable[randomNumber]);
            cubeDisable.RemoveAt(randomNumber);
            networkManager.numCubesToReplace++;
            networkManager.playerInLevel = true;
        }
    }

    void Update()
    {
        if(pv.IsMine)
        {
            //Head.gameObject.SetActive(false);
            //LeftHand.gameObject.SetActive(false);
            //RightHand.gameObject.SetActive(false);
            //MappingMovement(Head, headRig);
            //MappingMovement(LeftHand, leftHandRig); 
            //MappingMovement(RightHand, rightHandRig);
        }

    }

    void MappingMovement(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
    
}
