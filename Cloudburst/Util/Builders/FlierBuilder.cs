﻿using EntityStates;
using KinematicCharacterController;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Assists in the creation of survivors.
/// </summary>
/// 
public class FlierBuilder
{
    /// <summary>
    /// The name by which the game should refer to the character body object.
    /// </summary>
    public string prefabName;
    /// <summary>
    /// The name of the Unity prefab to load for the character's model.
    /// </summary>
    //public string modelName;
    /// <summary>
    /// The Unity prefab to load for the character's model.
    /// </summary>
    public GameObject model;

    public GameObject modelBase = new GameObject("ModelBase");
    public GameObject camPivot = new GameObject("CameraPivot");
    public GameObject aimOrigin = new GameObject("AimOrigin");

    public Sprite defaultSkinIcon;
    public Sprite masterySkinIcon;

    public delegate Material MasterySkinMaterial();

    public MasterySkinMaterial masterySkinDelegate;

    public string masteryAchievementUnlockable;

    public event Action<List<CharacterModel.RendererInfo>, Transform> GetAdditionalRenderInfos;

    public event Action<List<ItemDisplayRuleSet.NamedRuleGroup>> GetAdditionalItemDisplays;
    public event Action<List<ItemDisplayRuleSet.NamedRuleGroup>> GetAdditionalEquipmentDisplays;

