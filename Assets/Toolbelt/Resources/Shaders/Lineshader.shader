Shader "Custom/Lineshader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {		
		Pass {
			
			
			CGPROGRAM 			
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			
			float4 vert(float4 v:POSITION) : SV_POSITION {
					return mul (UNITY_MATRIX_MVP, v);
				}

			fixed4 frag() : COLOR {
				return fixed4(1.0,1.0,1.0,1.0);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
