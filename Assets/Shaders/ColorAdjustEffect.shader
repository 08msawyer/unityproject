Shader "Custom/ColorAdjustEffect"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Brightness ("Brightness", Float) = 1 // Adjust the brightness
        _SATURATION ("Saturation", FLOAT) = 1 // Adjusts saturation
        _Contrast ("Contrast", FLOAT) = 1 // Adjustment Contrast
    }

    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            sampler2D _MainTex;
            half _Brightness;
            half _Saturation;
            half _Contrast;

            // VERT and FRAG functions
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"


            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            // Parameters from Vertex Shader into Pixel Shader
            struct v2f
            {
                float4 pos: sv_position; // vertex location
                half2 uv: texcoord0; // UV coordinate
                half4 color : COLOR;
            };

            //vertex shader
            v2f vert(appdata_t v)
            {
                v2f o;
                // Turn from its own space to projection space
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                // UV coordinate assignments to Output
                o.uv = v.texcoord;
                return o;
            }

            //fragment shader
            fixed4 frag(v2f i) : COLOR
            {
                // Sampling from _maintex according to UV coordinates
                fixed4 renderTex = tex2D(_MainTex, i.uv) * i.color;
                // brigtness brightness directly multiplied by a coefficient, that is, RGB overall zoom, adjusting brightness
                fixed3 finalColor = renderTex * _Brightness;
                // Saturation Saturation: First, according to the equivalent brightness, the same brightness is calculated according to the formula:
                fixed gray = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
                fixed3 grayColor = fixed3(gray, gray, gray);
                // According to the difference between the image and the original image of the Saturation
                finalColor = lerp(grayColor, finalColor, _Saturation);
                // Contrast Contrast: First calculate the lowest value of contrast
                fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
                // Differences between the least contrast and the original image according to Contrast
                finalColor = lerp(avgColor, finalColor, _Contrast);
                // Return the result, the Alpha channel does not change
                return fixed4(finalColor, renderTex.a);
            }
            ENDCG
        }
    }
    // Prevent SHADER failure
    FallBack Off
}