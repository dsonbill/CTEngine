Shader "Custom/HoleCutter" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" } // "Opaque" is correct for this shader
        LOD 100

        // Depth Texture Pass (Renders only to the depth buffer)
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" // Correct include for standard pipeline

            struct Attributes {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS   : SV_POSITION;
            };

            Varyings vert (Attributes input) {
                Varyings output;
                output.positionCS = UnityObjectToClipPos(input.positionOS.xyz); 
                return output;
            }

            void frag () {
                // Do nothing in the fragment shader, just write to the depth buffer
            }
            ENDHLSL
        }

        // Main Pass (Samples depth texture and discards fragments)
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" 

            sampler2D _CameraDepthTexture; // The depth texture

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

            fixed4 frag (Varyings input) : SV_Target {
                // Sample the depth texture
                float depthValue = tex2D(_CameraDepthTexture, input.uv).r;

                // Get the object-space position of the fragment
                float3 worldPos = mul(unity_ObjectToWorld, input.positionCS.xyz); 

                // Calculate the distance from the camera to the fragment
                float cameraDistance = length(worldPos - _WorldSpaceCameraPos.xyz); 

                // Discard fragments behind the cutout mesh
                if (depthValue < cameraDistance) { 
                    discard;
                }

                // ... (Your other fragment shader logic) ... 

                return fixed4(1,1,1,1);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
