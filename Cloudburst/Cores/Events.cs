using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Cloudburst.Cores
{
    public class EventsCore
    {
        internal class Ecilpse : Event
        {
            public override void FixedUpdate()
            {
                base.FixedUpdate();
            }

            public override void OnEnable()
            {
                base.OnEnable();
            }

            public override void OnDisable()
            {
                base.OnDisable();
            }
        }

        internal class Event
        {
            internal abstract class Chance {
                /// <summary>
                /// Roll this.
                /// </summary>
                private Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)DateTime.UtcNow.Ticks);
                /// <summary>
                /// UpperLimit
                /// </summary>
                public abstract float lowerLimit { get; }
                /// <summary>
                /// UpperLimit
                /// </summary>
                public abstract float upperLimit { get; }

                public float GetChance() {
                    return rng.RangeFloat(lowerLimit, upperLimit);
                }

            }
            public Chance chance;

            public virtual void OnEnable()
            {

            }
            public virtual void OnDisable()
            {

            }

            public virtual void FixedUpdate()
            {

            }

        }
        public EventsCore() => FUCK();

        private List<Event> activeEvents = new List<Event>();
        private List<Event> events = new List<Event>();

        public void FUCK() {
            //CloudburstPlugin.onFixedUpdate += CloudburstPlugin_onFixedUpdate;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            foreach (Event eve in activeEvents)
            {
                eve.OnDisable();
                activeEvents.Remove(eve);
            }

                foreach (Event eve in events)
            {
                float chance = eve.chance.GetChance();
                bool isActive = Util.CheckRoll(chance);
                if (isActive) {
                    activeEvents.Add(eve);
                }

            }
            foreach (Event eve in activeEvents) {
                eve.OnEnable();
            }
            //fuck
        }

        private void CloudburstPlugin_onFixedUpdate()
        {
            
        }
    }
}
