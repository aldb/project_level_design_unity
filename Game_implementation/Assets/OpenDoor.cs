using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject Door;
    public GameObject Sound_secret;
    public GameObject Button_Indicator;

    GameObject Player;
    TextMesh instruction;

    public bool door_is_opening;
    bool door_is_opened = false;

    public string text_indication = "Press E to open the door";
    public float Display_distance = 2f;
    public float opening_size = 1.5f;
    float initial_y_door;


    // Start is called before the first frame update
    void Start()
    {
        Sound_secret = GameObject.Find("Sound_secret");
        Door = GameObject.Find("Door");
        initial_y_door = Door.transform.position.y;
        Button_Indicator = GameObject.Find("Button_indication");
        instruction = Button_Indicator.GetComponent<TextMesh>();
        instruction.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        Player = GameObject.FindWithTag("Player");
        if (!door_is_opened && !door_is_opening &&  //Not already opened or Opening
            Vector3.Distance(this.transform.position, Player.transform.position) < Display_distance && //Close enough to Button
            this.transform.position.z > Player.transform.position.z && //Player not behind the button 
            Player.transform.position.y - this.transform.position.y < 0.5f) //Player not to high 
        {
            instruction.text = text_indication;  //We print the text indication
            if (Input.GetKeyDown(KeyCode.E))
            { //If e is pr
                door_is_opening = true;
                Sound_secret.GetComponent<AudioSource>().Play();
                GetComponent<MeshRenderer>().enabled = false;
            }

        }
        else
        {
            instruction.text = "";
        }

        if (Door.transform.position.y - initial_y_door > opening_size) //Check if door is open enough
        {
            door_is_opening = false;
            door_is_opened = true;
        }
        if (door_is_opening == true)
        { //Keep opening the door

            Door.transform.Translate(Vector3.up * Time.deltaTime);
        }

    }

}
