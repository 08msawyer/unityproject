using UnityEngine;

/// <summary>
/// Constantly moves this game object to the [target], but with an offset of [offset] applied.
/// </summary>
public class MoveToWithOffset : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;

    /// <summary>
    /// Called every frame. Moves this object to the [target] with an [offset].
    /// </summary>
    private void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}