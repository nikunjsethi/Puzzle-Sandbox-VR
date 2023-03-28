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
                //networkManager.numCubesToReplace--;
                //PhotonNetwork.Destroy(other.gameObject);
                //Destroy(other.gameObject);
                //PhotonView newPhotonView = other.GetComponent<PhotonView>();
                //newPhotonView.RPC("DestroyViaMaster", RpcTarget.MasterClient, other);
                //pv.RPC("DestroyViaMaster", RpcTarget.MasterClient, other);
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("Master");
                    //PhotonNetwork.Destroy(other.gameObject);
                    Destroy(other.gameObject);
                    networkManager.numCubesToReplace--;
                }
                //else
                {
                    Debug.Log("Not master");

                    pv.RPC("DestroyViaMaster", RpcTarget.MasterClient, other);                           //only master client can destroy gameobjects
                }
                //other.gameObject.SetActive(false);
                //pv.RPC("DestroyViaMaster", RpcTarget.AllBuffered, other);
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
        Debug.Log("RPC called on everyone");
        //cube.gameObject.SetActive(false);
        Destroy(cube.gameObject);
        networkManager.numCubesToReplace--;
    }
}
