using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UniversalMachine
{
    public class ParticleField : MonoBehaviour
    {
        List<Particle> Simulands;

        List<int> PreviousParticles;
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
            PreviousParticles = new List<int>();
        }

        int Target()
        {
            int x = Simulands.Count > 0 ? r.Next(0, Simulands.Count - 1) : -1;
            if (PreviousParticles.Contains(x))
                return Target();

            return x;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            foreach (Particle particle in Simulands)
            {
                particle.Define(Time.deltaTime);
            }

        }

        void Update() {

            CurrentParticles = new int[ParticlesPerUpdate];

            int y;
            for (int i = 0; i < ParticlesPerUpdate; i++)
            {
                y = Target();

                if (y == -1)
                    return;

                if (CurrentParticles.Contains(y))
                    continue;

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

                Shackle.Bind(Simulands[y]);

                List<Particle> simulatedParticles = new List<Particle>();
                foreach (int c in CurrentParticles) 
                    simulatedParticles.Add(Simulands[c]);

                if (simulatedParticles.Count - 1 > 0)
                    ForceExchanger.Exchange(simulatedParticles, Simulands[y]);

                CurrentParticles[i] = y;
            }

            PreviousParticles = new List<int>(CurrentParticles);

            
            
        }
    }
}