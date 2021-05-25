Shader "PS1/VertexLitCutout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_JitterResolution("Jitter Resolution", Int) = 300
		_Cutoff("Cutoff", Range(0.0, 1.0)) = 0.5
		_Emission("Emission", Float) = 0.0
    }
    SubShader
    {
        LOD 100

        Pass
		{
			Tags {
				"RenderType" = "Opaque"
			}
			ZWrite On
			Cull Off
			Lighting On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag addshadow

            #include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
				float4 lighting : COLOR0;
				float fog : TEXCOORD1;
				LIGHTING_COORDS(2, 3)
				float4 screenPos : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			int _JitterResolution;
			float _Cutoff;
			float _Emission;

			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

            v2f vert (appdata_full v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
				o.pos.x *= _JitterResolution;
				o.pos.x = floor(o.pos.x);
				o.pos.x /= _JitterResolution;
				o.pos.y *= _JitterResolution;
				o.pos.y = floor(o.pos.y);
				o.pos.y /= _JitterResolution;
				o.pos.w *= _JitterResolution;
				o.pos.w = floor(o.pos.w);
				o.pos.w /= _JitterResolution;

				o.screenPos = ComputeScreenPos(o.pos);

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.lighting = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
				// float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				// o.lighting = lerp(unity_AmbientSky, 1.0 - _LightColor0, min(0.0, dot(worldNormal, 1.0 - _WorldSpaceLightPos0.xyz)));
				// o.lighting += max(0.0, dot(v.normal, float3(0.0, 1.0, 0.0))) * _DepthLigthingIntensity;
				// o.lighting *= lerp(1.0, 1.0 - max(0.0, dot(v.normal, float3(0.0, -1.0, 0.0))), _DepthLigthingIntensity);

				float distance = length(mul(UNITY_MATRIX_MV, v.vertex));
				o.fog = max(min((unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart), 1.0), 0.0);

				TRANSFER_VERTEX_TO_FRAGMENT(o);

                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * i.lighting;
				if (col.a < _Cutoff)
					discard;

				col = lerp(unity_FogColor, col * (_Emission + 1.0), i.fog);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
