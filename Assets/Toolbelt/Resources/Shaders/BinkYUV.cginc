inline float4 YUV_to_RGB (float2 uv, sampler2D yTex, sampler2D cBTex, sampler2D cRTex, sampler2D aTex) {
	float4 crc = float4(1.595, -0.813, 0, 0);
    float4 crb = float4(0, -0.391, 2.017, 0);
    float4 adj = float4(-0.870, 0.529, -1.0816, 0);	    
    
    float4 ytexcol = tex2D (yTex, uv);
    float4 cBtexcol = tex2D (cBTex, uv);
    float4 cRtexcol = tex2D (cRTex, uv);
    float4 atexcol = tex2D (aTex, uv);
    
    float Y = ytexcol.x;
    float cB = cBtexcol.x;
    float cR = cRtexcol.x;
    float A = atexcol.x;
    
    float4 p = float4(Y * 1.164, Y * 1.164, Y * 1.164, A);
    p += crc * cR;
    p += crb * cB;
   	p += adj;				   	

    return p;
    
}