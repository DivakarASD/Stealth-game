using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event System.Action OnPlayerReachedEndPoint;
    public float moveSpeed = 7;
    public float smoothMoveTime = 0.1f;
    public float turnSpeed = 8;  
    
    float smoothMoveVelocity;
    float angle;
    float smoothInputMagnitude;

    Rigidbody myRigidBody;
    Vector3 velocity;
    bool disabled;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        Guard.OnGuardSpottedPlayer += Disable;
    }

    private void Update()
    {
        Vector3 inputDirection = Vector3.zero;

        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, turnSpeed * Time.deltaTime * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            if (OnPlayerReachedEndPoint != null)
            {
                OnPlayerReachedEndPoint();
                Disable();
            }
        }
    }

    private void FixedUpdate()
    {
        myRigidBody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.deltaTime);
    }

    void Disable()
    {
        disabled = true;
    }

    private void OnDestroy()
    {
        Guard.OnGuardSpottedPlayer -= Disable;
    }
}