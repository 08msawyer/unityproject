using UnityEngine;

public class MoveToWithOffset : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;

    private void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}