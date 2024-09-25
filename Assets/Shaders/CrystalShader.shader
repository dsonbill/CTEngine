Shader "Custom/CrystalShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 10
        _RefractionAmount ("Refraction Amount", Float) = 0.1
        _RefractionColor ("Refraction Color", Color) = (0, 0.5, 1, 1)
        _GlowColor ("Glow Color", Color) = (1, 0, 1, 1)
        _GlowIntensity ("Glow Intensity", Float) = 1
        _EdgeSharpness ("Edge Sharpness", Float) = 0.2
        _CrystalSystem ("Crystal System", Float) = 0.35
        _UnitCellSize ("Unit Cell Size", Float) = 0.38
        _DistortionStrength ("Distortion Strength", Float) = 0.3
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _NoiseScale;
            float _RefractionAmount;
            fixed4 _RefractionColor;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _EdgeSharpness;
            float _CrystalSystem;
            float _UnitCellSize;
            float _DistortionStrength; 

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };
            
            // In your vertex shader:
            v2f vert (appdata v) {
                v2f o;

                // Calculate the noise value
               float2 noiseUV = v.vertex.xz * _NoiseScale;
               float noiseValue = tex2D(_NoiseTex, noiseUV).r;

               // Determine the lattice point based on the vertex position
               float3 latticePoint = float3(floor(v.vertex.x / _UnitCellSize) * _UnitCellSize, 
                                 floor(v.vertex.y / _UnitCellSize) * _UnitCellSize,
                                 floor(v.vertex.z / _UnitCellSize) * _UnitCellSize);

               // Calculate the distance to the lattice point
               float distanceToLattice = distance(v.vertex.xyz, latticePoint); 





               float3 CalculateHexagonalLatticePoint(float3 vertexPosition, float unitCellSize)
{
    // Calculate the distance from the vertex to the origin along the X and Z axes
    float xDist = abs(vertexPosition.x);
    float zDist = abs(vertexPosition.z);

    // Find the nearest hexagonal lattice point
    float hexX = 0.0;
    float hexZ = 0.0;

    if (zDist <= xDist * sqrt(3.0)) {
        hexX = floor(vertexPosition.x / (unitCellSize * sqrt(3.0)));
        hexZ = floor((vertexPosition.z + hexX * unitCellSize / 2.0) / (unitCellSize * 1.5));
    } else {
        hexX = floor((vertexPosition.x + vertexPosition.z / sqrt(3.0)) / (unitCellSize * sqrt(3.0)));
        hexZ = floor(vertexPosition.z / (unitCellSize * 1.5));
    }

    // Return the hexagonal lattice point
    return float3(hexX * unitCellSize * sqrt(3.0), vertexPosition.y, hexZ * unitCellSize * 1.5);
}


float3 CalculateTrigonalLatticePoint(float3 vertexPosition, float unitCellSize)
{
    // Calculate the distance from the vertex to the origin along the X and Z axes
    float xDist = abs(vertexPosition.x);
    float zDist = abs(vertexPosition.z);

    // Find the nearest trigonal lattice point
    float trigonalX = 0.0;
    float trigonalZ = 0.0;

    // The trigonal system is similar to hexagonal, but with some modifications
    if (zDist <= xDist * sqrt(3.0)) {
        trigonalX = floor(vertexPosition.x / (unitCellSize * sqrt(3.0)));
        trigonalZ = floor((vertexPosition.z + trigonalX * unitCellSize / 2.0) / (unitCellSize * 1.5));
    } else {
        trigonalX = floor((vertexPosition.x + vertexPosition.z / sqrt(3.0)) / (unitCellSize * sqrt(3.0)));
        trigonalZ = floor(vertexPosition.z / (unitCellSize * 1.5));
    }

    // Return the trigonal lattice point
    return float3(trigonalX * unitCellSize * sqrt(3.0), vertexPosition.y, trigonalZ * unitCellSize * 1.5); 
}

float3 CalculateMonoclinicLatticePoint(float3 vertexPosition, float unitCellSize)
{
    // For the monoclinic system, we need to define an angle for the lattice
    float betaAngle = 60.0; // Example - you can adjust this based on your desired monoclinic structure

    // Calculate the distance from the vertex to the origin along the X, Y, and Z axes
    float xDist = abs(vertexPosition.x);
    float yDist = abs(vertexPosition.y);
    float zDist = abs(vertexPosition.z);

    // Find the nearest monoclinic lattice point
    float monoclinicX = 0.0;
    float monoclinicY = 0.0;
    float monoclinicZ = 0.0;

    // Use the angle to calculate the lattice point
    if (zDist <= xDist * sqrt(3.0)) {
        monoclinicX = floor(vertexPosition.x / (unitCellSize * sqrt(3.0)));
        monoclinicZ = floor((vertexPosition.z + monoclinicX * unitCellSize / 2.0) / (unitCellSize * 1.5));
    } else {
        monoclinicX = floor((vertexPosition.x + vertexPosition.z / sqrt(3.0)) / (unitCellSize * sqrt(3.0)));
        monoclinicZ = floor(vertexPosition.z / (unitCellSize * 1.5));
    }

    // The y-dimension will be influenced by the angle
    monoclinicY = floor((vertexPosition.y + monoclinicX * unitCellSize * sin(betaAngle)) / unitCellSize);

    // Return the monoclinic lattice point
    return float3(monoclinicX * unitCellSize * sqrt(3.0), monoclinicY * unitCellSize, monoclinicZ * unitCellSize * 1.5);
}

float3 CalculateTriclinicLatticePoint(float3 vertexPosition, float unitCellSize)
{
    // For the triclinic system, we need to define three angles for the lattice
    float alphaAngle = 60.0; // Example - you can adjust this based on your desired triclinic structure
    float betaAngle = 70.0; // Example 
    float gammaAngle = 80.0; // Example

    // Calculate the distance from the vertex to the origin along the X, Y, and Z axes
    float xDist = abs(vertexPosition.x);
    float yDist = abs(vertexPosition.y);
    float zDist = abs(vertexPosition.z);

    // Find the nearest triclinic lattice point
    float triclinicX = 0.0;
    float triclinicY = 0.0;
    float triclinicZ = 0.0;

    // Use the angles to calculate the lattice point
    if (zDist <= xDist * sqrt(3.0)) {
        triclinicX = floor(vertexPosition.x / (unitCellSize * sqrt(3.0)));
        triclinicZ = floor((vertexPosition.z + triclinicX * unitCellSize / 2.0) / (unitCellSize * 1.5));
    } else {
        triclinicX = floor((vertexPosition.x + vertexPosition.z / sqrt(3.0)) / (unitCellSize * sqrt(3.0)));
        triclinicZ = floor(vertexPosition.z / (unitCellSize * 1.5));
    }

    // The y and z dimensions will be influenced by the angles
    triclinicY = floor((vertexPosition.y + triclinicX * unitCellSize * sin(betaAngle)) / unitCellSize);
    triclinicZ = floor((vertexPosition.z + triclinicX * unitCellSize * sin(gammaAngle)) / unitCellSize);

    // Return the triclinic lattice point
    return float3(triclinicX * unitCellSize * sqrt(3.0), triclinicY * unitCellSize, triclinicZ * unitCellSize * 1.5);
}


               
               

               if (_CrystalSystem == 0) // Cubic System
               {
                   v.vertex.xyz += normalize(v.vertex.xyz - latticePoint) * distanceToLattice * _DistortionStrength * noiseValue;
               }
               else if (_CrystalSystem == 1) // Tetragonal
               {
                   v.vertex.xyz += normalize(v.vertex.xyz - latticePoint) * float3(distanceToLattice, distanceToLattice * 1.5f, distanceToLattice) * _DistortionStrength * noiseValue; 
               }
               else if (_CrystalSystem == 2) // Orthorhombic
               {
                   // Stretch two axes
                   v.vertex.xyz += normalize(v.vertex.xyz - latticePoint) * float3(distanceToLattice * 1.2f, distanceToLattice * 1.5f, distanceToLattice) * _DistortionStrength * noiseValue; 
               }
               else if (_CrystalSystem == 3)
               {
                   // Use a hexagonal lattice point calculation 
                   float3 hexLatticePoint = CalculateHexagonalLatticePoint(v.vertex.xyz, _UnitCellSize); // Implement this function!
                   v.vertex.xyz += normalize(v.vertex.xyz - hexLatticePoint) * distanceToLattice * _DistortionStrength * noiseValue;
               }
               else if (_CrystalSystem == 4)
               {
                   // Use a trigonal lattice point calculation 
                   float3 trigonalLatticePoint = CalculateTrigonalLatticePoint(v.vertex.xyz, _UnitCellSize); // Implement this function!
                   v.vertex.xyz += normalize(v.vertex.xyz - trigonalLatticePoint) * distanceToLattice * _DistortionStrength * noiseValue;
               }
               else if (_CrystalSystem == 5)
               {
                   // Use a monoclinic lattice point calculation 
                   float3 monoclinicLatticePoint = CalculateMonoclinicLatticePoint(v.vertex.xyz, _UnitCellSize); // Implement this function!
                   v.vertex.xyz += normalize(v.vertex.xyz - monoclinicLatticePoint) * distanceToLattice * _DistortionStrength * noiseValue;
               }
               else if (_CrystalSystem == 6)
               {
                   // Use a triclinic lattice point calculation 
                   float3 triclinicLatticePoint = CalculateTriclinicLatticePoint(v.vertex.xyz, _UnitCellSize); // Implement this function!
                   v.vertex.xyz += normalize(v.vertex.xyz - triclinicLatticePoint) * distanceToLattice * _DistortionStrength * noiseValue;
               }

               
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Sample the main texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate the noise value
                float2 noiseUV = i.worldPos.xz * _NoiseScale;
                float noiseValue = tex2D(_NoiseTex, noiseUV).r;

                // Calculate the refraction effect
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 refractedDir = refract(viewDir, normalize(i.worldPos), _RefractionAmount);
                float3 refractionOffset = refractedDir * _RefractionAmount;

                // Apply glow effect
                float3 glowFactor = saturate(_GlowIntensity * (1.0 - length(refractionOffset)));
                col.rgb = lerp(col.rgb, _GlowColor.rgb, glowFactor);

                // Calculate edge sharpness
                float edgeSharpness = smoothstep(0.0, _EdgeSharpness, length(refractionOffset));

                // Multiply the color by the edge sharpness
                col.rgb *= edgeSharpness; 

                // Add noise to the color
                col.rgb += noiseValue * _Color.rgb; 

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
