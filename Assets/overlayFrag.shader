Shader "Custom/overlayFrag" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_Layer1 ("Layer1 (RGBA)", 2D) = "black" {}
		_Layer2 ("Layer2 (RGBA)", 2D) = "black" {}
		_Layer3 ("Layer3 (RGBA)", 2D) = "black" {}
		_Layer4 ("Layer4 (RGBA)", 2D) = "black" {}
		_Layer5 ("Layer5 (RGBA)", 2D) = "black" {}
		_Layer6 ("Layer6 (RGBA)", 2D) = "black" {}
	}
	SubShader {
        Tags {"Queue" = "Transparent"} 

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha 

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            //uniform sampler2D _MainTex;
            uniform sampler2D _Layer1;
            uniform sampler2D _Layer2;
            uniform sampler2D _Layer3;
            uniform sampler2D _Layer4;
            uniform sampler2D _Layer5;
            uniform sampler2D _Layer6;

            float4 frag(v2f_img i) : COLOR {
            	float4 colour;
   				float alpha;
            	//half4 basecol = tex2D (_MainTex, i.uv);
            	half4 layer1col = tex2D (_Layer1, i.uv);
            	half4 layer2col = tex2D (_Layer2, i.uv);
            	half4 layer3col = tex2D (_Layer3, i.uv);
            	half4 layer4col = tex2D (_Layer4, i.uv);
            	half4 layer5col = tex2D (_Layer5, i.uv);
            	half4 layer6col = tex2D (_Layer6, i.uv);
            	
				colour = (layer1col * layer1col.a) + 
				  (layer2col * layer2col.a) +
				  (layer3col * layer3col.a) +
				  (layer4col * layer4col.a) +
				  (layer5col * layer5col.a) +
				  (layer6col * layer6col.a);
                
                //colour += tex2D(_MainTex, i.uv);
	            alpha = 1.0 -
	            	layer1col.a - 
	            	layer3col.a - 
	            	layer4col.a - 
	            	layer5col.a - 
	            	layer6col.a - 
	            	layer2col.a;
				  
				 			
				// faster to not have this??
				if (alpha >= 0.95)
					discard;
					
				alpha = clamp(alpha, 0, 1);
				colour.a = 1.0 - alpha;

                return colour;
            }
            ENDCG
        }
    }
	FallBack "Diffuse"
}
