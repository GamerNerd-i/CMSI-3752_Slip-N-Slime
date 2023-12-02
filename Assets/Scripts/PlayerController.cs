using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum Lane
    {
        LEFT = -5,
        RIGHT = 5,
        CENTER = 0,
    }

    [Header("Movement")]
    public float initialSpeed = 10.0f;
    public float forwardSpeed = 5.0f;
    public float laneOffset = 5.0f;
    public float laneChangeSpeed = 500.0f;
    public float zSpeedCap = 150.0f;
    public Lane toLane = Lane.CENTER;
    public Lane prevLane;

    [Header("Jumping")]
    public bool jumping = false;
    public bool slideCancel = false;
    public float jumpVelocity = 12.0f;
    public bool isGrounded = true;

    [Header("Sliding")]
    public bool sliding = false;
    public float scaleRate = -0.05f;
    public float minSize = 0.5f;
    public float maxSize = 2.0f;
    public Vector3 slideScalar,
        positionAdjust;

    private Rigidbody rb;

    private Vector3 startPos;

    public bool amDead = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.player = this;
        rb = GetComponent<Rigidbody>();
        slideScalar = Vector3.one * scaleRate;
        positionAdjust = new Vector3(0, scaleRate / 2, 0);
        rb.velocity = Vector3.forward * initialSpeed;
    }

    void OnMove(InputValue moveCommand)
    {
        Vector2 command = moveCommand.Get<Vector2>();

        if (command.x != 0)
        {
            if (command.x > 0 && (float)toLane < laneOffset)
            {
                prevLane = toLane;
                toLane = (Lane)((float)toLane + laneOffset);
            }

            if (command.x < 0 && (float)toLane > -laneOffset)
            {
                prevLane = toLane;
                toLane = (Lane)((float)toLane - laneOffset);
            }
        }
        else if (command.y > 0 && !jumping && isGrounded)
        {
            jumping = true;
            if (sliding)
            {
                slideCancel = true;
            }
        }
        else if (command.y < 0)
        {
            if (sliding && slideScalar.y > 0 && isGrounded)
            {
                slideScalar = -slideScalar;
                positionAdjust = -positionAdjust;
            }
            sliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (other.CompareTag("NearMissZone"))
        {
            if (!amDead)
            {
                GameManager.Instance.GrantNearMiss();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            slideCancel = false;
        }

        if (other.CompareTag("Obstacle"))
        {
            amDead = true;
            GameManager.Instance.ShowEndScreen();
        }

        if (other.CompareTag("ScoreOrb"))
        {
            GameManager.Instance.CollectOrb(other.gameObject);
        }

        if (other.CompareTag("Consumable"))
        {
            GameManager.Instance.EatEntity(other.gameObject);
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(0.0f, 0.0f, forwardSpeed);

        movement += HandleLaneChange() * laneChangeSpeed;
        movement += HandleSlide();
        HandleJump();

        rb.AddForce(movement);
        if (rb.velocity.z >= zSpeedCap)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, zSpeedCap);
        }
    }

    private Vector3 HandleLaneChange()
    {
        if ((float)toLane != transform.position.x)
        {
            return changeLane(toLane);
        }
        return Vector3.zero;
    }

    private Vector3 changeLane(Lane targetLane)
    {
        switch (targetLane)
        {
            case Lane.LEFT:
                if (transform.position.x <= (float)toLane)
                {
                    snapToLane();
                }
                else
                {
                    return Vector3.left;
                }
                break;
            case Lane.RIGHT:
                if (transform.position.x >= (float)toLane)
                {
                    snapToLane();
                }
                else
                {
                    return Vector3.right;
                }
                break;
            case Lane.CENTER:
                return changeLane(prevLane == Lane.LEFT ? Lane.RIGHT : Lane.LEFT);
        }
        return Vector3.zero;
    }

    private void snapToLane()
    {
        rb.velocity = new Vector3(0.0f, rb.velocity.y, rb.velocity.z);
        transform.position = new Vector3((float)toLane, transform.position.y, transform.position.z);
    }

    private void HandleJump()
    {
        if (jumping)
        {
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
            jumping = false;
        }
        else if (rb.velocity.y <= jumpVelocity * 0.75f)
        {
            rb.AddForce(Vector3.down * (jumpVelocity * 1.5f));
        }
    }

    private Vector3 HandleSlide()
    {
        if (sliding)
        {
            if (isGrounded || slideCancel)
            {
                if (slideScalar.y < 0 || slideCancel)
                {
                    transform.localScale += slideScalar * 3;
                    transform.position += positionAdjust * 3;
                }
                else
                {
                    transform.localScale += slideScalar;
                    transform.position += positionAdjust;
                }

                if (transform.localScale.y > maxSize && slideScalar.y > 0)
                {
                    transform.localScale = Vector3.one * maxSize;
                    sliding = false;
                    slideCancel = false;

                    slideScalar = -slideScalar;
                    positionAdjust = -positionAdjust;
                }

                if (transform.localScale.y < minSize)
                {
                    transform.localScale = Vector3.one * minSize;
                    slideScalar = -slideScalar;
                    positionAdjust = -positionAdjust;
                }
            }
            else
            {
                return Vector3.down * (jumpVelocity * jumpVelocity) * 3;
            }
        }

        return Vector3.zero;
    }
}
