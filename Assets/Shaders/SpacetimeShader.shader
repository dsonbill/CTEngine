Shader "Custom/SpacetimeShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _WarpingVectors ("Warping Vectors", Vector) = (0,0,0,0) 
        _NumWarpingVectors ("Number of Warping Vectors", Int) = 0
        _WarpIntensity ("Warp Intensity", Float) = 1.0
        _WarpScale ("Warp Scale", Float) = 1.0
        _WarpFalloff ("Warp Falloff", Float) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "HDRenderPipeline" }
        LOD 100

        Pass {
            HLSLPROGRAM
            #pragma target 2.5 // Essential for HDRP
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _WarpingVectors[100]; 
            int _NumWarpingVectors;
            float _WarpIntensity;
            float _WarpScale;
            float _WarpFalloff;

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
                output.worldPos = mul(UNITY_MATRIX_M, input.positionOS).xyz;
                return output;
            }

            half4 frag (Varyings input) : SV_Target 
            {
                // 1. Sample Main Texture
                half4 col = tex2D(_MainTex, input.uv);

                // 2. Calculate Warped UV
                float2 warpedUV = input.uv; // Start with original UV
                for (int j = 0; j < _NumWarpingVectors; j++)
                {
                    float3 warpVectorPos = _WarpingVectors[j].xyz;
                    float warpMagnitude = _WarpingVectors[j].w * _WarpIntensity; // Apply warp intensity

                    // Calculate distance to warping vector
                    float distanceToWarp = distance(input.worldPos, warpVectorPos);

                    // Calculate warp amount based on distance and falloff
                    float warpAmount = warpMagnitude / (distanceToWarp * distanceToWarp + _WarpFalloff); // Custom falloff

                    // Calculate warp direction (you might need to adjust this based on Contact Theory)
                    float2 warpDirection = normalize(input.uv - float2(warpVectorPos.x, warpVectorPos.z)) * _WarpScale; 

                    // Apply warping to UV
                    warpedUV += warpAmount * warpDirection;
                }

                // 3. Sample Texture with Warped UV
                col = tex2D(_MainTex, warpedUV); 

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}