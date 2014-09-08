Shader "Custom/DepthToColour" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_nearClipMetres ("Near clip, metres", Float) = 0.9
		_farClipMetres ("Far clip, metres", Float) = 1000.0
	}
	SubShader {
        Pass 
        {
            cull off
            ZWrite On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"            
            sampler2D _CameraDepthTexture;
 			float _nearClipMetres;
 			float _farClipMetres;
            v2f_img vert( appdata_img v )
            {
                return vert_img(v);
            }
 
  			struct C2E2f_Output {
  				float4 col:COLOR;
                //float dep:DEPTH;
            };     
            
            
            //float4 _ZBufferParams (0-1 range): 
			//x is 1.0 - (camera's far plane) / (camera's near plane)
			//y is (camera's far plane) / (camera's near plane)
			//z is x / (camera's far plane)
			//w is y / (camera's far plane)

			// Z buffer to linear 0..1 depth (0 at eye, 1 at far plane)
			inline float Linear01Depth( float depth, float x, float y)//, float z, float w )
			{
				return 1.0 / (x * depth + y);
			}
			
            C2E2f_Output frag( v2f_img i ) {
           
                C2E2f_Output o;
                float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv.xy)) ; //atm depth is non-linear. 0m = 0.9, 1000f = 1.0, 5m = 0.654
                
                //due to unity being totally lame, we have to reconstruct the _ZBufferParams.
                float near = _nearClipMetres;
                float far = _farClipMetres;
                
                float x = 1.0 - far/near;
                float y = far/near;
                //float z = x/far; 
                //float w = y/far; 
                
                d = Linear01Depth(d,x,y);//,z,w); 
                
				o.col = float4(d,d,d,1);
                
                return o;
            }
 
            ENDCG
        }
	} 
	FallBack "Diffuse"
}
