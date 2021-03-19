using UnityEngine;

public class RepairShip : MonoBehaviour
{
    public GameObject Sound_repair;

    TextMesh instruction;

    public string text_indication = "Press E to repair SpaceShip";
    public float Display_distance = 1f;

    private bool IsRepaired;


    // Start is called before the first frame update
    void Start()
    {
        IsRepaired = false;
        Sound_repair = GameObject.Find("Sound_repair");
        var repair_text = GameObject.Find("Machine_indication");
        instruction = repair_text.GetComponent<TextMesh>();
        instruction.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, 1f) && //Close enough to Machine
            hit.transform.gameObject.tag == "Player" &&
            Vector3.Dot(hit.transform.forward, transform.forward) < -0.5 && // Player looking at the machine
            hit.transform.position.y - this.transform.position.y < 0.5f) //Player not to high
        {
            if (IsRepaired)
                instruction.text = "SpaceShip is repaired";
            else
            {
                if (ToolPickUpBar.NbToolPickedUp >= 1)
                {
                    instruction.text = text_indication;  //We print the text indication
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ToolPickUpBar.NbToolPickedUp--;
                        Sound_repair.GetComponent<AudioSource>().Play();
                        IsRepaired = true;
                    }
                }
                else
                    instruction.text = "Not enough tool to repair SpaceShip.";
            }
        }
        else
        {
            instruction.text = "";
        }
    }

}
