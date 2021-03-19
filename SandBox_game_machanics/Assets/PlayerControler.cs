using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    // Déclaration des variables
    public bool Grounded { get; set; }
    Animator Anim { get; set; }
    public Rigidbody Rb { get; set; }
    private bool _IsTouchingLeft;
    private bool _IsTouchingRight;
    private bool _WallJumped;
    private int _NbDoneJumps;
    private RaycastHit _NearObject;
    private bool _IsPushingObject;
    private int AidKitsBonus;
    private bool _LastFrameNearObject;

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    float WallForce = 5f;

    [SerializeField]
    int NbMaxJump = 2;

    [SerializeField]
    float PushSpeedFactor = 0.75f;

    [SerializeField]
    float PullSpeedFactor = 0.5f;

    [SerializeField]
    LayerMask WhatIsGround;

    [SerializeField]
    int nbStaminaJump = 10;

    [SerializeField]
    public GameObject GravityManager;

    // Awake se produit avant le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        Anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody>();
    }

    // Utile pour régler des valeurs aux objets
    void Start()
    {
        Grounded = true;
        _WallJumped = false;
        _IsTouchingLeft = false;
        _IsTouchingRight = false;
        _IsPushingObject = false;
        _NbDoneJumps = 0;
        _LastFrameNearObject = false;
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        AidKitsBonus = (AidKitBar.AidKits);
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        HorizontalMove(horizontal);
        CheckPushPull(horizontal);
        if (!_IsPushingObject)
        {
            CheckJump();
            FlipCharacter();
        }

        if (Input.GetKeyDown(KeyCode.F) && Grounded)
        {
            GravityManager.GetComponent<Gravity>().InverseGravity();
            Grounded = false;
        }
    }

    private void CheckPushPull(float horizontal)
    {
        if (_LastFrameNearObject)
        {
            _NearObject.rigidbody.gameObject.GetComponent<Renderer>().material.color = Color.white;
            _NearObject.rigidbody.velocity = Vector3.zero;
        }

        if (Physics.Raycast(transform.position, transform.forward, out _NearObject, 0.5f) && Grounded && _NearObject.transform.gameObject.tag == "Pushable")
        {
            _LastFrameNearObject = true;
            _IsPushingObject = true;
            Vector3 distance = transform.position - _NearObject.transform.position;
            distance.x = 0;
            _NearObject.rigidbody.gameObject.GetComponent<Renderer>().material.color = new Color(255, 1, 0);
            if (Input.GetKey(KeyCode.C))
            {
                _NearObject.rigidbody.gameObject.GetComponent<Renderer>().material.color = Color.blue;
                if (horizontal < 0 && distance.z < 0 || horizontal > 0 && distance.z > 0) // PULL FROM LEFT OR RIGHT
                {
                    if (distance.z > -0.6f && distance.z < 0.6f) // So we don't get squeez by the object
                        _NearObject.rigidbody.velocity = Vector3.zero;
                    else
                    {
                        Rb.velocity = new Vector3(0, Rb.velocity.y, horizontal * PullSpeedFactor);
                        Anim.SetFloat("MoveSpeed", -Mathf.Abs(Rb.velocity.z));
                        _NearObject.rigidbody.velocity = new Vector3(0, Rb.velocity.y, horizontal * (PullSpeedFactor + 0.03f));
                    }
                }
                else // PUSH FROM LEFT OR RIGHT
                {
                    Rb.velocity = new Vector3(0, Rb.velocity.y, horizontal * PushSpeedFactor);
                    _NearObject.rigidbody.velocity = new Vector3(0, Rb.velocity.y, horizontal * (PushSpeedFactor + 0.03f));
                }
            }
            else
            {
                _IsPushingObject = false;
            }
        }
        else
        {
            _LastFrameNearObject = false;
            _IsPushingObject = false;
        }
    }

    // Gère le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        if (_WallJumped)
            Rb.velocity = new Vector3(0, Rb.velocity.y, Mathf.Clamp(Rb.velocity.z, -WallForce, WallForce) + horizontal * 0.005f);
        else
            Rb.velocity = new Vector3(0, Rb.velocity.y, horizontal);

        Anim.SetFloat("MoveSpeed", Mathf.Abs(Rb.velocity.z));
    }

    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        bool hasJump = false;
        if (Input.GetButtonDown("Jump")&& (StaminaBar.instance == null || StaminaBar.instance != null && StaminaBar.instance.currentStamina>= nbStaminaJump)) // Add can't jump when out of stamina
        {
            if (Grounded)
            {
                float forceDirection = -Mathf.Sign(Physics.gravity.y);  // Gravity may be inversed
                Rb.AddForce(new Vector3(0, forceDirection * JumpForce + AidKitsBonus, 0), ForceMode.Impulse);
                Grounded = false;
                _NbDoneJumps = 1;
                Anim.SetBool("Grounded", false);
                Anim.SetBool("Jump", true);
                hasJump = true;

            }
            else // Wall Jump or double Jump
            {
                _IsTouchingLeft = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z - 0.1f * transform.localScale.z),
                    new Vector3(0.2f, transform.localScale.y - 0.5f, 0.15f * transform.localScale.z), Quaternion.identity, WhatIsGround).Length > 0;

                _IsTouchingRight = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z + 0.1f * transform.localScale.z),
                    new Vector3(0.2f, transform.localScale.y - 0.5f, 0.15f * transform.localScale.z), Quaternion.identity, WhatIsGround).Length > 0;

                if (_IsTouchingLeft || _IsTouchingRight) // Wall Jump
                {
                    hasJump = true;
                    _WallJumped = true;
                    float forceDirection = -Mathf.Sign(Physics.gravity.y);  // Gravity may be inversed
                    Rb.velocity = new Vector3(0f, forceDirection * JumpForce, (_IsTouchingLeft ? 1 : -1) * WallForce);
                }
                else if (_NbDoneJumps < NbMaxJump) // Double Jump
                {
                    hasJump = true;
                    _NbDoneJumps++;
                    float forceDirection = -Mathf.Sign(Physics.gravity.y);  // Gravity may be inversed
                    Rb.AddForce(new Vector3(0, forceDirection * JumpForce, 0), ForceMode.Impulse);
                }
            }
        }

        if (StaminaBar.instance != null && hasJump)
            StaminaBar.instance.UseStamina(nbStaminaJump);
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
    //    //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
    //    Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z - 0.1f * transform.localScale.z),
    //            new Vector3(0.2f, transform.localScale.y - 0.5f, 0.15f * transform.localScale.z));

    //    Gizmos.color = Color.blue;
    //    //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
    //    //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
    //    Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 0.1f * transform.localScale.z),
    //            new Vector3(0.2f, transform.localScale.y - 0.5f, 0.15f * transform.localScale.z));
    //}

    // Gère l'orientation du joueur et les ajustements de la camera
    void FlipCharacter()
    {
        if (Rb.velocity.z < 0)
            transform.eulerAngles = new Vector3(0, 180, transform.eulerAngles.z);  // Left orientation
        else if (Rb.velocity.z > 0)
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);  // Right orientation
    }

    // Collision avec le sol
    void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            float normal_y = -Mathf.Sign(Physics.gravity.y) * contact.normal.y;
            if (normal_y > 0.5f)
            {
                Grounded = true;
                _WallJumped = false;
                _NbDoneJumps = 0;
                Anim.SetBool("Grounded", Grounded);
                break;
            }
        }
    }
}
