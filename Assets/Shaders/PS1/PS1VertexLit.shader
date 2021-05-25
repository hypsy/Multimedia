Shader "PS1/VertexLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Emission("Emission", Float) = 0.0
		_JitterResolution("Jitter Resolution", Int) = 300
		_DepthLigthingIntensity("Depth Lighting Intensity", Range(0.0, 1.0)) = 0.5
        [KeywordEnum(None, Overlay, Add, Multiply)] _DetailTextureMode("Detail Texture Mode", Int) = 0
        _DetailTexture("Detail Texture", 2D) = "transparent" {}
        _DetailTextureScale("Detail Texture Scale", Float) = 4
        _DetailTextureIntensity("Detail Texture Intensity", Range(0.0, 1.0)) = 0.0
        [KeywordEnum(None, Color, Clip)] _VertexColorMode("Vertex Color Mode", Int) = 1
        _VertexColorClipFactor("Vertex Color Clip Factor", Float) = 1.0
        [KeywordEnum(Both, Front, Back)] _TwoSided("Two Sided", Int) = 2
    }
    SubShader
    {

        LOD 100

        Pass
        {        
            Tags {
			    "RenderType" = "Opaque"
                "Queue" = "Geometry"
		    }
            ZWrite On
			Lighting On

            Cull [_TwoSided]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag addshadow
            #pragma multi_compile _DETAILTEXTUREMODE_NONE _DETAILTEXTUREMODE_OVERLAY _DETAILTEXTUREMODE_ADD _DETAILTEXTUREMODE_MULTIPLY
            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_COLOR _VERTEXCOLORMODE_CLIP

            #include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
				float4 lighting : COLOR0;
				float fog : TEXCOORD1;
                float2 detailUV : TEXCOORD2;
				LIGHTING_COORDS(2, 3)
				float4 screenPos : TEXCOORD4;
                fixed4 color : COLOR1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Emission;
			int _JitterResolution;
			float _DepthLigthingIntensity;

            sampler2D _DetailTexture;
            float _DetailTextureScale;
            float _DetailTextureIntensity;

            int _VertexColorMode;
            float _VertexColorClipFactor;

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
				float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				// o.lighting = lerp(unity_AmbientSky, 1.0 - _LightColor0, min(0.0, dot(worldNormal, 1.0 - _WorldSpaceLightPos0.xyz)));
				// o.lighting += max(0.0, dot(v.normal, float3(0.0, 1.0, 0.0))) * _DepthLigthingIntensity;
				// o.lighting *= lerp(1.0, 1.0 - max(0.0, dot(v.normal, float3(0.0, -1.0, 0.0))), _DepthLigthingIntensity);

                //Detail UVs
                #ifndef _DETAILTEXTUREMODE_NONE
                    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                    float dotToX = dot(worldNormal, float3(1.0, 0.0, 0.0));
                    float dotToY = dot(worldNormal, float3(0.0, 1.0, 0.0));
                    float dotToZ = dot(worldNormal, float3(0.0, 0.0, 1.0));
                    //X
                    if(dotToX > 0.5 || dotToX < -0.5)
                        o.detailUV = float2(worldPos.z, worldPos.y) * 0.1;
                    //Y
                    if(dotToY > 0.5 || dotToY < -0.5)
                        o.detailUV = float2(worldPos.x, worldPos.z) * 0.1;
                    //Z
                    if(dotToZ > 0.5 || dotToZ < -0.5)
                        o.detailUV = float2(worldPos.x, worldPos.y) * 0.1;
                #else
                    o.detailUV = o.uv;
                #endif
                
				float distance = length(mul(UNITY_MATRIX_MV, v.vertex));
				o.fog = max(min((unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart), 1.0), 0.0);

                o.color = v.color;

				TRANSFER_VERTEX_TO_FRAGMENT(o);

                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
                //apply detail texture
                #ifndef _DETAILTEXTUREMODE_NONE
                    fixed4 detailCol = tex2D(_DetailTexture, i.detailUV * (1.0 / _DetailTextureScale));
                    #ifdef _DETAILTEXTUREMODE_OVERLAY
                        col = lerp(col, detailCol, detailCol.a * _DetailTextureIntensity);
                    #elif _DETAILTEXTUREMODE_ADD
                        col.rgb += detailCol.rgb * detailCol.a * _DetailTextureIntensity;
                    #elif _DETAILTEXTUREMODE_MULTIPLY
                        col = lerp(col, detailCol * col, detailCol.a * _DetailTextureIntensity);
                    #endif
                #endif
                col *= i.lighting * (_Emission + 1.0);
                #ifdef _VERTEXCOLORMODE_COLOR
                    col *= i.color;
                #elif _VERTEXCOLORMODE_CLIP
                    if((col.r * 0.3 + col.g * 0.59 + col.b * 0.11) > pow(i.color.r, _VertexColorClipFactor))
                        discard;
                #endif
				col = lerp(unity_FogColor, col, i.fog);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
