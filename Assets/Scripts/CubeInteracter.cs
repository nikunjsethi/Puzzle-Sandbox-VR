using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR.Interaction.Toolkit;

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
                other.gameObject.GetComponent<XRGrabNetworkInteractable>().deleteMe = true;
                networkManager.numCubesToReplace--;
            }
        }
    }

    [PunRPC]
    void RPC_ColorChanger()
    {
        gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

}
