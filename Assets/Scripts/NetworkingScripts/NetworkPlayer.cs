using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour
{
    public Transform Head;
    public Transform LeftHand;
    public Transform RightHand;
    private PhotonView pv;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    public List<GameObject> instantiationPoint;                                 //the points where the players will instantiate around the table
    public List<GameObject> cubeDisable;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            instantiationPoint = new List<GameObject>();
            instantiationPoint.AddRange(GameObject.FindGameObjectsWithTag("StartingPoints"));
            cubeDisable = new List<GameObject>();
            cubeDisable.AddRange(GameObject.FindGameObjectsWithTag("Hide"));
            XROrigin origin = FindObjectOfType<XROrigin>();
            headRig = origin.transform.Find("Camera Offset/Main Camera");
            leftHandRig = origin.transform.Find("Camera Offset/LeftHand Controller");
            rightHandRig = origin.transform.Find("Camera Offset/RightHand Controller");
            int playerCount = PhotonNetwork.CountOfPlayers;
            Debug.Log("Player count is : " + playerCount);
            origin.transform.position = instantiationPoint[playerCount].transform.position;
            origin.transform.rotation = instantiationPoint[playerCount].transform.rotation;
        }
            pv.RPC("RPC_Array_Update", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_Array_Update()
    {
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            int randomNumber = Random.Range(0, cubeDisable.Count);
            Debug.Log("Randon value : " + randomNumber);
            PhotonNetwork.Destroy(cubeDisable[randomNumber]);
            cubeDisable.RemoveAt(randomNumber);
        }
    }
    void Update()
    {
        if(pv.IsMine)
        {
            Head.gameObject.SetActive(false);
            LeftHand.gameObject.SetActive(false);
            RightHand.gameObject.SetActive(false);
            MappingMovement(Head, headRig);
            MappingMovement(LeftHand, leftHandRig);
            MappingMovement(RightHand, rightHandRig);
        }

    }

    void MappingMovement(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
