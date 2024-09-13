using UnityEngine;

namespace UniversalMachine
{
    public class PotentialHole : MonoBehaviour
    {
        public Transform Hole; // Transform of the visual representation of the hole

        public double Diameter; // Diameter of the hole
        public double Depth; // Depth of the hole

        // Additional properties for influence
        public float AttractionStrength = 10f; // Strength of the attractive force
        public float InfluenceRadius = 5f; // Radius of influence

        // Start is called before the first frame update
        void Start()
        {
            // Initialize visual representation
            Hole.localScale = new Vector3((float)Diameter, Hole.localScale.y, (float)Diameter);
            Hole.localPosition = new Vector3(0, (float)Depth, 0);
        }

        // Update is called once per frame
        void Update()
        {
            // Update visual representation (if needed)
            // ... (You can add code here if the hole's size or depth changes) ...
        }

        public void ApplyPositionBoost(Particle particle)
        {
            // Calculate position change based on distance and velocity
            float distanceToHole = Vector3.Distance(transform.position, particle.transform.position);
            Vector3 positionChange = (particle.transform.position - transform.position).normalized * particle.Velocity.magnitude * 0.1f;

            // Update particle's Assertion
            particle.Assertion -= new Vector4(positionChange.x, positionChange.y, positionChange.z, 0); // Subtract from position
            particle.Assertion.w -= Time.deltaTime; // Decrease "dimensionality" of positionment 
        }

        public void ApplyEnergyBoost(Particle particle)
        {
            // Calculate distance to the nearest wall of the cylinder
            float distanceToWall = CalculateDistanceToNearestWall(particle.transform.position);

            // Calculate energy increase based on distance to the wall
            float energyIncrease = (1f / (distanceToWall * distanceToWall + 1f)) * particle.Velocity.magnitude * 0.5f; // Example - Adjust scaling factor

            // Update particle's Ascription
            particle.Ascription = particle.Ascription + new Vector4(energyIncrease, energyIncrease, energyIncrease, Time.deltaTime); // Include Time.deltaTime
        }

        // Helper function to calculate distance to the nearest wall
        private float CalculateDistanceToNearestWall(Vector3 particlePosition)
        {
            // Get the cylinder's transform
            Transform cylinderTransform = Hole.transform;

            // Calculate the cylinder's radius
            float cylinderRadius = (float)Diameter / 2f;

            // Calculate the distance from the particle to the center of the cylinder
            float distanceToCenter = Vector3.Distance(particlePosition, cylinderTransform.position);

            // Calculate the distance to the nearest wall
            float distanceToWall = Mathf.Abs(distanceToCenter - cylinderRadius);

            return distanceToWall;
        }
    }
}