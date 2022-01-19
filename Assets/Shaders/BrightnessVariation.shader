Shader "Custom/BrightnessVariation"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 normal;
            float3 color;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float nrand1(float2 uv)
        {
            // return frac(uv.x * uv.y);
            return frac(sin(dot(uv, float2(12, 78))));
            return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
        }

        float nrand(float2 uv)
        {
            return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
        }

        float nrand2(float2 uv)
        {
            return frac(sin(dot(uv, float2(102.4, 29.233))) * 51254.5453);
        }

        float nrand3(float2 uv)
        {
            return frac(sin(dot(uv, float2(120.2193, 98.10))) * 30482.5453);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 normal = WorldNormalVector(IN, o.Normal);
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            float x = normal.x;
            float y = normal.y;
            float z = normal.z;
            o.Albedo = _Color * float3(nrand1(float2(x, y)), nrand1(float2(x, z)), nrand1(float2(y, z)));

            // o.Albedo = IN.color;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}