Shader "Self Illuminated AVIE Blended" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_BlendTex ("Blend (RGBA)", 2D) = "white" {}
}

SubShader {
	Material {
		Ambient [_Color]
	}
	Pass {
		Cull Off
		SetTexture [_MainTex] { combine texture }
		SetTexture [_BlendTex] { combine texture * previous }
	}
}

Fallback "VertexLit", 1

}
