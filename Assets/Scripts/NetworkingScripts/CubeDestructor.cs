using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class CubeDestructor : MonoBehaviour
{
    PhotonView pv;

    //private void Start()
    //{
    //    pv = GetComponent<PhotonView>();
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (pv.IsMine)
    //    {
    //        if (collision.gameObject.CompareTag("CubeTrigger"))
    //        {
    //            Debug.Log("Destroy Bitch collision");
    //            PhotonNetwork.Destroy(gameObject);
    //            //pv.RPC("DestroySelf", RpcTarget.MasterClient);
    //        }
    //    }
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(pv.IsMine)
    //    {
    //        if(other.CompareTag("CubeTrigger"))
    //        {
    //            Debug.Log("Destroy Bitch");
    //            PhotonNetwork.Destroy(gameObject);
    //            //pv.RPC("DestroySelf", RpcTarget.MasterClient);
    //        }
    //    }
    //}

    [PunRPC]
    void DestroySelf()
    {
        Debug.Log("Destroy Bitch");
        PhotonNetwork.Destroy(gameObject);
    }
}
