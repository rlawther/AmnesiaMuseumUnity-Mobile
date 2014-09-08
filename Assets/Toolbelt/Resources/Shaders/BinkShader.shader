Shader "RAD/BinkShader" {
	Properties {
	    yTexture ("yTexture", 2D) = "white" { }
	    cBTexture ("cBTexture", 2D) = "white" { }
	    cRTexture ("cRTexture", 2D) = "white" { }
	    aTexture ("aTexture", 2D) = "white" { }
	}
  
	SubShader {

	    Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "BinkYUV.cginc"
			
			sampler2D yTexture;
			sampler2D cBTexture;
			sampler2D cRTexture;
			sampler2D aTexture;
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};
			
			float4 yTexture_ST;
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, yTexture); //since textures are -1 width etc they need flipping.
			    return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				return YUV_to_RGB (i.uv, yTexture, cBTexture, cRTexture, aTexture); 				
			}
			ENDCG
	    }
	}
	FallBack "Diffuse"
}


