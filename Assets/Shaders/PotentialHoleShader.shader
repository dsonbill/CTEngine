Shader "Custom/PotentialHoleShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Hole Color", Color) = (0.5, 0.5, 0.5, 1)
        _Intensity ("Intensity", Float) = 1.0
        _Falloff ("Falloff", Float) = 0.5
        _Distortion ("Distortion", Float) = 0.2
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "HDRenderPipeline" }
        LOD 100

        Pass {
            HLSLPROGRAM
            #pragma target 2.5 // Essential for HDRP
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Intensity;
            float _Falloff;
            float _Distortion;

            struct Attributes {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 worldPos     : TEXCOORD1;
            };

            Varyings vert (Attributes input) {
                Varyings output;
                output.positionCS = UnityObjectToClipPos(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.worldPos = mul(unity_ObjectToWorld, input.positionOS).xyz;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // Calculate the distance from the fragment to the center of the hole
                float distToCenter = distance(input.worldPos, float3(0, 0, 0)); // Adjust center as needed

                // Calculate the hole intensity based on distance and falloff
                float intensity = saturate(_Intensity * exp(-distToCenter * _Falloff));

                // Calculate the distortion amount
                float distortion = intensity * _Distortion;

                // Sample the main texture and apply the hole color
                half4 texColor = tex2D(_MainTex, input.uv);
                half4 holeColor = _Color;
                half4 finalColor = lerp(texColor, holeColor, intensity);

                // Apply distortion to the UV coordinates
                float2 distortedUV = input.uv + distortion * float2(sin(input.uv.x * 10), cos(input.uv.y * 10));

                // Sample the texture with the distorted UV
                finalColor = tex2D(_MainTex, distortedUV); 

                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}