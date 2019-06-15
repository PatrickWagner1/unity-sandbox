// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TerrainShader"
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _HeightMin ("Height Min", Float) = 0
        _HeightMax ("Height Max", Float) = 1
        _Color1 ("Tint Color 1", Color) = (1,1,1,1)
        _Color2 ("Tint Color 2", Color) = (1,1,1,1)
        _Color3 ("Tint Color 3", Color) = (1,1,1,1)
        _Color4 ("Tint Color 4", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        CGPROGRAM
        #pragma surface surf Lambert
        
        sampler2D _MainTex;
        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        fixed4 _Color4;
        float _HeightMin;
        float _HeightMax;
        
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };
        
        void surf (Input IN, inout SurfaceOutput o) 
        {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            float h = (_HeightMax-IN.worldPos.y) / (_HeightMax-_HeightMin);

            float a_scaled = h * 3;
            float4 gradient_3_4 = lerp(_Color3, _Color4, saturate(a_scaled));
            float4 gradient_2_34 = lerp(_Color2, gradient_3_4, saturate(a_scaled));
            float4 gradient_1_2 = lerp(_Color1, _Color2, saturate(a_scaled));
            float4 gradient = lerp(_Color1, gradient_2_34, saturate(a_scaled));
            fixed4 tintColor = gradient;
            o.Albedo = c.rgb * tintColor.rgb;
            o.Alpha = c.a * tintColor.a;
        }
        ENDCG
    } 
    Fallback "Diffuse"
}
