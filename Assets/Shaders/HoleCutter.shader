Shader "Custom/HoleCutter" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _CutoutTexture ("Cutout Texture", 2D) = "white" {} // Custom depth texture
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "HDRenderPipeline" }
        LOD 100

        // Depth Texture Pass (Renders only to the cutout texture)
        Pass {
            HLSLPROGRAM
            #include "UnityCG.cginc"
            #pragma target 2.5 // Essential for HDRP
            #pragma vertex vert
            #pragma fragment frag

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
                // Render the cutout mesh to the cutout texture
                return half4(1,1,1,1);
            }
            ENDHLSL
        }

        // Main Pass (Samples the cutout texture and discards fragments)
        Pass {
            HLSLPROGRAM
            #pragma target 4.5 // Essential for HDRP
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _CutoutTexture; // Custom depth texture

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
                float depthValue = tex2D(_CutoutTexture, input.uv).r;

                // Discard fragments behind the cutout mesh
                if (depthValue < 0.5) { // Adjust threshold as needed
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