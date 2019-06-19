// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/UnlitShader"
{
    Properties {
        _Color("Color", Color) = (1, 1, 1, 1) //The color of our object
        _Tex("Pattern", 2D) = "white" {} //Optional texture

        _Shininess("Shininess", Float) = 10 //Shininess
        _SpecColor("Specular Color", Color) = (1, 1, 1, 1) //Specular highlights color

        _NormalMap("NormalMap", 2D) = "bump" {}
        _NormalMap2("NormalMap2", 2D) = "bump" {}
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

            uniform float4 _Color;
            uniform float4 _SpecColor;
            uniform float _Shininess;

            sampler2D _Tex; //Used for texture
            float4 _Tex_ST; //For tiling

            sampler2D _NormalMap;
            sampler2D _NormalMap2;

            struct appdata {
                float4 vertex : POSITION;			
                float3 normal : NORMAL;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float height : POSITION_HEIGHT;
                float4 posWorld : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert(appdata v) {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.normal = v.normal;
                o.height = v.vertex.y;

                o.posWorld = mul(unity_ObjectToWorld, v.vertex); //Calculate the world position for our point
                o.uv = TRANSFORM_TEX(v.uv, _Tex);

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
                
                if (height == 0)
                {

                    float3 normal = normalize((UnpackNormal(tex2D(_NormalMap, i.uv)) + UnpackNormal(tex2D(_NormalMap2, i.uv))));
                    //Phong Shader
                    float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);

                    float3 vert2LightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
                    float oneOverDistance = 1.0 / length(vert2LightSource);
                    float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w); //Optimization for spot lights. This isn't needed if you're just getting started.
                    lightDirection = _WorldSpaceLightPos0.xyz - i.posWorld.xyz * _WorldSpaceLightPos0.w;

                    float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb; //Ambient component
                    diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * max(0.0, dot(normalDirection, lightDirection)); //Diffuse component
                    float3 specularReflection;
                    if (dot(normal, lightDirection) < 0.0) //Light on the wrong side - no specular
                    {
                        specularReflection = float3(0.0, 0.0, 0.0);
                    }
                    else
                    {
                        //Specular component
                        specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
                    }

                    color = float4(((ambientLighting + diffuseReflection) * tex2D(_Tex, i.uv) + specularReflection), 1.0); //Texture is not applient on specularReflection

                }
                
                
                return color;
            }
            
            ENDCG	
        }		
    }
}
