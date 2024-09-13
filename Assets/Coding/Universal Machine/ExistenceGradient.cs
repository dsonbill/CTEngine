using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalMachine
{
    public class ExistenceGradient : MonoBehaviour
    {
        public static float FrictionCoefficient;
        public static float TorqueCoefficient;

        public Transform Primary;
        public Transform Secondary;

        public MeshRenderer PrimaryMesh;
        public MeshRenderer SecondaryMesh;

        public double Diameter;
        public double Distance;

        public Vector2 Position;

        public Vector3 Bifurcation;

        Vector3 PrimaryPosition;
        Vector3 PrimaryScale;

        Vector3 SecondaryPosition;
        Vector3 SecondaryScale;

        Quaternion Precession = Quaternion.identity;
        float precessionDelta = 0f;

        // x: Diameter, y: Height, z: Angular, w: Precessional
        public Vector4 Contact;

        public Gradient ColorGradient;
        public Gradient SecondaryColorGradient;

        public float Energy;

        public Func<List<Particle>> Quanta;

        public Func<Vector2> WellDistance;
        public Func<Vector3, Vector3> WellDirection;

        public float classicalThreshold = 0.3f; // Substantiation threshold for classical behavior
        public float quantumThreshold = 0.7f;   // Substantiation threshold for quantum behavior
        public float forceStrength = 1f;

        // Reference to the DescriptorContactor
        public Func<DescriptorContactor> Source;
        public Func<FluidDispensary> Well;

        float pTime;
        float sTime;

        // Start is called before the first frame update
        void Start()
        {

            sTime = 1 ;
        }

        public Vector3 Mul(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(
                vector1.x * vector2.x,
                vector1.y * vector2.y,
                vector1.z * vector2.z
                );
        }

        float CalculateSubstantiation(Particle particle)
        {
            // Calculate distance to Primary and Secondary
            float distToPrimary = Vector3.Distance(particle.transform.position, Primary.position);
            float distToSecondary = Vector3.Distance(particle.transform.position, Secondary.position);

            // Use a normalized distance to determine Substantiation
            float normalizedDistance = Mathf.Clamp01(distToPrimary / (distToPrimary + distToSecondary));

            return normalizedDistance;
        }

        void CalculateEnergy()
        {
            float positionOffset = Vector3.Distance(Primary.localPosition, Secondary.localPosition);
            float rotationalOffset = Quaternion.Angle(Primary.localRotation, Secondary.localRotation);

            Color offsetColor = PrimaryMesh.material.color - SecondaryMesh.material.color;
            float stateOffset = (offsetColor.r + offsetColor.g + offsetColor.b) / 3;

            Energy = (positionOffset * (1 / 360 * rotationalOffset) + 0.001f) * stateOffset;
        }


        void FixedUpdate()
        {
            pTime += Time.deltaTime;
            sTime -= Time.deltaTime;
            if (pTime >= 1)
            {
                pTime = 0;
            }
            if (sTime <= 0)
            {
                sTime = 1;
            }

            precessionDelta += Time.deltaTime;

            CalculateArena();
            Manifest();
            CalculateEnergy();
        }

        void CalculateArena()
        {
            PrimaryPosition = new Vector3(Position.x, (float)(Distance / 2), Position.y);
            PrimaryScale = new Vector3((float)Diameter, (float)(Distance), (float)Diameter);

            SecondaryPosition = Mul(PrimaryPosition, new Vector3(1, Contact.y, 1));
            SecondaryScale = Mul(PrimaryScale, new Vector3(Contact.x, Contact.y, Contact.x));

            Precession = Quaternion.AngleAxis(1 / 360 * (Contact.z * precessionDelta), Bifurcation);
        }

        void Manifest()
        {
            Primary.localPosition = PrimaryPosition;
            Primary.localScale = PrimaryScale;

            Secondary.localPosition = SecondaryPosition;
            Secondary.localScale = SecondaryScale;
            Secondary.localRotation = Precession;

            PrimaryMesh.material.color = ColorGradient.Evaluate(pTime);
            SecondaryMesh.material.color = SecondaryColorGradient.Evaluate(sTime);
        }

        public void ApplyForceAffair(Particle particle)
        {
            /// Calculate substantiation
            float substantiation = CalculateSubstantiation(particle);

            // Apply the appropriate force based on the substantiation
            Vector3 effect;
            effect = ApplyTransitionBehavior(particle, substantiation);

            // Apply the calculated effect to the particle
            particle.Delta += effect;
        }

        Vector3 CalculateClassicalForce(Particle particle, float substantiation)
        {
            // Classical force calculation, potentially utilizing properties from Particle and DescriptorContactor
            Vector3 deltaA = Well().Range() * WellDirection(Primary.position);
            Vector3 deltaB = Well().Range() * WellDirection(Secondary.position);

            Vector3 deltaP = particle.Assert(Time.deltaTime);

            Vector3 adjPosA = deltaP - deltaA;
            Vector3 adjPosB = deltaP - deltaB;

            Vector3 adjPosBTrans = Secondary.TransformPoint(adjPosB);
            Vector3 adjPosBInvTrans = Primary.InverseTransformPoint(adjPosBTrans);

            Vector3 delta = adjPosA - adjPosBInvTrans;

            float reductionRatio = (float)particle.PrimaryReduction(Source().Diameter);
            float amplificationRatio = (float)particle.SecondaryReduction(Source().Diameter);

            Vector3 rForce = reductionRatio * delta;
            Vector3 aForce = amplificationRatio * delta;

            // Use substantiation to scale classical force
            float classicalForceFactor = 1f - substantiation;

            Vector3 force;
            force = (rForce + aForce) * classicalForceFactor;
            force = particle.Applicate(particle.Ascribe(Time.deltaTime).magnitude * Particle.EnergeticResistance * force);
            force /= (float)Source().UnitAscriptiveDensity;

            return force;
        }

        Vector3 CalculateQuantumForce(Particle particle, float substantiation)
        {
            // Quantum force calculation (similar to classical, but with added uncertainty)
            Vector3 force = CalculateClassicalForce(particle, substantiation);

            // Add quantum uncertainty to the force
            float uncertaintyFactor = forceStrength * substantiation; // Higher uncertainty closer to quantum threshold
            Vector3 uncertainty = new Vector3(
                particle.Indiscern(0.1f * uncertaintyFactor),
                particle.Indiscern(0.1f * uncertaintyFactor),
                particle.Indiscern(0.1f * uncertaintyFactor));
            force = Mul(force, uncertainty);

            return force;
        }

        Vector3 ApplyTransitionBehavior(Particle particle, float substantiation)
        {
            // Interpolate between classical and quantum forces
            Vector3 classicalForce = CalculateClassicalForce(particle, substantiation);
            Vector3 quantumForce = CalculateQuantumForce(particle, substantiation);

            float t = (substantiation - classicalThreshold) / (quantumThreshold - classicalThreshold); // Interpolation factor
            return Vector3.Lerp(classicalForce, quantumForce, t);
        }

        public void Friction(Particle particle)
        {
            float substantiation = CalculateSubstantiation(particle);

            Vector3 particlePosition = particle.transform.position;
            Vector3 primaryToParticle = particlePosition - Primary.position;
            Vector3 secondaryToParticle = particlePosition - Secondary.position;

            // Determine the sign of the friction force based on the particle's position
            // relative to the primary and secondary wells
            float primarySign = Mathf.Sign(Vector3.Dot(primaryToParticle, Bifurcation));
            float secondarySign = Mathf.Sign(Vector3.Dot(secondaryToParticle, Bifurcation));

            // Calculate distance to wells and their influence
            float distToPrimary = Vector3.Distance(particlePosition, Primary.position);
            float distToSecondary = Vector3.Distance(particlePosition, Secondary.position);

            float primaryInfluence = Mathf.Clamp01(1f - distToPrimary / (float)Diameter);
            float secondaryInfluence = Mathf.Clamp01(1f - distToSecondary / (float)Diameter);

            // Friction and Torque Calculation
            Vector3 frictionForce = Bifurcation * (primaryInfluence * primarySign - secondaryInfluence * secondarySign) *
                                   particle.Ascribe(Time.deltaTime).magnitude * FrictionCoefficient;
            Vector3 torque = Vector3.Cross(Bifurcation, particle.Assert(Time.deltaTime)) * TorqueCoefficient;

            // Apply force and torque
            particle.AddForce(frictionForce, torque, Time.deltaTime);

            // Apply Indiscernment (Uncertainty)
            if (substantiation >= quantumThreshold)
            {
                // Scale down uncertainty factors based on substantiation (measurement)
                float uncertaintyFactor = 1f - substantiation;

                particle.IndiscernProperty(
                    particle.Conduct(Time.deltaTime),
                    Vector3.zero,
                    Time.deltaTime,
                    (v) => { particle.Conductance = v; },
                    () => { return particle.Conductance; });

                particle.IndiscernProperty(
                    particle.Attune(Time.deltaTime),
                    Vector3.zero,
                    Time.deltaTime,
                    (v) => { particle.Attunement = v; },
                    () => { return particle.Attunement; });
            }
        }

    }
}