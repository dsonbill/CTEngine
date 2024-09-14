using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalMachine
{
    public class PathMarker : MonoBehaviour
    {
        // The maximum number of path segments to keep
        public int Limit;

        // The level of influence for each path segment
        public float InfluenceRadius = 1.0f;  // Radius of influence for each path segment
        public float InfluenceStrength = 1.0f; // Strength of the force applied by the path

        // The line renderer used to draw the path
        public LineRenderer lineRenderer;     // Assign in Inspector
        public Gradient energyGradient;       // Assign a gradient in Inspector
        //public float energyLineWidthFactor = 2f; // Adjust this factor as needed

        // The list of path segments
        private List<Mark> Path = new List<Mark>();

        // Consolidation
        private float lastConsolidationTime = 0f;
        public float consolidationInterval = 5f; // Consolidate every 5 seconds

        public Transform Parent;

        // A single path segment
        public class Mark
        {
            public Vector3 Position;
            public Vector3 Direction;
            public Vector3 Energy;

            public Vector3 Proceed(Vector3 position, Vector3 destination, Vector3 energy)
            {
                // Calculate the direction between the destination and the current position
                Vector3 a = destination - position;

                // Calculate the angle between the direction and the path segment
                float angle = Vector3.SignedAngle(a, Direction, Vector3.one);

                // Calculate the distance between the particle path and the path segment
                float distance = Vector3.Distance(a, Direction);

                // Calculate the force based on the angle and the magnitude of the direction
                float force = Direction.magnitude / distance / angle * Energy.magnitude * Particle.EnergeticResistance;

                // Calculate the warp based on the force and the energy
                Vector3 warp = Direction * force;

                return warp;
            }
        }

        void Start()
        {
            // Initialize Line Renderer
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            lineRenderer.positionCount = 0;
        }
        void UpdateLineRenderer()
        {
            // Update the line renderer with the path segments
            lineRenderer.positionCount = Path.Count * 2;
            int offset = 0;
            for (int i = 0; i < Path.Count; i++)
            {
                int x = offset;
                offset += 2;

                // Set the position of the line renderer
                lineRenderer.SetPosition(x, (Path[i].Position));
                lineRenderer.SetPosition(x + 1, (Path[i].Position + Path[i].Direction));

                // Energy Visualization (Choose ONE option)
                // Option 1: Color
                float energyMagnitude = Path[i].Energy.magnitude;
                lineRenderer.startColor = energyGradient.Evaluate(energyMagnitude);
                lineRenderer.endColor = energyGradient.Evaluate(energyMagnitude);

                // Option 2: Line Width
                // float energyLineWidth = energyMagnitude * energyLineWidthFactor;
                // lineRenderer.startWidth = energyLineWidth;
                // lineRenderer.endWidth = energyLineWidth; 
            }
        }

        public Vector3 Project(Vector3 start, Vector3 end, Vector3 energy)
        {
            if (!enabled) return Vector3.zero;

            Vector3 final = Vector3.zero;

            foreach (Mark marker in Path)
            {
                final += marker.Proceed(start, end, energy);
                //Debug.Log("Proceed: " + marker.Proceed(start, end, energy));
            }

            Move(start, end, energy);

            return final;
        }

        
        public void Move(Vector3 start, Vector3 end, Vector3 energy)
        {
            // Make a new path segment
            Mark path = new Mark();

            // Set the path segment properties
            path.Position = start;
            path.Direction = (end - start);
            path.Energy = energy;

            // Add the new path segment
            Path.Add(path);
        }

        void ConsolidatePaths()
        {
            // Calculate the number of paths to remove
            int pathsToRemove = Path.Count - Limit / 2; // Remove half of the excess paths
            List<Mark> marksToConsolidate = Path.GetRange(0, pathsToRemove);
            Path.RemoveRange(0, pathsToRemove);

            // Calculate median values (adjust this logic as needed)
            Vector3 medianPosition = Vector3.zero;
            Vector3 averageDirection = Vector3.zero;
            Vector3 medianDirection = Vector3.zero;
            Vector3 totalEnergy = Vector3.zero;

            // Calculate the median values for the path segments to consolidate
            foreach (Mark mark in marksToConsolidate)
            {
                medianPosition += mark.Position;
                averageDirection += Mul(mark.Direction, mark.Energy.normalized);
                totalEnergy += mark.Energy;
            }

            // Calculate the median values
            medianPosition /= pathsToRemove;
            medianDirection = averageDirection.normalized;    // Normalize the sum

            // Add the consolidated mark
            Path.Add(new Mark
            {
                Position = medianPosition,
                Direction = medianDirection,
                Energy = totalEnergy
            });
        }

        void FixedUpdate()
        {
            // Consolidate paths if the limit is exceeded
            if (Time.time - lastConsolidationTime >= consolidationInterval && Path.Count > Limit)
            {
                ConsolidatePaths();
                lastConsolidationTime = Time.time;
            }
        }

        void Update()
        {
            UpdateLineRenderer();

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
}