﻿Shader "Custom/saturation" {
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _EffectAmount ("Effect Amount", Range (0, 1)) = 1.0
    }
 
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Lambert alpha
 
            sampler2D _MainTex;
            uniform float _EffectAmount;
 
            struct Input {
                float2 uv_MainTex;
            };
 
            void surf (Input IN, inout SurfaceOutput o) {
                half4 c = tex2D(_MainTex, IN.uv_MainTex);
                //o.Albedo = dot(c.rgb, float3(0.3, 0.59, 0.11));
                o.Albedo = lerp(c.rgb, dot(c.rgb, float3(1, 1, 1)), _EffectAmount);
                //o.Albedo = lerp(c.rgb, dot(c.rgb, float3(0.3, 0.59, 0.11)), _EffectAmount);
                o.Alpha = c.a;
            }
 
        ENDCG
    }
 
    Fallback "Transparent/VertexLit"
}

