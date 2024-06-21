using Net;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField, NetVariable(0)] float velocityY;
    [SerializeField, NetVariable(1)] float velocityX;

    PlayerController playerController;
    Rigidbody RB;

    void Awake()
    {
        RB = GetComponent<Rigidbody>();
        playerController = GetComponentInParent<PlayerController>();
    }

    void FixedUpdate()
    {
        if (playerController.currentPlayer)
        {
            if (Input.GetKey(KeyCode.W))
            {
                RB.AddForce(transform.forward * velocityY, ForceMode.Force);
            }

            if (Input.GetKey(KeyCode.S))
            {
                RB.AddForce(-transform.forward * velocityY);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.eulerAngles += Vector3.down * velocityX * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.eulerAngles += Vector3.up * velocityX * Time.deltaTime;
            }
        }
    }
}
