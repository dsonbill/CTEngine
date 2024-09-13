using System.Collections.Generic;
using UnityEngine;

namespace UniversalMachine
{
    public class ForceExchange : MonoBehaviour
    {
        // ... (Your existing properties) ...
        public double ContactDepth;
        public float ExchangerRatio;


        public void Exchange(List<Particle> particles, Particle subject)
        {
            if (!enabled) return;

            // ... (Implement spatial partitioning or other optimization) ...

            foreach (Particle b in particles)
            {
                if (subject == b) continue; // Skip self-interaction

                float distance = GetDistance(subject.transform.position, b.transform.position);

                // Check if particles are within each other's reach
                if (distance < Mathf.Abs((subject.Reach + b.Reach).magnitude))
                {
                    // Determine the dominant particle
                    Particle dominantParticle = subject.Reach.magnitude > b.Reach.magnitude ? subject : b;
                    Particle influencedParticle = subject.Reach.magnitude > b.Reach.magnitude ? b : subject;

                    // Apply folding logic based on properties and distance
                    // ... (You'll need to replace this with your specific folding logic) ...
                    // Example:
                    // float foldingFactor = 1 - (distance / (dominantParticle.Reach + influencedParticle.Reach));
                    // dominantParticle.Fold(EffectorFolding.Energy, foldingFactor * dominantParticle.Ascription);
                    // influencedParticle.Fold(EffectorFolding.Position, foldingFactor * influencedParticle.Assertion);

                    // Exchange forces using scaling based on properties
                    ExchangeForces(dominantParticle, influencedParticle);
                }

            }
        }

        // Helper function to calculate distance, taking warping into account
        private float GetDistance(Vector3 positionA, Vector3 positionB)
        {
            // ... (Use SpacetimeFabric to calculate distance along warped path) ...
            return 0f;
        }

        // Function to exchange forces between two particles
        private void ExchangeForces(Particle dominantParticle, Particle influencedParticle)
        {
            // Calculate force to be exchanged
            Vector3 forceDelta = (dominantParticle.transform.position - influencedParticle.transform.position).normalized;

            // Apply force exchange (scaling based on properties)
            dominantParticle.Ascription *= dominantParticle.Ascription.magnitude * 0.1f;
            influencedParticle.Assertion *= influencedParticle.Attunement.magnitude * 0.1f;
        }
    }
}