using EntityStates;
using RoR2;
using UnityEngine;
public class BaseEmote : BaseState
{
    protected string soundString;
    protected string animString;
    public float duration;
    public float animDuration;

    private uint activePlayID;
    private float initialTime;
    private Animator animator;
    private ChildLocator childLocator;

    public override void OnEnter()
    {
        base.OnEnter();
        this.animator = base.GetModelAnimator();
        this.childLocator = base.GetModelChildLocator();

        base.characterBody.hideCrosshair = true;

        if (base.GetAimAnimator()) base.GetAimAnimator().enabled = false;
        this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0);
        this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0);


        if (this.animDuration == 0 && this.duration != 0) this.animDuration = this.duration;
        //if (this.duration == 0 && this.animDuration != 0) this.duration = animDuration;

        if (this.duration > 0)
        {
            base.PlayAnimation("FullBody, Override", this.animString, "Emote.playbackRate", this.animDuration);
        }
        else
        {
            this.animator.SetFloat("Emote.playbackRate", 1f);
            base.PlayAnimation("FullBody, Override", this.animString);
        }

        if (!string.IsNullOrEmpty(soundString))
        {
            this.activePlayID = Util.PlaySound(soundString, base.gameObject);
        }

        this.initialTime = Time.fixedTime;

    }

    public override void OnExit()
    {
        base.OnExit();

        base.characterBody.hideCrosshair = false;

        if (base.GetAimAnimator()) base.GetAimAnimator().enabled = true;
        this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1);
        this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1);

        base.PlayAnimation("FullBody, Override", "BufferEmpty");
        if (this.activePlayID != 0) AkSoundEngine.StopPlayingID(this.activePlayID);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        bool flag = false;

        if (base.characterMotor)
        {
            if (!base.characterMotor.isGrounded) flag = true;
            //if (base.characterMotor.velocity != Vector3.zero) flag = true;
        }

        if (base.inputBank)
        {

            if (base.inputBank.CheckAnyButtonDown()) flag = true;
            if (base.inputBank.moveVector.sqrMagnitude > 0.1f) flag = true;

        }

        if (this.duration > 0 && base.fixedAge >= this.duration) flag = true;

        CameraTargetParams ctp = base.cameraTargetParams;
        float denom = (1 + Time.fixedTime - this.initialTime);
        float smoothFactor = 8 / Mathf.Pow(denom, 2);
        Vector3 smoothVector = new Vector3(-3 / 20, 1 / 16, -1);
        ctp.idealLocalCameraPos = new Vector3(0f, -1.4f, -6f) + smoothFactor * smoothVector;

        if (flag)
        {
            base.PlayAnimation("FullBody, Override", "EnigmaIsAwful");

            this.outer.SetNextStateToMain();
            return;
        }
    }

    public override InterruptPriority GetMinimumInterruptPriority()
    {
        return InterruptPriority.Any;
    }
    public class CustodianSickness : BaseEmote
    {
        public override void OnEnter()
        {
            this.animString = "TooCool4School";
            this.soundString = "";
            this.animDuration = 1.5f;
            base.OnEnter();
        }
    }
    //            if (base.inputBank.CheckAnyButtonDown()) flag = true;
    //if (base.inputBank.moveVector.sqrMagnitude > 0.1f) flag = true;

}
