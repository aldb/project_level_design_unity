using UnityEngine;

public class AidKitScript : MonoBehaviour
{

    private int count;
    void Start()
    {
        count = 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {

            if (count == 0)
            {
                count++;
                AidKitBar.CollectKit();
                Destroy(gameObject);
            }

        }
    }


}
