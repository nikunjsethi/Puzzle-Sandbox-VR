using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    // public variables used by this script
    public Transform playerHead;
    //public Transform leftController;
    //public Transform rightController;

    //public ConfigurableJoint headJoint;
    //public ConfigurableJoint leftHandJoint;
    //public ConfigurableJoint rightHandJoint;

    public CapsuleCollider bodyCollider;

    public float bodyHeightMin = 0.5f;
    public float bodyHeightMax = 2;

    /// <summary>
    /// FixedUpdate is called once per frame to update physics systems
    /// </summary>
    void FixedUpdate()
    {
        // position the body collider based on the head position
        bodyCollider.height = Mathf.Clamp(playerHead.localPosition.y, bodyHeightMin, bodyHeightMax);
        bodyCollider.center = new Vector3(playerHead.localPosition.x, (bodyCollider.height / 2), playerHead.localPosition.z);

        // position the left hand configurable joint based on the left controller position and rotation
        //leftHandJoint.targetPosition = leftController.localPosition;
        //leftHandJoint.targetRotation = leftController.localRotation;

        // position the right hand configurable joint based on the right controller position and rotation
        //rightHandJoint.targetPosition = rightController.localPosition;
        //rightHandJoint.targetRotation = rightController.localRotation;

        // position the head configurable joint based on the head position
        //headJoint.targetPosition = playerHead.localPosition;

    } // end FixedUpdate
}
