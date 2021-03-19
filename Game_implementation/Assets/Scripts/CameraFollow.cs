using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform _Target;
    public float _SmoothSpped = 0.125f;
    public Vector3 _Offset;

    void FixedUpdate()
    {
        float gravitySign = Mathf.Sign(Physics.gravity.y);
        Vector3 fixedOffset = _Offset;
        fixedOffset.y *= -gravitySign;
        Vector3 desiredPos = _Target.position + fixedOffset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * _SmoothSpped);
        transform.position = smoothedPos;
    }
}
