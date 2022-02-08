using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Portal
{
    public class PortalOptimization : MonoBehaviour
    {
        [SerializeField] private float cullRadius = 3f;
        [SerializeField] private PortalView portalView;
        [SerializeField] private PortalTransition portalTransition;
        [SerializeField] private Camera cullCamera;
        [SerializeField] private bool frustumCulling;

        private CullingGroup cullingGroup;
        private bool isVisible;

        private void OnEnable()
        {
            if (cullCamera == null)
            {
                Debug.LogWarning("cullCamera is null, so MainCamera (with tag MainCamera) will be used as cullCamera");
                cullCamera = Camera.main;
            }

            cullingGroup = new CullingGroup();
            var boundingSpheresArray = new BoundingSphere[1];
            boundingSpheresArray[0] = new BoundingSphere(transform.position, cullRadius);
            cullingGroup.SetBoundingSpheres(boundingSpheresArray);
            cullingGroup.SetBoundingSphereCount(1);
            cullingGroup.targetCamera = cullCamera;

            cullingGroup.onStateChanged += OnStateChanged;
            portalTransition.OnPortalOpen += OnPortalOpen;
            portalTransition.OnPortalClose += OnPortalClose;
            if(!portalTransition.IsPortalOpened)
                portalView.StopRendering();
        }

        private void OnPortalClose()
        {
            portalView.StopRendering();
        }

        private void OnPortalOpen()
        {
            if(isVisible || !frustumCulling)
                portalView.StartRendering();
        }

        private void OnStateChanged(CullingGroupEvent sphere)
        {
            if (!frustumCulling)
                return;

            if (sphere.hasBecomeInvisible)
            {
                portalView.StopRendering();
                isVisible = false;
            }
            else if (sphere.hasBecomeVisible)
            {
                if (portalTransition.IsPortalOpened)
                {
                    portalView.StartRendering();
                }
                isVisible = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, cullRadius);
        }

        private void OnDisable()
        {
            cullingGroup.onStateChanged -= OnStateChanged;
            portalTransition.OnPortalOpen -= OnPortalOpen;
            portalTransition.OnPortalOpen -= OnPortalClose;
            cullingGroup.Dispose();
        }
    }
}