v2f vert(appdata v)
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv.xy;

	float4x4 nonRotatedMatrix = _TS_Matrix;

	// billboard mesh towards camera
	float3 vpos = mul((float3x3)nonRotatedMatrix, v.vertex.xyz);
	float4 worldCoord = float4(nonRotatedMatrix[0][3], nonRotatedMatrix[1][3], nonRotatedMatrix[2][3], 1);

	float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
	float4 outPos = mul(UNITY_MATRIX_P, viewPos);

	o.pos = outPos;

	//forward vector of camera
	float3 camForward = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));

	//float3 camForward = normalize
	//	(mul((float3x3)unity_ObjectToWorld, float3(0, 0, 0)) -
	//	mul((float3x3)unity_CameraToWorld, float3(0, 0, 0)));
	float3 objForward = mul((float3x3)unity_ObjectToWorld, float3(0, 0, 1));
	//o.col.rgb = objForward;
	//angle=atan2(w2​v1​−w1​v2​,w1​v1​+w2​v2​)
	o.angle = (atan2(
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
			(pi2)-abs(o.angle - compAngle), //far angle
			abs(o.angle - compAngle) //near angle
		);
		//Branching bad but does it even matter in the vertex shader?
		if (minAngle > shortestAngleDiff) {
			minAngle = shortestAngleDiff;
			dirslice = float(i);
		}
	}
	o.dirslice = dirslice;
	o.col = (1, 1, 1, 1);
	UNITY_TRANSFER_FOG(o, o.vertex);
	return o;
}

/*				//Store angle in red channel
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
				v.color.g = dirslice;*/

/*Shader "Unlit/Billboard"
{
	Properties
	{
		[PerRenderData] _MainTex("Texture", 2D) = "white" {}
		_Framerate("Framerate", float) = 1
		_Clip("Clip", float) = 0.5
		//_TS_Matrix("Translation Scale Matrix", float4x4)
	}
		SubShader
	{
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Opaque" "DisableBatching" = "True" }

		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha

		//Pass
		//{
			//Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma surface surf Lambert vertex:vert
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
				INTERNAL_DATA
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4x4 _TS_Matrix;
			float _Framerate;
			float _Clip;

			void vert(inout appdata_full v)
			{
				//v2f o;
				//float4 vertPos = UnityObjectToClipPos(mul(v.vertex, unity_WorldToObject));
				float4 vertPos = v.vertex;//UnityWorldToClipPos(v.vertex);
				//o.uv = v.uv.xy;

				float4x4 nonRotatedMatrix = _TS_Matrix;

				// billboard mesh towards camera
				float3 vpos = mul((float3x3)nonRotatedMatrix, vertPos);
				//float4 worldCoord = float4(nonRotatedMatrix[0][3], nonRotatedMatrix[1][3], nonRotatedMatrix[2][3], 1);

				float4 viewPos = mul(UNITY_MATRIX_V, vpos); //+ float4(vpos, 0);
				float4 outPos = viewPos;//mul(UNITY_MATRIX_P, viewPos);

				v.vertex = float4(vpos, 0);//outPos;

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

			fixed4 frag(v2f i) : SV_Target
			{
				float2 offset = float2(i.dirslice / float(NUM_ANGLES), floor(fmod(_Time.y * _Framerate, 2))/ FRAMES);
				fixed4 col = tex2D(_MainTex, (i.uv / float2(NUM_ANGLES, FRAMES) - float2(offset.x, offset.y))) * i.col;
				clip(col.a - _Clip);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}

void surf(Input IN, inout SurfaceOutput o)
{
	//half4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = 0.5;
	o.Alpha = 1;
}
ENDCG
//}

		// shadow caster rendering pass, implemented manually
	// using macros from UnityCG.cginc
	}
}*/