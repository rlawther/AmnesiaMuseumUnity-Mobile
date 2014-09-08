#include "HSL.cginc" 
#include "GammaAdjust.cginc" 
#include "StereoAdjust.cginc" 

inline SurfaceOutput ImageShaderFunc( inout SurfaceOutput o,
								float2 uv_in,
								sampler2D _MainTex, 
								float4 _Color, 
								int _StereoType,
								int _uEye,
								float3 _HsvAdjust, 
								int _UseLighting, 
								sampler2D _uGammaLUT)
{
	uv_in = stereoAdjust(uv_in, _StereoType, _uEye <= 0);

	float4 c = tex2D (_MainTex, uv_in);
				
	//Multiply by final colour
	c *= _Color;
				
	//HSL correction
	c.rgb = doHSLCorrection(c.rgb, _HsvAdjust.rgb); 

	// Do gamma correction
	c.rgb = doGammaCorrection(c.rgb, _uGammaLUT);

	// If we are not using lighting, then we use Emission instead of Albedo
	o.Emission = clamp(c.rgb * (1 - _UseLighting), 0.0, 1.0);
	o.Albedo = clamp(c.rgb * _UseLighting, 0.0, 1.0);
	o.Alpha = clamp(c.a, 0.0, 1.0);
	
	return o;
}

