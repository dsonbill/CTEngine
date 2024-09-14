using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UniversalMachine
{
    public class ParticleField : MonoBehaviour
    {
        List<Particle> Simulands;

        int PreviousParticles;
        int[] CurrentParticles;

        public List<LightSource> LightSources = new List<LightSource>();

        public int ParticlesPerUpdate;

        public System.Random r = new System.Random();

        public static ParticleField Instance;

        public EnscribedDisc Disc;

        public ExistenceGradient Zone;

        public SimpleShacle Shackle;

        public ForceExchange ForceExchanger;

        public SpacetimeFabric Substrate;

        public void Attach(Particle particle)
        {
            Simulands.Add(particle);
        }

        public void Detach(Particle particle)
        {
            Simulands.Remove(particle);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Simulands = new List<Particle>();
            CurrentParticles = new int[0];
            PreviousParticles = 0;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            foreach (Particle particle in Simulands)
            {
                particle.Define(Time.deltaTime);
            }

        }

        void Update()
        {
            int y;
            for (int i = 0; i < ParticlesPerUpdate; i++)
            {
                y = PreviousParticles;
                if (y >= Simulands.Count) continue;


                Disc.ApplyForce(Simulands[y]);

                Zone.Friction(Simulands[y]);
                Zone.ApplyForceAffair(Simulands[y]);

                if(Simulands[y].TimeSinceWarped >= Substrate.WarpVectorThreshold)
                    Substrate.UpdateWarpingVectors(Simulands[y]);

                Substrate.CalculateWarpedPosition(Simulands[y]);

                //Substrate.UpdateParticleShaderProperties(Simulands[y].material);

                foreach (LightSource light in LightSources)
                {
                    light.UpdateParticle(Simulands[y]);
                }

                //Shackle.Bind(Simulands[y]);

                

                PreviousParticles++;
            }

            //List<Particle> simulatedParticles = new List<Particle>();
            //for (int i = 0; i < ParticlesPerUpdate; i++)
            //    simulatedParticles.Add(Simulands[i + PreviousParticles - ParticlesPerUpdate]);

            //if (simulatedParticles.Count - 1 > 0)
            //    ForceExchanger.Exchange(simulatedParticles, Simulands[y]);


            if (PreviousParticles == Simulands.Count)
                PreviousParticles = 0;

            
            
        }
    }
}