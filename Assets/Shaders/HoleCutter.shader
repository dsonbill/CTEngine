Shader "Custom/HoleCutter" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "HDRenderPipeline" }
        LOD 100

        

        // Main Pass (Samples the cutout texture and discards fragments)
        Pass {
            HLSLPROGRAM
            #pragma target 2.5 // Essential for HDRP
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _CameraDepthTexture; // Custom depth texture

            struct Attributes {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            Varyings vert (Attributes input) {
                Varyings output;
                output.positionCS = UnityObjectToClipPos(input.positionOS.xyz); 
                output.uv = input.uv; 
                return output;
            }

            half4 frag (Varyings input) : SV_Target {
                // Sample the cutout texture
                float depthValue = tex2D(_CameraDepthTexture, input.uv).r;

                float3 worldPos = mul(unity_ObjectToWorld, input.positionOS.xyz);

                float cameraDistance = length(worldPos - _WorldSpaceCameraPos.xyz);

                // Discard fragments behind the cutout mesh
                if (depthValue < cameraDistance) { // Adjust threshold as needed
                    discard;
                }

                // ... (Your other fragment shader logic) ...

                return half4(1,1,1,1);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}