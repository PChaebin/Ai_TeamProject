using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;

        [Header("moving")]
        public bool notMove = true;

        public Transform spotlightTransform;

        private Animator animator;
        private Rigidbody2D rb;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            notMove = true;
        }

        public void SetMove(bool set)
        {
            if (!set)
            {
                this.transform.position = new Vector3(0, 30, 0);
            }
            notMove = set;
        }

        public bool GetNotMove()
        {
            return notMove;
        }

        private void Update()
        {
            if (notMove)
            {
                this.transform.position = new Vector3 (25, -25, 0);
                return;
            }
            Vector2 dir = Vector2.zero;

            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2);
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0);
            }

            dir.Normalize();
            animator.SetBool("IsMoving", dir.sqrMagnitude > 0);

            rb.velocity = dir * speed;

            if (dir.x > 0f && dir.y > 0f)
            {
                spotlightTransform.localEulerAngles = new Vector3(0, 0, -45f);
            }
            else if (dir.x > 0f && dir.y < 0f)
            {
                spotlightTransform.localEulerAngles = new Vector3(0, 0, -135f);
            }
            else if (dir.x < 0f && dir.y > 0f)
            {
                spotlightTransform.localEulerAngles = new Vector3(0, 0, 45f);
            }
            else if (dir.x < 0f && dir.y < 0f)
            {
                spotlightTransform.localEulerAngles = new Vector3(0, 0, 135f);
            }
            else if (dir.x > 0f)
            {
                spotlightTransform.localEulerAngles = new Vector3(0, 0, -90f);
            }
            else if (dir.x < 0f)
            {
                spotlightTransform.localEulerAngles = new Vector3(0, 0, 90f);
            }
            else if (dir.y > 0f)
            {
                spotlightTransform.localEulerAngles = new Vector3(0, 0, 0f);
            }
            else if (dir.y < 0f)
            {
                spotlightTransform.localEulerAngles = new Vector3(0, 0, 180f);
            }
        }
    }
}
