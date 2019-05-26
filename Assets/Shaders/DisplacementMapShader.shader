Shader "Unlit/DisplacementMapShader"
{
	// Tutorial - Vertex und Fragment Shader examples: https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
	// DOC: How to write vertex and fragment shaders: https://docs.unity3d.com/Manual/SL-ShaderPrograms.html

	// Property Definition --> Visible in IDE
	Properties
	{
    _MainTex ("Texture", 2D) = "white" {}
    _Scale ("Terrain Scale", Range(0, 1000)) = 1000
	}

	// A Shader can contain one or more SubShaders, which are primarily used to implement shaders for different GPU capabilities
	SubShader
	{
		// Subshaders use tags to tell how and when
		// they expect to be rendered to the rendering engine.
		// https://docs.unity3d.com/Manual/SL-SubShaderTags.html
		Tags { "RenderType"="Opaque" }

		// Each SubShader is composed of a number of passes, and each Pass represents an execution of the vertex and fragment
		// code for the same object rendered with the material of the shader
		Pass
		{
			// CGPROGRAM ... ENDCG
			// These keywords surround portions of HLSL code within the vertex and fragment shaders
			CGPROGRAM

			// Definition shaders used and their function names
			#pragma vertex vert
			#pragma fragment frag

			// Builtin Includes
			// https://docs.unity3d.com/Manual/SL-BuiltinIncludes.html
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Scale;

			// struct to pass Data from Vertex Sahder to Fragment Shader
			struct v2f
			{
				// SV_POSITION: Shader semantic for position in Clip Space: https://docs.unity3d.com/Manual/SL-ShaderSemantics.html?_ga=2.64760810.432960686.1524081652-394573263.1524081652
				float4 vertex : SV_POSITION;
				float4 col : COLOR;
			};

			float _MaxDepth;

			// VERTEX SHADER
			// https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html
			// http://wiki.unity3d.com/index.php?title=Shader_Code
			v2f vert (appdata_full v)
			{
				v2f o;

				// get vertex Data
				float4 vertexPos = v.vertex;

				// Access texture and extract color value
				fixed4 texVal = tex2Dlod(_MainTex, float4(v.texcoord.xy, 0, 0));

				// displace z value of vertex by texture value multiplied with Scale
				vertexPos.z =  _Scale*-texVal.x;

				// Convert Vertex Data from Object to Clip Space
				o.vertex = UnityObjectToClipPos(vertexPos);

				// set texture value as color.
				o.col = texVal;

				return o;
			}

			// FRAGMENT / PIXEL SHADER
			// SV_Target: Shader semantic render target (SV_Target = SV_Target0): https://docs.unity3d.com/Manual/SL-ShaderSemantics.html?_ga=2.64760810.432960686.1524081652-394573263.1524081652
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.col;

				return col;
			}
			ENDCG
		}
	}
}
