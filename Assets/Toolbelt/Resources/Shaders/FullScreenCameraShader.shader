Shader "iCinema/FullScreenCameraShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}		
		_HsvAdjust("HSV Adjust", Color) = (1,1,1)		
		_uGammaLUT("Gamma Correction", 2D) = "white" {}		
	}
	SubShader {		
		CGPROGRAM 
		#pragma surface surf Lambert
		#pragma target 3.0 
		#pragma exclude_renderers gles
		#include "HSL.cginc" 
		#include "GammaAdjust.cginc" 
		#include "StereoAdjust.cginc" 
		uniform sampler2D _MainTex;				
		uniform float3 _HsvAdjust; //used by HSL		
		sampler2D _uGammaLUT; //used by GammaAdjust
		
		struct Input {
			float2 uv_MainTex;			
		};
		  
		void surf (Input IN, inout SurfaceOutput o) 
		{			
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
						
			//HSL correction
			c.rgb = doHSLCorrection(c.rgb,_HsvAdjust.rgb); 
			
			// Do gamma correction
			c.rgb = doGammaCorrection(c.rgb,_uGammaLUT);
			
			// If we are not using lighting, then we use Emission instead of Albedo			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