    public List<ItemDisplayRuleSet.NamedRuleGroup> itemRules;
    public List<ItemDisplayRuleSet.NamedRuleGroup> equipmentRules;
    /// <summary>
    /// Create a survivor prefab from a model. Don't register the prefab that it outputs, because the method already does that for you.
    /// </summary>
    /// <returns>The prefab created from the model.</returns>
    public GameObject CreatePrefab()
    {
        if (prefabName == "")
        {
            LogCore.LogW("Prefab name has not been set.");
            prefabName = "RandomAssSurvivorBody";
        }

        GameObject prefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), prefabName, true);
        prefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

        SetupModelBase();
        SetupCamera();
        SetupAim();

        void SetupModelBase()
        {
            Cloudburst.CloudburstPlugin.Destroy(prefab.transform.Find("ModelBase").gameObject);
            Cloudburst.CloudburstPlugin.Destroy(prefab.transform.Find("CameraPivot").gameObject);
            Cloudburst.CloudburstPlugin.Destroy(prefab.transform.Find("AimOrigin").gameObject);

            modelBase.transform.parent = prefab.transform;
            modelBase.transform.localPosition = new Vector3(0f, -0.81f, 0f);
            modelBase.transform.localRotation = Quaternion.identity;
            //modelBase.transform.localScale = Vector3.one;
        }

        void SetupCamera()
        {
            camPivot.transform.parent = prefab.transform;
            camPivot.transform.localPosition = new Vector3(0f, -0.81f, 0f);
            camPivot.transform.rotation = Quaternion.identity;
            camPivot.transform.localScale = Vector3.one;
        }

        void SetupAim()
        {
            aimOrigin.transform.parent = prefab.transform;
            aimOrigin.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            aimOrigin.transform.rotation = Quaternion.identity;
            aimOrigin.transform.localScale = Vector3.one;
        }

        if (!model)
        {
            LogCore.LogE("Character model has not been loaded, returning null. " + prefabName + " will not function properly.");
            return null;
        }

        Transform transform = model.transform;
        CharacterDirection dir = prefab.GetComponent<CharacterDirection>();
        CharacterBody body = prefab.GetComponent<CharacterBody>();
        CharacterMotor motor = prefab.GetComponent<CharacterMotor>();
        CameraTargetParams camParams = prefab.GetComponent<CameraTargetParams>();
        ModelLocator locator = prefab.GetComponent<ModelLocator>();
        CharacterModel charModel = transform.AddComponent<CharacterModel>();
        ChildLocator childLoc = model.GetComponent<ChildLocator>();

        TeamComponent teamComponent = null;
        if (prefab.GetComponent<TeamComponent>() != null) teamComponent = prefab.GetComponent<TeamComponent>();
        else teamComponent = prefab.GetComponent<TeamComponent>();

        HealthComponent health = prefab.GetComponent<HealthComponent>();
        CharacterDeathBehavior deathBehavior = prefab.GetComponent<CharacterDeathBehavior>();
        Rigidbody rigidbody = prefab.GetComponent<Rigidbody>();
        CapsuleCollider collider = prefab.GetComponent<CapsuleCollider>();
        KinematicCharacterMotor kMotor = prefab.GetComponent<KinematicCharacterMotor>();
        HurtBoxGroup hurtbox = model.AddComponent<HurtBoxGroup>();
        CapsuleCollider coll1 = model.GetComponentInChildren<CapsuleCollider>();
        HurtBox hb = coll1.gameObject.AddComponent<HurtBox>();
        FootstepHandler footstep = model.AddComponent<FootstepHandler>();
        AimAnimator aimer = model.AddComponent<AimAnimator>();

        SetupModelTransform();
        SetupCharacterDirection();
        SetupCharacterBody();
        SetupCharacterMotor();
        SetupCameraParams();
        SetupModelLocator();
        SetupModel();
        SetupSkins();
        SetupTeamComponent();
        SetupHealthComponent();
        SetupInteractors();
        SetupDeathBehavior();
        SetupRigidBody();
        SetupCollider();
        SetupKCharacterMotor();
        SetupHurtbox();
        SetupFootstep();
        SetupAimAnimator();
        SetupHitbox();
        RegisterItemDisplays();

        void SetupModelTransform()
        {
            transform.parent = modelBase.transform;
            //transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        void SetupCharacterDirection()
        {
            dir.moveVector = Vector3.zero;
            dir.targetTransform = modelBase.transform;
            dir.overrideAnimatorForwardTransform = null;
            dir.rootMotionAccumulator = null;
            dir.modelAnimator = model.GetComponentInChildren<Animator>();
            dir.driveFromRootRotation = false;
            dir.turnSpeed = 720f;
        }

        void SetupCharacterBody()
        {
            body.name = prefabName;
            body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            body.rootMotionInMainState = false;
            body.mainRootSpeed = 0;
            body.bodyIndex = BodyIndex.None;
            body.aimOriginTransform = aimOrigin.transform;
            body.hullClassification = HullClassification.Human;
        }

        void SetupCharacterMotor()
        { //CharacterMotor motor = prefab.GetComponent<CharacterMotor>();
            motor.walkSpeedPenaltyCoefficient = 1f;
            motor.characterDirection = dir;
            motor.muteWalkMotion = false;
            motor.mass = 100f;
            motor.airControl = 0.25f;
            motor.disableAirControlUntilCollision = false;
            motor.generateParametersOnAwake = true;
        }

        void SetupCameraParams()
        {
            camParams.cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<CameraTargetParams>().cameraParams;
            camParams.cameraPivotTransform = null;
            camParams.aimMode = CameraTargetParams.AimType.Standard;
            camParams.recoil = Vector2.zero;
            camParams.idealLocalCameraPos = Vector3.zero;
            camParams.dontRaycastToPivot = false;
        }

        void SetupModelLocator()
        {
            locator.modelTransform = transform;
            locator.modelBaseTransform = modelBase.transform;
            locator.dontReleaseModelOnDeath = false;
            locator.autoUpdateModelTransform = true;
            locator.dontDetatchFromParent = false;
            locator.noCorpse = false;
            locator.normalizeToFloor = false;
            locator.preserveModel = false;
        }

        void SetupTeamComponent()
        {
            teamComponent.hideAllyCardDisplay = false;
            teamComponent.teamIndex = TeamIndex.None;
        }

        void SetupHealthComponent()
        {
            health.body = null;
            health.dontShowHealthbar = false;
            health.globalDeathEventChanceCoefficient = 1f;
        }

        void SetupInteractors()
        {
            prefab.GetComponent<Interactor>().maxInteractionDistance = 3f;
            prefab.GetComponent<InteractionDriver>().highlightInteractor = true;
        }

        void SetupDeathBehavior()
        {
            deathBehavior.deathStateMachine = prefab.GetComponent<EntityStateMachine>();
            deathBehavior.deathState = new SerializableEntityStateType(typeof(GenericCharacterDeath));
        }

        void SetupRigidBody()
        {
            rigidbody.mass = 100f;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.constraints = RigidbodyConstraints.None;
        }

        void SetupCollider()
        {
            collider.isTrigger = false;
            collider.material = null;
            collider.center = Vector3.zero;
            collider.direction = 1;
        }

        void SetupModel()
        {
            charModel.body = body;
            EnigmaticList<CharacterModel.RendererInfo> infos = new EnigmaticList<CharacterModel.RendererInfo>();

            infos.Add(new CharacterModel.RendererInfo
            {
                defaultMaterial = transform.GetComponentInChildren<SkinnedMeshRenderer>().material,
                renderer = transform.GetComponentInChildren<SkinnedMeshRenderer>(),
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = false
            });

            Action<List<CharacterModel.RendererInfo>, Transform> action = this.GetAdditionalRenderInfos;
            action(infos, transform);

            charModel.baseRendererInfos = infos.ToArray();
            charModel.autoPopulateLightInfos = true;
            charModel.invisibilityCount = 0;
            charModel.temporaryOverlays = new List<TemporaryOverlay>();
            //charModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        void SetupKCharacterMotor()
        {
            kMotor.CharacterController = motor;
            kMotor.Capsule = collider;
            kMotor.Rigidbody = rigidbody;
            kMotor.DetectDiscreteCollisions = false;
            kMotor.GroundDetectionExtraDistance = 0f;
            kMotor.MaxStepHeight = 0.2f;
            kMotor.MinRequiredStepDepth = 0.1f;
            kMotor.MaxStableSlopeAngle = 55f;
            kMotor.MaxStableDistanceFromLedge = 0.5f;
            kMotor.PreventSnappingOnLedges = false;
            kMotor.MaxStableDenivelationAngle = 55f;
            kMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kMotor.PreserveAttachedRigidbodyMomentum = true;
            kMotor.HasPlanarConstraint = false;
            kMotor.PlanarConstraintAxis = Vector3.up;
            kMotor.StepHandling = StepHandlingMethod.None;
            kMotor.LedgeHandling = true;
            kMotor.InteractiveRigidbodyHandling = true;
            kMotor.SafeMovement = false;
        }

        void SetupHurtbox()
        {
            hb.gameObject.layer = LayerIndex.entityPrecise.intVal;

            hb.healthComponent = health;
            hb.isBullseye = true;
            hb.damageModifier = HurtBox.DamageModifier.Normal;
            hb.hurtBoxGroup = hurtbox;
            hb.indexInGroup = 0;

            hurtbox.hurtBoxes = new HurtBox[] { hb };
            hurtbox.mainHurtBox = hb;
            hurtbox.bullseyeCount = 1;
        }

        void SetupFootstep()
        {
            footstep.baseFootstepString = "Play_player_footstep";
            footstep.sprintFootstepOverrideString = "";
            footstep.enableFootstepDust = true;
            footstep.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");
        }
        //RagdollController ragdoll = model.GetComponent<RagdollController>();
        //TODO
        //ragdoll.bones = null;
        //ragdoll.componentsToDisableOnRagdoll = null;

        void SetupAimAnimator()
        {
            aimer.inputBank = prefab.GetComponent<InputBankTest>();
            aimer.directionComponent = dir;
            aimer.pitchRangeMax = 60f;
            aimer.pitchRangeMin = -60f;
            aimer.yawRangeMax = 90f;
            aimer.yawRangeMin = -90f;
            aimer.pitchGiveupRange = 30f;
            aimer.yawGiveupRange = 10f;
            aimer.giveupDuration = 3f;
        }

        void SetupHitbox()
        {
            foreach (Transform child in transform)
            {
                if (child.name.Contains("Hitbox"))
                {
                    LogCore.LogI(child.name);
                    var hitBoxGroup = transform.AddComponent<HitBoxGroup>();
                    var hitBox = child.AddComponent<HitBox>();
                    hitBoxGroup.groupName = child.name;
                    hitBoxGroup.hitBoxes = new HitBox[] { hitBox };
                }
            }
        }

        void SetupSkins()
        {
            //R2API.LanguageAPI.Add("NEMMANDO_DEFAULT_SKIN_NAME", "Default");

            var obj = transform.gameObject;
            var mdl = obj.GetComponent<CharacterModel>();
            var skinController = obj.AddComponent<ModelSkinController>();

            Cloudburst.Content.ContentHandler.Loadouts.SkinDefInfo skinDefInfo = new Cloudburst.Content.ContentHandler.Loadouts.SkinDefInfo
            {
                Name = "DEFAULT_SKIN",
                NameToken = "DEFAULT_SKIN",
                Icon = defaultSkinIcon,
                RootObject = obj,
                RendererInfos = mdl.baseRendererInfos,
                GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>(),
                MeshReplacements = Array.Empty<SkinDef.MeshReplacement>(),
                BaseSkins = Array.Empty<SkinDef>(),
                MinionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>(),
                ProjectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>(),
                UnlockableName = ""
            };


            CharacterModel.RendererInfo[] rendererInfos = skinDefInfo.RendererInfos;
            CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);

            array[0].defaultMaterial = masterySkinDelegate.Invoke();

            Cloudburst.Content.ContentHandler.Loadouts.SkinDefInfo masteryInfo = new Cloudburst.Content.ContentHandler.Loadouts.SkinDefInfo
            {
                Name = "DEFAULT_SKIN",
                NameToken = "DEFAULT_SKIN",
                Icon = defaultSkinIcon,
                RootObject = obj,
                RendererInfos = array,
                GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>(),
                MeshReplacements = Array.Empty<SkinDef.MeshReplacement>(),
                BaseSkins = Array.Empty<SkinDef>(),
                MinionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>(),
                ProjectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>(),
                UnlockableName = masteryAchievementUnlockable
            };

            SkinDef skinDefault = Cloudburst.Content.ContentHandler.Loadouts.CreateNewSkinDef(skinDefInfo);
            SkinDef mastery = Cloudburst.Content.ContentHandler.Loadouts.CreateNewSkinDef(masteryInfo);

            SkinDef[] skinDefs = new SkinDef[2]
            {
                skinDefault,
                mastery
            };

            skinController.skins = skinDefs;
        }
        void RegisterItemDisplays()
        {
            ItemDisplayRuleSet itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();

            itemRules = new List<ItemDisplayRuleSet.NamedRuleGroup>();
            equipmentRules = new List<ItemDisplayRuleSet.NamedRuleGroup>();


            Action<List<ItemDisplayRuleSet.NamedRuleGroup>> action2 = this.GetAdditionalItemDisplays;

            action2(itemRules);

            Action<List<ItemDisplayRuleSet.NamedRuleGroup>> action3 = this.GetAdditionalEquipmentDisplays;

            action3(equipmentRules);

            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            ItemDisplayRuleSet.NamedRuleGroup[] item = itemRules.ToArray();
            ItemDisplayRuleSet.NamedRuleGroup[] equip = equipmentRules.ToArray();
            typeof(ItemDisplayRuleSet).GetField("namedItemRuleGroups", bindingAttr).SetValue(itemDisplayRuleSet, item);
            typeof(ItemDisplayRuleSet).GetField("namedEquipmentRuleGroups", bindingAttr).SetValue(itemDisplayRuleSet, equip);

            charModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }


        CloudUtils.RegisterNewBody(prefab);

        return prefab;
    }
}