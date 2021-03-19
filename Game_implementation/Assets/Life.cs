using UnityEngine;
using UnityEngine.SceneManagement; //So you can use SceneManager
using UnityEngine.UI;
using System;

[RequireComponent(typeof(PlayerControler))]
public class Life : MonoBehaviour
{

    public PlayerControler Player;
    public GameObject life_slider;
    public Slider slider;
    public GameObject sound_dmg;


    public float speed_y; // Y speed of the player
    public float life;
    private bool Is_Grounded;

    // Start is called before the first frame update
    void Start()
    {
        life_slider = GameObject.Find("life_slider");
        sound_dmg = GameObject.Find("Dmg_sound");
        slider = life_slider.GetComponent<Slider>();
        Player = GetComponent<PlayerControler>();
        life = 100f; //100 pv au début
        speed_y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.Fall_Damage();
        slider.value = life;

        if (life <= 0)
        {
            Destroy(gameObject);
        }

    }

    public void Take_Damage(float dmg) //Fonction pour gérer l'affichage des dégats subit 
    {
        life -= dmg;
        sound_dmg.GetComponent<AudioSource>().Play();
    }

    void Fall_Damage()
    {
        Is_Grounded = Player.Grounded;


        if (Is_Grounded == false)
        {
            speed_y = Vector3.Dot(Player.Rb.velocity, -Vector3.up);

        }
        else
        {
            if (speed_y != 0)
            {
                if (Math.Abs(speed_y) > 12) //Il faut appliquer les dégats
                {
                    Take_Damage(4 * (Math.Abs(speed_y) - 10)* (Math.Abs(speed_y) - 10));
                }
                speed_y = 0;
            }
        }


    }


}
