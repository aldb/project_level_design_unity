using UnityEngine;

public class EnemisBehaviourScript : MonoBehaviour
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private float walkTarget;

    public Transform Target;
    // Déclaration des variables
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }
    bool _Flipped { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    //How far the enemis can go left or right 
    [SerializeField]
    float MaxZ = 13.0f;
    [SerializeField]
    float MinZ = 6.0f;


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
        walkTarget = MaxZ;
        _Anim.SetBool("Running", false);
        _Flipped = false;
    }

    void makeFacing(float TargetPosition)

    {
        if (!_Flipped && TargetPosition < transform.position.z - 1)
        {
            transform.Rotate(FlipRotation);
            _Flipped = true;

        }

        UnityEngine.Debug.Log("Target:" + Target.position.z);
        if (_Flipped && TargetPosition > transform.position.z + 1)
        {
            transform.Rotate(-FlipRotation);
            _Flipped = false;

        }
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log(transform.position);
        
        if (Vector3.Distance(Target.position, transform.position) < PursueDistance+1 && Physics.Raycast(transform.position, transform.forward, out var hit, PursueDistance) && hit.transform.gameObject.tag == "Player" )
        {
            _Anim.SetBool("Walking", false);
            UnityEngine.Debug.Log("La distance entre les deux personnage est inf a pursueDist");
            makeFacing(Target.position.z);

            if ((transform.position + transform.forward * MoveSpeed * Time.deltaTime).z < MaxZ && (transform.position + transform.forward * MoveSpeed * Time.deltaTime).z > MinZ)
            {
                _Anim.SetBool("Running", true);
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;

            }
            else
            {
                _Anim.SetBool("Running", false);
            }
        }
        else
        {   //The enemis walk in is zone 
            _Anim.SetBool("Running", false);
            _Anim.SetBool("Walking", true);

            if (System.Math.Abs(walkTarget - transform.position.z) > 1)
            {
                //UnityEngine.Debug.Log("Rejoint la position initiale");
                makeFacing(walkTarget);
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;
            }
            else
            {
                if (walkTarget == MaxZ)
                {
                    walkTarget = MinZ;
                }
                else { walkTarget = MaxZ; }

                _Anim.SetBool("Walking", false);

            }

        }

    }



    //Animation 


}
