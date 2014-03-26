using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class HERO : MonoBehaviour
{
    private string attackAnimation;
    private int attackLoop;
    private bool attackMove;
    private bool attackReleased;
    public AudioSource audio_ally;
    public AudioSource audio_hitwall;
    private float buffTime;
    public GameObject bulletLeft;
    public GameObject bulletRight;
    private bool buttonAttackRelease;
    public bool canJump = true;
    public GameObject checkBoxLeft;
    public GameObject checkBoxRight;
    private int currentBladeNum = 5;
    private float currentBladeSta = 100f;
    private BUFF currentBuff;
    public Camera currentCamera;
    private float currentGas = 100f;
    private Vector3 dashDirection;
    private bool EHold;
    private GameObject eren_titan;
    private int escapeTimes = 1;
    private float facingDirection;
    private float flare1CD;
    private float flare2CD;
    private float flare3CD;
    private float flareTotalCD = 30f;
    private float gravity = 20f; //20
    private bool grounded;
    private bool hasDied;
    public FengCustomInputs inputManager;
    private float invincible = 2f;
    private bool isAttack;
    public bool isGrabbed;
    private bool isLaunchLeft;
    private bool isLaunchRight;
    public float jumpHeight = 2f;
    private bool justGrounded;
    public Transform lastHook;
    private float launchElapsedTimeL;
    private float launchElapsedTimeR;
    private Vector3 launchForce;
    private Vector3 launchPointLeft;
    private Vector3 launchPointRight;
    public float maxVelocityChange = 10f;
    public AudioSource meatDie;
    public string modelId;
    private Animation myAnimation;
    private string myName;
    private GameObject myNetWorkName;
    public float myScale = 1f;
    private bool QHold;
    public AudioSource rope;
    private GameObject skillCD;
    public float skillCDDuration;
    public float skillCDLast;
    public AudioSource slash;
    public AudioSource slashHit;
    private ParticleSystem smoke_3dmg;
    private ParticleSystem sparks;
    public float speed = 10f;
    private bool throwedBlades;
    public bool titanForm;
    private GameObject titanWhoGrabMe;
    private NetworkViewID titanWhoGrabMeID;
    private int totalBladeNum = 5;
    private float totalBladeSta = 100f;
    private float totalGas = 100f;
    public float useGasSpeed = 0.2f;
    private bool wallJump;
    private float wallRunTime;
    private int tele = 0;

    public void attackAccordingToMouse()
    {
        if (Input.mousePosition.x < (Screen.width * 0.5))
        {
            this.attackAnimation = "attack2";
        }
        else
        {
            this.attackAnimation = "attack1";
        }
    }

    public void attackAccordingToTarget(Transform a)
    {
        Vector3 vector = a.transform.position - base.transform.position;
        float current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        float f = -Mathf.DeltaAngle(current, base.transform.rotation.eulerAngles.y - 90f);
        if (((Mathf.Abs(f) < 90f) && (vector.magnitude < 6f)) && ((a.position.y <= (base.transform.position.y + 2f)) && (a.position.y >= (base.transform.position.y - 5f))))
        {
            this.attackAnimation = "attack4";
        }
        else if (f > 0f)
        {
            this.attackAnimation = "attack1";
        }
        else
        {
            this.attackAnimation = "attack2";
        }
    }

    private void Awake()
    {
        base.rigidbody.freezeRotation = true;
        base.rigidbody.useGravity = false;
    }

    public void backToHuman()
    {
        base.gameObject.GetComponent<MovementUpdate>().disabled = false;
        base.rigidbody.velocity = Vector3.zero;
        this.titanForm = false;
        this.ungrabbed();
        this.falseAttack();
        this.skillCDDuration = this.skillCDLast;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(base.gameObject);
        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
        {
            base.networkView.RPC("backToHumanRPC", RPCMode.Others, new object[0]);
        }
    }

    [RPC]
    private void backToHumanRPC()
    {
        this.titanForm = false;
        this.eren_titan = null;
        base.gameObject.GetComponent<MovementUpdate>().disabled = false;
    }

    [RPC]
    public void blowAway(Vector3 force)
    {
        if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine)
        {
            base.rigidbody.AddForce(force, ForceMode.Impulse);
            base.transform.LookAt(base.transform.position);
        }
    }

    private void calcFlareCD()
    {
        if (this.flare1CD > 0f)
        {
            this.flare1CD -= Time.deltaTime;
            if (this.flare1CD < 0f)
            {
                this.flare1CD = 0f;
            }
        }
        if (this.flare2CD > 0f)
        {
            this.flare2CD -= Time.deltaTime;
            if (this.flare2CD < 0f)
            {
                this.flare2CD = 0f;
            }
        }
        if (this.flare3CD > 0f)
        {
            this.flare3CD -= Time.deltaTime;
            if (this.flare3CD < 0f)
            {
                this.flare3CD = 0f;
            }
        }
    }

    private void calcSkillCD()
    {
        if (this.skillCDDuration > 0f)
        {
            this.skillCDDuration -= Time.deltaTime;
            if (this.skillCDDuration < 0f)
            {
                this.skillCDDuration = 0f;
            }
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt((2f * this.jumpHeight) * this.gravity);
    }

    public void continueAnimation()
    {
        foreach (AnimationState state in this.myAnimation)
        {
            if (state.speed == 1f)
            {
                return;
            }
            state.speed = 1f;
        }
        this.playAnimation(this.currentPlayingClipName());
        if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
        {
            base.networkView.RPC("netContinueAnimation", RPCMode.Others, new object[0]);
        }
    }

    private void crossFade(string aniName, float time)
    {
        this.myAnimation.CrossFade(aniName, time);
        if ((Network.peerType != NetworkPeerType.Disconnected) && base.networkView.isMine)
        {
            object[] args = new object[] { aniName, time };
            base.networkView.RPC("netCrossFade", RPCMode.Others, args);
        }
    }

    public string currentPlayingClipName()
    {
        foreach (AnimationState state in this.myAnimation)
        {
            if (this.myAnimation.IsPlaying(state.name))
            {
                return state.name;
            }
        }
        return string.Empty;
    }

    public void die(Vector3 v, bool isBite)
    {
        if (this.invincible <= 0f)
        {
            if (this.titanForm && (this.eren_titan != null))
            {
                this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
            if (this.bulletLeft != null)
            {
                this.bulletLeft.GetComponent<Bullet>().removeMe();
            }
            if (this.bulletRight != null)
            {
                this.bulletRight.GetComponent<Bullet>().removeMe();
            }
            this.meatDie.Play();
            GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/" + this.modelId + "_body1"));
            GameObject obj3 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/" + this.modelId + "_body2"));
            GameObject obj4 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/" + this.modelId + "_body3"));
            GameObject obj5 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/" + this.modelId + "_body4"));
            GameObject obj6 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/blade_left"));
            GameObject obj7 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/blade_right"));
            obj2.transform.position = base.transform.position;
            obj3.transform.position = base.transform.position;
            obj4.transform.position = base.transform.position;
            obj5.transform.position = base.transform.position;
            obj6.transform.position = base.transform.position;
            obj7.transform.position = base.transform.position;
            obj2.rigidbody.AddForce(v);
            obj3.rigidbody.AddForce(v);
            obj4.rigidbody.AddForce(v);
            obj5.rigidbody.AddForce(v);
            obj6.rigidbody.AddForce(v);
            obj7.rigidbody.AddForce(v);
            obj2.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
            obj3.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
            obj4.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
            obj5.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
            obj6.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
            obj7.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj2);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            if (isBite)
            {
                UnityEngine.Object.Destroy(obj2);
                UnityEngine.Object.Destroy(obj3);
                UnityEngine.Object.Destroy(obj4);
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj5);
            }
            this.falseAttack();
            Transform transform = base.transform.Find("cloth");
            if (transform != null)
            {
                transform.parent = null;
                transform.GetComponent<SkinnedCloth>().SetEnabledFading(false);
                transform.GetComponent<SkinnedMeshRenderer>().rootBone = null;
                transform.GetComponent<SkinnedMeshRenderer>().material = null;
                transform.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
            this.hasDied = true;
            Transform transform2 = base.transform.Find("audio_die");
            transform2.parent = null;
            transform2.GetComponent<AudioSource>().Play();
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    public void die2(Transform tf)
    {
        if (this.invincible <= 0f)
        {
            if (this.titanForm && (this.eren_titan != null))
            {
                this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
            if (this.bulletLeft != null)
            {
                this.bulletLeft.GetComponent<Bullet>().removeMe();
            }
            if (this.bulletRight != null)
            {
                this.bulletRight.GetComponent<Bullet>().removeMe();
            }
            Transform transform = base.transform.Find("audio_die");
            transform.parent = null;
            transform.GetComponent<AudioSource>().Play();
            this.meatDie.Play();
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            this.falseAttack();
            Transform transform2 = base.transform.Find("cloth");
            if (transform2 != null)
            {
                transform2.GetComponent<SkinnedCloth>().SetEnabledFading(false);
                transform2.GetComponent<SkinnedMeshRenderer>().rootBone = null;
                transform2.GetComponent<SkinnedMeshRenderer>().material = null;
                transform2.GetComponent<SkinnedMeshRenderer>().enabled = false;
                transform2.parent = null;
            }
            this.hasDied = true;
            GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("hitMeat2"));
            obj2.transform.position = base.transform.position;
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    private void erenTransform()
    {
        this.skillCDDuration = this.skillCDLast;
        if (this.bulletLeft != null)
        {
            this.bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if (this.bulletRight != null)
        {
            this.bulletRight.GetComponent<Bullet>().removeMe();
        }
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            this.eren_titan = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("TITAN_EREN"), base.transform.position, base.transform.rotation);
        }
        else
        {
            this.eren_titan = (GameObject) Network.Instantiate(Resources.Load("TITAN_EREN"), base.transform.position, base.transform.rotation, 0);
        }
        this.eren_titan.GetComponent<TITAN_EREN>().realBody = base.gameObject;
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().flashBlind();
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(this.eren_titan);
        this.eren_titan.GetComponent<TITAN_EREN>().born();
        this.eren_titan.rigidbody.velocity = base.rigidbody.velocity;
        base.rigidbody.velocity = Vector3.zero;
        base.transform.position = this.eren_titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
        this.titanForm = true;
        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
        {
            object[] args = new object[] { this.eren_titan.networkView.viewID };
            base.networkView.RPC("whoIsMyErenTitan", RPCMode.Others, args);
        }
        if ((this.smoke_3dmg.enableEmission && (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)) && base.networkView.isMine)
        {
            object[] objArray2 = new object[] { false };
            base.networkView.RPC("net3DMGSMOKE", RPCMode.Others, objArray2);
        }
        this.smoke_3dmg.enableEmission = false;
    }

    private void escapeFromGrab()
    {
    }

    public void falseAttack()
    {
        this.isAttack = false;
        this.attackMove = false;
        this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
        this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
        this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
        this.checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
        this.attackLoop = 0;
        if (!this.attackReleased)
        {
            this.continueAnimation();
            this.attackReleased = true;
        }
    }

    private GameObject findNearestTitan()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("titan");
        GameObject obj2 = null;
        float positiveInfinity = float.PositiveInfinity;
        Vector3 position = base.transform.position;
        foreach (GameObject obj3 in objArray)
        {
            Vector3 vector2 = obj3.transform.position - position;
            float sqrMagnitude = vector2.sqrMagnitude;
            if (sqrMagnitude < positiveInfinity)
            {
                obj2 = obj3;
                positiveInfinity = sqrMagnitude;
            }
        }
        return obj2;
    }

    private void FixedUpdate()
    {
        if ((!this.titanForm && (!IN_GAME_MAIN_CAMERA.isPausing || (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE))) && ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine))
        {
            if (this.isGrabbed)
            {
                base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
            }
            else
            {
                Quaternion quaternion;
                if (this.IsGrounded())
                {
                    if (!this.grounded)
                    {
                        this.justGrounded = true;
                    }
                    this.grounded = true;
                }
                else
                {
                    this.grounded = false;
                }
                float x = 0f;
                float z = 0f;
                if (!IN_GAME_MAIN_CAMERA.isTyping)
                {
                    if (this.inputManager.isInput[0])
                    {
                        z = 1f;
                    }
                    else if (this.inputManager.isInput[1])
                    {
                        z = -1f;
                    }
                    else
                    {
                        z = 0f;
                    }
                    if (this.inputManager.isInput[2])
                    {
                        x = -1f;
                    }
                    else if (this.inputManager.isInput[3])
                    {
                        x = 1f;
                    }
                    else
                    {
                        x = 0f;
                    }
                }
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                if (this.isLaunchLeft)
                {
                    if ((this.bulletLeft != null) && this.bulletLeft.GetComponent<Bullet>().isHooked())
                    {
                        Vector3 to = this.bulletLeft.transform.position - base.transform.position;
                        to.Normalize();
                        to = (Vector3) (to * 10f);
                        if (!this.isLaunchRight)
                        {
                            to = (Vector3) (to * 2f);
                        }
                        if ((Vector3.Angle(base.rigidbody.velocity, to) > 90f) && this.inputManager.isInput[4])
                        {
                            flag2 = true;
                            flag = true;
                        }
                        if (!flag2)
                        {
                            base.rigidbody.AddForce(to);
                            if (Vector3.Angle(base.rigidbody.velocity, to) > 90f)
                            {
                                base.rigidbody.AddForce((Vector3) (-base.rigidbody.velocity * 2f), ForceMode.Acceleration);
                            }
                        }
                        this.facingDirection = Mathf.Atan2(to.x, to.z) * 57.29578f;
                        quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
                        base.gameObject.transform.rotation = quaternion;
                        base.rigidbody.rotation = quaternion;
                    }
                    this.launchElapsedTimeL += Time.deltaTime;
                    if (this.QHold && (this.currentGas > 0f))
                    {
                        this.useGas(this.useGasSpeed * Time.deltaTime);
                    }
                    else if (this.launchElapsedTimeL > 0.3f)
                    {
                        this.isLaunchLeft = false;
                        if (this.bulletLeft != null)
                        {
                            this.bulletLeft.GetComponent<Bullet>().disable();
                            this.bulletLeft = null;
                            flag2 = false;
                        }
                    }
                }
                if (this.isLaunchRight)
                {
                    if ((this.bulletRight != null) && this.bulletRight.GetComponent<Bullet>().isHooked())
                    {
                        Vector3 vector2 = this.bulletRight.transform.position - base.transform.position;
                        vector2.Normalize();
                        vector2 = (Vector3) (vector2 * 10f);
                        if (!this.isLaunchLeft)
                        {
                            vector2 = (Vector3) (vector2 * 2f);
                        }
                        if ((Vector3.Angle(base.rigidbody.velocity, vector2) > 90f) && this.inputManager.isInput[4])
                        {
                            flag3 = true;
                            flag = true;
                        }
                        if (!flag3)
                        {
                            base.rigidbody.AddForce(vector2);
                            if (Vector3.Angle(base.rigidbody.velocity, vector2) > 90f)
                            {
                                base.rigidbody.AddForce((Vector3) (-base.rigidbody.velocity * 2f), ForceMode.Acceleration);
                            }
                        }
                        this.facingDirection = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
                        quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
                        base.gameObject.transform.rotation = quaternion;
                        base.rigidbody.rotation = quaternion;
                    }
                    this.launchElapsedTimeR += Time.deltaTime;
                    if (this.EHold && (this.currentGas > 0f))
                    {
                        this.useGas(this.useGasSpeed * Time.deltaTime);
                    }
                    else if (this.launchElapsedTimeR > 0.3f)
                    {
                        this.isLaunchRight = false;
                        if (this.bulletRight != null)
                        {
                            this.bulletRight.GetComponent<Bullet>().disable();
                            this.bulletRight = null;
                            flag3 = false;
                        }
                    }
                }
                if (this.grounded)
                {
                    Vector3 zero = Vector3.zero;
                    if (this.isAttack)
                    {
                        if (this.attackAnimation == "attack5")
                        {
                            if ((this.myAnimation[this.attackAnimation].normalizedTime > 0.4f) && (this.myAnimation[this.attackAnimation].normalizedTime < 0.61f))
                            {
                                base.rigidbody.AddForce((Vector3) (base.gameObject.transform.forward * 200f));
                            }
                        }
                        else if (this.attackAnimation == "special_petra")
                        {
                            if ((this.myAnimation[this.attackAnimation].normalizedTime > 0.35f) && (this.myAnimation[this.attackAnimation].normalizedTime < 0.48f))
                            {
                                base.rigidbody.AddForce((Vector3) (base.gameObject.transform.forward * 200f));
                            }
                        }
                        else if (this.myAnimation.IsPlaying("attack3_2"))
                        {
                            zero = Vector3.zero;
                        }
                        else if (this.myAnimation.IsPlaying("attack1") || this.myAnimation.IsPlaying("attack2"))
                        {
                            base.rigidbody.AddForce((Vector3) (base.gameObject.transform.forward * 200f));
                        }
                    }
                    if (this.justGrounded)
                    {
                        if (!this.isAttack || (((this.attackAnimation != "attack3_1") && (this.attackAnimation != "attack5")) && (this.attackAnimation != "special_petra")))
                        {
                            if (((!this.isAttack && (x == 0f)) && ((z == 0f) && (this.bulletLeft == null))) && ((this.bulletRight == null) && !this.myAnimation.IsPlaying("supply")))
                            {
                                this.crossFade("dash_land", 0.1f);
                            }
                            else
                            {
                                this.buttonAttackRelease = true;
                                if (!((this.isAttack || (((base.rigidbody.velocity.x * base.rigidbody.velocity.x) + (base.rigidbody.velocity.z * base.rigidbody.velocity.z)) <= ((this.speed * this.speed) * 1.5f))) || this.myAnimation.IsPlaying("supply")))
                                {
                                    this.crossFade("slide", 0.05f);
                                    this.facingDirection = Mathf.Atan2(base.rigidbody.velocity.x, base.rigidbody.velocity.z) * 57.29578f;
                                    base.transform.Find("slideSparks").GetComponent<ParticleSystem>().enableEmission = true;
                                }
                            }
                        }
                        this.justGrounded = false;
                        zero = base.rigidbody.velocity;
                    }
                    if ((this.isAttack && (this.attackAnimation == "attack3_1")) && (this.myAnimation[this.attackAnimation].normalizedTime >= 1f))
                    {
                        this.playAnimation("attack3_2");
                        this.resetAnimationSpeed();
                        Vector3 vector23 = Vector3.zero;
                        base.rigidbody.velocity = vector23;
                        zero = vector23;
                        this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f, 0.95f);
                    }
                    if (this.myAnimation.IsPlaying("dodge"))
                    {
                        if ((this.myAnimation["dodge"].normalizedTime >= 0.2f) && (this.myAnimation["dodge"].normalizedTime < 0.8f))
                        {
                            zero = (Vector3) ((-base.transform.forward * 2.4f) * this.speed);
                        }
                        if (this.myAnimation["dodge"].normalizedTime > 0.8f)
                        {
                            zero = (Vector3) (base.rigidbody.velocity * 0.9f);
                        }
                    }
                    else if (((!this.myAnimation.IsPlaying("dash_land") && !this.isAttack) && (!this.myAnimation.IsPlaying("attack3_1") && !this.myAnimation.IsPlaying("attack3_2"))) && ((!this.myAnimation.IsPlaying("slide") && !this.myAnimation.IsPlaying("changeBlade")) && !this.myAnimation.IsPlaying("supply")))
                    {
                        Vector3 vector4 = new Vector3(x, 0f, z);
                        float y = this.currentCamera.transform.rotation.eulerAngles.y;
                        float num6 = Mathf.Atan2(z, x) * 57.29578f;
                        num6 = -num6 + 90f;
                        float num3 = y + num6;
                        float num4 = -num3 + 90f;
                        float num7 = Mathf.Cos(num4 * 0.01745329f);
                        float num8 = Mathf.Sin(num4 * 0.01745329f);
                        zero = new Vector3(num7, 0f, num8);
                        float num9 = (vector4.magnitude <= 0.95f) ? ((vector4.magnitude >= 0.25f) ? vector4.magnitude : 0f) : 1f;
                        zero = (Vector3) (zero * num9);
                        zero = (Vector3) (zero * this.speed);
                        if ((this.buffTime > 0f) && (this.currentBuff == BUFF.SpeedUp))
                        {
                            zero = (Vector3) (zero * 4f);
                        }
                        if ((x != 0f) || (z != 0f))
                        {
                            if ((!this.myAnimation.IsPlaying("run") && !this.myAnimation.IsPlaying("jump")) && !this.myAnimation.IsPlaying("run_sasha"))
                            {
                                if ((this.buffTime > 0f) && (this.currentBuff == BUFF.SpeedUp))
                                {
                                    this.crossFade("run_sasha", 0.1f);
                                }
                                else
                                {
                                    this.crossFade("run", 0.1f);
                                }
                            }
                        }
                        else
                        {
                            if (!(((this.myAnimation.IsPlaying("stand") || this.myAnimation.IsPlaying("dash_land")) || (this.myAnimation.IsPlaying("dodge") || this.myAnimation.IsPlaying("jump"))) || this.myAnimation.IsPlaying("salute")))
                            {
                                this.crossFade("stand", 0.1f);
                                zero = (Vector3) (zero * 0f);
                            }
                            num3 = -874f;
                        }
                        if (num3 != -874f)
                        {
                            this.facingDirection = num3;
                        }
                    }
                    if (this.myAnimation.IsPlaying("attack3_2"))
                    {
                        zero = Vector3.zero;
                    }
                    if (this.myAnimation.IsPlaying("dash_land"))
                    {
                        zero = (Vector3) (base.rigidbody.velocity * 0.96f);
                    }
                    if (this.myAnimation.IsPlaying("slide"))
                    {
                        zero = (Vector3) (base.rigidbody.velocity * 0.99f);
                        if (base.rigidbody.velocity.magnitude < (this.speed * 1.2f))
                        {
                            this.crossFade("stand", 0f);
                            this.sparks.enableEmission = false;
                        }
                    }
                    Vector3 velocity = base.rigidbody.velocity;
                    Vector3 force = zero - velocity;
                    force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                    force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                    force.y = 0f;
                    if (this.myAnimation.IsPlaying("jump") && (this.myAnimation["jump"].normalizedTime > 0.18f))
                    {
                        force.y += 8f;
                    }
                    base.rigidbody.AddForce(force, ForceMode.VelocityChange);
                    base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
                }
                else
                {
                    if (this.sparks.enableEmission)
                    {
                        this.sparks.enableEmission = false;
                    }
                    if ((((!this.isAttack && !this.myAnimation.IsPlaying("dash")) && (!this.myAnimation.IsPlaying("dodge") || (this.myAnimation["dodge"].normalizedTime >= 0.6f))) && ((!this.myAnimation.IsPlaying("hitWall") && !this.myAnimation.IsPlaying("air")) && ((!this.myAnimation.IsPlaying("air2") && !this.myAnimation.IsPlaying("wallrun")) && !this.myAnimation.IsPlaying("toRoof")))) || ((this.myAnimation["dash"].normalizedTime >= 0.99f) || (this.myAnimation.IsPlaying("hitWall") && (this.myAnimation["hitWall"].normalizedTime >= 1f))))
                    {
                        if ((base.rigidbody.velocity.magnitude > 20f) && (Vector3.Angle(base.rigidbody.velocity, base.gameObject.transform.forward) < 90f))
                        {
                            this.crossFade("air2", 0.1f);
                        }
                        else
                        {
                            this.crossFade("air", 0.1f);
                        }
                    }
                    if (this.myAnimation.IsPlaying("toRoof"))
                    {
                        if (this.myAnimation["toRoof"].normalizedTime < 0.22f)
                        {
                            base.rigidbody.velocity = Vector3.zero;
                            base.rigidbody.AddForce(new Vector3(0f, this.gravity * base.rigidbody.mass, 0f));
                        }
                        else
                        {
                            if (!this.wallJump)
                            {
                                this.wallJump = true;
                                base.rigidbody.AddForce((Vector3) (Vector3.up * 8f), ForceMode.Impulse);
                            }
                            base.rigidbody.AddForce((Vector3) (base.transform.forward * 0.05f), ForceMode.Impulse);
                        }
                        if (this.myAnimation["toRoof"].normalizedTime >= 1f)
                        {
                            this.playAnimation("air");
                        }
                    }
                    else if (((!this.isAttack && (z >= 1f)) && (!this.inputManager.isInput[6] && !this.inputManager.isInput[7])) && ((!this.inputManager.isInput[8] && this.IsFrontGrounded()) && (!this.myAnimation.IsPlaying("wallrun") && !this.myAnimation.IsPlaying("dodge"))))
                    {
                        this.crossFade("wallrun", 0.1f);
                        this.wallRunTime = 0f;
                    }
                    else if (this.myAnimation.IsPlaying("wallrun"))
                    {
                        base.rigidbody.AddForce(((Vector3) (Vector3.up * this.speed)) - base.rigidbody.velocity, ForceMode.VelocityChange);
                        this.wallRunTime += Time.deltaTime;
                        if ((this.wallRunTime > 1f) || (z != 1f))
                        {
                            base.rigidbody.AddForce((Vector3) ((-base.transform.forward * this.speed) * 0.75f), ForceMode.Impulse);
                            this.playAnimation("dodge");
                            this.playAnimationAt("dodge", 0.2f);
                        }
                        else if (!this.IsUpFrontGrounded())
                        {
                            this.wallJump = false;
                            this.crossFade("toRoof", 0.1f);
                        }
                        else if (!this.IsFrontGrounded())
                        {
                            this.crossFade("air", 0.1f);
                        }
                    }
                    else if ((!this.myAnimation.IsPlaying("attack5") && !this.myAnimation.IsPlaying("special_petra")) && (!this.myAnimation.IsPlaying("dash") && !this.myAnimation.IsPlaying("jump")))
                    {
                        Vector3 vector7 = new Vector3(x, 0f, z);
                        float num10 = this.currentCamera.transform.rotation.eulerAngles.y;
                        float num11 = Mathf.Atan2(z, x) * 57.29578f;
                        num11 = -num11 + 90f;
                        float num12 = num10 + num11;
                        float num13 = -num12 + 90f;
                        float num14 = Mathf.Cos(num13 * 0.01745329f);
                        float num15 = Mathf.Sin(num13 * 0.01745329f);
                        Vector3 vector8 = new Vector3(num14, 0f, num15);
                        float num16 = (vector7.magnitude <= 0.95f) ? ((vector7.magnitude >= 0.25f) ? vector7.magnitude : 0f) : 1f;
                        vector8 = (Vector3) (vector8 * num16);
                        vector8 = (Vector3) (vector8 * (this.speed * 2f));
                        if ((x == 0f) && (z == 0f))
                        {
                            if (this.isAttack)
                            {
                                vector8 = (Vector3) (vector8 * 0f);
                            }
                            num12 = -874f;
                        }
                        if (num12 != -874f)
                        {
                            this.facingDirection = num12;
                        }
                        if ((!flag2 && !flag3) && (this.inputManager.isInput[4] && (this.currentGas > 0f)))
                        {
                            if ((x != 0f) || (z != 0f))
                            {
                                base.rigidbody.AddForce(vector8, ForceMode.Acceleration);
                            }
                            else
                            {
                                base.rigidbody.AddForce((Vector3) (base.transform.forward * vector8.magnitude), ForceMode.Acceleration);
                            }
                            flag = true;
                        }
                        if ((!this.myAnimation.IsPlaying(string.Empty) && !this.myAnimation.IsPlaying("attack3_2")) && (!this.myAnimation.IsPlaying("attack5") && !this.myAnimation.IsPlaying("special_petra")))
                        {
                            base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 6f);
                        }
                    }
                    if (this.myAnimation.IsPlaying("air2") && ((base.rigidbody.velocity.magnitude < 10f) || (Vector3.Angle(base.rigidbody.velocity, base.gameObject.transform.forward) > 90f)))
                    {
                        this.crossFade("air", 0.3f);
                    }
                    if (this.myAnimation.IsPlaying("air"))
                    {
                        if ((base.rigidbody.velocity.magnitude < 0.2f) && this.IsFrontGrounded())
                        {
                            this.crossFade("onWall", 0.3f);
                        }
                        if ((base.rigidbody.velocity.magnitude > 20f) && (Vector3.Angle(base.rigidbody.velocity, base.gameObject.transform.forward) < 30f))
                        {
                            this.crossFade("air2", 0.3f);
                        }
                    }
                }
                if (flag2 && flag3)
                {
                    float num17 = base.rigidbody.velocity.magnitude + 0.1f;
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    float num18;
                    Vector3 current = ((Vector3) ((this.bulletRight.transform.position + this.bulletLeft.transform.position) * 0.5f)) - base.transform.position;
                    if (FengMultiplayerScript.isReelKeyChanged)
                    {
                        if (Input.GetKey(FengMultiplayerScript.reeloutkey))
                        {                                                           
                            num18 = 2777.5f;
                       }
                        else if (Input.GetKey(FengMultiplayerScript.reelinkey) || Input.GetKey(FengMultiplayerScript.reelinkey2))
                        {
                            num18 = -2777.5f;
                        }
                        else
                        {
                            num18 = 0f;
                        }
                    }
                    else
                    {
                        num18 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    }
                    num18 = Mathf.Clamp(num18, -0.8f, 0.8f);
                    float num19 = 1f + num18;
                    Vector3 vector10 = Vector3.RotateTowards(current, base.rigidbody.velocity, 1.53938f * num19, 1.53938f * num19);
                    vector10.Normalize();
                    Debug.DrawRay(base.transform.position, (Vector3) (vector10 * 10f), Color.yellow);
                    Debug.DrawRay(base.transform.position, base.rigidbody.velocity, Color.red);
                    base.rigidbody.velocity = (Vector3) (vector10 * num17);
                }
                else if (flag2)
                {
                    float num20 = base.rigidbody.velocity.magnitude + 0.1f;
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    float num21;
                    Vector3 vector11 = this.bulletLeft.transform.position - base.transform.position;
                    if (FengMultiplayerScript.isReelKeyChanged)
                    {
                        if (Input.GetKey(FengMultiplayerScript.reeloutkey))
                        {
                            num21 = 2777.5f;                        
                        }
                        else if (Input.GetKey(FengMultiplayerScript.reelinkey) || Input.GetKey(FengMultiplayerScript.reelinkey2))
                        {
                            num21 = -2777f;
                        }
                        else
                        {
                            num21 = 0f;
                        }
                    }
                    else
                    {
                        num21 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    }
                    num21 = Mathf.Clamp(num21, -0.8f, 0.8f);
                    float num22 = 1f + num21;
                    Vector3 vector12 = Vector3.RotateTowards(vector11, base.rigidbody.velocity, 1.53938f * num22, 1.53938f * num22);
                    vector12.Normalize();
                    Debug.DrawRay(base.transform.position, (Vector3) (vector12 * 10f), Color.yellow);
                    Debug.DrawRay(base.transform.position, base.rigidbody.velocity, Color.red);
                    base.rigidbody.velocity = (Vector3) (vector12 * num20);
                }
                else if (flag3)
                {
                    float num23 = base.rigidbody.velocity.magnitude + 0.1f;
                    float num24;
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    Vector3 vector13 = this.bulletRight.transform.position - base.transform.position;
                    if (FengMultiplayerScript.isReelKeyChanged)
                    {
                        if (Input.GetKey(FengMultiplayerScript.reeloutkey))
                        {
                            num24 = 2777.5f;
                        }
                        else if (Input.GetKey(FengMultiplayerScript.reelinkey) || Input.GetKey(FengMultiplayerScript.reelinkey2)) 
                        {
                            num24 = -2777f;
                        }
                        else
                        {
                            num24 = 0f;
                        }
                    }
                    else
                    {
                        num24 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
                    }
                    num24 = Mathf.Clamp(num24, -0.8f, 0.8f);
                    float num25 = 1f + num24;
                    Vector3 vector14 = Vector3.RotateTowards(vector13, base.rigidbody.velocity, 1.53938f * num25, 1.53938f * num25);
                    vector14.Normalize();
                    Debug.DrawRay(base.transform.position, (Vector3) (vector14 * 10f), Color.yellow);
                    Debug.DrawRay(base.transform.position, base.rigidbody.velocity, Color.red);
                    base.rigidbody.velocity = (Vector3) (vector14 * num23);
                }
                if ((this.isAttack && ((this.attackAnimation == "attack5") || (this.attackAnimation == "special_petra"))) && ((this.myAnimation[this.attackAnimation].normalizedTime > 0.4f) && !this.attackMove))
                {
                    this.attackMove = true;
                    if (this.launchPointRight.magnitude > 0f)
                    {
                        Vector3 vector15 = this.launchPointRight - base.transform.position;
                        vector15.Normalize();
                        vector15 = (Vector3) (vector15 * 13f);
                        base.rigidbody.AddForce(vector15, ForceMode.Impulse);
                    }
                    if ((this.attackAnimation == "special_petra") && (this.launchPointLeft.magnitude > 0f))
                    {
                        Vector3 vector16 = this.launchPointLeft - base.transform.position;
                        vector16.Normalize();
                        vector16 = (Vector3) (vector16 * 13f);
                        base.rigidbody.AddForce(vector16, ForceMode.Impulse);
                    }
                    base.rigidbody.AddForce((Vector3) (Vector3.up * 2f), ForceMode.Impulse);
                }
                bool flag4 = false;
                if ((this.bulletLeft != null) || (this.bulletRight != null))
                {
                    if (((this.bulletLeft != null) && (this.bulletLeft.transform.position.y > base.gameObject.transform.position.y)) && (this.isLaunchLeft && this.bulletLeft.GetComponent<Bullet>().isHooked()))
                    {
                        flag4 = true;
                    }
                    if (((this.bulletRight != null) && (this.bulletRight.transform.position.y > base.gameObject.transform.position.y)) && (this.isLaunchRight && this.bulletRight.GetComponent<Bullet>().isHooked()))
                    {
                        flag4 = true;
                    }
                }
                if (flag4)
                {
                    base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
                }
                else
                {
                    base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
                }
                if (base.rigidbody.velocity.magnitude > 10f)
                {
                    this.currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min((float) 100f, (float) (base.rigidbody.velocity.magnitude + 40f)), 0.1f);
                }
                else
                {
                    this.currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
                }
                if (flag)
                {
                    this.useGas(this.useGasSpeed * Time.deltaTime);
                    if ((!this.smoke_3dmg.enableEmission && (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)) && base.networkView.isMine)
                    {
                        object[] args = new object[] { true };
                        base.networkView.RPC("net3DMGSMOKE", RPCMode.Others, args);
                    }
                    this.smoke_3dmg.enableEmission = true;
                }
                else
                {
                    if ((this.smoke_3dmg.enableEmission && (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)) && base.networkView.isMine)
                    {
                        object[] objArray2 = new object[] { false };
                        base.networkView.RPC("net3DMGSMOKE", RPCMode.Others, objArray2);
                    }
                    this.smoke_3dmg.enableEmission = false;
                }
            }
        }
    }

    public void getSupply()
    {
        if (((base.animation.IsPlaying("stand") || base.animation.IsPlaying("run")) || base.animation.IsPlaying("run_sasha")) && (((this.currentBladeSta != this.totalBladeSta) || (this.currentBladeNum != this.totalBladeNum)) || (this.currentGas != this.totalGas)))
        {
            this.crossFade("supply", 0.1f);
        }
    }

    public void grabbed(GameObject titan, bool leftHand)
    {
        this.isGrabbed = true;
        base.GetComponent<CapsuleCollider>().isTrigger = true;
        this.falseAttack();
        this.titanWhoGrabMe = titan;
        if (this.titanForm && (this.eren_titan != null))
        {
            this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
    }

    public bool HasDied()
    {
        return (this.hasDied || this.isInvincible());
    }

    private bool IsFrontGrounded()
    {
        LayerMask mask = ((int) 1) << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(base.gameObject.transform.position + ((Vector3) (base.gameObject.transform.up * 1f)), base.gameObject.transform.forward, (float) 1f, mask3.value);
    }

    public bool isInvincible()
    {
        return (this.invincible > 0f);
    }
    public bool IsGrounded()
    {
        LayerMask mask = ((int) 1) << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(base.gameObject.transform.position + ((Vector3) (Vector3.up * 0.1f)), -Vector3.up, (float) 0.3f, mask3.value);
    }

    private bool IsUpFrontGrounded()
    {
        LayerMask mask = ((int) 1) << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(base.gameObject.transform.position + ((Vector3) (base.gameObject.transform.up * 3f)), base.gameObject.transform.forward, (float) 1.2f, mask3.value);
    }

    [RPC]
    private void killObject()
    {
        UnityEngine.Object.Destroy(base.gameObject);
    }

    private void LateUpdate()
    {
        if (!this.titanForm)
        {
            if ((IN_GAME_MAIN_CAMERA.cameraTilt == 1) && ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine))
            {
                Quaternion quaternion;
                Vector3 zero = Vector3.zero;
                Vector3 position = Vector3.zero;
                if ((this.isLaunchLeft && (this.bulletLeft != null)) && this.bulletLeft.GetComponent<Bullet>().isHooked())
                {
                    zero = this.bulletLeft.transform.position;
                }
                if ((this.isLaunchRight && (this.bulletRight != null)) && this.bulletRight.GetComponent<Bullet>().isHooked())
                {
                    position = this.bulletRight.transform.position;
                }
                Vector3 vector3 = Vector3.zero;
                if ((zero.magnitude != 0f) && (position.magnitude == 0f))
                {
                    vector3 = zero;
                }
                else if ((zero.magnitude == 0f) && (position.magnitude != 0f))
                {
                    vector3 = position;
                }
                else if ((zero.magnitude != 0f) && (position.magnitude != 0f))
                {
                    vector3 = (Vector3) ((zero + position) * 0.5f);
                }
                Vector3 from = Vector3.Project(vector3 - base.transform.position, GameObject.Find("MainCamera").transform.up);
                Vector3 vector5 = Vector3.Project(vector3 - base.transform.position, GameObject.Find("MainCamera").transform.right);
                if (vector3.magnitude > 0f)
                {
                    Vector3 to = from + vector5;
                    float num = Vector3.Angle(vector3 - base.transform.position, base.rigidbody.velocity) * 0.005f;
                    Vector3 vector9 = GameObject.Find("MainCamera").transform.right + vector5.normalized;
                    quaternion = Quaternion.Euler(GameObject.Find("MainCamera").transform.rotation.eulerAngles.x, GameObject.Find("MainCamera").transform.rotation.eulerAngles.y, (vector9.magnitude >= 1f) ? (-Vector3.Angle(from, to) * num) : (Vector3.Angle(from, to) * num));
                }
                else
                {
                    quaternion = Quaternion.Euler(GameObject.Find("MainCamera").transform.rotation.eulerAngles.x, GameObject.Find("MainCamera").transform.rotation.eulerAngles.y, 0f);
                }
                GameObject.Find("MainCamera").transform.rotation = Quaternion.Lerp(GameObject.Find("MainCamera").transform.rotation, quaternion, Time.deltaTime * 2f);
            }
            if (this.isGrabbed && (this.titanWhoGrabMe != null))
            {
                if (this.titanWhoGrabMe.GetComponent<TITAN>() != null)
                {
                    base.transform.position = this.titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.position;
                    base.transform.rotation = this.titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.rotation;
                }
                else if (this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>() != null)
                {
                    base.transform.position = this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.position;
                    base.transform.rotation = this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.rotation;
                }
            }
        }
    }

    public void launch(Vector3 des, bool left = true, bool leviMode = false)
    {
        Vector3 vector = des - base.transform.position;
        if (left)
        {
            this.launchPointLeft = des;
        }
        else
        {
            this.launchPointRight = des;
        }
        vector.Normalize();
        vector = (Vector3) (vector * 20f);
        if (((this.bulletLeft != null) && (this.bulletRight != null)) && (this.bulletLeft.GetComponent<Bullet>().isHooked() && this.bulletRight.GetComponent<Bullet>().isHooked()))
        {
            vector = (Vector3) (vector * 0.8f);
        }
        if (base.animation.IsPlaying("attack5") || base.animation.IsPlaying("special_petra"))
        {
            leviMode = true;
        }
        else
        {
            leviMode = false;
        }
        if (!leviMode)
        {
            this.falseAttack();
            this.crossFade("dash", 0.1f);
            this.myAnimation["dash"].time = 0f;
        }
        if (left)
        {
            this.isLaunchLeft = true;
        }
        if (!left)
        {
            this.isLaunchRight = true;
        }
        this.launchForce = vector;
        if (!leviMode)
        {
            if (vector.y < 30f)
            {
                this.launchForce += (Vector3) (Vector3.up * (30f - vector.y));
            }
            if (des.y >= base.transform.position.y)
            {
                this.launchForce += (Vector3) ((Vector3.up * (des.y - base.transform.position.y)) * 10f);
            }
            base.rigidbody.AddForce(this.launchForce);
        }
        this.facingDirection = Mathf.Atan2(this.launchForce.x, this.launchForce.z) * 57.29578f;
        Quaternion quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
        base.gameObject.transform.rotation = quaternion;
        base.rigidbody.rotation = quaternion;
        if (left)
        {
            this.launchElapsedTimeL = 0f;
        }
        else
        {
            this.launchElapsedTimeR = 0f;
        }
        if (leviMode)
        {
            this.launchElapsedTimeR = -100f;
            if (base.animation.IsPlaying("special_petra"))
            {
                this.launchElapsedTimeL = -100f;
                if (this.bulletRight != null)
                {
                    this.bulletRight.GetComponent<Bullet>().disable();
                }
                if (this.bulletLeft != null)
                {
                    this.bulletLeft.GetComponent<Bullet>().disable();
                }
            }
        }
        this.sparks.enableEmission = false;
    }

    private void launchLeftRope(RaycastHit hit, bool single,  int mode = 0)
    {
        if (this.currentGas != 0f)
        {
            this.useGas(0f);
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                this.bulletLeft = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("bulletPrefab"));
            }
            else if (base.networkView.isMine)
            {
                this.bulletLeft = (GameObject) Network.Instantiate(Resources.Load("bulletPrefab"), base.transform.position, base.transform.rotation, 0);
            }
            this.bulletLeft.transform.position = base.transform.position;
            Vector3 offset = new Vector3(0f, 0.4f, 0f);
            offset -= (Vector3) (base.transform.right * 0.3f);
            Transform transform = this.bulletLeft.transform;
            transform.position += offset;
            Bullet component = this.bulletLeft.GetComponent<Bullet>();
            component.master = base.gameObject; 
            float num = !single ? ((hit.distance <= 50f) ? (hit.distance * 0.05f) : (hit.distance * 0.3f)) : 0f;
            Vector3 vector2 = (hit.point - ((Vector3) (base.transform.right * num))) - this.bulletLeft.transform.position;
            vector2.Normalize();
            component.launch((Vector3) (vector2 * 3f), hit.point - ((Vector3) (base.transform.right * num)), offset, true, false);
            this.launchPointLeft = Vector3.zero;
        }
    }

    private void launchRightRope(RaycastHit hit, bool single,  int mode = 0)
    {
        if (this.currentGas != 0f)
        {
            this.useGas(0f);
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                this.bulletRight = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("bulletPrefab"));
            }
            else if (base.networkView.isMine)
            {
                this.bulletRight = (GameObject) Network.Instantiate(Resources.Load("bulletPrefab"), base.transform.position, base.transform.rotation, 0);
            }
            this.bulletRight.transform.position = base.transform.position;
            Vector3 offset = new Vector3(0f, 0.4f, 0f);
            offset += (Vector3) (base.transform.right * 0.3f);
            Transform transform = this.bulletRight.transform;
            transform.position += offset;
            Bullet component = this.bulletRight.GetComponent<Bullet>();
            component.master = base.gameObject;
            float num = !single ? ((hit.distance <= 50f) ? (hit.distance * 0.05f) : (hit.distance * 0.3f)) : 0f;
            Vector3 vector2 = (hit.point + ((Vector3) (base.transform.right * num))) - this.bulletRight.transform.position;
            vector2.Normalize();
            if (mode == 1)
            {
                component.launch((Vector3) (vector2 * 5f), hit.point + ((Vector3) (base.transform.right * num)), offset, false, true);
            }
            else
            {
                component.launch((Vector3) (vector2 * 3f), hit.point + ((Vector3) (base.transform.right * num)), offset, false, false);
            }
            this.launchPointRight = Vector3.zero;
        }
    }

    public void markDie()
    {
        this.hasDied = true;
    }

    [RPC]
    private void myNameIs(string name)
    {
        GameObject obj2 = GameObject.Find("UI_IN_GAME");
        this.myNetWorkName = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
        this.myNetWorkName.name = "LabelNameOverHead";
        this.myNetWorkName.transform.parent = obj2.GetComponent<UIReferArray>().panels[0].transform;
        this.myNetWorkName.transform.localScale = new Vector3(22f, 22f, 22f);
        this.myNetWorkName.GetComponent<UILabel>().text = name;
        this.myName = name;
    }

    [RPC]
    private void net3DMGSMOKE(bool ifON)
    {
        this.smoke_3dmg.enableEmission = ifON;
    }

    [RPC]
    private void netContinueAnimation()
    {
        foreach (AnimationState state in this.myAnimation)
        {
            if (state.speed == 1f)
            {
                return;
            }
            state.speed = 1f;
        }
        this.playAnimation(this.currentPlayingClipName());
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        this.myAnimation.CrossFade(aniName, time);
    }

    [RPC]
    public void netDie(Vector3 v, bool isBite)
    {
        if ((base.networkView.isMine && this.titanForm) && (this.eren_titan != null))
        {
            this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
        if (this.bulletLeft != null)
        {
            this.bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if (this.bulletRight != null)
        {
            this.bulletRight.GetComponent<Bullet>().removeMe();
        }
        this.meatDie.Play();
        GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/" + this.modelId + "_body1"));
        GameObject obj3 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/" + this.modelId + "_body2"));
        GameObject obj4 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/" + this.modelId + "_body3"));
        GameObject obj5 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/" + this.modelId + "_body4"));
        GameObject obj6 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/blade_left"));
        GameObject obj7 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/blade_right"));
        obj2.transform.position = base.transform.position;
        obj3.transform.position = base.transform.position;
        obj4.transform.position = base.transform.position;
        obj5.transform.position = base.transform.position;
        obj6.transform.position = base.transform.position;
        obj7.transform.position = base.transform.position;
        obj2.rigidbody.AddForce(v);
        obj3.rigidbody.AddForce(v);
        obj4.rigidbody.AddForce(v);
        obj5.rigidbody.AddForce(v);
        obj6.rigidbody.AddForce(v);
        obj7.rigidbody.AddForce(v);
        obj2.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
        obj3.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
        obj4.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
        obj5.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
        obj6.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
        obj7.rigidbody.AddTorque(UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f), UnityEngine.Random.Range((float) -10f, (float) 10f));
        if (base.networkView.isMine)
        {
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj2);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myRespawnTime = 0f;
        }
        if (isBite)
        {
            UnityEngine.Object.Destroy(obj2);
            UnityEngine.Object.Destroy(obj3);
            UnityEngine.Object.Destroy(obj4);
            if (base.networkView.isMine)
            {
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj5);
            }
        }
        this.falseAttack();
        Transform transform = base.transform.Find("cloth");
        if (transform != null)
        {
            transform.parent = null;
            transform.GetComponent<SkinnedCloth>().SetEnabledFading(false);
            transform.GetComponent<SkinnedMeshRenderer>().rootBone = null;
            transform.GetComponent<SkinnedMeshRenderer>().material = null;
            transform.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        this.hasDied = true;
        Transform transform2 = base.transform.Find("audio_die");
        transform2.parent = null;
        transform2.GetComponent<AudioSource>().Play();
        if (base.networkView.isMine)
        {
            Network.RemoveRPCs(base.networkView.viewID);
        }
        base.gameObject.GetComponent<MovementUpdate>().disabled = true;
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
        {
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playerWhoTheFuckIsDead(base.networkView.owner.ToString());
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    [RPC]
    private void netDie2()
    {
        GameObject obj2;
        if (base.networkView.isMine)
        {
            Network.RemoveRPCs(base.networkView.viewID);
            if (this.titanForm && (this.eren_titan != null))
            {
                this.eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
        }
        this.meatDie.Play();
        if (this.bulletLeft != null)
        {
            this.bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if (this.bulletRight != null)
        {
            this.bulletRight.GetComponent<Bullet>().removeMe();
        }
        Transform transform = base.transform.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        if (base.networkView.isMine)
        {
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myRespawnTime = 0f;
        }
        this.falseAttack();
        Transform transform2 = base.transform.Find("cloth");
        if (transform2 != null)
        {
            transform2.GetComponent<SkinnedCloth>().SetEnabledFading(false);
            transform2.GetComponent<SkinnedMeshRenderer>().rootBone = null;
            transform2.GetComponent<SkinnedMeshRenderer>().material = null;
            transform2.GetComponent<SkinnedMeshRenderer>().enabled = false;
            transform2.parent = null;
        }
        this.hasDied = true;
        base.gameObject.GetComponent<MovementUpdate>().disabled = true;
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
        {
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playerWhoTheFuckIsDead(base.networkView.owner.ToString());
        }
        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
        {
            obj2 = (GameObject) Network.Instantiate(Resources.Load("hitMeat2"), base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("hitMeat2"));
        }
        obj2.transform.position = base.transform.position;
        UnityEngine.Object.Destroy(base.gameObject);
    }

    [RPC]
    private void netGameLose()
    {
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().gameLose();
    }

    [RPC]
    private void netGrabbed(NetworkViewID id, bool leftHand)
    {
        this.titanWhoGrabMeID = id;
        this.grabbed(NetworkView.Find(id).gameObject, leftHand);
    }

    [RPC]
    private void netlaughAttack()
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if (((Vector3.Distance(obj2.transform.position, base.transform.position) < 50f) && (Vector3.Angle(obj2.transform.forward, base.transform.position - obj2.transform.position) < 90f)) && (obj2.GetComponent<TITAN>() != null))
            {
                obj2.GetComponent<TITAN>().beLaughAttacked();
            }
        }
    }

    [RPC]
    private void netPauseAnimation()
    {
        foreach (AnimationState state in this.myAnimation)
        {
            state.speed = 0f;
        }
    }

    [RPC]
    private void netPlayAnimation(string aniName)
    {
        this.myAnimation.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime)
    {
        this.myAnimation.Play(aniName);
        this.myAnimation[aniName].normalizedTime = normalizedTime;
    }

    [RPC]
    private void netSetIsGrabbedFalse()
    {
        this.isGrabbed = false;
    }

    [RPC]
    private void netTauntAttack(float tauntTime, float distance = 100f)
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if ((Vector3.Distance(obj2.transform.position, base.transform.position) < distance) && (obj2.GetComponent<TITAN>() != null))
            {
                obj2.GetComponent<TITAN>().beTauntedBy(base.gameObject, tauntTime);
            }
        }
    }

    [RPC]
    private void netUngrabbed()
    {
        this.ungrabbed();
        this.netPlayAnimation("stand");
        this.falseAttack();
    }

    private void OnDestroy()
    {
        if (this.myNetWorkName != null)
        {
            UnityEngine.Object.Destroy(this.myNetWorkName);
        }
    }

    public void pauseAnimation()
    {
        foreach (AnimationState state in this.myAnimation)
        {
            state.speed = 0f;
        }
        if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
        {
            base.networkView.RPC("netPauseAnimation", RPCMode.Others, new object[0]);
        }
    }

    public void playAnimation(string aniName)
    {
        this.myAnimation.Play(aniName);
        if ((Network.peerType != NetworkPeerType.Disconnected) && base.networkView.isMine)
        {
            object[] args = new object[] { aniName };
            base.networkView.RPC("netPlayAnimation", RPCMode.Others, args);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        this.myAnimation.Play(aniName);
        this.myAnimation[aniName].normalizedTime = normalizedTime;
        if ((Network.peerType != NetworkPeerType.Disconnected) && base.networkView.isMine)
        {
            object[] args = new object[] { aniName, normalizedTime };
            base.networkView.RPC("netPlayAnimationAt", RPCMode.Others, args);
        }
    }

    public void resetAnimationSpeed()
    {
        foreach (AnimationState state in this.myAnimation)
        {
            state.speed = 1f;
        }
    }

    public void setSkillHUDPosition()
    {
        GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (-Screen.height * 0.5f) + 5f, 0f);
        this.skillCD = GameObject.Find("skill_cd_" + this.modelId);
        this.skillCD.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
        GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
    }

    public void shootFlare(int type)
    {
        bool flag = false;
        if ((type == 1) && (this.flare1CD == 0f))
        {
            this.flare1CD = this.flareTotalCD;
            flag = true;
        }
        if ((type == 2) && (this.flare2CD == 0f))
        {
            this.flare2CD = this.flareTotalCD;
            flag = true;
        }
        if ((type == 3) && (this.flare3CD == 0f))
        {
            this.flare3CD = this.flareTotalCD;
            flag = true;
        }
        if (flag)
        {
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/flareBullet" + type), base.transform.position, base.transform.rotation);
                obj2.GetComponent<FlareMovement>().dontShowHint();
                UnityEngine.Object.Destroy(obj2, 25f);
            }
            else
            {
                GameObject obj3 = (GameObject) Network.Instantiate(Resources.Load("FX/flareBullet" + type), base.transform.position, base.transform.rotation, 0);
                obj3.GetComponent<FlareMovement>().dontShowHint();
                UnityEngine.Object.Destroy(obj3, 25f);
            }
        }
    }

    private void showAimUI()
    {
        Vector3 vector5;
        if (Screen.showCursor)
        {
            GameObject obj2 = GameObject.Find("cross1");
            GameObject obj3 = GameObject.Find("cross2");
            GameObject obj4 = GameObject.Find("crossL1");
            GameObject obj5 = GameObject.Find("crossL2");
            GameObject obj6 = GameObject.Find("crossR1");
            GameObject obj7 = GameObject.Find("crossR2");
            GameObject obj8 = GameObject.Find("LabelDistance");
            vector5 = (Vector3) (Vector3.up * 10000f);
            obj7.transform.localPosition = vector5;
            obj6.transform.localPosition = vector5;
            obj5.transform.localPosition = vector5;
            obj4.transform.localPosition = vector5;
            obj8.transform.localPosition = vector5;
            obj3.transform.localPosition = vector5;
            obj2.transform.localPosition = vector5;
        }
        else
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            LayerMask mask = ((int) 1) << LayerMask.NameToLayer("Ground");
            LayerMask mask2 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
            LayerMask mask3 = mask2 | mask;
            if (Physics.Raycast(ray, out hit, 1E+07f, mask3.value))
            {
                RaycastHit hit2;
                GameObject obj9 = GameObject.Find("cross1");
                GameObject obj10 = GameObject.Find("cross2");
                obj9.transform.localPosition = Input.mousePosition;
                Transform transform = obj9.transform;
                transform.localPosition -= new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
                obj10.transform.localPosition = obj9.transform.localPosition;
                vector5 = hit.point - base.transform.position;
                float magnitude = vector5.magnitude;
                GameObject obj11 = GameObject.Find("LabelDistance");
                string str = (magnitude <= 1000f) ? ((int) magnitude).ToString() : "???";
                obj11.GetComponent<UILabel>().text = str;
                if (magnitude > 120f)
                {
                    Transform transform2 = obj9.transform;
                    transform2.localPosition += (Vector3) (Vector3.up * 10000f);
                    obj11.transform.localPosition = obj10.transform.localPosition;
                }
                else
                {
                    Transform transform3 = obj10.transform;
                    transform3.localPosition += (Vector3) (Vector3.up * 10000f);
                    obj11.transform.localPosition = obj9.transform.localPosition;
                }
                Transform transform4 = obj11.transform;
                transform4.localPosition -= new Vector3(0f, 15f, 0f);
                Vector3 vector = new Vector3(0f, 0.4f, 0f);
                vector -= (Vector3) (base.transform.right * 0.3f);
                Vector3 vector2 = new Vector3(0f, 0.4f, 0f);
                vector2 += (Vector3) (base.transform.right * 0.3f);
                float num2 = (hit.distance <= 50f) ? (hit.distance * 0.05f) : (hit.distance * 0.3f);
                Vector3 vector3 = (hit.point - ((Vector3) (base.transform.right * num2))) - (base.transform.position + vector);
                Vector3 vector4 = (hit.point + ((Vector3) (base.transform.right * num2))) - (base.transform.position + vector2);
                vector3.Normalize();
                vector4.Normalize();
                vector3 = (Vector3) (vector3 * 1000000f);
                vector4 = (Vector3) (vector4 * 1000000f);
                if (Physics.Linecast(base.transform.position + vector, (base.transform.position + vector) + vector3, out hit2, mask3.value))
                {
                    GameObject obj12 = GameObject.Find("crossL1");
                    obj12.transform.localPosition = this.currentCamera.WorldToScreenPoint(hit2.point);
                    Transform transform5 = obj12.transform;
                    transform5.localPosition -= new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
                    obj12.transform.localRotation = Quaternion.Euler(0f, 0f, (Mathf.Atan2(obj12.transform.localPosition.y - (Input.mousePosition.y - (Screen.height * 0.5f)), obj12.transform.localPosition.x - (Input.mousePosition.x - (Screen.width * 0.5f))) * 57.29578f) + 180f);
                    GameObject obj13 = GameObject.Find("crossL2");
                    obj13.transform.localPosition = obj12.transform.localPosition;
                    obj13.transform.localRotation = obj12.transform.localRotation;
                    if (hit2.distance > 120f)
                    {
                        Transform transform6 = obj12.transform;
                        transform6.localPosition += (Vector3) (Vector3.up * 10000f);
                    }
                    else
                    {
                        Transform transform7 = obj13.transform;
                        transform7.localPosition += (Vector3) (Vector3.up * 10000f);
                    }
                }
                if (Physics.Linecast(base.transform.position + vector2, (base.transform.position + vector2) + vector4, out hit2, mask3.value))
                {
                    GameObject obj14 = GameObject.Find("crossR1");
                    obj14.transform.localPosition = this.currentCamera.WorldToScreenPoint(hit2.point);
                    Transform transform8 = obj14.transform;
                    transform8.localPosition -= new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
                    obj14.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(obj14.transform.localPosition.y - (Input.mousePosition.y - (Screen.height * 0.5f)), obj14.transform.localPosition.x - (Input.mousePosition.x - (Screen.width * 0.5f))) * 57.29578f);
                    GameObject obj15 = GameObject.Find("crossR2");
                    obj15.transform.localPosition = obj14.transform.localPosition;
                    obj15.transform.localRotation = obj14.transform.localRotation;
                    if (hit2.distance > 120f)
                    {
                        Transform transform9 = obj14.transform;
                        transform9.localPosition += (Vector3) (Vector3.up * 10000f);
                    }
                    else
                    {
                        Transform transform10 = obj15.transform;
                        transform10.localPosition += (Vector3) (Vector3.up * 10000f);
                    }
                }
            }
        }
    }

    private void showFlareCD()
    {
        if (GameObject.Find("UIflare1") != null)
        {
            GameObject.Find("UIflare1").GetComponent<UISprite>().fillAmount = (this.flareTotalCD - this.flare1CD) / this.flareTotalCD;
            GameObject.Find("UIflare2").GetComponent<UISprite>().fillAmount = (this.flareTotalCD - this.flare2CD) / this.flareTotalCD;
            GameObject.Find("UIflare3").GetComponent<UISprite>().fillAmount = (this.flareTotalCD - this.flare3CD) / this.flareTotalCD;
        }
    }

    private void showGas()
    {
        GameObject.Find("gasL1").GetComponent<UISprite>().fillAmount = this.currentGas / this.totalGas;
        GameObject.Find("gasR1").GetComponent<UISprite>().fillAmount = this.currentGas / this.totalGas;
        GameObject.Find("bladeCL").GetComponent<UISprite>().fillAmount = this.currentBladeSta / this.totalBladeSta;
        GameObject.Find("bladeCR").GetComponent<UISprite>().fillAmount = this.currentBladeSta / this.totalBladeSta;
        if (this.currentBladeNum <= 4)
        {
            GameObject.Find("bladel5").GetComponent<UISprite>().enabled = false;
            GameObject.Find("blader5").GetComponent<UISprite>().enabled = false;
        }
        else
        {
            GameObject.Find("bladel5").GetComponent<UISprite>().enabled = true;
            GameObject.Find("blader5").GetComponent<UISprite>().enabled = true;
        }
        if (this.currentBladeNum <= 3)
        {
            GameObject.Find("bladel4").GetComponent<UISprite>().enabled = false;
            GameObject.Find("blader4").GetComponent<UISprite>().enabled = false;
        }
        else
        {
            GameObject.Find("bladel4").GetComponent<UISprite>().enabled = true;
            GameObject.Find("blader4").GetComponent<UISprite>().enabled = true;
        }
        if (this.currentBladeNum <= 2)
        {
            GameObject.Find("bladel3").GetComponent<UISprite>().enabled = false;
            GameObject.Find("blader3").GetComponent<UISprite>().enabled = false;
        }
        else
        {
            GameObject.Find("bladel3").GetComponent<UISprite>().enabled = true;
            GameObject.Find("blader3").GetComponent<UISprite>().enabled = true;
        }
        if (this.currentBladeNum <= 1)
        {
            GameObject.Find("bladel2").GetComponent<UISprite>().enabled = false;
            GameObject.Find("blader2").GetComponent<UISprite>().enabled = false;
        }
        else
        {
            GameObject.Find("bladel2").GetComponent<UISprite>().enabled = true;
            GameObject.Find("blader2").GetComponent<UISprite>().enabled = true;
        }
        if (this.currentBladeNum <= 0)
        {
            GameObject.Find("bladel1").GetComponent<UISprite>().enabled = false;
            GameObject.Find("blader1").GetComponent<UISprite>().enabled = false;
        }
        else
        {
            GameObject.Find("bladel1").GetComponent<UISprite>().enabled = true;
            GameObject.Find("blader1").GetComponent<UISprite>().enabled = true;
        }
    }

    [RPC]
    private void showHitDamage()
    {
        GameObject target = GameObject.Find("LabelScore");
        if (target != null)
        {
            this.speed = Mathf.Max(10f, this.speed);
            target.GetComponent<UILabel>().text = this.speed.ToString();
            target.transform.localScale = Vector3.zero;
            this.speed = (int) (this.speed * 0.1f);
            this.speed = Mathf.Clamp(this.speed, 40f, 150f);
            iTween.Stop(target);
            object[] args = new object[] { "x", this.speed, "y", this.speed, "z", this.speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f };
            iTween.ScaleTo(target, iTween.Hash(args));
            object[] objArray2 = new object[] { "x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f };
            iTween.ScaleTo(target, iTween.Hash(objArray2));
        }
    }

    private void showSkillCD()
    {
        this.skillCD.GetComponent<UISprite>().fillAmount = (this.skillCDLast - this.skillCDDuration) / this.skillCDLast;
    }

    private void Start()
    {
        this.myAnimation = base.GetComponent<Animation>();
        bool flag = false;

        if ((Application.platform == RuntimePlatform.WindowsWebPlayer) || (Application.platform == RuntimePlatform.OSXWebPlayer))
        {
            if (Application.srcValue != "aog.unity3d")
            {
                flag = true;
            }
            if (string.Compare(Application.absoluteURL, "http://fenglee.com/game/aog/aog.unity3d", true) != 0)
            {
                flag = true;
            }
            if (flag)
            {
                Application.ExternalEval("top.location.href=\"http://fenglee.com/\";");
                UnityEngine.Object.Destroy(base.gameObject);
            }
            Application.ExternalEval("if(window != window.top) {document.location='http://fenglee.com/game/aog/playhere.html'}");
        }
        this.sparks = base.transform.Find("slideSparks").GetComponent<ParticleSystem>();
        this.smoke_3dmg = base.transform.Find("3dmg_smoke").GetComponent<ParticleSystem>();
        base.transform.localScale = new Vector3(this.myScale, this.myScale, this.myScale);
        this.facingDirection = base.transform.rotation.eulerAngles.y;
        this.smoke_3dmg.enableEmission = false;
        this.sparks.enableEmission = false;
        if (((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER) || (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.CLIENT)) && base.networkView.isMine)
        {
            GameObject obj2 = GameObject.Find("UI_IN_GAME");
            this.myNetWorkName = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
            this.myNetWorkName.name = "LabelNameOverHead";
            this.myNetWorkName.transform.parent = obj2.GetComponent<UIReferArray>().panels[0].transform;
            this.myNetWorkName.transform.localScale = new Vector3(22f, 22f, 22f);
            this.myNetWorkName.GetComponent<UILabel>().text = string.Empty;
            this.myNetWorkName.GetComponent<UILabel>().text = base.name;
            object[] args = new object[] { base.name };
            base.networkView.RPC("myNameIs", RPCMode.OthersBuffered, args);
        }
        if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && !base.networkView.isMine)
        {
            base.gameObject.layer = LayerMask.NameToLayer("NetworkObject");
        }
        else
        {
            this.currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            this.inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
            this.skillCDLast = 1.5f;
            if (this.modelId == "levi")
            {
                this.skillCDLast = 3.5f;
            }
            if (this.modelId == "armin")
            {
                this.skillCDLast = 5f;
            }
            if (this.modelId == "marco")
            {
                this.skillCDLast = 10f;
            }
            if (this.modelId == "jean")
            {
                this.skillCDLast = 0.001f;
            }
            if (this.modelId == "eren")
            {
                this.skillCDLast = 120f;
            }
            if (this.modelId == "sasha")
            {
                this.skillCDLast = 20f;
            }
            if (this.modelId == "petra")
            {
                this.skillCDLast = 3.5f;
            }
            this.skillCDDuration = this.skillCDLast;
            GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (-Screen.height * 0.5f) + 5f, 0f);
            this.skillCD = GameObject.Find("skill_cd_" + this.modelId);
            this.skillCD.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
            GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
        }
    }

    private void throwBlades()
    {
        GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/blade_left"));
        GameObject obj3 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("BODY_PART/blade_right"));
        obj2.transform.position = base.transform.position;
        obj3.transform.position = base.transform.position;
        obj2.transform.rotation = base.transform.rotation;
        obj3.transform.rotation = base.transform.rotation;
        Vector3 force = (base.transform.forward + ((Vector3) (base.transform.up * 2f))) - base.transform.right;
        obj2.rigidbody.AddForce(force, ForceMode.Impulse);
        Vector3 vector2 = (base.transform.forward + ((Vector3) (base.transform.up * 2f))) + base.transform.right;
        obj3.rigidbody.AddForce(vector2, ForceMode.Impulse);
        Vector3 torque = new Vector3((float) UnityEngine.Random.Range(-100, 100), (float) UnityEngine.Random.Range(-100, 100), (float) UnityEngine.Random.Range(-100, 100));
        torque.Normalize();
        obj2.rigidbody.AddTorque(torque);
        torque = new Vector3((float) UnityEngine.Random.Range(-100, 100), (float) UnityEngine.Random.Range(-100, 100), (float) UnityEngine.Random.Range(-100, 100));
        torque.Normalize();
        obj3.rigidbody.AddTorque(torque);
        base.transform.Find("bladeL").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
        base.transform.Find("bladeR").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
        this.currentBladeNum--;
        if (this.currentBladeNum == 0)
        {
            this.currentBladeSta = 0f;
        }
        if (this.isAttack)
        {
            this.falseAttack();
        }
    }

    public void ungrabbed()
    {
        this.isGrabbed = false;
        base.rigidbody.rotation = Quaternion.Euler(0f, 0f, 0f);
        base.transform.parent = null;
        base.GetComponent<CapsuleCollider>().isTrigger = false;
        Debug.Log("UNGRABBED!!!");
    }

    private void Update()
    {
        if (!IN_GAME_MAIN_CAMERA.isPausing)
        {
#if DEBUG
            if (GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().lawn)
            {
                this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
                this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
            }
#endif
          //  Vector3 vector22 = currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity;
         //   int num22 = (int)((vector22.magnitude * 10f));
          //  GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().netShowHUDInfoCenter("             "+num22.ToString());
            if (this.invincible > 0f)
            {
                this.invincible -= Time.deltaTime;
            }
            if (!this.hasDied)
            {
                if (this.titanForm && (this.eren_titan != null))
                {
                    base.transform.position = this.eren_titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
                    base.gameObject.GetComponent<MovementUpdate>().disabled = true;
                }
                if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && (this.myNetWorkName != null))
                {
                    if (this.titanForm && (this.eren_titan != null))
                    {
                        this.myNetWorkName.transform.localPosition = (Vector3) ((Vector3.up * Screen.height) * 2f);
                    }
                    Vector3 start = new Vector3(base.transform.position.x, base.transform.position.y + 2f, base.transform.position.z);
                    GameObject obj2 = GameObject.Find("MainCamera");
                    LayerMask mask = ((int) 1) << LayerMask.NameToLayer("Ground");
                    LayerMask mask2 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
                    LayerMask mask3 = mask2 | mask;
                    if ((Vector3.Angle(obj2.transform.forward, start - obj2.transform.position) > 90f) || Physics.Linecast(start, obj2.transform.position, (int) mask3))
                    {
                        this.myNetWorkName.transform.localPosition = (Vector3) ((Vector3.up * Screen.height) * 2f);
                    }
                    else
                    {
                        Vector2 vector2 = GameObject.Find("MainCamera").GetComponent<Camera>().WorldToScreenPoint(start);
                        this.myNetWorkName.transform.localPosition = new Vector3(vector2.x - (Screen.width * 0.5f), vector2.y - (Screen.height * 0.5f), 0f);
                    }
                }
                if (((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine) && this.isGrabbed)
                {
                    if (this.modelId == "jean")
                    {
                        if ((!this.isAttack && this.inputManager.isInputDown[10]) && (this.escapeTimes > 0))
                        {
                            this.attackAnimation = "grabbed_jean";
                            this.playAnimation(this.attackAnimation);
                            this.myAnimation[this.attackAnimation].time = 0f;
                            this.isAttack = true;
                            this.attackReleased = true;
                            this.buttonAttackRelease = true;
                            this.escapeTimes--;
                        }
                        if ((this.myAnimation.IsPlaying("grabbed_jean") && (base.animation["grabbed_jean"].normalizedTime > 0.64f)) && (this.titanWhoGrabMe.GetComponent<TITAN>() != null))
                        {
                            this.ungrabbed();
                            base.rigidbody.velocity = (Vector3) (Vector3.up * 30f);
                            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                            {
                                this.titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                            }
                            else
                            {
                                base.networkView.RPC("netSetIsGrabbedFalse", RPCMode.All, new object[0]);
                                if (Network.isServer)
                                {
                                    this.titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                                }
                                else
                                {
                                    NetworkView.Find(this.titanWhoGrabMeID).networkView.RPC("grabbedTargetEscape", RPCMode.Server, new object[0]);
                                }
                            }
                        }
                    }
                    else if (this.modelId == "eren")
                    {
                        this.showSkillCD();
                        if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) || ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) && !IN_GAME_MAIN_CAMERA.isPausing))
                        {
                            this.calcSkillCD();
                            this.calcFlareCD();
                        }
                        if (((IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.WOW) && this.inputManager.isInput[1]) || this.inputManager.isInputDown[11])
                        {
                            bool flag = false;
                            if (((IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.WOW) && this.inputManager.isInputDown[11]) && (this.inputManager.inputKey[11] == KeyCode.Mouse1))
                            {
                                flag = true;
                            }
                            if ((this.skillCDDuration > 0f) || flag)
                            {
                                flag = true;
                            }
                            else
                            {
                                this.skillCDDuration = this.skillCDLast;
                                if ((this.modelId == "eren") && (this.titanWhoGrabMe.GetComponent<TITAN>() != null))
                                {
                                    this.ungrabbed();
                                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                                    {
                                        this.titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                                    }
                                    else
                                    {
                                        base.networkView.RPC("netSetIsGrabbedFalse", RPCMode.All, new object[0]);
                                        if (Network.isServer)
                                        {
                                            this.titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                                        }
                                        else
                                        {
                                            NetworkView.Find(this.titanWhoGrabMeID).networkView.RPC("grabbedTargetEscape", RPCMode.Server, new object[0]);
                                        }
                                    }
                                    this.erenTransform();
                                }
                            }
                        }
                    }
                }
                else if (!this.titanForm && ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine))
                {
                    if (!this.isAttack)
                    {
                        if (this.inputManager.isInputDown[0x13])
                        {
                            #if DEBUG
                          //  if (GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().dick && this.tele < 1)
                           // {
                                GameObject titan = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().findNearestTitan();
                               base.transform.position = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
                                this.tele++;
                           // }
                            #endif
                            this.shootFlare(1);
                        }
                        if (this.inputManager.isInputDown[20])
                        {
                            this.shootFlare(2);
                        }
                        if (this.inputManager.isInputDown[0x15])
                        {
                            this.shootFlare(3);
                        }
                    }
                    if (this.buffTime > 0f)
                    {
                        this.buffTime -= Time.deltaTime;
                        if (this.buffTime <= 0f)
                        {
                            this.buffTime = 0f;
                            if ((this.currentBuff == BUFF.SpeedUp) && base.animation.IsPlaying("run_sasha"))
                            {
                                this.crossFade("run", 0.1f);
                            }
                            this.currentBuff = BUFF.NoBuff;
                        }
                    }
                    if (((!this.isAttack && !this.myAnimation.IsPlaying("dash_land")) && (!this.myAnimation.IsPlaying("hitWall") && !this.myAnimation.IsPlaying("changeBlade"))) && (this.inputManager.isInputDown[10] || this.inputManager.isInputDown[11]))
                    {
                        bool flag2 = false;
                        if (((IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.WOW) && this.inputManager.isInput[1]) || this.inputManager.isInputDown[11])
                        {
                            if (((IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.WOW) && this.inputManager.isInputDown[11]) && (this.inputManager.inputKey[11] == KeyCode.Mouse1))
                            {
                                flag2 = true;
                            }
                            if ((this.skillCDDuration > 0f) || flag2)
                            {
                                flag2 = true;
                            }
                            else
                            {
                                this.skillCDDuration = this.skillCDLast;
                                if (this.modelId == "eren")
                                {
                                    this.erenTransform();
                                    return;
                                }
                                if (this.modelId == "marco")
                                {
                                    if (this.IsGrounded())
                                    {
                                        this.attackAnimation = (UnityEngine.Random.Range(0, 2) != 0) ? "special_marco_1" : "special_marco_0";
                                        this.playAnimation(this.attackAnimation);
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        this.skillCDDuration = 0f;
                                    }
                                }
                                else if (this.modelId == "armin")
                                {
                                    if (this.IsGrounded())
                                    {
                                        this.attackAnimation = "special_armin";
                                        this.playAnimation("special_armin");
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        this.skillCDDuration = 0f;
                                    }
                                }
                                else if (this.modelId == "sasha")
                                {
                                    if (this.IsGrounded())
                                    {
                                        this.attackAnimation = "special_sasha";
                                        this.playAnimation("special_sasha");
                                        this.currentBuff = BUFF.SpeedUp;
                                        this.buffTime = 10f;
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        this.skillCDDuration = 0f;
                                    }
                                }
                                else if (this.modelId == "mikasa")
                                {
                                    this.attackAnimation = "attack3_1";
                                    this.playAnimation("attack3_1");
                                    base.rigidbody.velocity = (Vector3) (Vector3.up * 10f);
                                }
                                else
                                {
                                    Quaternion quaternion;
                                    if (this.modelId == "levi")
                                    {
                                        RaycastHit hit;
                                        this.attackAnimation = "attack5";
                                        this.playAnimation("attack5");
                                        Rigidbody rigidbody = base.rigidbody;
                                        rigidbody.velocity += (Vector3) (Vector3.up * 5f);
                                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                                        LayerMask mask4 = ((int) 1) << LayerMask.NameToLayer("Ground");
                                        LayerMask mask5 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
                                        LayerMask mask6 = mask5 | mask4;
                                        if (Physics.Raycast(ray, out hit, 1E+07f, mask6.value))
                                        {
                                            if (this.bulletRight != null)
                                            {
                                                this.bulletRight.GetComponent<Bullet>().disable();
                                            }
                                            this.dashDirection = hit.point - base.transform.position;
                                            this.launchRightRope(hit, true, 1);
                                            this.rope.Play();
                                        }
                                        this.facingDirection = Mathf.Atan2(this.dashDirection.x, this.dashDirection.z) * 57.29578f;
                                        quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
                                        base.gameObject.transform.rotation = quaternion;
                                        base.rigidbody.rotation = quaternion;
                                        this.attackLoop = 2;
                                    }
                                    else if (this.modelId == "petra")
                                    {
                                        RaycastHit hit2;
                                        this.attackAnimation = "special_petra";
                                        this.playAnimation("special_petra");
                                        Rigidbody rigidbody2 = base.rigidbody;
                                        rigidbody2.velocity += (Vector3) (Vector3.up * 5f);
                                        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                        LayerMask mask7 = ((int) 1) << LayerMask.NameToLayer("Ground");
                                        LayerMask mask8 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
                                        LayerMask mask9 = mask8 | mask7;
                                        if (Physics.Raycast(ray2, out hit2, 1E+07f, mask9.value))
                                        {
                                            if (this.bulletRight != null)
                                            {
                                                this.bulletRight.GetComponent<Bullet>().disable();
                                            }
                                            if (this.bulletLeft != null)
                                            {
                                                this.bulletLeft.GetComponent<Bullet>().disable();
                                            }
                                            this.dashDirection = hit2.point - base.transform.position;
                                            this.launchLeftRope(hit2, true, 0);
                                            this.launchRightRope(hit2, true, 0);
                                            this.rope.Play();
                                        }
                                        this.facingDirection = Mathf.Atan2(this.dashDirection.x, this.dashDirection.z) * 57.29578f;
                                        quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
                                        base.gameObject.transform.rotation = quaternion;
                                        base.rigidbody.rotation = quaternion;
                                        this.attackLoop = 3;
                                    }
                                    else
                                    {
                                        this.attackAnimation = "attack1";
                                        this.playAnimation("attack1");
                                    }
                                }
                            }
                        }
                        else if (this.inputManager.isInput[2])
                        {
                            this.attackAnimation = "attack2";
                        }
                        else if (this.inputManager.isInput[3])
                        {
                            this.attackAnimation = "attack1";
                        }
                        else if (this.lastHook != null)
                        {
                            this.attackAccordingToTarget(this.lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck"));
                        }
                        else if ((this.bulletLeft != null) && (this.bulletLeft.transform.parent != null))
                        {
                            Transform a = this.bulletLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                            if (a != null)
                            {
                                this.attackAccordingToTarget(a);
                            }
                            else
                            {
                                this.attackAccordingToMouse();
                            }
                        }
                        else if ((this.bulletRight != null) && (this.bulletRight.transform.parent != null))
                        {
                            Transform transform2 = this.bulletRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                            if (transform2 != null)
                            {
                                this.attackAccordingToTarget(transform2);
                            }
                            else
                            {
                                this.attackAccordingToMouse();
                            }
                        }
                        else
                        {
                            GameObject obj3 = this.findNearestTitan();
                            if (obj3 != null)
                            {
                                Transform transform3 = obj3.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                if (transform3 != null)
                                {
                                    this.attackAccordingToTarget(transform3);
                                }
                                else
                                {
                                    this.attackAccordingToMouse();
                                }
                            }
                            else
                            {
                                this.attackAccordingToMouse();
                            }
                        }
                        if (!flag2)
                        {
                            this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                            this.checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                            if (this.grounded)
                            {
                                base.rigidbody.AddForce((Vector3) (base.gameObject.transform.forward * 200f));
                            }
                            this.playAnimation(this.attackAnimation);
                            this.myAnimation[this.attackAnimation].time = 0f;
                            this.buttonAttackRelease = false;
                            this.isAttack = true;
                            if ((this.grounded || (this.attackAnimation == "attack3_1")) || ((this.attackAnimation == "attack5") || (this.attackAnimation == "special_petra")))
                            {
                                this.attackReleased = true;
                                this.buttonAttackRelease = true;
                            }
                            else
                            {
                                this.attackReleased = false;
                            }
                            this.sparks.enableEmission = false;
                        }
                    }
                    if (!this.inputManager.isInput[10])
                    {
                        this.buttonAttackRelease = true;
                    }
                    if (this.isAttack)
                    {
                        if (!this.attackReleased)
                        {
                            if (this.buttonAttackRelease)
                            {
                                this.continueAnimation();
                                this.attackReleased = true;
                            }
                            else if (this.myAnimation[this.attackAnimation].normalizedTime >= 0.32f)
                            {
                                this.pauseAnimation();
                            }
                        }
                        if ((this.attackAnimation == "attack3_1") && (this.currentBladeSta > 0f))
                        {
                            if (this.myAnimation[this.attackAnimation].normalizedTime >= 0.8f)
                            {
                                if (!this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                                {
                                    this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
                                    base.rigidbody.velocity = (Vector3) (-Vector3.up * 30f);
                                }
                                if (!this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
                                {
                                    this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
                                    this.slash.Play();
                                }
                            }
                            else
                            {
                                this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                                this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                                this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                                this.checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                            }
                        }
                        else
                        {
                            float num;
                            float num2;
                            if (this.currentBladeSta == 0f)
                            {
                                num = num2 = -1f;
                            }
                            else if (this.attackAnimation == "attack5")
                            {
                                num = 0.4f;
                                num2 = 0.61f;
                            }
                            else if (this.attackAnimation == "special_petra")
                            {
                                num = 0.35f;
                                num2 = 0.48f;
                            }
                            else if (this.attackAnimation == "special_armin")
                            {
                                num = 0.25f;
                                num2 = 0.35f;
                            }
                            else if (this.attackAnimation == "attack4")
                            {
                                num = 0.6f;
                                num2 = 0.9f;
                            }
                            else if (this.attackAnimation == "special_sasha")
                            {
                                num = num2 = -1f;
                            }
                            else
                            {
                                num = 0.5f;
                                num2 = 0.85f;
                            }
                            if ((this.myAnimation[this.attackAnimation].normalizedTime > num) && (this.myAnimation[this.attackAnimation].normalizedTime < num2))
                            {
                                if (!this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
                                {
                                    this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
                                    this.slash.Play();
                                }
                                if (!this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
                                {
                                    this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
                                }
                            }
                            else
                            {
                                this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                                this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
                                this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
                                this.checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
                            }
                            if ((this.attackLoop > 0) && (this.myAnimation[this.attackAnimation].normalizedTime > num2))
                            {
                                this.attackLoop--;
                                this.playAnimationAt(this.attackAnimation, num);
                            }
                        }
                        if (this.myAnimation[this.attackAnimation].normalizedTime >= 1f)
                        {
                            if ((this.attackAnimation == "special_marco_0") || (this.attackAnimation == "special_marco_1"))
                            {
                                if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
                                {
                                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.CLIENT)
                                    {
                                        object[] args = new object[] { 5f, 100f };
                                        base.networkView.RPC("netTauntAttack", RPCMode.Server, args);
                                    }
                                    else
                                    {
                                        this.netTauntAttack(5f, 100f);
                                    }
                                }
                                else
                                {
                                    this.netTauntAttack(5f, 100f);
                                }
                                this.falseAttack();
                            }
                            else if (this.attackAnimation == "special_armin")
                            {
                                if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
                                {
                                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.CLIENT)
                                    {
                                        base.networkView.RPC("netlaughAttack", RPCMode.Server, new object[0]);
                                    }
                                    else
                                    {
                                        this.netlaughAttack();
                                    }
                                }
                                else
                                {
                                    foreach (GameObject obj4 in GameObject.FindGameObjectsWithTag("titan"))
                                    {
                                        if (((Vector3.Distance(obj4.transform.position, base.transform.position) < 50f) && (Vector3.Angle(obj4.transform.forward, base.transform.position - obj4.transform.position) < 90f)) && (obj4.GetComponent<TITAN>() != null))
                                        {
                                            obj4.GetComponent<TITAN>().beLaughAttacked();
                                        }
                                    }
                                }
                                this.falseAttack();
                            }
                            else if (this.attackAnimation == "attack3_1")
                            {
                                Rigidbody rigidbody3 = base.rigidbody;
                                rigidbody3.velocity -= (Vector3) ((Vector3.up * Time.deltaTime) * 30f);
                            }
                            else
                            {
                                this.falseAttack();
                            }
                        }
                    }
                    if (((this.inputManager.isInput[6] && !this.myAnimation.IsPlaying("hitWall")) && (!this.myAnimation.IsPlaying("attack3_1") && !this.myAnimation.IsPlaying("attack5"))) && ((!this.myAnimation.IsPlaying("special_petra") && !this.myAnimation.IsPlaying("wallrun")) && (!this.myAnimation.IsPlaying("toRoof") && !this.myAnimation.IsPlaying("changeBlade"))))
                    {
                        if (this.bulletLeft != null)
                        {
                            this.QHold = true;
                        }
                        else
                        {
                            RaycastHit hit3;
                            Ray ray3 = Camera.main.ScreenPointToRay(Input.mousePosition);
                            LayerMask mask10 = ((int) 1) << LayerMask.NameToLayer("Ground");
                            LayerMask mask11 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
                            LayerMask mask12 = mask11 | mask10;
                            if (Physics.Raycast(ray3, out hit3, 10000f, mask12.value))
                      //      if (Physics.Linecast(ray3.origin, ray3.GetPoint(20000f), out hit3, mask12.value))

                            {
                                this.launchLeftRope(hit3, true, 0);
                                this.rope.Play();
                            }
                        }
                    }
                    else
                    {
                        this.QHold = false;
                    }
                    if (((this.inputManager.isInput[7] && !this.myAnimation.IsPlaying("hitWall")) && (!this.myAnimation.IsPlaying("attack3_1") && !this.myAnimation.IsPlaying("attack5"))) && ((!this.myAnimation.IsPlaying("special_petra") && !this.myAnimation.IsPlaying("wallrun")) && (!this.myAnimation.IsPlaying("toRoof") && !this.myAnimation.IsPlaying("changeBlade"))))
                    {
                        if (this.bulletRight != null)
                        {
                            this.EHold = true;
                        }
                        else
                        {
                            RaycastHit hit4;
                            Ray ray4 = Camera.main.ScreenPointToRay(Input.mousePosition);
                            LayerMask mask13 = ((int) 1) << LayerMask.NameToLayer("Ground");
                            LayerMask mask14 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
                            LayerMask mask15 = mask14 | mask13;
                            if (Physics.Raycast(ray4, out hit4, 10000f, mask15.value))
                     //         if (Physics.Linecast(ray4.origin , ray4.GetPoint(20000f) , out hit4, mask15.value))

                            {
                                this.launchRightRope(hit4, true, 0);
                                this.rope.Play();
                            }
                        }
                    }
                    else
                    {
                        this.EHold = false;
                    }
                    if (((this.inputManager.isInput[8] && !this.myAnimation.IsPlaying("hitWall")) && (!this.myAnimation.IsPlaying("attack3_1") && !this.myAnimation.IsPlaying("attack5"))) && ((!this.myAnimation.IsPlaying("special_petra") && !this.myAnimation.IsPlaying("wallrun")) && (!this.myAnimation.IsPlaying("toRoof") && !this.myAnimation.IsPlaying("changeBlade"))))
                    {
                        this.QHold = true;
                        this.EHold = true;
                        if ((this.bulletLeft == null) && (this.bulletRight == null))
                        {
                            RaycastHit hit5;
                            Ray ray5 = Camera.main.ScreenPointToRay(Input.mousePosition);
                            LayerMask mask16 = ((int) 1) << LayerMask.NameToLayer("Ground");
                            LayerMask mask17 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
                            LayerMask mask18 = mask17 | mask16;
                            if (Physics.Raycast(ray5, out hit5, 1000000f, mask18.value))
                            {
                                this.launchLeftRope(hit5, false, 0);
                                this.launchRightRope(hit5, false, 0);
                                this.rope.Play();
                            }
                        }
                    }
                    if (((this.grounded && !this.isAttack) && (!this.myAnimation.IsPlaying("dodge") && !this.myAnimation.IsPlaying("changeBlade"))) && (!this.myAnimation.IsPlaying("jump") && this.inputManager.isInputDown[5]))
                    {
                        float num4;
                        float num5;
                        if (this.inputManager.isInput[0])
                        {
                            num5 = 1f;
                        }
                        else if (this.inputManager.isInput[1])
                        {
                            num5 = -1f;
                        }
                        else
                        {
                            num5 = 0f;
                        }
                        if (this.inputManager.isInput[2])
                        {
                            num4 = -1f;
                        }
                        else if (this.inputManager.isInput[3])
                        {
                            num4 = 1f;
                        }
                        else
                        {
                            num4 = 0f;
                        }
                        float y = this.currentCamera.transform.rotation.eulerAngles.y;
                        float num7 = Mathf.Atan2(num5, num4) * 57.29578f;
                        num7 = -num7 + 90f;
                        float num8 = y + num7;
                        if ((num4 != 0f) || (num5 != 0f))
                        {
                            this.facingDirection = num8 + 180f;
                            base.rigidbody.rotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                        }
                        this.crossFade("dodge", 0.1f);
                        this.sparks.enableEmission = false;
                    }
                    if (((this.grounded && !this.isAttack) && (!this.myAnimation.IsPlaying("jump") && !this.myAnimation.IsPlaying("changeBlade"))) && (!this.myAnimation.IsPlaying("dodge") && this.inputManager.isInputDown[4]))
                    {
                        this.crossFade("jump", 0.1f);
                        this.sparks.enableEmission = false;
                    }
                    if (this.myAnimation.IsPlaying("stand") && this.inputManager.isInputDown[0x12])
                    {
                        this.crossFade("changeBlade", 0.1f);
                        this.throwedBlades = false;
                    }
                    if (this.myAnimation.IsPlaying("stand") && this.inputManager.isInputDown[12])
                    {
                        this.crossFade("salute", 0.1f);
                    }
                    if (this.myAnimation.IsPlaying("dodge") && (this.myAnimation["dodge"].normalizedTime >= 1f))
                    {
                        this.crossFade("stand", 0.1f);
                    }
                    if (this.myAnimation.IsPlaying("salute") && (this.myAnimation["salute"].normalizedTime >= 1f))
                    {
                        this.crossFade("stand", 0.1f);
                    }
                    if (this.myAnimation.IsPlaying("supply") && (this.myAnimation["supply"].normalizedTime >= 1f))
                    {
                        this.crossFade("stand", 0.1f);
                        this.currentBladeSta = this.totalBladeSta;
                        this.currentBladeNum = this.totalBladeNum;
                        this.currentGas = this.totalGas;
                        base.transform.Find("bladeL").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        base.transform.Find("bladeR").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    if (this.myAnimation.IsPlaying("changeBlade"))
                    {
                        if ((this.myAnimation["changeBlade"].normalizedTime >= 0.2f) && !this.throwedBlades)
                        {
                            this.throwedBlades = true;
                            if (base.transform.Find("bladeL").gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                            {
                                this.throwBlades();
                            }
                        }
                        if ((this.myAnimation["changeBlade"].normalizedTime >= 0.56f) && (this.currentBladeNum > 0))
                        {
                            base.transform.Find("bladeL").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                            base.transform.Find("bladeR").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                            this.currentBladeSta = this.totalBladeSta;
                        }
                        if (this.myAnimation["changeBlade"].normalizedTime >= 1f)
                        {
                            this.crossFade("stand", 0.1f);
                        }
                    }
                    if (this.myAnimation.IsPlaying("dash_land") && (this.myAnimation["dash_land"].normalizedTime >= 1f))
                    {
                        this.crossFade("stand", 0.1f);
                        if (this.isAttack)
                        {
                            this.falseAttack();
                        }
                    }
                    if (this.myAnimation.IsPlaying("attack3_2") && (this.myAnimation["attack3_2"].normalizedTime >= 1f))
                    {
                        this.crossFade("stand", 0.1f);
                        if (this.isAttack)
                        {
                            this.falseAttack();
                        }
                    }
                    if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) || ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) && !IN_GAME_MAIN_CAMERA.isPausing))
                    {
                        this.calcSkillCD();
                        this.calcFlareCD();
                    }
                    if (!IN_GAME_MAIN_CAMERA.isPausing)
                    {
                        this.showSkillCD();
                        this.showFlareCD();
                        this.showGas();
                        this.showAimUI();
                    }
                }
            }
        }
    }

    public void useBlade(int amount = 0)
    {
        if (amount == 0)
        {
            amount = 1;
        }
        if (this.currentBladeSta > 0f)
        {
            #if DEBUG
            if (GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().lawn)
                amount = 0;
            #endif
            this.currentBladeSta -= amount;
            if (this.currentBladeSta <= 0f)
            {
                this.currentBladeSta = 0f;
                this.throwBlades();
                this.checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
                this.checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
            }
        }
    }

    private void useGas(float amount = 0f)
    {
        if (amount == 0f)
        {
            amount = this.useGasSpeed;
        }
        if (this.currentGas > 0f)
        {
            this.currentGas -= amount;
            if (this.currentGas < 0f)
            {
                this.currentGas = 0f;
            }
        }
    }

    [RPC]
    private void whoIsMyErenTitan(NetworkViewID id)
    {
        this.eren_titan = NetworkView.Find(id).gameObject;
        this.titanForm = true;
    }

}

