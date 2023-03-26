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
            }
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(other.gameObject);
                networkManager.numCubesToReplace--;
            }
            else
            {
                pv.RPC("DestroyViaMaster", RpcTarget.MasterClient, other.gameObject);                           //only master client can destroy gameobjects
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
    }
}
