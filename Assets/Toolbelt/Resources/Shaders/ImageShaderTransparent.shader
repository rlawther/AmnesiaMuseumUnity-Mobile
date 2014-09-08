Shader "iCinema/ImageShaderTransparent" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_HsvAdjust("HSV Adjust", Color) = (1,1,1)
		_StereoType("Stereo Mode", Float)= 0.0
		_uGammaLUT("Gamma Correction", 2D) = "white" {}
		_uEye("Eye", Float) = 0
		_UseLighting("Use Lighting", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Transparent"
			   "Queue"="Transparent" }
		LOD 200
		
		CGPROGRAM 
		#pragma surface surf Lambert alpha
		#pragma target 3.0 
		#pragma exclude_renderers gles

		#include "ImageShaderFunction.cginc"
		
		uniform sampler2D _MainTex;
		uniform float4 _Color;
		uniform int _StereoType; //used by StereoAdjust
		uniform int _uEye; //used by StereoAdjust		
		uniform float3 _HsvAdjust; //used by HSL
		uniform int _UseLighting;
		sampler2D _uGammaLUT; //used by GammaAdjust
		
		struct Input {
			float2 uv_MainTex;			
		};
		  
		void surf (Input IN, inout SurfaceOutput o) 
		{
			ImageShaderFunc(o, IN.uv_MainTex, _MainTex, _Color, _StereoType, _uEye, _HsvAdjust, _UseLighting, _uGammaLUT);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
