using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    void LateUpdate()
    {
        if (target) transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }
}

