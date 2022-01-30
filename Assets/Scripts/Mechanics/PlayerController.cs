using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        public Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        // bullet
        public bool touchShoot;
        public bool isShooting;
        public bool lookingRight; 
        
        public GameObject bullet;
        public float bulletSpeed;
        public float bulletCD;
        public float bulletHeightOffset;
        private float previousShootingTime = 0;

        // bouncing
        public bool isBouncing;

        // touch screen move
        public bool isTouchScreenMove;

        // is keyboard
        public bool isKeyboard;

        public bool isRed;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            isBouncing = false;
            isTouchScreenMove = false;
            touchShoot = false;
            isKeyboard = false;
        }

        protected override void Update()
        {
            if (Input.GetKey(KeyCode.Z) || touchShoot)
            {
                isShooting = true;
            } else {
                isShooting = false;
            }

            if (controlEnabled)
            {
                
                if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01)
                {
                    move.x = Input.GetAxis("Horizontal");
                } else {
                    move.x = 0;
                }
            
                if (move.x > 0.01) {
                    lookingRight = true;
                } else if (move.x < -0.01) {
                    lookingRight = false;
                }

                // bullet
                // if (isShooting && isRed)
                // {
                //     if (Time.time - previousShootingTime > bulletCD)
                //     {
                //         previousShootingTime = Time.time;
                //         GameObject newbullet = Instantiate(bullet);
                //         newbullet.transform.position = this.transform.position + new Vector3(0, bulletHeightOffset, 0);
                //         float direction = lookingRight ? 1 : -1;
                //         newbullet.GetComponent<Rigidbody2D>().velocity = new Vector2(direction * bulletSpeed, 0);
                //     }
                // }

                // button jumb
                if ((isBouncing && jumpState == JumpState.Grounded))
                {
                    jumpState = JumpState.PrepareToJump;
                } else if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump")){
                    jumpState = JumpState.PrepareToJump;
                    isKeyboard = true;
                } else if ((!isBouncing && !isKeyboard) || Input.GetButtonUp("Jump")) {
                    stopJump = true;
                    isKeyboard = false;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}