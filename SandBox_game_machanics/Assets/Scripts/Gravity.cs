using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float _GravityScale = 1;
    public float _RotationSpeed = 7f;
    public List<GameObject> _InverseObjects = new List<GameObject>();
    private List<(GameObject, float)> ToRotate = new List<(GameObject, float)>();

    // Start is called before the first frame update
    void Start()
    {
        Vector3 gravityForce = Physics.gravity;
        gravityForce.y *= _GravityScale;
        Physics.gravity = gravityForce;
    }

    // Inverse the gravity if needed
    // Rotate all needed objects
    void Update()
    {
        if (ToRotate.Count > 0)
        {
            // Some objects need to be rotated
            List<int> removeFromRotate = new List<int>();
            for (int i = 0; i < ToRotate.Count; i++)
            {
                (GameObject o, float target) = ToRotate[i];
                Vector3 eulerTarget = new Vector3(o.transform.rotation.eulerAngles.x,
                                                  o.transform.rotation.eulerAngles.y,
                                                  target);  // Only changes the z orientation
                o.transform.eulerAngles = Vector3.Lerp(o.transform.rotation.eulerAngles,
                                                       eulerTarget,
                                                       Time.deltaTime * _RotationSpeed);

                if (Vector3.Distance(o.transform.rotation.eulerAngles, eulerTarget) <= 0.01f)
                {
                    removeFromRotate.Add(i);
                    o.transform.eulerAngles = eulerTarget;
                }
            }

            // Remove objects that are fully rotated
            for (int i = removeFromRotate.Count - 1; i >= 0; i--)  // Backward to not mess with the index numbers
                ToRotate.RemoveAt(removeFromRotate[i]);
        }
    }

    public void InverseGravity()
    {
        // Inverse gravity
        Vector3 gravityForce = Physics.gravity;
        gravityForce.y *= -1;
        Physics.gravity = gravityForce;

        transform.Rotate(180, 180, 0);
        float target = transform.rotation.eulerAngles.z;
        ToRotate.Clear();

        // Rotate each objects
        foreach (GameObject o in _InverseObjects)
            ToRotate.Add((o, target));
    }

    // Objects can also be added from the unity scene
    public void AddInverseObject(GameObject o)
    {
        _InverseObjects.Add(o);
    }
}
