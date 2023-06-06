using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStatus : MonoBehaviour
{
    public string Name;

    private BallController ballController;

    private void Start()
    {
        ballController = GameObject.Find("BallController").GetComponent<BallController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("triggerLeft"))
        {
            ballController.left = false;
        }
        else if (other.CompareTag("triggerRight"))
        {
            ballController.left = true;
        }
    }
}
