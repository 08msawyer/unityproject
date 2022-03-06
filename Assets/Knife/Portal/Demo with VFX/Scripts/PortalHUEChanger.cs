using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Portal
{
    [DefaultExecutionOrder(100)]
    public class PortalHUEChanger : MonoBehaviour
    {
        [SerializeField] private Renderer[] targets;
        [SerializeField] private string hueParamName = "_HueOffset";
        [SerializeField] [Range(0f, 1f)] private float value = 0;

        private void OnValidate()
        {
            if (targets != null)
            {
                if (Application.isPlaying)
                {
                    foreach (var target in targets)
                    {
                        if (target != null)
                        {
                            foreach (var m in target.materials)
                            {
                                m.SetFloat(hueParamName, value);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var target in targets)
                    {
                        if (target != null)
                        {
                            foreach (var m in target.sharedMaterials)
                            {
                                m.SetFloat(hueParamName, value);
                            }
                        }
                    }
                }
            }
        }
    }
}