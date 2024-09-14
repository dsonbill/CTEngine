using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalMachine
{
    public class EnscriptionLimit : MonoBehaviour
    {
        public Transform Base;

        public List<Particle> Subjects = new List<Particle>();
        public Queue<Particle> Queue = new Queue<Particle>();

        Queue<Particle> RemovalQueue = new Queue<Particle>();

        public Action<Particle> DestroyParticle;

        void FixedUpdate()
        {
            while (Queue.Count > 0)
            {
                Subjects.Add(Queue.Dequeue());
            }

            foreach (Particle p in Subjects)
            {
                if (p.transform.position.y >= transform.position.y || p.transform.position.y <= Base.position.y)
                {
                    DestroyParticle?.Invoke(p);
                    RemovalQueue.Enqueue(p);
                }
            }

            while (RemovalQueue.Count > 0)
            {
                Subjects.Remove(RemovalQueue.Dequeue());
            }
        }
    }
}