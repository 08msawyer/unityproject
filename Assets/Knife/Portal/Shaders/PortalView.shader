// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Knife/PortalView"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset][Normal]_DistortionMap1("DistortionMap1", 2D) = "bump" {}
		[NoScaleOffset][Normal]_DistortionMap2("DistortionMap2", 2D) = "bump" {}
		_DistortionTiling1("DistortionTiling1", Float) = 1
		_DistortionTiling2("DistortionTiling2", Float) = 1
		_DistortionAmount1("DistortionAmount1", Float) = 1
		_DistortionAmount2("DistortionAmount2", Float) = 1
		_DistrotionSpeed1("DistrotionSpeed1", Vector) = (0,0,0,0)
		_DistrotionSpeed2("DistrotionSpeed2", Vector) = (0,0,0,0)
		_TotalDistortionAmount("TotalDistortionAmount", Float) = 0
		_DistrotionDistanceSoftness("DistrotionDistanceSoftness", Range( 0 , 1)) = 0
		_DistrotionDistanceMul("DistrotionDistanceMul", Float) = 0
		_OpacityMap("OpacityMap", 2D) = "white" {}
		_Opacity("Opacity", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float _DistortionAmount1;
		uniform sampler2D _DistortionMap1;
		uniform float2 _DistrotionSpeed1;
		uniform float _DistortionTiling1;
		uniform float _DistortionAmount2;
		uniform sampler2D _DistortionMap2;
		uniform float2 _DistrotionSpeed2;
		uniform float _DistortionTiling2;
		uniform float _TotalDistortionAmount;
		uniform float _DistrotionDistanceSoftness;
		uniform float _DistrotionDistanceMul;
		uniform sampler2D _OpacityMap;
		uniform float4 _OpacityMap_ST;
		uniform float _Opacity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float2 temp_output_27_0 = (ase_vertex3Pos).xz;
			float2 panner9 = ( 1.0 * _Time.y * _DistrotionSpeed1 + ( temp_output_27_0 * _DistortionTiling1 ));
			float2 panner16 = ( 1.0 * _Time.y * _DistrotionSpeed2 + ( temp_output_27_0 * _DistortionTiling2 ));
			float clampResult41 = clamp( ( ase_screenPosNorm.z * _DistrotionDistanceMul ) , 0.0 , 1.0 );
			float smoothstepResult45 = smoothstep( 0.0 , _DistrotionDistanceSoftness , clampResult41);
			o.Emission = tex2D( _MainTex, ( (ase_screenPosNorm).xy + ( ( (UnpackScaleNormal( tex2D( _DistortionMap1, panner9 ), _DistortionAmount1 )).xy + (UnpackScaleNormal( tex2D( _DistortionMap2, panner16 ), _DistortionAmount2 )).xy ) * _TotalDistortionAmount * ( 1.0 - smoothstepResult45 ) ) ) ).rgb;
			float2 uv_OpacityMap = i.uv_texcoord * _OpacityMap_ST.xy + _OpacityMap_ST.zw;
			o.Alpha = ( tex2D( _OpacityMap, uv_OpacityMap ).a * _Opacity );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
-1885;7;1839;1004;1229.383;277.3325;1;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;25;-3856.358,196.0717;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-3236.258,375.8717;Inherit;False;Property;_DistortionTiling1;DistortionTiling1;3;0;Create;True;0;0;False;0;1;0.12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;27;-3626.358,262.0717;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-3558.358,826.3217;Inherit;False;Property;_DistortionTiling2;DistortionTiling2;4;0;Create;True;0;0;False;0;1;0.08;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-2744.358,233.0717;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;17;-2931.358,504.0717;Inherit;False;Property;_DistrotionSpeed1;DistrotionSpeed1;7;0;Create;True;0;0;False;0;0,0;0.05,0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-3057.358,755.0217;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-1943.448,1132.923;Inherit;False;Property;_DistrotionDistanceMul;DistrotionDistanceMul;11;0;Create;True;0;0;False;0;0;7.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;39;-2118.047,958.8126;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;19;-2979.358,851.0717;Inherit;False;Property;_DistrotionSpeed2;DistrotionSpeed2;8;0;Create;True;0;0;False;0;0,0;-0.05,-0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1611.448,999.923;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-2415.603,426.2728;Inherit;False;Property;_DistortionAmount1;DistortionAmount1;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-2395.603,706.2728;Inherit;False;Property;_DistortionAmount2;DistortionAmount2;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;16;-2644.603,650.7728;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;9;-2663.603,392.2728;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;13;-2103.603,569.2728;Inherit;True;Property;_DistortionMap2;DistortionMap2;2;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;-1;None;93f81d0181f84754c855d4a5ce4f1d7a;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-2073.603,332.2728;Inherit;True;Property;_DistortionMap1;DistortionMap1;1;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;-1;None;93f81d0181f84754c855d4a5ce4f1d7a;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;44;-1632.448,1197.923;Inherit;False;Property;_DistrotionDistanceSoftness;DistrotionDistanceSoftness;10;0;Create;True;0;0;False;0;0;0.098;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;41;-1449.432,963.3593;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;6;-1750.603,378.2728;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;45;-1274.448,962.923;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;21;-1746.358,538.0717;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-1659.358,647.0717;Inherit;False;Property;_TotalDistortionAmount;TotalDistortionAmount;9;0;Create;True;0;0;False;0;0;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-1501.358,358.0717;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;46;-1064.995,861.973;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-1918.5,-3;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;3;-1644.5,10;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1372.358,372.0717;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;47;-782.3827,299.6675;Inherit;True;Property;_OpacityMap;OpacityMap;12;0;Create;True;0;0;False;0;-1;None;bbabb68934aa1b54bbfcd3b64922e9f3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-689.3827,568.6675;Inherit;False;Property;_Opacity;Opacity;13;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-1355.603,145.2728;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-310.3827,410.6675;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-670.5,-29;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Knife/PortalView;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;0;25;0
WireConnection;28;0;27;0
WireConnection;28;1;26;0
WireConnection;32;0;27;0
WireConnection;32;1;29;0
WireConnection;43;0;39;3
WireConnection;43;1;42;0
WireConnection;16;0;32;0
WireConnection;16;2;19;0
WireConnection;9;0;28;0
WireConnection;9;2;17;0
WireConnection;13;1;16;0
WireConnection;13;5;14;0
WireConnection;4;1;9;0
WireConnection;4;5;7;0
WireConnection;41;0;43;0
WireConnection;6;0;4;0
WireConnection;45;0;41;0
WireConnection;45;2;44;0
WireConnection;21;0;13;0
WireConnection;22;0;6;0
WireConnection;22;1;21;0
WireConnection;46;0;45;0
WireConnection;3;0;2;0
WireConnection;23;0;22;0
WireConnection;23;1;24;0
WireConnection;23;2;46;0
WireConnection;8;0;3;0
WireConnection;8;1;23;0
WireConnection;49;0;47;4
WireConnection;49;1;48;0
WireConnection;1;1;8;0
WireConnection;0;2;1;0
WireConnection;0;9;49;0
ASEEND*/
//CHKSM=880B676C6D3048B2E0E7390DFD292506987EBFAC