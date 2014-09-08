Shader "WireFrame/Transparent"
{
    Properties
    {
        _Color ("Line Color", Color) = (1,1,1,1)
        _GridColor ("Grid Color", Color) = (0,0,0,0)
        _LineWidth ("Line Width", float) = 0.05     
    }
    SubShader
    {
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members dist)
#pragma exclude_renderers d3d11 xbox360
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float4 _Color;
			uniform float4 _GridColor;
			uniform float _LineWidth;
			
			// vertex input: position, uv1, uv2
			struct appdata {
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
			    
			};
			
			struct v2f {			    
				float4 pos : POSITION;
			    float4 texcoord0 : TEXCOORD0;			    
			    float4 distanceCamera: TEXCOORD1;
			};
			
			v2f vert (appdata v) {
			    v2f o;
			    o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
			    o.texcoord0 = v.texcoord0;			    
			    o.distanceCamera = float4(distance(_WorldSpaceCameraPos, mul(_Object2World,v.vertex)),0,0,0);
			    return o;
			}
			
			float4 frag(v2f i ) : COLOR
			{
			    float2 uv = i.texcoord0;
			    float d = uv.x - uv.y;
			    float clampDistance = clamp(i.distanceCamera.x,0.0,500);
			    float lw = _LineWidth * (1 - clampDistance/500);
		    	if (uv.x < lw) // 0,0 to 1,0		    	
			        return _Color;
			    else if(uv.x > 1 - lw)             // 1,0 to 1,1
			        return _Color;
			    else if(uv.y < lw)                 // 0,0 to 0,1
			        return _Color;
			    else if(uv.y > 1 - lw)             // 0,1 to 1,1
			        return _Color;
			    else if(d < lw && d > 1 - lw) // 0,0 to 1,1
			        return _Color;
			    else
                    return _GridColor;
			}
			ENDCG
        }
    }
    Fallback "Vertex Colored", 1
}