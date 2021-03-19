using UnityEngine;

public class ToolPickUp : MonoBehaviour
{


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            ToolPickUpBar.CollectTool();
            Destroy(gameObject);
        }
    }


}
