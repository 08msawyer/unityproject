using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Portal
{
    public class PortalPhysicsAffector : MonoBehaviour
    {
        [SerializeField] private LayerMask physicsMask = ~0;
        [SerializeField] private float radius = 5f;
        [SerializeField] private float force = 100f;
        [SerializeField] private AnimationCurve distanceCurve;
        [SerializeField] private PortalTransition portalTransition;
        [SerializeField] private bool applyForceToClosestPointOnCollider = false;

        private void Start()
        {
            portalTransition.OnPortalOpen += OnPortalOpen;
            portalTransition.OnPortalClose += OnPortalClose;
        }

        private void OnPortalClose()
        {
            AffectBodiesInRadius(true);
        }

        private void OnPortalOpen()
        {
            AffectBodiesInRadius(false);
        }

        private void AffectBodiesInRadius(bool attract)
        {
            var colliders = Physics.OverlapSphere(transform.position, radius, physicsMask);

            foreach (var c in colliders)
            {
                if (c.attachedRigidbody != null)
                {
                    Affect(c.attachedRigidbody, c, attract);
                }
            }
        }

        private void Affect(Rigidbody body, Collider collider, bool attract)
        {
            var direction = body.position - transform.position;
            float distance = direction.magnitude;

            float fraction = distance / radius;

            float affectValue = distanceCurve.Evaluate(fraction);

            float sign = attract ? -1 : 1;

            if (applyForceToClosestPointOnCollider)
            {
                var point = collider.ClosestPoint(transform.position);

                body.AddForceAtPosition(direction.normalized * force * affectValue * sign, point);
            }
            else
            {
                body.AddForce(direction.normalized * force * affectValue * sign);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}