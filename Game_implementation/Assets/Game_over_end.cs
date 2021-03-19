using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Game_over_end : MonoBehaviour
{
    public GameObject text_over;
    public GameObject end_button;

    // Start is called before the first frame update
    void Start()
    {
        text_over = GameObject.Find("GameOver");
        end_button = GameObject.Find("GameOver_button");
        Text bordel = text_over.GetComponent<Text>();
        bordel.text = "";
        end_button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindWithTag("Player") == null)
        {
            if (text_over.GetComponent<Text>().text ==  "")
            {
                Text bordel = text_over.GetComponent<Text>();
                bordel.text = "Game Over";
            }
            end_button.SetActive(true);
        } 
    }

    public void Back_to_menu()
    {
        Application.LoadLevel("StartGame");

    }
}
