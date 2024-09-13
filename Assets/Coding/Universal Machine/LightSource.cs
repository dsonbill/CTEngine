using System.Collections.Generic;
using UnityEngine;

namespace UniversalMachine
{
    public class LightSource : MonoBehaviour
    {
        // The strength of the light source as it applies a torque to particles
        public float TorqueStrength = 1.0f;

        // The intensity of the light source and its potential
        public float Intensity;
        public float Potential;

        public Light Light;

        // Shader property names (make sure these match your shader)
        //private const string LightTorqueProperty = "_LightTorque";

        public float constantAttenuation = 1.0f;
        public float linearAttenuation = 0.1f;
        public float quadraticAttenuation = 0.01f;

        public float Attenuation;

        // Update is called once per frame
        void Update()
        {
            // Update the light's visual properties
            Light.intensity = (float)Intensity;
            Light.range = (float)Potential;
        }

        public Vector3 CalculateTorque(Particle particle)
        {
            // Calculate distance and direction from light source to particle
            Vector3 directionToParticle = particle.transform.position - transform.position;
            float distanceToParticle = directionToParticle.magnitude;

            // Normalize the direction vector
            directionToParticle.Normalize();

            // Calculate the force based on intensity, potential, and inverse square of distance
            Vector3 forceDelta = (float)(Intensity * Potential / (distanceToParticle * distanceToParticle)) * directionToParticle;

            // Calculate torque
            Vector3 torqueDirection = Vector3.Cross(directionToParticle, particle.Velocity); // Perpendicular to both direction and velocity
            float torqueMagnitude = TorqueStrength * forceDelta.magnitude; // Scale torque based on force

            // Return the torque vector
            return torqueDirection * torqueMagnitude;
        }

        public void UpdateParticle(Particle particle)
        {
            Vector3 lightPosition = Light.transform.position;
            Vector3 objectPosition = particle.transform.position;
            float distance = Vector3.Distance(objectPosition, lightPosition);

            // Calculate attenuation
            Attenuation = 1.0f / (constantAttenuation + linearAttenuation * distance + quadraticAttenuation * distance * distance);

            //Material particleMaterial = particle.GetComponent<Renderer>().material;

            // Pass light properties to the shader
            //particleMaterial.SetVector("_LightPosition", lightPosition);
            //particleMaterial.SetFloat("_LightIntensity", (float)Intensity);
            //particleMaterial.SetFloat("_LightPotential", (float)Potential);
            //particleMaterial.SetVector("_LightDirection", Light.transform.forward); // Pass light direction
            //particleMaterial.SetFloat("_LightAttenuation", attenuation); // Pass light attenuation
        }
    }
}