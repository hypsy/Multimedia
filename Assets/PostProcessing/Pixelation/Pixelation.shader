Shader "Hidden/Pixelation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LUT("LUT", 3D) = "white" {}
		_Pixels("Pixels", Range(250,800)) = 300
		_DitheringMatrix("Dirething Matrix", 2D) = "white" {}
		_DitheringMatrixSize("Dithering Matrix Size", Int) = 16
		_ColorSpread("Color Spread", Float) = 0.125
		_NumColors("Num Colors", Int) = 16
		_LowerBound("Lower Bound", Range(0.0, 1.0)) = 0.0
		_UpperBound("Uppe Bound", Range(0.0, 1.0)) = 1.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile COLORREMAPMODE_REDUCE COLORREMAPMODE_LUT
			#pragma multi_compile COLORREDUCTIONMODE_PRE COLORREDUCTIONMODE_POST
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			uint _Pixels;

			sampler2D _DitheringMatrix;
			uint _DitheringMatrixSize;

			sampler3D _LUT;
			float _ColorSpread;
			uint _NumColors;

			float _LowerBound;
			float _UpperBound;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;

				//quantize screenpos
				uv.x *= _Pixels;
				uv.y *= _Pixels / 2;
				uv.x = round(uv.x);
				uv.y = round(uv.y);
				uv.x /= _Pixels;
				uv.y /= _Pixels / 2;

				//normalize screenpos
				float2 ditheringUV = i.uv;
				ditheringUV.x *= _Pixels;
				ditheringUV.y *= _Pixels / 2;
				ditheringUV.xy += 0.5;
				ditheringUV.xy /= _DitheringMatrixSize;
				ditheringUV %= 1.0;

				fixed4 col = (tex2D(_MainTex, uv));
				#ifdef COLORREDUCTIONMODE_POST
					col = (col + _ColorSpread * (tex2D(_DitheringMatrix, ditheringUV)));
				#endif

				#ifdef COLORREMAPMODE_REDUCE
				col.r *= _NumColors;
				col.g *= _NumColors;
				col.b *= _NumColors;
				col.r = round(col.r);
				col.g = round(col.g);
				col.b = round(col.b);
				col.r /= _NumColors;
				col.g /= _NumColors;
				col.b /= _NumColors;
				#elif COLORREMAPMODE_LUT
				col = tex3D(_LUT, col.rgb);
				#endif

				#ifdef COLORREDUCTIONMODE_PRE
					col = (col + _ColorSpread * (tex2D(_DitheringMatrix, ditheringUV)));
				#endif

				//Range remapping
				float b = (col.r + col.g + col.b) / 3.0;
				float nb = saturate((1.0 / (_UpperBound - _LowerBound)) * (b - _LowerBound));
				float f = nb / b;
				col *= f;

				return col;
			}
			ENDCG
		}
	}
}
