Shader "Unlit/Billboard"
{
	Properties
	{
		[PerRenderData] _MainTex("Texture", 2D) = "white" {}
		_Framerate("Framerate", float) = 1
		_Clip("Clip", float) = 0.5
		_Color("Color", Color) = (1,1,1,1)
		//_TS_Matrix("Translation Scale Matrix", float4x4)
	}
		SubShader
	{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }

		ZWrite On
		ZTest LEqual
		//Blend SrcAlpha OneMinusSrcAlpha

		//Pass
		//{
			//Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma surface surf Lambert vertex:vert addshadow
			//#pragma vertex vert
			//#pragma fragment frag
			#pragma enable_d3d11_debug_symbols
			// make fog work
			//#pragma multi_compile_fog

			//#include "UnityCG.cginc"
			
			#define pi 3.141592653589793238462
			#define pi2 6.283185307179586476924
			#define NUM_ANGLES 4
			#define FRAMES 2
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float angle : BLENDWEIGHT0;
				float dirslice : BLENDWEIGHT1;
			};

			struct Input {
				float2 uv_MainTex;
				float2 uv_Bump;
				float3 worldNormal;
				float4 color : COLOR;
				INTERNAL_DATA
			};

			sampler2D _MainTex;
			//float4 _MainTex_ST;
			float4x4 _TS_Matrix;
			float _Framerate;
			float _Clip;
			float4 _Color;

			void vert(inout appdata_full v, out Input o)
			{
				//https://forum.unity.com/threads/cylindrical-billboard-shader-shader-for-billboard-with-rotation-restricted-to-the-y-axis.498406/
				//used for reference for a part of the initial part of this billboard shader due to weirdness with the standard
				//shader legacy pipeline
				UNITY_INITIALIZE_OUTPUT(Input, o);

				// apply object scale
				v.vertex.xy *= float2(length(unity_ObjectToWorld._m00_m10_m20), length(unity_ObjectToWorld._m01_m11_m21));

				// get the camera basis vectors
				float3 forward = -normalize(UNITY_MATRIX_V._m20_m21_m22);
				float3 up = float3(0, 1, 0);//normalize(UNITY_MATRIX_V._m10_m11_m12);
				float3 right = normalize(UNITY_MATRIX_V._m00_m01_m02);

				// rotate to face camera
				float4x4 rotationMatrix = float4x4(right, 0,
					up, 0,
					forward, 0,
					0, 0, 0, 1);
				v.vertex = mul(v.vertex, rotationMatrix);
				v.normal = mul(v.normal, rotationMatrix);

				// undo object to world transform surface shader will apply
				v.vertex.xyz = mul((float3x3)unity_WorldToObject, v.vertex.xyz);
				v.normal = mul(v.normal, (float3x3)unity_ObjectToWorld);

				//forward vector of camera
				float3 camForward = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));

				//float3 camForward = normalize
				//	(mul((float3x3)unity_ObjectToWorld, float3(0, 0, 0)) -
				//	mul((float3x3)unity_CameraToWorld, float3(0, 0, 0)));
				float3 objForward = mul((float3x3)unity_ObjectToWorld, float3(0, 0, 1));

				v.color.r = (atan2(
					camForward.z * objForward.x - camForward.x * objForward.z,
					camForward.x * objForward.x + camForward.z * objForward.z) + pi); //Offset by -pi bc atan2 


				//min((2 * PI) - abs(x - y), abs(x - y))
				float angleStep = (pi2 / float(NUM_ANGLES));
				float minAngle = 500;
				float dirslice = 0;

				//#pragma unroll (NUM_ANGLES)
				for (int i = 0; i < NUM_ANGLES; i++) {
					float compAngle = angleStep * i;// fmod((angleStep * i) - (angleStep * 0.5f), pi2);
					float shortestAngleDiff = min(
						(pi2)-abs(v.color.r - compAngle), //far angle
						abs(v.color.r - compAngle) //near angle
					);
					//Branching bad but does it even matter in the vertex shader?
					if (minAngle > shortestAngleDiff) {
						minAngle = shortestAngleDiff;
						dirslice = float(i);
					}
				}
				v.color.g = dirslice;
			}

			//#include "billboardvert.cginc"

			/*fixed4 frag(v2f i) : SV_Target
			{
				float2 offset = float2(i.dirslice / float(NUM_ANGLES), floor(fmod(_Time.y * _Framerate, 2))/ FRAMES);
				fixed4 col = tex2D(_MainTex, (i.uv / float2(NUM_ANGLES, FRAMES) - float2(offset.x, offset.y))) * i.col;
				clip(col.a - _Clip);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}*/

			void surf(Input IN, inout SurfaceOutput o)
			{
				// Albedo comes from a texture tinted by color
				//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				float2 offset = float2(IN.color.g / float(NUM_ANGLES), floor(fmod(_Time.y * _Framerate, 2)) / FRAMES);
				float4 col = tex2D(_MainTex, (IN.uv_MainTex / float2(NUM_ANGLES, FRAMES) - float2(offset.x, offset.y))) * _Color;
				o.Albedo = col.rgb;
				// Metallic and smoothness come from slider variables
				//o.Metallic = _Metallic;
				//o.Smoothness = _Glossiness;
				clip(col.a - _Clip);
				//o.Alpha = col.a;
			}
		ENDCG
	//}

			// shadow caster rendering pass, implemented manually
		// using macros from UnityCG.cginc
	}
}