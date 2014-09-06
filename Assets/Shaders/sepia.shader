Shader "Custom/Sepia" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SepiaAmount("Sepia Amount", Range(0, 1)) = 0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off Lighting Off  
        CGPROGRAM
        #pragma surface surf Lambert
        #pragma target 2.0 
        
        

        struct Input 
        {
            float2 uv_MainTex;
        };
        
        sampler2D _MainTex;
        uniform float _SepiaAmount ;
        
        void surf (Input IN, inout SurfaceOutput o)
         {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            float red =  c.r ;
            float green = c.g ;
            float blue = c.b ;
            
                 
            float gr = 0.30 * red + 0.59 * green + 0.11 * blue; // get brightness
            float r2, g2, b2 ;
            r2 = red ;
            g2 = green ;
            b2 = blue ;
            r2 = r2 *(1 - _SepiaAmount) + gr * 1 * _SepiaAmount ; // add tint
            g2 = g2 *(1 - _SepiaAmount)+ gr * 0.71 * _SepiaAmount ;
            b2 = b2 *(1 - _SepiaAmount)+ gr * 0.41 * _SepiaAmount ;
            
            
            o.Albedo = float3 (r2, g2, b2) ;     
            o.Alpha = c.a;
        }
        ENDCG
    } 
    FallBack "Diffuse"
} 