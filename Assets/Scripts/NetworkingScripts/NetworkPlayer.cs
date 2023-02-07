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
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        //XRRig rig = FindObjectOfType<XRRig>();
        XROrigin origin = FindObjectOfType<XROrigin>();
        headRig = origin.transform.Find("Camera Offset/Main Camera");
        leftHandRig = origin.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = origin.transform.Find("Camera Offset/RightHand Controller");
    }

    // Update is called once per frame
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
