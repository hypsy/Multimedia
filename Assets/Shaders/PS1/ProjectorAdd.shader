Shader "Custom/Projector Add" {
   Properties {
      _ShadowTex ("Projected Image", 2D) = "white" {}
      _JitterResolution("Jitter Resolution", Int) = 300
   }
   SubShader {
      Pass {      
         Blend SrcAlpha OneMinusSrcAlpha
            // add color of _ShadowTex to the color in the framebuffer 
         ZWrite Off // don't change depths
         Offset -1, -1 // avoid depth fighting (should be "Offset -1, -1")
         Lighting On

         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag
 
         #include "AutoLight.cginc"
			#include "Lighting.cginc"

         // User-specified properties
         uniform sampler2D _ShadowTex; 
         int _JitterResolution;
 
         // Projector-specific uniforms
         uniform float4x4 unity_Projector; // transformation matrix 
            // from object space to projector space 
 
          struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posProj : TEXCOORD0;
            float fog : TEXCOORD1;
               // position in projector space
         };
 
         uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

         vertexOutput vert(vertexInput v) 
         {
            vertexOutput o;

            o.posProj = mul(unity_Projector, v.vertex);
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

            float distance = length(mul(UNITY_MATRIX_MV, v.vertex));
				o.fog = max(min((unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart), 1.0), 0.0);

            return o;
         }
 
 
         float4 frag(vertexOutput input) : COLOR
         {
            if (input.posProj.w > 0.0) // in front of projector?
            {
               float4 col = tex2D(_ShadowTex, input.posProj.xy / input.posProj.w); 
               if(col.a > 0.0)
                  col = lerp(unity_FogColor, col, input.fog);
               col.a *= saturate(1.0 - input.posProj.w * 0.05);
               return col;
               // alternatively: return tex2Dproj(  
               //    _ShadowTex, input.posProj);
            }
            else // behind projector
            {
               return float4(0.0, 0.0, 0.0, 0.0);
            }
         }
 
         ENDCG
      }
   }  
   Fallback "Projector/Light"
}