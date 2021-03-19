using UnityEngine;
using UnityEngine.UI;

public class AidKitBar : MonoBehaviour
{

    public static AidKitBar instance;
    public static int AidKits = 0;
    public Text TotalAidKits;
    // Start is called before the first frame update
    void Start()
    {
        TotalAidKits = GetComponent<Text>();

    }
    public static void CollectKit() { AidKits += 1; }
    // Update is called once per frame
    void Update()
    {
        TotalAidKits.text = "Aid Kits: " + AidKits;
    }
}
