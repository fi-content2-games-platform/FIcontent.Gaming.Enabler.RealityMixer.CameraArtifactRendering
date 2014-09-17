Shader "Custom/MergeTextures" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_FgTexture ("Color", 2D) = "grey" {}
		_BgTexture ("Color", 2D) = "grey" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
	};

	sampler2D _FgTexture;
	sampler2D _BgTexture;
	sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	half4 _UV_Transform = half4(1, 0, 0, 1);

	v2f vert( appdata_img v ) { 
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv[0] =  v.texcoord.xy;	
		return o;
	}


	half4 fragAlphaBlend (v2f i) : COLOR {
		half4 toAdd = tex2D(_FgTexture, i.uv[0]) ;

		return lerp(tex2D(_BgTexture, i.uv[0]), toAdd, toAdd.a);
	}	


	ENDCG 

Subshader {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }  
      ColorMask RGB	  
  	
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAlphaBlend
      ENDCG
  }   
}

Fallback off

} // shader
