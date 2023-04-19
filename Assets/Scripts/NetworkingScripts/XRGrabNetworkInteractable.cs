using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    // boolean to try and delete this
    public bool deleteMe;

    // private variables used by this script
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        deleteMe = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ( (deleteMe) && (PhotonNetwork.IsMasterClient))
        {
            if (photonView.Owner == PhotonNetwork.MasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                photonView.RequestOwnership();
            }
        }
    }

    [System.Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        photonView.RequestOwnership();
        base.OnSelectEntered(interactor);
    }
}
