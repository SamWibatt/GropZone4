Shader "GrabPassInvert"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TapeColor("Tape Color", color) = (0.5,0.5,0.5,1)
		_Opacity ("Opacity", Float) = 0.5
	}

    SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _GrabTexture
        //actually appears to grab everything all over the entire screen.
        GrabPass { }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            SetTexture [_GrabTexture] { combine one-texture }
        }
    }
}