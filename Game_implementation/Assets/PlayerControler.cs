using UnityEngine;
using UnityEngine.SceneManagement; //So you can use SceneManager
using UnityEngine.UI;


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
    private Quaternion InitialRotation;

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
    public GameObject GravityManager;

    [SerializeField]
    public GameObject conso;

    public GameObject gameover;


    int nbStaminaJump = 10;
    // Awake se produit avant le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        Anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody>();
        InitialRotation = Rb.rotation;
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
        Rb.freezeRotation = true;
        gameover = GameObject.Find("GameOver");
        conso = GameObject.Find("Console_réparation");
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        if (transform.position.z < -16 && transform.position.y > 20) //Fin du jeu
        {
           
            Text bordel = gameover.GetComponent<Text>();
            bordel.text = "Niveau terminé !";
            Destroy(gameObject);

        }
        CheckGround();
        AidKitsBonus = (AidKitBar.AidKits);
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        HorizontalMove(horizontal);
        CheckPushPull(horizontal);
        if (!_IsPushingObject)
        {
            CheckJump();
            FlipCharacter();
        }
        RepairShip Repair = conso.GetComponent<RepairShip>();
        if (Input.GetKeyDown(KeyCode.F) && Grounded && Repair.IsRepaired)
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
                    if (distance.z > -1.2f && distance.z < 1.2f) // So we don't get squeez by the object
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

        Rb.position = new Vector3(0, Rb.position.y, Rb.position.z);
        Anim.SetFloat("MoveSpeed", Mathf.Abs(Rb.velocity.z));
    }

    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        bool hasJump = false;
        if (_NbDoneJumps == 2)
        {
            var animInfo = Anim.GetCurrentAnimatorStateInfo(0);
            if (animInfo.IsName("Midair") || animInfo.IsName("Land"))
                Anim.SetBool("DoubleJump", false);
        }
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
            else // Wall Jump or double Jump or Free fall
            {
                float grav = Mathf.Sign(Physics.gravity.y);
                _IsTouchingLeft = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y - grav * 0.47f, transform.position.z - 0.2f),
                new Vector3(0.2f, transform.localScale.y - transform.localScale.y * 0.90f, 0.05f * transform.localScale.z), Quaternion.identity, WhatIsGround).Length > 0;

                _IsTouchingRight = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y - grav * 0.47f, transform.position.z + 0.1f),
                new Vector3(0.2f, transform.localScale.y - transform.localScale.y * 0.90f, 0.05f * transform.localScale.z), Quaternion.identity, WhatIsGround).Length > 0;

                if (AidKitsBonus > 1 && (_IsTouchingLeft || _IsTouchingRight)) // Wall Jump
                {
                    hasJump = true;
                    _WallJumped = true;
                    float forceDirection = -Mathf.Sign(Physics.gravity.y);  // Gravity may be inversed
                    Rb.velocity = new Vector3(0f, forceDirection * JumpForce, (_IsTouchingLeft ? 1 : -1) * WallForce);
                }
                else if (AidKitsBonus > 0 && (_NbDoneJumps < NbMaxJump)) // Double Jump
                {
                    Rb.freezeRotation = false;
                    Anim.SetBool("DoubleJump", true);
                    hasJump = true;
                    _NbDoneJumps = NbMaxJump;
                    float forceDirection = -Mathf.Sign(Physics.gravity.y);  // Gravity may be inversed
                    Rb.AddForce(new Vector3(0, forceDirection *0.7f*JumpForce, 0), ForceMode.Impulse);
                }
            }
        }

        if (StaminaBar.instance != null && hasJump)
            StaminaBar.instance.UseStamina(nbStaminaJump);
    }

    void CheckGround()
    {
        float sign = Mathf.Sign(Physics.gravity.y);
        Grounded = Physics.Raycast(transform.position, sign*Vector3.up, 0.5f); // 0.5 d'offset
        Anim.SetBool("Grounded", Grounded);
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

        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 11)
        {
            Life other = (Life)this.GetComponent(typeof(Life));
            other.Take_Damage(100);
        }

        foreach (var contact in collision.contacts)
        {
            float normal_y = -Mathf.Sign(Physics.gravity.y) * contact.normal.y;
            if (normal_y > 0.5f)
            {
                Grounded = true;
                _WallJumped = false;
                _NbDoneJumps = 0;
                Anim.SetBool("Grounded", Grounded);
                Anim.SetBool("DoubleJump", false);
                Rb.freezeRotation = true;
                break;
            }
        }
    }
}
