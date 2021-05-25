Shader "PS1/VertexLitTerrainAddPass"
{
	Properties
	{
		_JitterResolution("Jitter Resolution", Int) = 10
		_DepthLigthingIntensity("Depth Lighting Intensity", Range(0.0, 1.0)) = 0.5

		// Splat Map Control Texture
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}

		// Textures
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}

		// Normal Maps
		[HideInInspector] _Normal3("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0("Normal 0 (R)", 2D) = "bump" {}
	}
		SubShader
	{
		
		LOD 100

		Pass
		{
			Tags {
				"SplatCount" = "4"
				"RenderType" = "Opaque"
				"Queue" = "Geometry-99"
				"IgnoreProjector" = "True"
			}
			ZWrite On
			Lighting On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma decal:add
	
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uvs0 : TEXCOORD1;
				float2 uvs1 : TEXCOORD2;
				float2 uvs2 : TEXCOORD3;
				float2 uvs3 : TEXCOORD4;
				float4 pos : SV_POSITION;
				float4 lighting : COLOR0;
				float fog : TEXCOORD5;
				float3 normal : NORMAL;
				LIGHTING_COORDS(5, 6)
			};

			sampler2D _Control;
			float4 _Control_ST;

			sampler2D _Splat0;
			float4 _Splat0_ST;
			sampler2D _Splat1;
			float4 _Splat1_ST;
			sampler2D _Splat2;
			float4 _Splat2_ST;
			sampler2D _Splat3;
			float4 _Splat3_ST;

			int _JitterResolution;
			float _DepthLigthingIntensity;

			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.pos.x *= _JitterResolution;
				o.pos.x = floor(o.pos.x);
				o.pos.x /= _JitterResolution;
				o.pos.y *= _JitterResolution;
				o.pos.y = floor(o.pos.y);
				o.pos.y /= _JitterResolution;

				o.uv = TRANSFORM_TEX(v.texcoord, _Control);
				o.uvs0 = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uvs1 = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uvs2 = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uvs3 = TRANSFORM_TEX(v.texcoord, _Splat3);

				o.lighting = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);

				float distance = length(mul(UNITY_MATRIX_MV, v.vertex));
				o.fog = max(min((unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart), 1.0), 0.0);

				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 cont = tex2D(_Control, i.uv);

				if (cont.r == 0.0 && cont.g == 0.0 && cont.b == 0.0 && cont.a == 0.0)
					discard;

				fixed4 s1 = tex2D(_Splat0, i.uvs0) * cont.r;
				fixed4 s2 = tex2D(_Splat1, i.uvs1) * cont.g;
				fixed4 s3 = tex2D(_Splat2, i.uvs2) * cont.b;
				fixed4 s4 = tex2D(_Splat3, i.uvs3) * cont.a;

				fixed4 col = s1 + s2 + s3 + s4;
				col *= i.lighting;
				col = lerp(unity_FogColor, col, i.fog);
				col.a = 0.0;
				return col;
			}
			ENDCG
		}
	}
}
