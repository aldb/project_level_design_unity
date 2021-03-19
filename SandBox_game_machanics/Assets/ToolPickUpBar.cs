using UnityEngine;
using UnityEngine.UI;

public class ToolPickUpBar : MonoBehaviour
{
    public static int NbToolPickedUp = 0;
    public Text TextTool;
    // Start is called before the first frame update
    void Start()
    {
        TextTool = GetComponent<Text>();
    }

    public static void CollectTool() { NbToolPickedUp += 1; }
    // Update is called once per frame
    void Update()
    {
        TextTool.text = "Tools: " + NbToolPickedUp;
    }
}
