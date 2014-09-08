inline float3 doGammaCorrection(float3 colour, sampler2D gammaLUT) {
	colour.r = tex2D(gammaLUT, float2(colour.r,0)).a;
	colour.g = tex2D(gammaLUT, float2(colour.g,0)).a;
	colour.b = tex2D(gammaLUT, float2(colour.b,0)).a;
	return colour;
}