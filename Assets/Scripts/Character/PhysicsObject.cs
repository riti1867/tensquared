﻿using System.Collections.Generic;
using UnityEngine;

/*
 * powers most of the physics calculations for the player to move
 */

public class PhysicsObject : MonoBehaviour {

  public float minGroundNormalY = .65f;

  // scale gravity with this value
  protected float gravityModifier = 4f;

  protected string state = "Circle";
  protected bool inDoubleJump = false; // is true, if player executed double jump and is still in air
  protected Vector2 doubleJumpMovement = Vector2.zero;

  protected Vector2 velocity, targetVelocity, groundNormal;
  protected bool grounded;

  protected ContactFilter2D contactFilter;
  protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
  protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

  protected const float minMoveDistance = 0.001f,
                        shellRadius = 0.01f;

  private LineRenderer triangleLineRenderer;

  // reference to the 2D rigid body connected to the object
  protected Rigidbody2D rb2d;

  void OnEnable() {
    rb2d = GetComponent<Rigidbody2D>();
    triangleLineRenderer = GetComponent<LineRenderer>();
  }

  void Start() {
    contactFilter.useTriggers = false;
    contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    contactFilter.useLayerMask = true;
  }

  void Update() {
    targetVelocity = Vector2.zero;
    ComputeVelocity();
  }

  protected virtual void ComputeVelocity() {}

  // has frequency of physics system; called every fixed frame-rate frame
  void FixedUpdate() {

    // if player is set to frozen, don't calculate movement
    if (PlayerController.Instance.isFrozen) return;

    velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
    velocity.x = targetVelocity.x;

    grounded = false;

    // calculate position after double jump of triangle
    if (!grounded && state == "Triangle") {

      if (inDoubleJump) {

        if (doubleJumpMovement.x == 0f && doubleJumpMovement.y == 0f) setDoubleJumpMovement();

        // reset line renderer
        triangleLineRenderer.SetPositions(new Vector3[2] { transform.position, transform.position });

      }
      else {
        // draw line from player to mouse cursor
        triangleLineRenderer.SetPositions(new Vector3[2] { transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) });
        doubleJumpMovement = Vector2.zero;
      }

    }
    else {
      // reset line renderer
      triangleLineRenderer.SetPositions(new Vector3[2] { transform.position, transform.position });
      doubleJumpMovement = Vector2.zero;
    }

    // change in position
    Vector2 deltaPosition = velocity * Time.deltaTime;

    // apply normal player movement (left, right, jump)
    xMovement((new Vector2(groundNormal.y, -groundNormal.x)) * deltaPosition.x);
    yMovement(Vector2.up * deltaPosition.y);

  }

  private void setDoubleJumpMovement() {

    Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    float doubleJumpReducer = 6f,
          angle = getAngleMousePlayer();

    // leftwards
    if (angle < 90f) { 
      doubleJumpMovement.x = -((90f - angle) / (100f * doubleJumpReducer));

      // less upwards movement the closer the cursor is to the vertical vector standing on player
      doubleJumpMovement.y = -((90f - angle) / (100f * doubleJumpReducer));

    }
    // rightwards
    else if (angle > 90f) {
      doubleJumpMovement.x = (angle - 90f) / (100f * doubleJumpReducer);

      // less upwards movement the closer the cursor is to the vertical vector standing on player
      doubleJumpMovement.y = -((angle - 90f) / (100f * doubleJumpReducer));

    }
    else {
      doubleJumpMovement.x = 0f;
    }

    Debug.Log(doubleJumpMovement);

    float getAngleMousePlayer() {

      Vector2 v1 = Vector2.right,
              v2 = Vector2.zero;

      v2.x = transform.position.x - mouseCoords.x;
      v2.y = transform.position.y - mouseCoords.y;


      // angle between two vectors
      float angleMousePlayer = v1.x * v2.x + v1.y * v2.y;
      angleMousePlayer /= (Mathf.Sqrt(Mathf.Pow(v1.x, 2) + Mathf.Pow(v1.y, 2)) * Mathf.Sqrt(Mathf.Pow(v2.x, 2) + Mathf.Pow(v2.y, 2)));

      angleMousePlayer = Mathf.Rad2Deg * Mathf.Acos(angleMousePlayer);

      return angleMousePlayer;

    }

  }

  // apply movement to rigid body on 2D axes
  private void xMovement(Vector2 move) {Movement(move, false); }
  private void yMovement(Vector2 move) {Movement(move, true); }
  private void Movement(Vector2 move, bool yMovement) {

    float distance = move.magnitude; // betrag vektor

    if (distance > minMoveDistance) {

      int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
      hitBufferList.Clear();

      for (int i = 0; i < count; i++) hitBufferList.Add(hitBuffer[i]);

      for (int i = 0; i < hitBufferList.Count; i++) {

        Vector2 currentNormal = hitBufferList[i].normal;

        if (currentNormal.y > minGroundNormalY) {
          grounded = true;
          if (yMovement) {
            groundNormal = currentNormal;
            currentNormal.x = 0;
          }
        }

        float projection = Vector2.Dot(velocity, currentNormal);

        if (projection < 0) velocity = velocity - projection * currentNormal;

        float modifiedDistance = hitBufferList[i].distance - shellRadius;
        distance = modifiedDistance < distance ? modifiedDistance : distance;

      }

    }

    // calculate final position of rigid body
    Vector2 pos = rb2d.position + move.normalized * distance + doubleJumpMovement;
    rb2d.position = pos;

  }

}