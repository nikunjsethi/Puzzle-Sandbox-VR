using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class CubeInteracter : MonoBehaviour
{
    public PhotonView pv;

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
                PhotonNetwork.Destroy(other.gameObject);
                pv.RPC("RPC_ColorChanger",RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void RPC_ColorChanger()
    {
        gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

}
