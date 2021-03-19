using UnityEngine;
using UnityEngine.UI;

public class AidKitBar : MonoBehaviour
{
    public static int AidKits = 0;
    // Start is called before the first frame update
    void Start()
    {
        AidKits = 0;
    }
    public static void CollectKit() { AidKits += 1; }
    // Update is called once per frame
    void Update()
    {
    }
}
