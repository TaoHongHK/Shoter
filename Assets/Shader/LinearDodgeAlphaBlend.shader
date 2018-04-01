﻿Shader "Custom/LinearDodgeAlphaBlend" {
	Properties 
	{
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Tex", 2D) = "white" {}
		_AlphaScale ("Alpha Scale", Range(0, 1)) = 1
	}

	SubShader 
	{
		Tags 
		{
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}
		
		Pass 
		{
			ZWrite Off
			Blend SrcAlpha One
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
						
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _AlphaScale;
			
			struct a2v 
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD2;
			};
			
			v2f vert(a2v v) 
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);				
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target 
			{				
				fixed4 texColor = tex2D(_MainTex, i.uv);
				
				fixed3 albedo = texColor.rgb * _Color.rgb;
				
				return fixed4(albedo, texColor.a * _AlphaScale);
			}
			
			ENDCG
		}
	} 
	FallBack "Transparent/VertexLit"
}
