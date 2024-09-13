using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalMachine
{
    public class Contactor : MonoBehaviour
    {
        public EnscribedDisc Meaning;
        public LightSource Radiance;

        public ExistenceGradient Zone;

        public PathMarker Marker;

        public ForceExchange Contacts;

        public FluidDispensary Well;

        public DescriptorContactor Source;
        public PotentialHole Depths;

        public SimpleShacle Binding;

        public EnscriptionLimit Limit;

        public List<Particle> UnitQuanta = new List<Particle>();

        System.Random r = new System.Random();

        void Awake()
        {
            ContactorSetup();
            DispensarySetup();
            ExistenceSetup();
            LimitSetup();
        }

        //void DarkRadiance()
        //{
        //    foreach (Particle particle in UnitQuanta)
        //    {
        //        float distance = Vector3.Distance(Meaning.transform.position, particle.PointPosition(Time.deltaTime));
        //        double energy = Radiance.GetIntensity(distance);
        //        Vector3 offset = Meaning.transform.right * (float)Radiance.GetReactivity(distance) * (float)energy;
        //        particle.AddForce(offset, Vector3.zero, Time.deltaTime);
        //    }
        //}

        void ContactorSetup()
        {
            Source.Ascriptions = () => { return UnitQuanta.Count; }; //Particle Counter Function
            Source.ContactRatio = () => { return (float)(1 / Zone.Diameter * Source.Diameter);  }; //Unit/Area Contact Ratio Amount
        }

        void DispensarySetup()
        {
            Well.Quanta = () => { return UnitQuanta.Count; };
            Well.Range = () => { return (int)Source.AssertationScale; };

            Well.OnDestroy = (p) => { UnitQuanta.Remove(p);
                                      if (Source.FutureContacts.Contains(p)) { Source.FutureContacts.Remove(p); }
            };

            Well.Approach = () => { return (float)r.NextDouble(); };
            Well.SafetyZone = () => { return Source.Diameter * 0.01f; };
            Well.Diameter = () => { return Zone.Diameter; };

            Well.IndexParticle = (p) => { UnitQuanta.Add(p); };

            Well.ProjectionReceivance = () => { return Marker.Project; };

            Well.ExistentCapacity = () => { return (float)Source.UnitAscriptiveDensity; };
            Well.ContactDepth = () => { return (float)Source.ContactRatio() * (float)Source.UnitAscriptiveDensity; };

            Well.SpawnAction = (p) =>
            {
                Source.Birth(p);
                Limit.Queue.Enqueue(p);
            };
        }

        void ExistenceSetup()
        {
            Zone.Quanta = () => { return UnitQuanta; };
            Zone.WellDistance = () =>
            {
                return new Vector2(
                    Vector3.Distance(Well.transform.position, Zone.Primary.transform.position),
                    Vector3.Distance(Well.transform.position, Zone.Secondary.transform.position)
                    );
            };

            Zone.WellDirection = (exPos) =>
            {
                return (Well.transform.position - exPos).normalized;
            };

            Zone.Well = () => { return Well; };
            Zone.Source = () => { return Source; };
        }

        private void LimitSetup()
        {
            Limit.DestroyParticle = (p) =>
            {
                Well.OnDestroy(p);
                Destroy(p.gameObject);
            };
        }

        void FixedUpdate()
        {
            
            
            //DarkRadiance();
            
            //Contacts.Exchange(Particles);
            
            //Zone.Height = Meaning.Height;
            //Zone.Friction(Particles);
            
            //Binding.Bind(Particles);
        }
    }
}