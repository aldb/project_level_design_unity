using UnityEngine;

public class AidKitScript : MonoBehaviour
{
    public GameObject text;
    private int count;
    void Start()
    {
        count = 0;
        text.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {

            if (count == 0)
            {
                count++;
                AidKitBar.CollectKit();
                text.SetActive(true);
                Destroy(gameObject);
                Destroy(text, 4);
            }

        }
    }


}
