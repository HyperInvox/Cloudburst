using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

using UnityEngine.Rendering.PostProcessing;
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
            public static GameObject ppv;
            internal static List<Vector3> safeLocals = new List<Vector3>()
            {
                new Vector3(191.3f, -115.1f, 124.0f),
                new Vector3(232.7f, -118.0f, 104.7f),
                new Vector3(77.0f, -119.1f, 145.0f),
                new Vector3(211.7f, -111.0f, 138.1f),
                new Vector3(120.1f, -119.6f, 119.4f),
                new Vector3(109.6f, -76.9f, -183.7f),
                new Vector3(75.6f, -119.8f, 139.2f),
                new Vector3(175.0f, -74.8f, -158.6f),
                new Vector3(58.3f, -118.3f, 15.7f),
                new Vector3(135.1f, -79.1f, -152.2f),
                new Vector3(69.6f, -120.8f, 123.6f),
                new Vector3(84.1f, -120.4f, 141.0f),
                new Vector3(196.5f  , -122.1f, 67.1f),
                new Vector3(186.7f, -114.7f, 129.7f),
                new Vector3(139.2f, -117.2f, 82.0f),
                new Vector3(78.6f, -120.2f, 111.8f),
                new Vector3(254.5f, -118.2f, 105.0f),
                new Vector3(-24.6f, -109.9f, -162.1f),
                new Vector3(146.1f, -79.4f, -137.2f),
                new Vector3(211.6f, -121.6f, 73.8f),
                new Vector3(164.9f, -120.1f, 79.3f),
                new Vector3(62.9f, -123.5f, 201.3f),
                new Vector3(12.8f, -127.0f, 73.8f),
                new Vector3(258.3f, -117.5f, 116.8f),
            };

            internal class TarRiverSlow : MonoBehaviour
            {
                public static GameObject entryEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/ClayGooOrbImpact");
                private List<CharacterBody> bodies = new List<CharacterBody>();
                private float stopwatch = 0;
                public void OnTriggerEnter(Collider collider)
                {
                    LogCore.LogI(collider);
                    if (NetworkServer.active)
                    {
                        TeleporterInteraction tp = collider.GetComponentInParent<TeleporterInteraction>();
                        if (tp)
                        {
                            LogCore.LogE("Teleporter found, replacing!");
                            Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)DateTime.UtcNow.Ticks);
                            int roll = rng.RangeInt(0, safeLocals.Count);
                            tp.transform.position = safeLocals[roll];
                            gameObject.layer = LayerIndex.defaultLayer.intVal;
                        }
                        gameObject.layer = LayerIndex.defaultLayer.intVal;
                        CharacterBody body = collider.GetComponentInParent<CharacterBody>();
                        if (body)
                        {
                            EffectManager.SpawnEffect(entryEffect, new EffectData()
                            {
                                origin = body.transform.position,
                                scale = 25,
                            }, true);
                            bodies.Add(body);

                            /*var thingie = body.GetComponent<DestroyCameraTargetEffectOnBuffEnd>();
                            LogCore.LogI(thingie);
                            if (!thingie) {
                                LogCore.LogI(thingie);
                                thingie = body.AddComponent<DestroyCameraTargetEffectOnBuffEnd>();
                                thingie.SetShit(RoR2Content.Buffs.ClayGoo, body, CloudburstPlugin.Instantiate<GameObject>(obj, body.transform.position, body.transform.rotation));
                            }*/
                        }
                    }
                }

                public void FixedUpdate()
                {
                    if (NetworkServer.active)
                    {
                        stopwatch += Time.fixedDeltaTime;
                        if (stopwatch >= 0.1f)
                        {
                            foreach (CharacterBody body in bodies)
                            {
                                body.AddTimedBuff(RoR2Content.Buffs.ClayGoo/*BuffCore.instance.riverGoo/*RoR2Content.Buffs.ClayGoo*/, 3);
                            }
                            stopwatch = 0;
                        }
                    }
                }

                public void OnTriggerExit(Collider collider)
                {
                    LogCore.LogI(collider);
                    if (NetworkServer.active)
                    {
                        CharacterBody body = collider.GetComponentInParent<CharacterBody>();
                        if (body)
                        {
                            bodies.Remove(body);
                        }
                    }
                }

                public void OnDestroy()
                {
                    bodies.Clear();
                }
            }

            public override float chance => 75;

            public static GameObject obj;
            public override void Init()
            {
                base.Init();
                var asyncStageLoad = SceneManager.LoadSceneAsync("goolake", LoadSceneMode.Additive);
                asyncStageLoad.allowSceneActivation = false;
                GameObject yo = null;
                asyncStageLoad.completed += ___ =>
                {
                    var scene = SceneManager.GetSceneByName("goolake"); ;
                    var root = scene.GetRootGameObjects();

                    for (int i = 0; i < root.Length; i++)
                    {
                        GameObject obja = root[i];
                        if (obja.name == "HOLDER: GameplaySpace")
                        {
                            yo = obja.transform.Find("Terrain").Find("mdlGlDam").Find("mdlGlAqueductPartial").Find("GooWaterfall").Find("DEBUFF ZONE: Waterfall").Find("PP Goo").gameObject.InstantiateClone("PPVThingie", false);                            //LogCore.LogI("found");
                            //goo = obja.transform.Find("GooPlane, High");
                        }
                    }
                };
                obj = yo;
            }

            public override void Start()
            {
                base.Start();

            }

            public override void OnEnable()
            {
                base.OnEnable();
                LogCore.LogI("good morning chat");
                var scene = SceneManager.GetActiveScene();

                GameObject parent = null;
                GameObject particles = null;
                foreach (GameObject obja in scene.GetRootGameObjects())
                {
                    //LogCore.LogI(obj);
                    if (obja.name == "HOLDER: Misc Props")
                    {
                        parent = obja;
                        //LogCore.LogI("found");
                        //goo = obja.transform.Find("GooPlane, High");
                    }
                    if (obja.name == "HOLDER: GameplaySpace")
                    {
                        particles = obja;
                        //LogCore.LogI("found");
                        //goo = obja.transform.Find("GooPlane, High");
                    }
                }

                AddGoo();
                AddWarnigSigns();
                AddPPVs();
                /*
                goo.localScale = new Vector3(42.92972f, 1, 42.04618f);
                goo.gameObject.SetActive(true);
                MeshCollider col = goo.gameObject.AddComponent<MeshCollider>();
                col.convex = true;
                col.isTrigger = true;
                goo.AddComponent<TarRiverSlow>();*/

                void AddPPVs()
                {
                    ppv = particles.transform.Find("Terrain").Find("mdlGlDam").Find("mdlGlAqueductPartial").Find("GooWaterfall").Find("DEBUFF ZONE: Waterfall").Find("PP Goo").gameObject;
                    //CloudburstPlugin.Instantiate<GameObject>(ppBase, new Vector3(201.7f, -139.5f, 246.2f), Quaternion.Euler(new Vector3(1, 1, 1))).GetComponent<PostProcessVolume>().blendDistance = 20;
                    //CloudburstPlugin.Instantiate<GameObject>(ppBase, new Vector3(307.5f, -135.1f, 174.2f), Quaternion.Euler(new Vector3(1, 1, 1))).GetComponent<PostProcessVolume>().blendDistance = 20;
                    //CloudburstPlugin.Instantiate<GameObject>(ppBase, new Vector3(260.9f, -135.7f, 42.8f), Quaternion.Euler(new Vector3(1, 1, 1))).GetComponent<PostProcessVolume>().blendDistance = 20;

                }

                void AddGoo()
                {
                    Transform goo = parent.transform.Find("GooPlane, High");

                    if (NetworkServer.active)
                    {

                        //LogCore.LogI("hi");
                        var obj = CloudburstPlugin.Instantiate<GameObject>(AssetsCore.tarRiver, new Vector3(201f, -128.8f, 143f), Quaternion.Euler(new Vector3(0, -43.019f, 0)));

                        //LogCore.LogI("h2");
                        obj.transform.Find("Single Floating Particle").GetComponent<ParticleSystemRenderer>().material = particles.transform.Find("Terrain").Find("mdlGlDam").Find("mdlGlAqueductPartial").Find("GooWaterfall").Find("Single Floating Particle").GetComponent<ParticleSystemRenderer>().material;

                        //LogCore.LogI("hi3");
                        /*obj.AddComponent<TarRiverSlow>();
                        obj.layer = LayerIndex.world.intVal;*/
                        obj.transform.position = new Vector3(201f, -134.1f, 143f);
                        obj.transform.rotation = Quaternion.Euler(0, -43.019f, 0);
                        obj.transform.localScale = new Vector3(429.2972f, 10, 420.4618f);
                        obj.GetComponent<Renderer>().material = goo.GetComponent<Renderer>().material;
                        /*obj.AddComponent<NetworkIdentity>();
                        obj.AddComponent<NetworkTransform>();*/
                        //LogCore.LogI("hi4");


                        NetworkServer.Spawn(obj);
                        LogCore.LogI("hi5");
                    }

                    /*GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    obj.transform.position = new Vector3(201f, -134.1f, 143f);
                    obj.transform.rotation = Quaternion.Euler(0, -43.019f, 0);
                    obj.transform.localScale = new Vector3(429.2972f, 10, 420.4618f);
                    Renderer renderer = obj.AddOrGetComponent<Renderer>();
                    renderer.material = goo.GetComponent<Renderer>().material;
                    BoxCollider collider = obj.AddOrGetComponent<BoxCollider>();
                    collider.isTrigger = true;*/
                }
                void AddWarnigSigns()
                {
                    Transform warningParent = parent.transform.Find("Warning Signs");
                    Transform warning = warningParent.Find("GlWarningSign");
                    warning.GetComponent<MeshCollider>().convex = false;
                    warning.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                    CloudburstPlugin.Instantiate<GameObject>(warning.gameObject, new Vector3(50.71f, -117.16f, 105.15f), Quaternion.Euler(new Vector3(1, 1, 1)));
                    CloudburstPlugin.Instantiate<GameObject>(warning.gameObject, new Vector3(16.44f, -122.26f, 108f), Quaternion.Euler(new Vector3(6.082f, 54.268f, -11.764f)));

                    /*if (NetworkServer.active)
                    {
                       var quirky = CloudburstPlugin.Instantiate<GameObject>(AssetsCore.tarRaft, new Vector3(151.4241f, -129.9794f, 221.6763f), Quaternion.Euler(new Vector3(0, -0, 0)));
                       NetworkServer.Spawn(quirky);
                    }*/
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

        /*internal class DayTimeDelta : Event
        {
            public override float chance => 0;

            public override void Init()
            {
                base.Init();
            }

            public override void Start()
            {
                base.Start();

            }

            public override void OnEnable()
            {
                base.OnEnable();
                PostProcessProfile[] source = Resources.FindObjectsOfTypeAll<PostProcessProfile>();
                PostProcessProfile profile = (from p in source
                                              where p.name == "ppLocalClayBossDeath"//"ppLocalUnderwater"
                                              select p).FirstOrDefault<PostProcessProfile>();
                //var thang = profile.settings[0];
                
                profile.settings[0] = (from p in source where p.name == "ppLocalRez" select p).FirstOrDefault<PostProcessProfile>().settings[0];


                CloudUtils.AlterCurrentPostProcessing(profile);
            }

            public override void OnDisable()
            {
                base.OnDisable();
            }

            public override bool CanBeActivated()
            {
                return SceneCatalog.GetSceneDefForCurrentScene() && SceneCatalog.GetSceneDefForCurrentScene().nameToken == "MAP_FROZENWALL_TITLE";
            }
        }*/

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
            {
                get
                {
                    return CanBeActivated();
                }
            }

            public virtual void Init()
            {
                CloudburstPlugin.start += Start;
            }

            public virtual void Start()
            {
                //throw new NotImplementedException();
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

            public abstract bool CanBeActivated();

        }
        public EventsCore() => FUCK();

        private List<Event> activeEvents = new List<Event>();
        private List<Event> events = new List<Event>();

        public void FUCK()
        {
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
            if (NetworkServer.active) { 
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
            }//*/
            //fuck
        }

        private void CloudburstPlugin_onFixedUpdate()
        {

        }
    }
}