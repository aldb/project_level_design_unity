using UnityEngine;
using System;
public class EnemisBehaviourScript : MonoBehaviour
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private float walkTarget;
    private GameObject player;

    public Transform Target;
    // Déclaration des variables
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }
    bool _Flipped { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeedWalk = 3.0f;

    [SerializeField]
    float MoveSpeedRun = 5.0f;

    [SerializeField]
    //How far the player need to be for the enemeis to pursue him
    float PursueDistance = 10.0f;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        _Anim.SetBool("Running", false);   
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.layer == 10 || collision.gameObject.layer == 9)
        {
            transform.Rotate(FlipRotation);
        }
        if (collision.gameObject.layer == 12)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

        //To overcome the problem when gravity change
        transform.position = new Vector3(0f, transform.position.y, transform.position.z);

        if (Math.Abs(Vector3.Dot(_Rb.velocity, -Vector3.up)) < 0.001f && player.transform.position.y>20.40 && player.transform.position.z > 20.40)
        {
            if (Vector3.Distance(Target.position, transform.position) < PursueDistance + 1 && Physics.Raycast(transform.position, transform.forward, out var hit, PursueDistance) && hit.transform.gameObject.tag == "Player")
            {
                _Anim.SetBool("Walking", false);
                _Anim.SetBool("Running", true);
                transform.position += transform.forward * MoveSpeedRun * Time.deltaTime;
            }
            else
            {   //The enemis walk in the zone 
                _Anim.SetBool("Running", false);
                _Anim.SetBool("Walking", true);
                transform.position += transform.forward * MoveSpeedWalk * Time.deltaTime;
            }
        }
        else 
        {
            _Anim.SetBool("Running", false);
            _Anim.SetBool("Walking", false);
        }
    }

}
