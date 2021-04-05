using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cloudburst.Cores
{
    public class EventsCore
    {
        internal class Ecilpse : Event
        {
            public override float chance => 25;

            public override void FixedUpdate()
            {
                base.FixedUpdate();
            }

            public override void OnEnable()
            {
                base.OnEnable();
                var scene = SceneManager.GetActiveScene();
                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    LogCore.LogI(obj);
                    if (obj.name == "HOLDER: Misc Props")
                    {
                        LogCore.LogI("found");
                        Transform goo = obj.transform.Find("GooPlane, High");
                        goo.gameObject.SetActive(true);
                        goo.gameObject.AddComponent<MeshCollider>();
                    }
                }
            }

            public override void OnDisable()
            {
                base.OnDisable();
            }

            public override bool CanBeActivated()
            {
                return false;
                //throw new NotImplementedException();
            }
        }

        internal class TarRiver : Event
        {
            internal class TarRiverSlow : MonoBehaviour {
                public void OnTriggerEnter(Collider collider)
                {
                    LogCore.LogI(collider);
                }
                public void OnTriggerExit(Collider collider)
                {
                    LogCore.LogI(collider);
                }
            }

            public override float chance => 100;

            public override void OnEnable()
            {
                base.OnEnable();
                LogCore.LogI("good morning chat");
                var scene = SceneManager.GetActiveScene();

                GameObject parent = null;
                foreach (GameObject obja in scene.GetRootGameObjects())
                {
                    //LogCore.LogI(obj);
                    if (obja.name == "HOLDER: Misc Props")
                    {
                        parent = obja;
                        //LogCore.LogI("found");
                        //goo = obja.transform.Find("GooPlane, High");
                    }
                }

                AddGoo();
                AddWarnigSigns();

                /*
                goo.localScale = new Vector3(42.92972f, 1, 42.04618f);
                goo.gameObject.SetActive(true);
                MeshCollider col = goo.gameObject.AddComponent<MeshCollider>();
                col.convex = true;
                col.isTrigger = true;
                goo.AddComponent<TarRiverSlow>();*/

                void AddGoo()
                {
                    Transform goo = parent.transform.Find("GooPlane, High");
    

                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    obj.transform.position = new Vector3(201f, -134.1f, 143f);
                    obj.transform.rotation = Quaternion.Euler(0, -43.019f, 0);
                    obj.transform.localScale = new Vector3(429.2972f, 10, 420.4618f);

                    Renderer renderer = obj.AddOrGetComponent<Renderer>();
                    renderer.material = goo.GetComponent<Renderer>().material;

                    BoxCollider collider = obj.AddOrGetComponent<BoxCollider>();
                    collider.isTrigger = true;
                }
                void AddWarnigSigns() {
                    Transform warningParent =  parent.transform.Find("Warning Signs");
                    Transform warning = warningParent.Find("GlWarningSign");
                    warning.GetComponent<MeshCollider>().convex = false;
                    warning.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                    CloudburstPlugin.Instantiate<GameObject>(warning.gameObject, new Vector3(50.71f, -117.16f, 105.15f), Quaternion.Euler(new Vector3(1, 1, 1)));
                    CloudburstPlugin.Instantiate<GameObject>(warning.gameObject, new Vector3(16.44f, -122.26f, 108f), Quaternion.Euler(new Vector3(6.082f, 54.268f, -11.764f)));
                }
            }

            public override void OnDisable()
            {
                base.OnDisable();
            }

            public override bool CanBeActivated()
            {
                return SceneCatalog.GetSceneDefForCurrentScene() && SceneCatalog.GetSceneDefForCurrentScene().nameToken == "MAP_GOOLAKE_TITLE";
            }
        }

        internal abstract class Event
        {
            //employers, don't take this as a sign of my code quality
            //sometimes, i'll just do something incredibly worthless and unneeded
            //but its very rare so pls hire me

            /*internal abstract class Chance {
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

            }*/

            public abstract float chance { get; }

            public bool canBeActivated
            { get {
                    return CanBeActivated();
                }
            }

            public virtual void Init()
            {

            }

            public virtual void OnEnable()
            {

            }
            public virtual void OnDisable()
            {

            }

            public virtual void FixedUpdate()
            {

            }

            public  abstract bool CanBeActivated();

        }
        public EventsCore() => FUCK();

        private List<Event> activeEvents = new List<Event>();
        private List<Event> events = new List<Event>();

        public void FUCK() {
            //CloudburstPlugin.onFixedUpdate += CloudburstPlugin_onFixedUpdate;
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(Event)));
            foreach (var itemType in ItemTypes)
            {
                Event item = (Event)Activator.CreateInstance(itemType);
                LogCore.LogI(item);
                item.Init();
                events.Add(item);
            }
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            foreach (Event eve in activeEvents)
            {
                eve.OnDisable();
            }
            activeEvents.Clear();

            foreach (Event eve in events)
            {
                float chance = eve.chance;
                bool isActive = Util.CheckRoll(chance) && eve.canBeActivated;
                if (isActive)
                {
                    activeEvents.Add(eve);
                }

            }
            foreach (Event eve in activeEvents)
            {
                eve.OnEnable();
            }
            //fuck
        }

        private void CloudburstPlugin_onFixedUpdate()
        {
            
        }
    }
}
