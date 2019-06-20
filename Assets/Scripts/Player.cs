using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Car
{
    [SerializeField] private float acceleration;
    [SerializeField] private float deacceleration;
    [SerializeField] private float turnSpeed;

    private float speed;
    private bool isMoving;
    private float rotationAngle;
    private Vector3 motion;
    private Vector2 rotation;
    private Rigidbody rb;

    private GameManager gm;

    public float Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }

    public float MaxSpeed
    {
        get => maxSpeed;
        set => maxSpeed = value;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 0;
        isMoving = false;
        rotationAngle = 0;
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        GetMovementInput();
        GetRotationInput();
        UpdateSpeed();
    }

    private void FixedUpdate()
    {
        UpdatePosition();

        if (rotation.x != 0 || rotation.y != 0)
        {
            CalculateRotationAngle();
            UpdateRotation();
        }
    }

    private void GetMovementInput()
    {
        isMoving = (Input.GetButton("Car Movement")) ? true : false;
    }

    private void GetRotationInput()
    {
        rotation.x = Input.GetAxis("Horizontal Rotation");
        rotation.y = Input.GetAxis("Vertical Rotation");
    }

    private void CalculateRotationAngle()
    {
        rotationAngle = Mathf.Atan2(rotation.x, rotation.y) * Mathf.Rad2Deg;
    }

    private void UpdateSpeed()
    {
        if (isMoving)
        {
            motion = transform.forward * Input.GetAxis("Car Movement");
            speed += acceleration * Time.deltaTime;
        }
        else
            speed -= deacceleration * Time.deltaTime;

        speed = Mathf.Clamp(speed, 0, maxSpeed);
    }

    private void UpdatePosition()
    {
        rb.MovePosition(transform.position + motion * speed * Time.deltaTime);

        //transform.position += motion * speed * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(0, rotationAngle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Car")
        {
            /*
            Rigidbody rb = collision.transform.GetComponent<Rigidbody>();
            Vector3 force = transform.position - collision.transform.position;
            force.Normalize();

            rb.AddForce(force * speed * 200, ForceMode.Impulse);
            this.rb.velocity = rb.velocity; */
            Enemy enemy = collision.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!enemy.GoldState)
                    gm.CheckWin(enemy);
                else
                    gm.GrantCoins(enemy);
            }
        }
    }
}
