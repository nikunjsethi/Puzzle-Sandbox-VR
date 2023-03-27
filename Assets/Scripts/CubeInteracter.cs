using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CubeInteracter : MonoBehaviour
{
    public PhotonView pv;

    [SerializeField] NetworkManager networkManager;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(pv.IsMine)
        {
            if (other.CompareTag("Interactable"))
            {
                pv.RPC("RPC_ColorChanger",RpcTarget.AllBuffered);
                pv.RPC("DestroyViaMaster", RpcTarget.MasterClient, other);
                //if (PhotonNetwork.IsMasterClient)
                //{
                //    Debug.Log("Master");
                //    PhotonNetwork.Destroy(other.gameObject);
                //    networkManager.numCubesToReplace--;
                //}
                //else if (!PhotonNetwork.IsMasterClient)
                //{
                //    Debug.Log("Not master");
                //    pv.RPC("DestroyViaMaster", RpcTarget.MasterClient, other.gameObject);                           //only master client can destroy gameobjects
                //}
            }
            
        }
    }

    [PunRPC]
    void RPC_ColorChanger()
    {
        gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    [PunRPC]
    void DestroyViaMaster(Collider cube)
    {
        PhotonNetwork.Destroy(cube.gameObject);
        networkManager.numCubesToReplace--;
    }
}
