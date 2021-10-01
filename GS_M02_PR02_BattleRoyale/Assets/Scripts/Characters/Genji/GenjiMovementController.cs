using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GenjiMovementController : PlayerController
{
        
        [SerializeField] protected float mass = 1f;
        [SerializeField] protected float damping = 5f;

        [SerializeField] private float edgeUpForce;
        [SerializeField] private float wallRayCastDistance;
        [SerializeField] private float climbSpeed;

        private int jumpCount = 0;
        private bool isClimbing = false;

        public CharacterController characterController;

        protected float velocityY;
        protected Vector3 currentImpact;

        private readonly float gravity = Physics.gravity.y;

        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        protected override void Update()
        {
            if (!isClimbing)
            {
                TryJump();
                Move();
            }

            if (characterController.isGrounded)
            {
                jumpCount = 0;
            }

            if (Input.GetMouseButtonDown(0))
                weapon.TryShoot(0);
        
            if (Input.GetMouseButtonDown(1))
                weapon.TryShoot(1);
    }
        protected override void TryJump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, wallRayCastDistance))
                {
                    if(hit.collider.GetComponent<Climbable>() != null)
                    {
                        StartCoroutine(Climb(hit.collider));
                        return;
                    }
                }

                if(jumpCount == 0)
                {
                    ResetImpactY();
                    AddForce(Vector3.up, jumpForce);

                    if(characterController.isGrounded)
                    {
                        jumpCount = 1;
                    }
                    else
                    {
                        jumpCount = 2;
                    }
                }
                else if (jumpCount == 1)
                {
                    ResetImpactY();
                    AddForce(Vector3.up, jumpForce);
                    jumpCount = 2;
                }
            }
        }
        private IEnumerator Climb(Collider climbableCollider)
        {
            isClimbing = true;

            while (Input.GetKey(KeyCode.Space))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, wallRayCastDistance))
                {
                    if(hit.collider == climbableCollider)
                    {
                        characterController.Move(new Vector3(0f, climbSpeed * Time.deltaTime, 0f));
                        yield return null;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            ResetImpactY();

            AddForce(Vector3.up, edgeUpForce);

            isClimbing = false;
        }

        public void ResetImpact()
        {
            currentImpact = Vector3.zero;
            velocityY = 0f;
        }

        public void ResetImpactY()
        {
            currentImpact.y = 0f;
            velocityY = 0f;
        }

        protected override void Move()
        {
            Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

            movementInput = transform.TransformDirection(movementInput);

            if (characterController.isGrounded && velocityY < 0f)
            {
                velocityY = 0f;
            }

            velocityY += gravity * Time.deltaTime;

            Vector3 velocity = movementInput * moveSpeed + Vector3.up * velocityY;

            if (currentImpact.magnitude > 0.2f)
            {
                velocity += currentImpact;
            }

            characterController.Move(velocity * Time.deltaTime);

            currentImpact = Vector3.Lerp(currentImpact, Vector3.zero, damping * Time.deltaTime);

        }

        public void AddForce(Vector3 direction, float magnitude)
        {
            currentImpact += direction.normalized * magnitude / mass;
        }
}
