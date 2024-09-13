using UnityEngine;
using System.Collections.Generic;
using UniversalMachine;
using Unity.Mathematics;

public class ParticlePositionManager : MonoBehaviour
{
    // List to store all particles
    public List<Particle> particles = new List<Particle>();

    // Reference to the SpacetimeFabric
    public SpacetimeFabric spacetimeFabric;

    // Reference to the LightSource
    public LightSource lightSource;

    void FixedUpdate()
    {
        // Update particle positions based on spacetime warping
        UpdateParticlePositions();

        // Apply Delta updates to particle positions
        ApplyDeltaUpdates();
    }

    // Method to add a particle to the list
    public void AddParticle(Particle particle)
    {
        particles.Add(particle);
    }

    // Method to remove a particle from the list
    public void RemoveParticle(Particle particle)
    {
        particles.Remove(particle);
    }
    // Method to apply Delta updates to particle positions
    private void ApplyDeltaUpdates()
    {
        foreach (Particle particle in particles)
        {
            // Light-based force calculation
            Vector3 lightDir = particle.GetLightDirection(lightSource.Light);
            float distToLight = lightDir.magnitude;
            Vector3 forceDelta = particle.Delta;
            forceDelta *= lightSource.Intensity * lightSource.Potential * lightSource.Attenuation / (distToLight * distToLight);

            // Add the torque to the force
            Vector3 torque = Mul(Vector3.Cross(lightDir, particle.Velocity) * lightSource.TorqueStrength, lightSource.CalculateTorque(particle));
            forceDelta = Mul(forceDelta, torque); // Apply the torque to the force

            // Apply the force to the particle's position
            Vector3 tryMeFaggot = particle.Applicate(forceDelta);
            particle.transform.position += tryMeFaggot;

            // Reset Delta for the next frame
            particle.Delta = Vector3.zero;
        }
    }

    // Method to update particle positions
    private void UpdateParticlePositions()
    {
        // Get the warping vectors from SpacetimeFabric
        SpacetimeFabric.WarpVector[] warpingVectors = spacetimeFabric.WarpVectors.ToArray();

        foreach (Particle particle in particles)
        {
            // Calculate the warped position for this particle
            Vector3 warpedPosition = particle.transform.position;

            // 1. Sort the warp vectors by distance from the particle
            SpacetimeFabric.WarpVector[] sortedWarpVectors = SortWarpVectorsByDistance(warpingVectors, warpedPosition);

            // 2. Apply warping based on sorted warp vectors
            for (int i = 0; i < sortedWarpVectors.Length; i++)
            {
                float distance = Vector3.Distance(warpedPosition, sortedWarpVectors[i].Position);

                //Debug.Log($"Particle Position: {particle.transform.position}");
                //Debug.Log($"Warp Vector Position: {sortedWarpVectors[i].Position}");
                //Debug.Log($"Distance: {distance}");
                //Debug.Log($"Warp Vector Direction: {sortedWarpVectors[i].Direction}");
                //Debug.Log($"Warp Vector Magnitude: {sortedWarpVectors[i].Magnitude}");

                // Check for divide by zero
                if (distance > 0.001f)
                {
                    // Calculate the angle between the particle's Delta and the warp vector direction
                    float angle = Vector3.Angle(particle.transform.position, sortedWarpVectors[i].Direction);

                    //Debug.Log($"Particle Delta: {particle.Delta}");
                    //Debug.Log($"Angle: {angle}");

                    // Apply warping based on angle and magnitude
                    warpedPosition += sortedWarpVectors[i].Direction * sortedWarpVectors[i].Magnitude * Mathf.Cos(angle * Mathf.Deg2Rad) / (distance * distance);
                }
            }

            // Update the particle's position (assuming you're not using a Rigidbody)
            Vector3 applicated = particle.Applicate(warpedPosition);
            particle.Delta += applicated;
        }
    }

    // Helper function to sort warp vectors by distance
    SpacetimeFabric.WarpVector[] SortWarpVectorsByDistance(SpacetimeFabric.WarpVector[] warpVectors, Vector3 position)
    {
        for (int i = 0; i < warpVectors.Length - 1; i++)
        {
            for (int j = 0; j < warpVectors.Length - i - 1; j++)
            {
                if (Vector3.Distance(warpVectors[j].Position, position) > Vector3.Distance(warpVectors[j + 1].Position, position))
                {
                    // Swap elements
                    SpacetimeFabric.WarpVector temp = warpVectors[j];
                    warpVectors[j] = warpVectors[j + 1];
                    warpVectors[j + 1] = temp;
                }
            }
        }
        return warpVectors;
    }

    public Vector3 Mul(Vector3 vector1, Vector3 vector2)
    {
        return new Vector3(
            vector1.x * vector2.x,
            vector1.y * vector2.y,
            vector1.z * vector2.z
            );
    }
}
