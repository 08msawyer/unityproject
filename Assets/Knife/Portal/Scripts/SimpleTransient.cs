using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Portal
{
    public class SimpleTransient : MonoBehaviour, IPortalTransient
    {
        [SerializeField] private bool useThreshold = false;
        [SerializeField] private bool rigidBody = false;
        [SerializeField] private TeleportOriginType teleportOriginType;

        public bool UseThreshold
        {
            get
            {
                return useThreshold;
            }
        }

        public Vector3 Position
        {
            get
            {
                switch(teleportOriginType)
                {
                    case TeleportOriginType.TransformPivot:
                        return transform.position;
                    case TeleportOriginType.RigibodyCenterOfMass:
                        return body.worldCenterOfMass;
                    case TeleportOriginType.RendererBoundsCenter:
                        return attachedRenderer.bounds.center;
                    default:
                        return transform.position;
                }
            }
        }

        private Rigidbody body;
        private Renderer attachedRenderer;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
            attachedRenderer = GetComponentInChildren<Renderer>();
        }

        public void Teleport(Vector3 position, Quaternion rotation, Transform entry, Transform exit)
        {
            Vector3 offset = transform.position - Position;
            transform.position = position - offset;
            transform.rotation = rotation;

            if(rigidBody)
            {
                var velocity = body.velocity;
                var angularVelocity = body.angularVelocity;

                var localVelocity1 = entry.InverseTransformVector(velocity);
                localVelocity1.x *= -1;
                localVelocity1.z *= -1;
                var localAngularVelocity1 = entry.InverseTransformVector(angularVelocity);

                var worldVelocity2 = exit.TransformVector(localVelocity1);
                var worldAngularVelocity2 = exit.TransformVector(localAngularVelocity1);

                body.velocity = worldVelocity2;
                body.angularVelocity = worldAngularVelocity2;
            }
        }

        public enum TeleportOriginType
        {
            TransformPivot,
            RigibodyCenterOfMass,
            RendererBoundsCenter
        }
    }
}