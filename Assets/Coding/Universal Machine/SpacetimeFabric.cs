using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace UniversalMachine
{
    public class SpacetimeFabric : MonoBehaviour
    {
        // Warping Vector Structure
        public struct WarpVector
        {
            public Vector3 Position;
            public Vector3 Direction;
            public float Magnitude;

            public WarpVector(Vector3 position, Vector3 direction, float magnitude)
            {
                Position = position;
                Direction = direction;
                Magnitude = magnitude;
            }
        }

        // List to store warping vectors
        public List<WarpVector> WarpVectors = new List<WarpVector>();

        // Consolidation Parameters
        public float ConsolidationRadius = 1.0f;
        public float ConsolidationThreshold = 0.5f;
        public float ConsolidationInterval = 0.5f;

        // Expostulant Affair
        public float ExpostulantAffair = 1f;

        // Spacetime Material
        public List<Renderer> Renderers;

        // Maximum number of warping vectors to send to the shader
        private const int MaxWarpingVectors = 100;

        private float lastConsolidationTime = 0f;

        // The threshold for recording a warp vector
        public float WarpVectorThreshold = 0.5f;

        public Vector3 Mul(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(
                vector1.x * vector2.x,
                vector1.y * vector2.y,
                vector1.z * vector2.z
                );
        }

        void FixedUpdate()
        {
            // 1. Update Warping Vectors (from particle expostulation)
            //UpdateWarpingVectors();

            // 2. Consolidate Warping Vectors (periodically)
            if (Time.time - lastConsolidationTime >= ConsolidationInterval)
            {
                ConsolidateWarpingVectors();
                lastConsolidationTime = Time.time;
            }

            // 3. Update Shader Properties (for visualization and particle positioning)
            //UpdateSpacetimeShaderProperties();
        }

        // Function for calculating the warped spacetime position of a particle
        public Vector3 CalculateWarpedPosition(Particle particle)
        {
            // Create tensors from particle properties and warping vectors
            Tensor particleTensor = new Tensor();
            particleTensor.Init(
                // Ascription
                new float[3] { particle.Ascription.x, particle.Ascription.y, particle.Ascription.z },
                // Assertion
                new float[3] { particle.Assertion.x, particle.Assertion.y, particle.Assertion.z },
                // Conductance
                new float[3] { particle.Conductance.x, particle.Conductance.y, particle.Conductance.z },
                // Attunement
                new float[3] { particle.Attunement.x, particle.Attunement.y, particle.Attunement.z }
            );

            // Create a tensor for the warp vectors
            Tensor warpTensor = new Tensor();
            IEnumerable<float[]> warpVectorsSelection = WarpVectors.Select(warpVector => new float[3] { warpVector.Direction.x, warpVector.Direction.y, warpVector.Direction.z });
            
            warpTensor.Init(
                // Get the x, y, and z components of the warp vectors
                warpVectorsSelection.ToArray()[0],
                warpVectorsSelection.ToArray()[1],
                warpVectorsSelection.ToArray()[2],
                // Get the magnitudes of the warp vectors
                WarpVectors.Select(warpVectors => warpVectors.Magnitude).ToArray()
            );

            // Calculate the tensor product 
            Tensor spacetimeTensor = particleTensor.TensorProduct(warpTensor);

            // Apply the tensor to the particle's position
            // ... (You will need to implement this based on Contact Theory's principles) ...
            Vector3 warpedPosition = particle.transform.position;

            return warpedPosition;
        }

        // Method to add new warping vectors from particles
        public void AddWarpVector(Vector3 position, Vector3 direction, float magnitude)
        {
            WarpVectors.Add(new WarpVector(position, direction, magnitude));
        }

        // Method to update warping vectors based on particle expostulation
        public void UpdateWarpingVectors(Particle particle)
        {
            // TODO: Calculate expostulation value and contact area
            float expostulationValue = particle.Expostulate(WarpVectorThreshold);
            //Debug.Log($"Expostulation Value: {expostulationValue}");
            float contactArea = particle.CalculateContactArea(); // Replace 'someArea' with your area calculation
            //Debug.Log($"Contact Area: {contactArea}");

            // Calculate warp direction using particle velocity and warp vector threshold
            Vector3 warpDirection = particle.Velocity.normalized * Mathf.Pow(Mul(particle.Velocity.normalized, particle.Velocity).magnitude, WarpVectorThreshold) * Particle.ContactWindow;
            //Debug.Log($"Velocity: {particle.Velocity}");
            //Debug.Log($"Velocity Magnitude: {particle.Velocity.magnitude}");
            //Debug.Log($"Velocity Normalized: {particle.Velocity.normalized}");
            //Debug.Log($"Warp Direction: {warpDirection}");

            // Calculate force reduction using TertiaryReduction
            float forceReduction = (float)particle.TertiaryReduction(contactArea);
            warpDirection *= forceReduction;
            //Debug.Log($"Force Reduction: {forceReduction}");

            // Calculate warp magnitude using QuaternaryReduction (for torque influence)
            float warpMagnitude = expostulationValue * ExpostulantAffair * (float)particle.QuaternaryReduction(contactArea); // Replace 'someArea' with your area calculation
            //Debug.Log($"Warp Magnitude: {warpMagnitude}");

            // Add the warp vector
            AddWarpVector(particle.transform.position, warpDirection, warpMagnitude);
            //Debug.Log($"Warp Vector Position: {particle.transform.position}");
            //Debug.Log($"Warp Vector Direction: {warpDirection}");
            //Debug.Log($"Warp Vector Magnitude: {warpMagnitude}");

            // Reset the particle's time since warped
            particle.TimeSinceWarped = 0f;
        }

        // Method to consolidate nearby warping vectors
        private void ConsolidateWarpingVectors()
        {
            // TODO: Implement your intelligent consolidation algorithm here
            // This is where you'll group and average warping vectors based on:
            // - ConsolidationRadius
            // - ConsolidationThreshold
            // - And your categorization logic to preserve important features

            // Temporary simple consolidation (replace with your algorithm)
            if (WarpVectors.Count > MaxWarpingVectors)
            {
                WarpVectors.RemoveRange(MaxWarpingVectors, WarpVectors.Count - MaxWarpingVectors);
            }
        }

        // Method to update shader properties
        public void UpdateParticleShaderProperties(Material particleMaterial)
        {
            // Prepare warping vector data for shaders
            Vector4[] warpingVectorsArray = new Vector4[MaxWarpingVectors];
            for (int i = 0; i < WarpVectors.Count && i < MaxWarpingVectors; i++)
            {
                warpingVectorsArray[i] = new Vector4(
                    WarpVectors[i].Position.x,
                    WarpVectors[i].Position.y,
                    WarpVectors[i].Position.z,
                    WarpVectors[i].Magnitude
                );
            }

            // Update Particle Shader
            particleMaterial.SetVectorArray("_WarpingVectors", warpingVectorsArray);
            particleMaterial.SetInt("_NumWarpingVectors", warpingVectorsArray.Length);
        }

        private void UpdateSpacetimeShaderProperties()
        {
            // Prepare warping vector data for shaders
            Vector4[] warpingVectorsArray = new Vector4[MaxWarpingVectors];
            for (int i = 0; i < WarpVectors.Count && i < MaxWarpingVectors; i++)
            {
                warpingVectorsArray[i] = new Vector4(
                    WarpVectors[i].Position.x,
                    WarpVectors[i].Position.y,
                    WarpVectors[i].Position.z,
                    WarpVectors[i].Magnitude
                );
            }

            // Update Spacetime Shader
            foreach(Renderer renderer in Renderers)
            {
                renderer.material.SetVectorArray("_WarpingVectors", warpingVectorsArray);
                renderer.material.SetInt("_NumWarpingVectors", warpingVectorsArray.Length);
            }
            
        }
        }
}