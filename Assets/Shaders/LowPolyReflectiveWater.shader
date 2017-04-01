Shader "Custom/LowPolyReflectiveWater" {
	Properties
	{
		_Diffuse("Diffuse Color", Color) = (1,1,1,1)
		_Specular("Specular Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 1.0
		//
		_WaveLength("Wave Length", Float) = 0.5
		_WaveHeight("Wave Height", Float) = 0.5
		_WaveSpeed("Wave Speed", Float) = 1.0
		//
		_RandomHeight("Random Height", Float) = 0.5
		_RandomSpeed("Random Speed", Float) = 0.5
		//
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
			#include "UnityStandardUtils.cginc"
			#include "UnityLightingCommon.cginc"
			
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#if UNITY_VERSION < 540
				#define UNITY_VERTEX_INPUT_INSTANCE_ID
				#define UNITY_VERTEX_OUTPUT_STEREO
				#define UNITY_SETUP_INSTANCE_ID(i)
				#define UNITY_TRANSFER_INSTANCE_ID(i, output)
				#define UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output)
				#define COMPUTESCREENPOS ComputeScreenPos
			#else
				#define COMPUTESCREENPOS ComputeNonStereoScreenPos
			#endif
			
			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(1, 5, 10))) * 20);
			}
			
			float rand2(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(2, 6, 12))) * 24);
			}

			float _WaveLength;
			float _WaveHeight;
			float _WaveSpeed;
			float _RandomHeight;
			float _RandomSpeed;

			uniform float4 _Diffuse;
			uniform float4 _Specular;
			uniform float _Shininess;
			sampler2D _CameraDepthTexture;
			float _ShoreIntensity, _ShoreDistance;
			float4 _ShoreColor;

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

			v2g vert(appdata_full i)
			{

				float phase1 = (_WaveHeight) * sin((_Time[1] * _WaveSpeed) + (i.vertex.x * _WaveLength) + (i.vertex.z * _WaveLength) + rand(i.vertex.xzz));
				float phase2 = (_RandomHeight) * sin(cos(rand(i.vertex.xzz) * _RandomHeight * cos(_Time[1] * _RandomSpeed * sin(rand2(i.vertex.xxz)))));

				i.vertex.y = phase1 + phase2 - 0.91 + (_WaveHeight);

				half4 vpos = mul(unity_ObjectToWorld, i.vertex);
				vpos = mul(UNITY_MATRIX_VP, vpos);

				v2g output;
				output.pos = i.vertex;
				output.screenPos = COMPUTESCREENPOS(vpos);
				output.screenPos.z = lerp(vpos.w, mul(UNITY_MATRIX_V, vpos).z, unity_OrthoParams.w);
				output.norm = i.normal;
				output.uv = i.texcoord;
				
				return output;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g i[3], inout TriangleStream<g2f> triangles)
			{
				float3 v0 = i[0].pos.xyz;
				float3 v1 = i[1].pos.xyz;
				float3 v2 = i[2].pos.xyz;
				float3 centerPos = (v0 + v1 + v2) / 3.0;

				float3 vn = normalize(cross(v1 - v0, v2 - v0));

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(vn, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, float4(centerPos, 0.0)).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float attenuation = 1.0;

				float4 ambientLighting = float4(0.5,0.5,0.5,1.0) * _Diffuse.rgba;

				float4 diffuseReflection = attenuation * _LightColor0.rgba * _Diffuse.rgba * max(0.0, dot(normalDirection, lightDirection));

				float4 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0)
				{
					specularReflection = float4(0.0, 0.0, 0.0, 0.0);
				}
				else
				{
					specularReflection = attenuation * _LightColor0.rgba * _Specular.rgba * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
				}

				g2f output;

				for (int index = 0; index < 3; index++)
				{
					output.pos = mul(UNITY_MATRIX_MVP, i[index].pos);
					output.screenPos = i[index].screenPos;
					output.norm = vn;
					output.uv = i[index].uv;
					output.diffuseColor = ambientLighting + diffuseReflection;
					output.specularColor = specularReflection;
					triangles.Append(output);
				}

			}

			float4 frag(g2f i) : COLOR
			{

				float4 c = float4(i.specularColor + i.diffuseColor);

				float sceneZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
				float perpectiveZ = LinearEyeDepth(sceneZ);
				
				float orthoZ = sceneZ * (_ProjectionParams.y - _ProjectionParams.z) - _ProjectionParams.y;

				sceneZ = lerp(perpectiveZ, orthoZ, unity_OrthoParams.w);

				float difference = abs(sceneZ - i.screenPos.z) / _ShoreDistance;
				difference = smoothstep(_ShoreIntensity, 1, difference);
				c = lerp(lerp(c, _ShoreColor, _ShoreColor.a), c, difference);
				
				return c;
			}

			ENDCG

		}
	}
}
