using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Use TextMeshPro for curved text

namespace UniversalMachine
{
    public class EnscribedDisc : MonoBehaviour
    {
        public Transform Disc;
        public float Diameter;
        public string Enscription;
        public double RotationRate;
        public float Height;
        public float Rotation;
        public Vector3 Flow;

        //Torque strength
        public float TorqueStrength = 1f;

        // Force multiplier
        public float forceMultiplier = 1f;
        public float maxForce = 1f;

        // Lists for characters
        private List<Transform> CharacterTransforms = new List<Transform>();
        private List<TextMeshPro> CharacterTexts = new List<TextMeshPro>();
        private List<int> CharacterIndices = new List<int>();

        // Random number generator
        System.Random r = new System.Random();

        // The number of the enscriptive characters
        int Number = 0;

        void Start()
        {
            // Set the total number value of the enscriptive characters
            Number = NumberValue();

            // Update disc scale and position
            Disc.localScale = new Vector3((float)Diameter * 1.45f, (float)Height * 0.03f, (float)Diameter * 1.45f);
            Disc.localPosition = new Vector3(Disc.localPosition.x, (float)Height, Disc.localPosition.z);

            // Create character objects
            CreateCharacterObjects();

            // Set axis of force application
            Flow = Disc.up + Disc.right;
        }

        void CreateCharacterObjects()
        {
            char[] characters = Enscription.ToCharArray();
            float circumference = Mathf.PI * Diameter;
            float characterArcLength = circumference / characters.Length;

            // Reverse the order of the characters so they appear in the correct order on the disc
            Array.Reverse(characters);

            // Create a parent object for the text objects
            GameObject textParent = new GameObject("TextParent");
            textParent.transform.parent = Disc.Find("Mesh");
            textParent.transform.localPosition = Vector3.zero;
            textParent.transform.localRotation = Quaternion.identity;
            textParent.transform.localScale = Vector3.one;

            // Create a text object for each character
            for (int i = 0; i < characters.Length; i++)
            {
                // Create a new GameObject for the character
                GameObject characterObject = new GameObject($"Character_{i}");
                characterObject.transform.parent = textParent.transform;
                CharacterTransforms.Add(characterObject.transform);

                // Add a TextMeshPro component to the character object
                TextMeshPro characterText = characterObject.AddComponent<TextMeshPro>();
                characterText.text = characters[i].ToString();
                characterText.enableAutoSizing = true;
                characterText.fontSizeMin = 1;
                characterText.fontSizeMax = 5; // Fine-tune this value
                characterText.alignment = TextAlignmentOptions.Center;
                CharacterTexts.Add(characterText);

                // Get the ASCII value of the character
                CharacterIndices.Add(characterText.text[0]);

                // Adjust angle and position for curved placement, taking into account text width
                float angle = (i * characterArcLength / circumference) * 360f;
                angle -= 90f; // Rotate 90 degrees to start at the top of the disc

                // Get the radius of the disc
                float radius = Diameter / 2f;
                radius /= 1.25f; // AScale down the radius to fit the text on the disc

                // Calculate x and z positions based on angle and radius
                float x = Mathf.Sin(angle * Mathf.Deg2Rad) * (radius);
                float z = Mathf.Cos(angle * Mathf.Deg2Rad) * (radius);

                // Adjust the y position so the text object is flush with the top of the disc
                float y = Height / 2f + 0.04f;

                // Set position on the top outer edge of the disc
                characterObject.transform.localPosition = new Vector3(x, y, z);  // Adjust 0.1f as needed


                // Rotate to face upwards (this should be fine)
                characterObject.transform.localRotation = Quaternion.Euler(90f, 0f, -angle + 180);
            }
        }

        public int NumberValue()
        {
            // Calculate the value of the enscriptive characters
            char[] enscr = Enscription.ToCharArray();

            // Get the ASCII value of each character and sum them up
            int i = 0;
            foreach (char c in enscr)
            {
                i += c - 65;
            }

            return i;
        }

        public float Energy()
        {
            // Cakcukate the energy of the disc based on the value of the enscriptive characters and the area of the disc
            return Number / Height * Diameter;
        }

        public Vector3 Offset()
        {
            // Calculate the angle of the projection of the disc along the axis of the flow direction
            return Diameter * Rotation * Flow;
        }

        void FixedUpdate()
        {
            // Rotate the disc
            Disc.Rotate(new Vector3(0, 1, 0), (float)RotationRate);

            // Update rotation value
            Rotation = Mathf.Repeat(Disc.localRotation.eulerAngles.y / 360f, 1f);

        }


        public void ApplyForce(Particle particle)
        {
            if (!enabled) return;

            // Calculate energy based on the enscriptive characters
            double energy = r.NextDouble() * Energy() / Particle.EnergeticResistance;

            for (int i = 0; i < CharacterTransforms.Count; i++)
            {
                Transform charTransform = CharacterTransforms[i];
                TextMeshPro charText = CharacterTexts[i];

                if (charTransform != null && charTransform.gameObject.activeInHierarchy && charText != null)
                {
                    Vector3 characterPosition = charTransform.position;
                    Vector3 direction = (characterPosition - particle.transform.position).normalized;
                    float distance = Vector3.Distance(characterPosition, particle.transform.position);

                    // Calculate force based on ASCII value and distance
                    float asciiValue = CharacterIndices[i];

                    // Non-linear force scaling (adjust the exponent for desired behavior)
                    float forceMagnitude = Mathf.Pow(asciiValue, 1.5f) / (distance * distance) * (float)energy * forceMultiplier;

                    // Clamp force magnitude (adjust maxForce as needed)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, -maxForce, maxForce);

                    // Calculate torque
                    Vector3 torqueDirection = Vector3.Cross(direction, particle.Velocity); // Perpendicular to both direction and velocity
                    float torqueMagnitude = TorqueStrength * forceMagnitude * Mathf.Sin(Vector3.Angle(direction, charTransform.up) * Mathf.Deg2Rad); // Scale torque based on force and angle
                    Vector3 torque = torqueMagnitude * torqueDirection;

                    // Apply force to the particle
                    particle.AddForce(direction * forceMagnitude, torque, Time.deltaTime);
                }
            }
        }
    }
}
