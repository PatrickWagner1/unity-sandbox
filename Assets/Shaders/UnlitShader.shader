Shader "Custom/UnlitShader"
{
    Properties {
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalMap2 ("Normal Map 2", 2D) = "bump" {}
        _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
        _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
        _Shininess ("Shininess", Float) = 10
        _ContourLinesColor("Contour Lines Color", Color) = (1,1,1,1)
        _ContourLinesDistance("Contour Lines Distance", float) = 10
    }
    SubShader {
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform float4 _LightColor0;
            uniform int _SHOW_CONTOUR_LINES;

            uniform sampler2D _NormalMap;   
            uniform float4 _NormalMap_ST;
            uniform sampler2D _NormalMap2;
            uniform float4 _NormalMap2_ST;
            uniform float4 _Color; 
            uniform float4 _SpecColor; 
            uniform float _Shininess;
            uniform float4 _ContourLinesColor;
            uniform float _ContourLinesDistance;

            struct appdata {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 color : COLOR;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float4 tex : TEXCOORD1;
                float3 tangentWorld : TEXCOORD2;  
                float3 normalWorld : TEXCOORD3;
                float3 binormalWorld : TEXCOORD4;
                float4 color : COLOR;
                float height : POSITION_HEIGHT;
            };
            
            v2f vert(appdata v) {
                v2f o;

                o.tangentWorld = normalize(mul(unity_ObjectToWorld,
                float4(v.tangent.xyz, 0.0)).xyz);
                
                o.normalWorld = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.binormalWorld = normalize(cross(o.normalWorld, o.tangentWorld) * v.tangent.w);

                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.tex = v.texcoord;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.color = v.color;
                o.height = v.vertex.y;

                return o;			
            }
            
            float4 frag(v2f i) : COLOR {

                float3 lightDirection;
                float attenuation;

                attenuation = 1.0; // no attenuation
                lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                
                float4 color;
                float3 diffuseReflection;

                // Sets the color for the water
                if (i.height == 0)
                {
                    float4 encodedNormal = normalize(
                    tex2D(_NormalMap, _NormalMap_ST.xy * i.tex.xy + _NormalMap_ST.zw)
                    + tex2D(_NormalMap2, _NormalMap2_ST.xy * i.tex.xy + _NormalMap2_ST.zw));

                    float3 localCoords = float3(2.0 * encodedNormal.a - 1.0, 2.0 * encodedNormal.g
                    - 1.0, 0.0);

                    localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));

                    float3x3 local2WorldTranspose =
                    float3x3(i.tangentWorld, i.binormalWorld, i.normalWorld);

                    float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

                    float3 viewDirection = normalize(
                    _WorldSpaceCameraPos - i.posWorld.xyz);

                    diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb
                    * max(0.0, dot(normalDirection, lightDirection));

                    float3 specularReflection;
                    if (dot(normalDirection, lightDirection) < 0.0) 
                    // light source on the wrong side?
                    {
                        specularReflection = float3(0.0, 0.0, 0.0); 
                        // no specular reflection
                    }
                    else // light source on the right side
                    {
                        specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb
                        * pow(max(0.0, dot(reflect(-lightDirection, normalDirection),
                        viewDirection)), _Shininess);
                    }
                    color = float4(diffuseReflection + specularReflection, 1.0);
                }
                // Sets the color for the terrain
                else {
                    diffuseReflection = attenuation * _LightColor0.rgb
                    * max(0.0, dot(i.normalWorld, lightDirection));

                    color = i.color -1 + float4(diffuseReflection, 1.0);
                }

                // Adding contour lines, if selected in the UI
                if (_SHOW_CONTOUR_LINES > 0)
                {
                    float sinAngle = length(dot(i.normalWorld, float3(0,1,0)))
                    / (length(i.normalWorld) * length(float3(0,1,0)));

                    float angleConstant = 0.04 + 0.3 * (1 - sinAngle);

                    if (i.height % _ContourLinesDistance > _ContourLinesDistance - angleConstant
                    && sinAngle < 0.99)
                    {
                        color = _ContourLinesColor;
                    }
                }
                
                return color;
            }
            
            ENDCG	
        }		
    }
}
