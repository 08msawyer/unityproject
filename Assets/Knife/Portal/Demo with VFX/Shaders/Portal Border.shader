// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Knife/Portal Border"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_DisplacementNoise("DisplacementNoise", 2D) = "white" {}
		_DisplacementNoiseSpeed("DisplacementNoiseSpeed", Vector) = (0,0,0,0)
		_DisplacementNoiseRemap("DisplacementNoiseRemap", Vector) = (0,1,0,1)
		_HueOffset("Hue Offset", Range( 0 , 1)) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "SubShader 0 Pass 0"
			Tags { "LightMode"="ForwardBase" "RenderType"="Transparent" "Queue"="Transparent+3000" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};

			uniform sampler2D _DisplacementNoise;
			uniform float2 _DisplacementNoiseSpeed;
			uniform float4 _DisplacementNoise_ST;
			uniform float4 _DisplacementNoiseRemap;
			uniform float4 _Color;
			uniform float _HueOffset;
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 uv0_DisplacementNoise = v.ase_texcoord.xy * _DisplacementNoise_ST.xy + _DisplacementNoise_ST.zw;
				float2 panner37 = ( 1.0 * _Time.y * _DisplacementNoiseSpeed + uv0_DisplacementNoise);
				
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = ( (_DisplacementNoiseRemap.z + (tex2Dlod( _DisplacementNoise, float4( panner37, 0, 0.0) ).r - _DisplacementNoiseRemap.x) * (_DisplacementNoiseRemap.w - _DisplacementNoiseRemap.z) / (_DisplacementNoiseRemap.y - _DisplacementNoiseRemap.x)) * v.ase_normal );
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
#endif
				float3 hsvTorgb43 = RGBToHSV( _Color.rgb );
				float3 hsvTorgb42 = HSVToRGB( float3(( hsvTorgb43.x + _HueOffset ),hsvTorgb43.y,hsvTorgb43.z) );
				float4 appendResult46 = (float4(hsvTorgb42 , _Color.a));
				
				
				finalColor = appendResult46;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
-1885;528;1839;1004;1309.692;268.9584;1;True;False
Node;AmplifyShaderEditor.ColorNode;7;-1021.522,-69.87986;Float;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;1,1,1,1;0.884853,1.479931,2.188,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;38;-1602.232,990.948;Float;False;Property;_DisplacementNoiseSpeed;DisplacementNoiseSpeed;2;0;Create;True;0;0;False;0;0,0;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-1595.999,826.1307;Inherit;False;0;33;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-793.6919,330.0416;Inherit;False;Property;_HueOffset;Hue Offset;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;43;-830.6919,157.0416;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PannerNode;37;-1305.999,864.1307;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-604.6919,58.04156;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;40;-827.2323,936.948;Float;False;Property;_DisplacementNoiseRemap;DisplacementNoiseRemap;3;0;Create;True;0;0;False;0;0,1,0,1;0,1,0,0.15;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;33;-1128.999,785.1307;Inherit;True;Property;_DisplacementNoise;DisplacementNoise;1;0;Create;True;0;0;False;0;-1;None;ecea10d8fc11e8d4b9ab34bdd682f532;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;34;-1266.14,1274.007;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;41;-512.2323,805.948;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;42;-353.6919,66.04156;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;46;-77.69202,-10.9584;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-205.9988,816.1307;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;190,-46;Float;False;True;-1;2;ASEMaterialInspector;0;1;Knife/Portal Border;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;3;LightMode=ForwardBase;RenderType=Transparent=RenderType;Queue=Transparent=Queue=3000;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;43;0;7;0
WireConnection;37;0;36;0
WireConnection;37;2;38;0
WireConnection;44;0;43;1
WireConnection;44;1;45;0
WireConnection;33;1;37;0
WireConnection;41;0;33;1
WireConnection;41;1;40;1
WireConnection;41;2;40;2
WireConnection;41;3;40;3
WireConnection;41;4;40;4
WireConnection;42;0;44;0
WireConnection;42;1;43;2
WireConnection;42;2;43;3
WireConnection;46;0;42;0
WireConnection;46;3;7;4
WireConnection;35;0;41;0
WireConnection;35;1;34;0
WireConnection;0;0;46;0
WireConnection;0;1;35;0
ASEEND*/
//CHKSM=0EEDD641AF8C5965B1081DDEA8406F41262AD2DB