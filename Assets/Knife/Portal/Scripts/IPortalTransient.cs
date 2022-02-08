using UnityEngine;

namespace Knife.Portal
{
    public interface IPortalTransient
    {
        void Teleport(Vector3 position, Quaternion rotation, Transform entry, Transform exit);
        bool UseThreshold { get; }
        Vector3 Position { get; }
    }
}