using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart
{
    Feet,
    LeftSide,
    RightSide
}

public class PlayerBodyPart : MonoBehaviour
{
    public BodyPart bodyPart;
    private PlayerController playerController;

    // Use this for initialization
    private void Start()
    {
        playerController = this.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        playerController.UpdateBodyPartCollider(bodyPart, true);
    }

    private void OnTriggerExit(Collider other)
    {
        playerController.UpdateBodyPartCollider(bodyPart, false);
    }

    private void OnTriggerStay(Collider other)
    {
        playerController.UpdateBodyPartCollider(bodyPart, true);
    }
}
