//OK HACKING UP WHAT I FOUND AT http://www.alanzucconi.com/2015/07/01/vertex-and-fragment-shaders-in-unity3d/
//IS HOW I DID THIS. The line with "Zwrite On" etc. is the last change that made it work, not sure why, could be the
//blend - can look into later.
Shader "Custom/ColorTapeShader"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
		_TapeColor("Tape Color", color) = (0.5,0.5,0.5,1)
		_Opacity ("Opacity", Float) = 0.5
	}

	SubShader
	{
		//set up for transparent rendering...? -1 to stay behind the bezel? Doesn't help
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

		//orig-  No culling or depth
		//Cull Off ZWrite Off ZTest Always
		//zample - SOMETHING IN THIS IS WHAT MADE IT WORK as opposed to the list above, which didn't.
		//...well, no, now it's working without...
		//changing it back to this made it work with quad bezel. Probably the z test.... yup
		ZWrite On Lighting Off Cull Off Fog { Mode Off } Blend One Zero

		//get what underlies this - actually this snags the whole screen! Stuff below fiddles it
		GrabPass{"_GrabTexture"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct vin_vct
			{
				float4 vertex : POSITION;
			};
 
			struct v2f_vct
			{
				float4 vertex : POSITION;
				float4 uvgrab : TEXCOORD1;
			};
 
			// Vertex function 
			v2f_vct vert (vin_vct v)
			{
				v2f_vct o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uvgrab = ComputeGrabScreenPos(o.vertex);
				return o;
			}
 
			float _Opacity;
			fixed4 _TapeColor;
			sampler2D _GrabTexture;

			// Fragment function
			half4 frag (v2f_vct i) : COLOR
			{
				fixed4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				return col - (_Opacity * (1- _TapeColor));
			}
		
			ENDCG
		}
	}
}
