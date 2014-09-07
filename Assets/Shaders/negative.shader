Shader "Custom/Negative" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NegativeAmount("Amount", Range(0, 1)) = 0
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
        uniform float _NegativeAmount ;
        
        void surf (Input IN, inout SurfaceOutput o)
         {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            float red =  c.r ;
            float green = c.g ;
            float blue = c.b ;
            red = _NegativeAmount - red ;
            green = _NegativeAmount- green ;
            blue = _NegativeAmount- blue ;
            
            if(red < 0)
            red = -red ;
            if(green <0)
            green = -green ;
            if(blue <0)
            blue = -blue ;
 
            o.Albedo = float3 (red, green, blue) ;    
            o.Alpha = c.a;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}
