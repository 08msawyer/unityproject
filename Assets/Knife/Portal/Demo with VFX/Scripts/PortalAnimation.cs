using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Portal
{
    public class PortalAnimation : MonoBehaviour
    {
        [SerializeField] private Animator portalAnimator;
        [SerializeField] private string openPortalAnimation = "OpenPortal";
        [SerializeField] private string closePortalAnimation = "ClosePortal";

        [SerializeField] private PortalTransition portalTransition;

        private void Start()
        {
            if(portalTransition.IsPortalOpened)
            {
                portalAnimator.Play(openPortalAnimation, 0, 1);
            }
            else
            {
                portalAnimator.Play(closePortalAnimation, 0, 1);
            }

            portalTransition.OnPortalOpen += OnPortalOpen;
            portalTransition.OnPortalClose += OnPortalClose;
        }

        private void OnPortalClose()
        {
            portalAnimator.Play(closePortalAnimation);
        }

        private void OnPortalOpen()
        {
            portalAnimator.Play(openPortalAnimation);
        }
    }
}