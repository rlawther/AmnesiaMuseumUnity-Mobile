Shader "Custom/overlay" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Layer1 ("Layer1 (RGBA)", 2D) = "black" {}
		_Layer2 ("Layer2 (RGBA)", 2D) = "black" {}
		_Layer3 ("Layer3 (RGBA)", 2D) = "black" {}
		_Layer4 ("Layer4 (RGBA)", 2D) = "black" {}
		_Layer5 ("Layer5 (RGBA)", 2D) = "black" {}
		_Layer6 ("Layer6 (RGBA)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma exclude_renderers flash

		sampler2D _MainTex;
		sampler2D _Layer1;
		sampler2D _Layer2;
		sampler2D _Layer3;
		sampler2D _Layer4;
		sampler2D _Layer5;
		sampler2D _Layer6;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float alpha;
			half4 basecol = tex2D (_MainTex, IN.uv_MainTex);
			half4 layer1col = tex2D (_Layer1, IN.uv_MainTex);
			half4 layer2col = tex2D (_Layer2, IN.uv_MainTex);
			half4 layer3col = tex2D (_Layer3, IN.uv_MainTex);
			half4 layer4col = tex2D (_Layer4, IN.uv_MainTex);
			half4 layer5col = tex2D (_Layer5, IN.uv_MainTex);
			half4 layer6col = tex2D (_Layer6, IN.uv_MainTex);
			half4 colour;
			
			
			colour = (layer1col * layer1col.a) + 
			  (layer2col * layer2col.a) +
			  (layer3col * layer3col.a) +
			  (layer4col * layer4col.a) +
			  (layer5col * layer5col.a) +
			  (layer6col * layer6col.a);
			  
			//colour = clamp(colour, 0, 1);
			  
			alpha = 1.0 -
			  layer1col.a -
			  layer2col.a -
			  layer3col.a -
			  layer4col.a -
			  layer5col.a -
			  layer6col.a;
			 			
			alpha = clamp(alpha, 0, 1);
			colour += (basecol * alpha);

			o.Albedo = colour.rgb;
			o.Alpha = basecol.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

