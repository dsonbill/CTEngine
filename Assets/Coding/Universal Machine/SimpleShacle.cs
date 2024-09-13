using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalMachine
{
    public class SimpleShacle : MonoBehaviour
    {
        public GameObject Cylinder;
        public float Diameter;

        public float Binding = 1f;

        System.Random r = new System.Random();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public void Bind(Particle particle)
        {
            if (!enabled) return;

            Vector3 particlePosition = particle.transform.position;
            Vector3 shacklePosition = Cylinder.transform.position;

            Vector3 direction = shacklePosition - particlePosition;
            float distance = direction.magnitude;

            // Check if particle is within the shackle's influence radius
            if (distance <= Diameter / 2f)
            {
                float normalizedDistance = distance / (Diameter / 2f); // 0 at center, 1 at edge
                float forceMagnitude = Binding * normalizedDistance * particle.Ascribe(Time.deltaTime).magnitude / Particle.EnergeticResistance;  // Scale based on energy
                Vector3 inwardForce = -direction.normalized * forceMagnitude;

                particle.AddForce(inwardForce, Vector3.zero, Time.deltaTime);
            }
        }
    }
}