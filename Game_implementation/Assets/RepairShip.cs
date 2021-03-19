using UnityEngine;
using System;
public class RepairShip : MonoBehaviour
{
    public GameObject Sound_repair;

    TextMesh instruction;

    public string text_indication = "Press E to repair SpaceShip";

    public bool IsRepaired;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
    
        player= GameObject.FindWithTag("Player");
        IsRepaired = false;
        Sound_repair = GameObject.Find("Sound_repair");
        var repair_text = GameObject.Find("Machine_indication");
        instruction = repair_text.GetComponent<TextMesh>();
        instruction.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        //Physics.Raycast(transform.position, transform.forward, out var hit, 1f) && //Close enough to Machine
        //hit.transform.gameObject.tag == "Player" &&
        //Vector3.Dot(hit.transform.forward, transform.forward) < -0.5 && // Player looking at the machine
        //Math.Abs(hit.transform.position.y - this.transform.position.y) < 0.5f) //Player not to high
        

        if ( Vector3.Dot(player.transform.forward, transform.forward) < -0.5 && // Player looking at the machine
             Math.Abs(player.transform.position.y - this.transform.position.y) < 0.20f &&
             Math.Abs(player.transform.position.z - this.transform.position.z) < 1.0f
            )
        {
            if (IsRepaired)
                instruction.text = "SpaceShip is repaired \n You can now inverse gravity with f \n Be careful of height ! ";
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
