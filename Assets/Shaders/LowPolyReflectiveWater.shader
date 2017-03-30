// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/LowPolyReflectiveWater" {
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_SpecColor("Specular Material Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 1.0
		_WaveLength("Wave Length", Float) = 0.5
		_WaveHeight("Wave Height", Float) = 0.5
		_WaveSpeed("Wave Speed", Float) = 1.0
		_RandomHeight("Random Height", Float) = 0.5
		_RandomSpeed("Random Speed", Float) = 0.5
		[Toggle] _EdgeBlend("Edge Blend", Float) = 0
		_ShoreColor("Shore Color", Color) = (1,1,1,1)
		_ShoreIntensity("Shore Intensity", Range(-1,1)) = 0
		_ShoreDistance("Shore Distance", Float) = 1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#define COMPUTESCREENPOS ComputeScreenPos

			#include "UnityStandardUtils.cginc"
			#include "UnityLightingCommon.cginc"
			
			#if UNITY_VERSION < 540
				#define UNITY_VERTEX_INPUT_INSTANCE_ID
				#define UNITY_VERTEX_OUTPUT_STEREO
				#define UNITY_SETUP_INSTANCE_ID(v)
				#define UNITY_TRANSFER_INSTANCE_ID(v,o)
				#define UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)
				#define COMPUTESCREENPOS ComputeScreenPos
			#else
				#define COMPUTESCREENPOS ComputeNonStereoScreenPos
			#endif
			
			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}
			
			float rand2(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(19.9128, 75.2, 5122))) * 12765.5213);
			}

			float _WaveLength;
			float _WaveHeight;
			float _WaveSpeed;
			float _RandomHeight;
			float _RandomSpeed;

			uniform float4 _Color;
			uniform float _Shininess;

			struct v2g
			{
				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD3;
				float3 norm : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct g2f
			{
				float4 pos : SV_POSITION;
				float3 norm : NORMAL;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD3;
				float4 diffuseColor : TEXCOORD1;
				float4 specularColor : TEXCOORD2;
			};

			sampler2D_float _CameraDepthTexture;
			half _ShoreIntensity, _ShoreDistance;
			fixed4 _ShoreColor;

			v2g vert(appdata_full v)
			{

				float phase0 = (_WaveHeight)* sin((_Time[1] * _WaveSpeed) + (v.vertex.x * _WaveLength) + (v.vertex.z * _WaveLength) + rand2(v.vertex.xzz));
				float phase0_1 = (_RandomHeight)* sin(cos(rand(v.vertex.xzz) * _RandomHeight * cos(_Time[1] * _RandomSpeed * sin(rand(v.vertex.xxz)))));

				v.vertex.y = phase0 + phase0_1 - 0.91;

				half4 pos0 = mul(unity_ObjectToWorld, v.vertex);
				pos0 = mul(UNITY_MATRIX_VP, pos0);

				v2g OUT;
				OUT.pos = v.vertex;
				OUT.screenPos = COMPUTESCREENPOS(pos0);
				OUT.screenPos.z = lerp(pos0.w, mul(UNITY_MATRIX_V, pos0).z, unity_OrthoParams.w);
				OUT.norm = v.normal;
				OUT.uv = v.texcoord;
				
				UNITY_TRANSFER_FOG(OUT, OUT.pos);

				return OUT;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
			{
				float3 v0 = IN[0].pos.xyz;
				float3 v1 = IN[1].pos.xyz;
				float3 v2 = IN[2].pos.xyz;
				float3 centerPos = (v0 + v1 + v2) / 3.0;

				float3 vn = normalize(cross(v1 - v0, v2 - v0));

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(vn, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, float4(centerPos, 0.0)).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float attenuation = 1.0;

				float4 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgba * _Color.rgba;

				float4 diffuseReflection = attenuation * _LightColor0.rgba * _Color.rgba * max(0.0, dot(normalDirection, lightDirection));

				float4 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0)
				{
					specularReflection = float4(0.0, 0.0, 0.0, 0.0);
				}
				else
				{
					specularReflection = attenuation * _LightColor0.rgba * _SpecColor.rgba * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
				}

				g2f OUT;

				OUT.pos = mul(UNITY_MATRIX_MVP, IN[0].pos);
				OUT.screenPos = IN[0].screenPos;
				OUT.norm = vn;
				OUT.uv = IN[0].uv;
				OUT.diffuseColor = ambientLighting + diffuseReflection;
				OUT.specularColor = specularReflection;
				triStream.Append(OUT);
				UNITY_TRANSFER_FOG(OUT, OUT.pos);

				OUT.pos = mul(UNITY_MATRIX_MVP, IN[1].pos);
				OUT.screenPos = IN[1].screenPos;
				OUT.norm = vn;
				OUT.uv = IN[1].uv;
				OUT.diffuseColor = ambientLighting + diffuseReflection;
				OUT.specularColor = specularReflection;
				triStream.Append(OUT);
				UNITY_TRANSFER_FOG(OUT, OUT.pos);

				OUT.pos = mul(UNITY_MATRIX_MVP, IN[2].pos);
				OUT.screenPos = IN[2].screenPos;
				OUT.norm = vn;
				OUT.uv = IN[2].uv;
				OUT.diffuseColor = ambientLighting + diffuseReflection;
				OUT.specularColor = specularReflection;
				triStream.Append(OUT);

			}

			half4 frag(g2f IN) : COLOR
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				float4 c = float4(IN.specularColor + IN.diffuseColor);

				float sceneZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
				float perpectiveZ = LinearEyeDepth(sceneZ);

				#if defined(UNITY_REVERSED_Z)
					sceneZ = 1 - sceneZ;
				#endif

				float orthoZ = sceneZ*(_ProjectionParams.y - _ProjectionParams.z) - _ProjectionParams.y;

				sceneZ = lerp(perpectiveZ, orthoZ, unity_OrthoParams.w);

				half diff = abs(sceneZ - IN.screenPos.z) / _ShoreDistance;
				diff = smoothstep(_ShoreIntensity, 1, diff);
				c = lerp(lerp(c, _ShoreColor, _ShoreColor.a), c, diff);
				
				UNITY_APPLY_FOG(OUT.fogCoord, c);

				return c;
			}

			ENDCG

		}
	}
}
