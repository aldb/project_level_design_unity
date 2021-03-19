using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform _Target;
    public float _SmoothSpped = 0.125f;
    public Vector3 _Offset;

    void FixedUpdate()
    {
        Vector3 desiredPos = _Target.position + _Offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * _SmoothSpped);
        transform.position = smoothedPos;
    }
}
