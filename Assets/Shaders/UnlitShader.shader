// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/UnlitShader"
{
    Properties {
    }
    SubShader {
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            uniform float4 _LightColor0;
            uniform int _SHOW_CONTOUR_LINES;

            struct appdata {
                float4 vertex : POSITION;			
                float3 normal : NORMAL;
                float4 color : COLOR;
            };
            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float height : POSITION_HEIGHT;
            };
            
            v2f vert(appdata v) {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.normal = v.normal;
                o.height = v.vertex.y;
                return o;			
            }
            
            float4 frag(v2f i) : COLOR {

                float3 normalDirection = normalize( mul( float4( i.normal, 0.0 ), unity_WorldToObject ).xyz );
                float3 lightDirection;
                float atten = 1.0;
                
                lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                
                float3 diffuseReflection = atten * _LightColor0.xyz * max(0.0, dot(normalDirection, lightDirection));
                
                float4 color = i.color - 1 + float4(diffuseReflection,1.0);
                float height = i.height;
                if (_SHOW_CONTOUR_LINES > 0)
                {
                    float sinAngle = length(dot(i.normal, float3(0,1,0))) / (length(i.normal) * length(float3(0,1,0)));
                    float angleConstant = 0.1 + 0.5 * (1 - sinAngle);
                    if (height % 10 > 10 - angleConstant && sinAngle < 0.99)
                    {
                        color = float4(0,0,0,1);
                    }
                }
                
                return color;
            }
            
            ENDCG	
        }		
    }
}
