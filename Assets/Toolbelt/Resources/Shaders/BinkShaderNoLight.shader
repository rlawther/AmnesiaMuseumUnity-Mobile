Shader "RAD/BinkShaderNoLight" {
	Properties {
	    yTexture ("yTexture", 2D) = "white" { }
	    cBTexture ("cTexture", 2D) = "white" { }
	    cRTexture ("bTexture", 2D) = "white" { }
	    aTexture ("aTexture", 2D) = "white" { }	    
	}
  
	SubShader {
		
	    //Blend SrcAlpha OneMinusSrcAlpha
		
	    Pass {
	    	
			CGPROGRAM
			#pragma vertex vert_img
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
						
			
			float4 frag (v2f i) : COLOR
			{
				return YUV_to_RGB(i.uv, yTexture,cBTexture,cRTexture,aTexture);					
			}
			ENDCG
	    }

	}
	FallBack "Diffuse"
}
