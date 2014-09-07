Shader "Custom/Vignette" {

    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Brightness("Brightness Value", Range(-0.5, 0.5)) = 0
      _Contrast("Contrast Value", Range(-0.5, 0.5)) = 0
      _VignetteStart("vignette start", Range(-0.3, 0.3)) = 0.25
      _VignetteEnd("vignette end", Range(0.9, 0.4)) = 0.75
    }
    SubShader 
    {
      Tags { "RenderType" = "Opaque" }
      Cull Off Lighting Off  
      CGPROGRAM
      #pragma surface surf Lambert
      
      struct Input 
      {
          float2 uv_MainTex;
      };
      uniform float _Brightness ;
      uniform float _Contrast ;
      uniform float _VignetteStart ;
      uniform float _VignetteEnd ;
      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) 
      {         
              float2 vignetteCenter = (0.5, 0.5) ;
              float3 vignettecolor = (0, 0, 0) ;       
              float red = tex2D (_MainTex, IN.uv_MainTex).r ;
              float green = tex2D (_MainTex, IN.uv_MainTex).g ;
              float blue = tex2D (_MainTex, IN.uv_MainTex).b ;
            float d= distance(IN.uv_MainTex, vignetteCenter) ;
            float percent = smoothstep(_VignetteStart, _VignetteEnd, d) ;
            float3 mixed = lerp(tex2D (_MainTex, IN.uv_MainTex).rgb, vignettecolor, percent) ;

            
            o.Albedo = mixed.xyz ;
            o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a ;
      }
      
      
      
      ENDCG
    } 
    Fallback "Diffuse"
    
}