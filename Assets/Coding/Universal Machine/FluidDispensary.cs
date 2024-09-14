using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalMachine
{
    public class FluidDispensary : MonoBehaviour
    {
        public GameObject Prefabricant;

        public GameObject QuantaTree;

        public Action<Particle> OnDestroy;

        public int QuantaPerSecond;

        public Func<int> Quanta;

        public Func<int> Range;

        public Func<double> Diameter;

        public Func<float> Approach;

        public Func<float> SafetyZone;

        public Func<float> ExistentCapacity;

        public Func<Func<Vector3, Vector3, Vector3, Vector3>> ProjectionReceivance;

        public Func<float> ContactDepth;

        public Action<Particle> IndexParticle;

        public Action<Particle> SpawnAction;


        int quantaRelesed;

        System.Random r = new System.Random();

        // 12/30/2021 : 11:34 PM

        void FixedUpdate()
        {
            //quantaRelesed = 0;

            //int quantaAvailable = (int)(QuantaPerSecond * Time.deltaTime);
            for (int i = Quanta(); i < Range(); i++)
            {
                //if (quantaRelesed >= quantaAvailable)
                //{
                //    return;
                //}

                BeginParticle();
                //quantaRelesed++;
            }
        }

        public void BeginParticle()
        {
            int posX = 1;
            int posZ = 1;

            int rand = r.Next(0, 2);
            if (rand == 1) posX = -1;

            rand = r.Next(0, 2);
            if (rand == 1) posZ = -1;

            float x = (float)Diameter() / 2 * (float)r.NextDouble() * posX;
            float y = SafetyZone();
            float z = (float)Diameter() / 2 * (float)r.NextDouble() * posZ;

            // No initial force needed
            // Vector3 initialForce = new Vector3(fx, fy, fz);

            Vector3 initialPosition = new Vector3(x, y, z);
            // No initial energy needed
            // Vector3 initialEnergy = Vector3.one * (float)ascriptiveDensity;

            GameObject particle = Instantiate(Prefabricant, initialPosition, Quaternion.identity);
            particle.transform.parent = QuantaTree.transform;
            particle.transform.localPosition = initialPosition;
            particle.SetActive(true);

            Particle p = particle.GetComponent<Particle>();

            p.Project = ProjectionReceivance();

            // Initialize Ascription with a default value 
            p.Assertion = Vector4.one * 0.001f;
            p.Ascription = new Vector4(1, 1, 1, 0.001f);

            // Initialize Depth with a default value
            p.Depth = 0.4f; // Replace with a more meaningful calculation if necessary 
            p.Reach = Vector4.one;

            // No initial force needed
            // p.AddForce(initialForce, Vector3.zero, Time.deltaTime);

            IndexParticle(p);

            p.onDestroy += () => { OnDestroy(p); };

            SpawnAction(p);
        }
    }
}