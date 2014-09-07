Shader "Custom/Exposure" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ExposureValue("Exposure Value", Range(-2, 6)) = 0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off Lighting Off  
        CGPROGRAM
        #pragma surface surf Lambert

        
        

        struct Input 
        {
            float2 uv_MainTex;
        };
        
        sampler2D _MainTex;
        uniform float _ExposureValue ;
        
        void surf (Input IN, inout SurfaceOutput o)
         {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            float red =  c.r ;
            float green = c.g ;
            float blue = c.b ;
            red = red*pow(2, _ExposureValue) ;
            green = green*pow(2, _ExposureValue) ;
            blue = blue*pow(2, _ExposureValue) ;
             
            o.Albedo = float3 (red, green, blue) ;    
            o.Alpha = c.a;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}