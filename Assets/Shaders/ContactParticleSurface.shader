Shader "Custom/ContactParticleSurface" {
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _DepthColor ("Depth Color", Color) = (0,0.5,1,1)
        _ParticleDepth ("Depth", Float) = 0.0
        _StriationTexture ("Striation Texture", 2D) = "white" {}
        _StriationScale ("Striation Scale", Float) = 10.0
        _ReachColor ("Reach Color", Color) = (1,0,1,1)
        _ReachIntensity ("Reach Intensity", Float) = 1.0
        _ParticleReach ("Reach", Float) = 0.0
        _ParticleAscription ("Ascription", Vector) = (0,0,0,0) 
        _WarpingVectors ("Warping Vectors", Vector) = (0,0,0,0) 
        _NumWarpingVectors ("Number of Warping Vectors", Int) = 0
        _ParticleDelta ("Delta", Vector) = (0,0,0)
        _LightPosition ("Light Position", Vector) = (0,0,0,0)
        _LightIntensity ("Light Intensity", Float) = 1.0
        _LightPotential ("Light Potential", Float) = 1.0
        _LightDirection ("Light Direction", Vector) = (0,0,0,0)
        _LightAttenuation ("Light Attenuation", Float) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "HDRenderPipeline" }
        LOD 100

        HLSLPROGRAM
        #pragma target 2.5 // Essential for HDRP
        #pragma surface surf Lambert

        #include "UnityCG.cginc"
        
        // Properties
        half4 _MainColor;
        half4 _DepthColor;
        float _ParticleDepth;
        sampler2D _StriationTexture;
        float4 _StriationTexture_ST;
        float _StriationScale;
        half4 _ReachColor;
        float _ReachIntensity;
        float _ParticleReach; 
        float4 _ParticleAscription; 
        float4 _WarpingVectors[100]; 
        int _NumWarpingVectors;
        float3 _ParticleDelta;
        float4 _LightPosition;
        float _LightIntensity;
        float _LightPotential;
        float4 _LightDirection;
        float _LightAttenuation;
        
        struct Input {
            float2 uv_StriationTexture;
            float3 worldPos;
        };
        
        void surf (Input IN, inout SurfaceOutput o) {
            // 1. Basic Depth and Energy Color
            half4 col = lerp(_MainColor, _DepthColor, _ParticleDepth);
            float energy = length(_ParticleAscription.xyz);
            col.rgb *= 1.0 + energy;
        
            // 2. Striation Visualization
            float2 st = IN.worldPos.xz * _StriationScale;
            half4 striationCol = tex2D(_StriationTexture, st);
            col.rgb += striationCol.rgb * 0.5;
        
            // 3. Reach Effect
            float reachFactor = saturate(_ParticleReach * _ReachIntensity);
            col.rgb = lerp(col.rgb, _ReachColor.rgb, reachFactor);
        
            // 4. Geometric Hints (Example)
            float shapeFactor = sin(IN.worldPos.y * 10.0 + _Time.y * 5.0) * 0.5 + 0.5;
            col.rgb *= shapeFactor;
        
            // 5. Spacetime Warping (Applied to worldPos in Input struct)
            float3 warpedPosition = IN.worldPos;
            for (int i = 0; i < _NumWarpingVectors; i++) {
                float3 warpVectorPos = _WarpingVectors[i].xyz;
                float warpMagnitude = _WarpingVectors[i].w;
                float distanceToWarp = distance(IN.worldPos, warpVectorPos);
        
                // Apply warping function (customize this!)
                float warpAmount = warpMagnitude / (distanceToWarp * distanceToWarp + 0.1);
                warpedPosition += warpAmount * (IN.worldPos - warpVectorPos);
            }
            
            IN.worldPos = warpedPosition; // Update worldPos in Input struct
        
            // 6. Light-based Force (adapt from ApplyForceToParticle)
            float3 lightDir = _LightDirection.xyz;
            float distToLight = length(lightDir);
            float3 forceDelta = _ParticleDelta;
            forceDelta *= _LightIntensity * _LightPotential * _LightAttenuation / (distToLight * distToLight);
        
            // Modify SurfaceOutput based on forceDelta (adjust as needed)
            o.Normal += forceDelta; 
        
            o.Albedo = col.rgb;
        }
        ENDHLSL
    }
    FallBack "Diffuse"
}