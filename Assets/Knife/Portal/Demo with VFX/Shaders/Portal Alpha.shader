// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Knife/Portal Alpha"
{
	Properties
	{
		_LinePower("LinePower", Float) = 1
		_LineMul("LineMul", Float) = 1
		[HDR]_LineTint("LineTint", Color) = (0,0,0,0)
		_LineSub("LineSub", Float) = 0
		_Noise("Noise", 2D) = "white" {}
		_NoiseSpeed("NoiseSpeed", Vector) = (0,0,0,0)
		_UVRemap("UVRemap", Vector) = (0,1,0,0)
		_LineTex("LineTex", 2D) = "white" {}
		[HDR]_ThinLineColor("ThinLineColor", Color) = (0,0,0,0)
		_NoiseFactor("NoiseFactor", Float) = 1
		_NoiseSub("NoiseSub", Float) = 1
		_DisplacementNoise("DisplacementNoise", 2D) = "white" {}
		_DisplacementNoiseSpeed("DisplacementNoiseSpeed", Vector) = (0,0,0,0)
		_DisplacementNoiseRemap("DisplacementNoiseRemap", Vector) = (0,1,0,1)
		_GradientPower("GradientPower", Float) = 0
		_GradientLength("GradientLength", Float) = 0
		[HDR]_Gradient("Gradient", Color) = (0,0,0,0)
		_SoftDepthDistance("SoftDepthDistance", Float) = 0
		_HueOffset("Hue Offset", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "SubShader 0 Pass 0"
			Tags { "LightMode"="ForwardBase" }
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
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
			};

			uniform sampler2D _DisplacementNoise;
			uniform float2 _DisplacementNoiseSpeed;
			uniform float4 _DisplacementNoise_ST;
			uniform float4 _DisplacementNoiseRemap;
			uniform sampler2D _Noise;
			uniform float2 _NoiseSpeed;
			uniform float4 _Noise_ST;
			uniform float _NoiseFactor;
			uniform float4 _UVRemap;
			uniform float _LineSub;
			uniform float _NoiseSub;
			uniform float _HueOffset;
			uniform float4 _LineTint;
			uniform float _LinePower;
			uniform float _LineMul;
			uniform sampler2D _LineTex;
			uniform float4 _LineTex_ST;
			uniform float4 _ThinLineColor;
			uniform float _GradientLength;
			uniform float _GradientPower;
			uniform float4 _Gradient;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _SoftDepthDistance;
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
				
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
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
				float2 uv0_Noise = i.ase_texcoord1.xy * _Noise_ST.xy + _Noise_ST.zw;
				float2 panner14 = ( 1.0 * _Time.y * _NoiseSpeed + uv0_Noise);
				float2 uv01 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_2_0 = ( 1.0 - (_UVRemap.z + (uv01.y - _UVRemap.x) * (_UVRemap.w - _UVRemap.z) / (_UVRemap.y - _UVRemap.x)) );
				float clampResult11 = clamp( ( temp_output_2_0 - ( _LineSub / 10.0 ) ) , 0.0 , 1.0 );
				float clampResult30 = clamp( ( ( _NoiseFactor * clampResult11 ) - ( _NoiseSub / 10.0 ) ) , 0.0 , 1.0 );
				float lerpResult23 = lerp( tex2D( _Noise, panner14 ).r , 1.0 , clampResult30);
				float4 LineTint_col76 = _LineTint;
				float3 hsvTorgb70 = RGBToHSV( LineTint_col76.rgb );
				float3 hsvTorgb69 = HSVToRGB( float3(( _HueOffset + hsvTorgb70.x ),hsvTorgb70.y,hsvTorgb70.z) );
				float3 LineTint_col185 = hsvTorgb69;
				float2 uv_LineTex = i.ase_texcoord1.xy * _LineTex_ST.xy + _LineTex_ST.zw;
				float4 ThinLineColor_col84 = _ThinLineColor;
				float3 hsvTorgb81 = RGBToHSV( ThinLineColor_col84.rgb );
				float3 hsvTorgb79 = HSVToRGB( float3(( _HueOffset + hsvTorgb81.x ),hsvTorgb81.y,hsvTorgb81.z) );
				float3 ThinLineColor_col186 = hsvTorgb79;
				float clampResult56 = clamp( temp_output_2_0 , 0.0 , 1.0 );
				float clampResult50 = clamp( pow( ( clampResult56 - _GradientLength ) , _GradientPower ) , 0.0 , 1.0 );
				float grad52 = clampResult50;
				float4 grad_col92 = _Gradient;
				float3 hsvTorgb89 = RGBToHSV( grad_col92.rgb );
				float3 hsvTorgb87 = HSVToRGB( float3(( _HueOffset + hsvTorgb89.x ),hsvTorgb89.y,hsvTorgb89.z) );
				float3 grad_col191 = hsvTorgb87;
				float4 temp_output_20_0 = ( float4( ( lerpResult23 * LineTint_col185 * ( pow( clampResult11 , _LinePower ) * _LineMul ) ) , 0.0 ) + ( tex2D( _LineTex, uv_LineTex ) * float4( ThinLineColor_col186 , 0.0 ) ) + float4( ( grad52 * grad_col191 ) , 0.0 ) );
				float clampResult46 = clamp( ( length( (temp_output_20_0).rgb ) + grad52 ) , 0.0 , 1.0 );
				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth59 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth59 = abs( ( screenDepth59 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _SoftDepthDistance ) );
				float clampResult62 = clamp( distanceDepth59 , 0.0 , 1.0 );
				float4 appendResult42 = (float4((temp_output_20_0).rgb , ( clampResult46 * clampResult62 )));
				
				
				finalColor = appendResult42;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
-1868;29;1839;1004;4023.605;-789.9039;1;True;False
Node;AmplifyShaderEditor.Vector4Node;18;-2625.507,-188.1346;Float;False;Property;_UVRemap;UVRemap;6;0;Create;True;0;0;False;0;0,1,0,0;0,1,0,2;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-2694.215,-352.3348;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;7;-1979.252,238.2952;Float;False;Property;_LineTint;LineTint;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1.309785,3.003473,4.313261,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;57;-731.2973,500.4145;Inherit;False;Property;_Gradient;Gradient;16;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.6202588,0.4382099,0.81,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;17;-2308.507,-305.1346;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1772.476,-144.0127;Float;False;Property;_LineSub;LineSub;3;0;Create;True;0;0;False;0;0;2.51;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;-1315.578,452.9948;Float;False;Property;_ThinLineColor;ThinLineColor;8;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1.701362,2.845556,4.207005,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;-420.9067,580.1539;Inherit;False;grad_col;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;2;-2020.086,-297.3764;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;76;-1719.909,264.3194;Inherit;False;LineTint_col;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;12;-1533.276,-69.91269;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-751.7402,393.2566;Inherit;False;ThinLineColor_col;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-3704.824,859.6595;Inherit;False;76;LineTint_col;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;9;-1274.576,-224.6127;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;56;-1919.507,-439.2039;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;-3845.965,1751.48;Inherit;False;92;grad_col;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-3807.42,1247.635;Inherit;False;84;ThinLineColor_col;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2143.238,-48.20142;Inherit;False;Property;_GradientLength;GradientLength;15;0;Create;True;0;0;False;0;0;-0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-1234.728,-527.4135;Float;False;Property;_NoiseFactor;NoiseFactor;9;0;Create;True;0;0;False;0;1;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;70;-3399.823,932.313;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;73;-3575.548,648.2355;Inherit;False;Property;_HueOffset;Hue Offset;18;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1850.507,-241.2039;Inherit;False;Property;_GradientPower;GradientPower;14;0;Create;True;0;0;False;0;0;7.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;89;-3540.964,1824.134;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;49;-1787.953,-391.7281;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-970.764,-356.1055;Float;False;Property;_NoiseSub;NoiseSub;10;0;Create;True;0;0;False;0;1;14.55;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;81;-3503.419,1320.289;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;11;-1114.676,-197.3127;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-2187.691,-995.4308;Inherit;False;0;13;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;31;-799.764,-392.1055;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-989.795,-497.5724;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-3117.908,824.4175;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;16;-1914.974,-883.2272;Float;False;Property;_NoiseSpeed;NoiseSpeed;5;0;Create;True;0;0;False;0;0,0;-0.2,0.3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-3221.504,1212.393;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;-3259.049,1716.238;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;55;-1608.507,-371.2039;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;14;-1668.749,-996.9894;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;50;-1477.614,-374.2765;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1176.686,-22.77637;Float;False;Property;_LinePower;LinePower;0;0;Create;True;0;0;False;0;1;2.94;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;87;-2942.048,1764.879;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;69;-2800.907,873.0587;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;79;-2904.503,1261.034;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;29;-821.764,-543.1055;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;86;-2681.642,1284.019;Inherit;False;ThinLineColor_col1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;13;-1321.071,-1173.819;Inherit;True;Property;_Noise;Noise;4;0;Create;True;0;0;False;0;-1;None;ecea10d8fc11e8d4b9ab34bdd682f532;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;30;-668.764,-548.1055;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;3;-965.6862,-121.7764;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-956.6862,40.22363;Float;False;Property;_LineMul;LineMul;1;0;Create;True;0;0;False;0;1;4.26;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;-2546.239,908.7798;Inherit;False;LineTint_col1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-1327.217,-339.7709;Inherit;False;grad;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;91;-2719.187,1787.864;Inherit;False;grad_col1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;23;-649.207,-868.0529;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-419.7964,482.3045;Inherit;False;91;grad_col1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-788.6862,-27.77637;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-1105.796,377.3045;Inherit;False;86;ThinLineColor_col1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-382.5596,303.1621;Inherit;False;52;grad;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-1436.257,134.6643;Inherit;False;85;LineTint_col1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;19;-1086.227,169.6739;Inherit;True;Property;_LineTex;LineTex;7;0;Create;True;0;0;False;0;-1;None;cef6570a721e7994abff60c965b7aca2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-164.4204,353.8514;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-653.1757,-192.1127;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-748.7772,238.0902;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-495.0499,-27.80978;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;44;-390.2914,94.8197;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;38;-1602.232,990.948;Float;False;Property;_DisplacementNoiseSpeed;DisplacementNoiseSpeed;12;0;Create;True;0;0;False;0;0,0;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;60;9.605469,351.7902;Inherit;False;Property;_SoftDepthDistance;SoftDepthDistance;17;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-1595.999,826.1307;Inherit;False;0;33;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LengthOpNode;45;-170.2914,102.8197;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;59;218.6055,247.7902;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-43.73779,189.0094;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;37;-1305.999,864.1307;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;40;-827.2323,936.948;Float;False;Property;_DisplacementNoiseRemap;DisplacementNoiseRemap;13;0;Create;True;0;0;False;0;0,1,0,1;0,1,0,0.3;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;46;28.70862,82.8197;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-1128.999,785.1307;Inherit;True;Property;_DisplacementNoise;DisplacementNoise;11;0;Create;True;0;0;False;0;-1;None;ecea10d8fc11e8d4b9ab34bdd682f532;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;62;353.6055,160.7902;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;132.6055,-52.20984;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;43;-342.5374,-19.33118;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;41;-512.2323,805.948;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;34;-1266.14,1274.007;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-205.9988,816.1307;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;144.7625,-226.9312;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;32;-1018.786,1644.489;Float;False;Constant;_HolePosition;HolePosition;11;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;505.9,-65.5;Float;False;True;-1;2;ASEMaterialInspector;0;1;Knife/Portal Alpha;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;2;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;17;0;1;2
WireConnection;17;1;18;1
WireConnection;17;2;18;2
WireConnection;17;3;18;3
WireConnection;17;4;18;4
WireConnection;92;0;57;0
WireConnection;2;0;17;0
WireConnection;76;0;7;0
WireConnection;12;0;10;0
WireConnection;84;0;21;0
WireConnection;9;0;2;0
WireConnection;9;1;12;0
WireConnection;56;0;2;0
WireConnection;70;0;78;0
WireConnection;89;0;90;0
WireConnection;49;0;56;0
WireConnection;49;1;48;0
WireConnection;81;0;82;0
WireConnection;11;0;9;0
WireConnection;31;0;28;0
WireConnection;26;0;24;0
WireConnection;26;1;11;0
WireConnection;74;0;73;0
WireConnection;74;1;70;1
WireConnection;80;0;73;0
WireConnection;80;1;81;1
WireConnection;88;0;73;0
WireConnection;88;1;89;1
WireConnection;55;0;49;0
WireConnection;55;1;54;0
WireConnection;14;0;15;0
WireConnection;14;2;16;0
WireConnection;50;0;55;0
WireConnection;87;0;88;0
WireConnection;87;1;89;2
WireConnection;87;2;89;3
WireConnection;69;0;74;0
WireConnection;69;1;70;2
WireConnection;69;2;70;3
WireConnection;79;0;80;0
WireConnection;79;1;81;2
WireConnection;79;2;81;3
WireConnection;29;0;26;0
WireConnection;29;1;31;0
WireConnection;86;0;79;0
WireConnection;13;1;14;0
WireConnection;30;0;29;0
WireConnection;3;0;11;0
WireConnection;3;1;4;0
WireConnection;85;0;69;0
WireConnection;52;0;50;0
WireConnection;91;0;87;0
WireConnection;23;0;13;1
WireConnection;23;2;30;0
WireConnection;5;0;3;0
WireConnection;5;1;6;0
WireConnection;58;0;53;0
WireConnection;58;1;93;0
WireConnection;8;0;23;0
WireConnection;8;1;77;0
WireConnection;8;2;5;0
WireConnection;22;0;19;0
WireConnection;22;1;94;0
WireConnection;20;0;8;0
WireConnection;20;1;22;0
WireConnection;20;2;58;0
WireConnection;44;0;20;0
WireConnection;45;0;44;0
WireConnection;59;0;60;0
WireConnection;51;0;45;0
WireConnection;51;1;53;0
WireConnection;37;0;36;0
WireConnection;37;2;38;0
WireConnection;46;0;51;0
WireConnection;33;1;37;0
WireConnection;62;0;59;0
WireConnection;61;0;46;0
WireConnection;61;1;62;0
WireConnection;43;0;20;0
WireConnection;41;0;33;1
WireConnection;41;1;40;1
WireConnection;41;2;40;2
WireConnection;41;3;40;3
WireConnection;41;4;40;4
WireConnection;35;0;41;0
WireConnection;35;1;34;0
WireConnection;42;0;43;0
WireConnection;42;3;61;0
WireConnection;0;0;42;0
WireConnection;0;1;35;0
ASEEND*/
//CHKSM=A143FFCD1EB980F9B18C50E91CA1D6AE7D24296A