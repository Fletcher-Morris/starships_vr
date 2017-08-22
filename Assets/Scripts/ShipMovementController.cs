using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShipMovementController : NetworkBehaviour
{
    public GameObject externalShip;
    private Rigidbody shipRb;
    public GameObject speedDisplayScreen;
    public GameObject speedTargetScreen;

    public int speedNotchSize = 100;

    public int targetSpeed = 0;
    public float currentSpeed = 0;
    public float accelerationSpeed = 5000;
    public int maxSpeed = 1000;

    private void Start()
    {
        if (externalShip)
        {
            shipRb = externalShip.GetComponent<Rigidbody>();
        }
    }

    [Command]
    public void CmdSetDesiredSpeed(int speed)
    {
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
        RpcSetDesiredSpeed(speed);
    }
    [ClientRpc]
    private void RpcSetDesiredSpeed(int speed)
    {
        targetSpeed = speed;
    }
    public void SpeedUp()
    {
        CmdSetDesiredSpeed(targetSpeed + speedNotchSize);
    }
    public void SlowDown()
    {
        CmdSetDesiredSpeed(targetSpeed - speedNotchSize);
    }

    private void Update()
    {
        AccellerateShip();
        speedDisplayScreen.GetComponent<Text>().text = "SPEED: " + Mathf.RoundToInt(currentSpeed).ToString();
        speedTargetScreen.GetComponent<Text>().text = targetSpeed.ToString();

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SpeedUp();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SlowDown();
        }
    }

    private void AccellerateShip()
    {
        if (targetSpeed > currentSpeed)
        {
            shipRb.AddRelativeForce(externalShip.transform.forward * -accelerationSpeed);
        }
        else if (targetSpeed < currentSpeed)
        {
            shipRb.AddRelativeForce(externalShip.transform.forward * accelerationSpeed);
        }

        shipRb.velocity = Vector3.ClampMagnitude(shipRb.velocity, maxSpeed);

        currentSpeed = shipRb.velocity.magnitude;
    }
}