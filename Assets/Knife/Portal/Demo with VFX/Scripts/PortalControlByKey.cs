using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Portal
{
    public class PortalControlByKey : MonoBehaviour
    {
        [SerializeField] private KeyCode openKey;
        [SerializeField] private KeyCode closeKey;
        [SerializeField] private PortalTransition[] portalTransitions;

        private void Update()
        {
            if (Input.GetKeyDown(openKey))
            {
                foreach (var p in portalTransitions)
                {
                    p.OpenPortal();
                }
            }
            if (Input.GetKeyDown(closeKey))
            {
                foreach (var p in portalTransitions)
                {
                    p.ClosePortal();
                }
            }
        }
    }
}