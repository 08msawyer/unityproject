using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Knife.Portal
{
    [DefaultExecutionOrder(80)]
    public class PortalView : MonoBehaviour
    {
        public event UnityAction OnRenderTextureChanged;

        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera portalViewCamera;
        [SerializeField] private PortalViewResolution portalViewResolution = PortalViewResolution.High;
        [SerializeField] private Transform portalRoot1;
        [SerializeField] private Transform portalRoot2;

        private RenderTexture renderTexture;

        public RenderTexture RenderTexture
        {
            get
            {
                return renderTexture;
            }

            private set
            {
                renderTexture = value;
            }
        }

        private void Start()
        {
            if (portalViewCamera.transform.parent != portalRoot2)
            {
                Debug.LogErrorFormat("portalViewCamera {0} must be child of portalRoot2 {1}", portalViewCamera.name, portalRoot2.name);
            }

            CreateRT();
        }

        private void CreateRT()
        {
            if (RenderTexture != null)
                DestroyImmediate(RenderTexture, true);

            int resolution = (int)portalViewResolution;

            RenderTexture = new RenderTexture(resolution, resolution, 32, RenderTextureFormat.ARGB32);

            portalViewCamera.targetTexture = RenderTexture;
            portalViewCamera.enabled = true;
            portalViewCamera.aspect = playerCamera.aspect;
            if (OnRenderTextureChanged != null)
                OnRenderTextureChanged();
        }

        public void StartRendering()
        {
            portalViewCamera.gameObject.SetActive(true);
            portalViewCamera.Render();
        }

        public void StopRendering()
        {
            portalViewCamera.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (this != null)
            {
                if (portalViewCamera != null)
                    portalViewCamera.targetTexture = null;

                if (RenderTexture != null)
                    DestroyImmediate(RenderTexture, true);
            }
        }

        private void Update()
        {
            var localPosition1 = portalRoot1.InverseTransformPoint(playerCamera.transform.position);
            var localRotation1 = Quaternion.Inverse(portalRoot1.rotation) * playerCamera.transform.rotation;

            var worldPosition2 = portalRoot2.TransformPoint(localPosition1);
            var worldRotation2 = portalRoot2.rotation * localRotation1;

            portalViewCamera.transform.position = worldPosition2;
            portalViewCamera.transform.rotation = worldRotation2;
        }

        public enum PortalViewResolution : int
        {
            Low = 256,
            Medium = 512,
            High = 1024,
            VeryHigh = 2048
        }
    }
}