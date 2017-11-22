using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState
{
    Spawn,
    Stopped,
    Moving,
    WallJumping,
    Jumping,
}

public class PlayerController : MonoBehaviour
{
    public float acceleration = 20f; // left/right movement accelleration
    public float maxMoveSpeed = 20f; // max left/right speed
    public float maxVerticalSpeed = 50f; // max up/down speed
    public float jumpPower = 25f; // vertical force applied when jumping
    public float failureLine = -50f; // lowest y value player can be before auto resetting
    public float wallJumpPower = 25f; // horizontal force applied when wall jumping
    public float wallJumpAngle = 60f; // Angle from x axis for wall jumping
    public Rigidbody rigidBody; // the players physics body
    public Vector3 playerStartPos; // the players starting position, used when resetting player, maybe use this for checkpoints
    public MoveState moveState = MoveState.Spawn;
    public bool touchingGround = false;
    public bool touchingLeft = false;
    public bool touchingRight = false;

    // Use this for initialization
    private void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
        playerStartPos = transform.position;
    }

    private void Update()
    {
        UpdateJumping();
        CheckFailure();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UI_ButtonAction()
    {
        if (moveState == MoveState.Spawn) // start the level
            moveState = MoveState.Moving;
        else
            Jump();
    }

    private void CheckFailure()
    {
        // check if the player has fallen below the level
        if (rigidBody.position.y < failureLine)
        {
            ResetPlayer();
        }

        // check if the player has manually reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayer();
        }
    }

    private void ResetPlayer()
    {
        rigidBody.position = playerStartPos; // reset the players position
        rigidBody.velocity = Vector3.zero; // reset their velocity
        moveState = MoveState.Spawn;
    }

    private void UpdateMovement()
    {
        switch (moveState)
        {
            case MoveState.Stopped:
                break;
            case MoveState.Moving:
                if (touchingLeft || touchingRight)
                    break;
                Vector3 moveForce = Vector3.right * acceleration;
                rigidBody.AddForce(moveForce, ForceMode.VelocityChange);
                rigidBody.velocity = new Vector3(Mathf.Clamp(rigidBody.velocity.x, -maxMoveSpeed, maxMoveSpeed), Mathf.Clamp(rigidBody.velocity.y, -maxVerticalSpeed, maxVerticalSpeed), rigidBody.velocity.z);
                break;
            case MoveState.WallJumping:
                if (touchingGround)
                {
                    moveState = MoveState.Moving;
                }
                break;
            case MoveState.Jumping:
                if (!(touchingLeft || touchingRight))
                {
                    moveState = MoveState.Moving;
                }
                break;
        }
    }

    private void UpdateJumping()
    {
        if (Input.anyKeyDown)
        {
            UI_ButtonAction();
        }
    }

    private void Jump()
    {
        if (touchingGround)
        {
            rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            moveState = MoveState.Jumping;
        }
        else if (touchingRight)
        {
            // stop gravity
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
            Vector3 force = new Vector3(
                (float)Math.Cos((Math.PI / 180) * (180 - wallJumpAngle)) * wallJumpPower,
                (float)Math.Sin((Math.PI / 180) * (180 - wallJumpAngle)) * jumpPower,
                0);
            rigidBody.AddForce(force, ForceMode.Impulse);
            moveState = MoveState.WallJumping;
        }
        else if (touchingLeft)
        {
            // stop gravity
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
            Vector3 force = new Vector3(
                (float)Math.Cos((Math.PI / 180) * wallJumpAngle) * wallJumpPower,
                (float)Math.Sin((Math.PI / 180) * wallJumpAngle) * jumpPower,
                0);
            rigidBody.AddForce(force, ForceMode.Impulse);
            moveState = MoveState.WallJumping;
        }
    }

    private void UpdateDashing()
    {
        
    }

    public void UpdateBodyPartCollider(BodyPart part, bool state)
    {
        switch (part)
        {
            case BodyPart.Feet:
                touchingGround = state;
                break;
            case BodyPart.LeftSide:
                touchingLeft = state;
                break;
            case BodyPart.RightSide:
                touchingRight = state;
                break;
        }
    }
}
