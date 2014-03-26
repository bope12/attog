using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class IN_GAME_MAIN_CAMERA : MonoBehaviour
{
    public RotationAxes axes;
    public AudioSource bgmusic;
    public static CAMERA_TYPE cameraMode;
    public static int cameraTilt = 1;
    public static int character = 1;
    private float closestDistance;
    private float decay;
    public static int difficulty;
    private float distance = 10f;
    private float distanceMulti;
    private float distanceOffsetMulti;
    private float duration;
    private float flashDuration;
    private bool flip;
    public static GAMEMODE gamemode;
    public bool gameOver;
    public static GAMETYPE gametype = GAMETYPE.STOP;
    private Transform head;
    private float height = 5f;
    private float heightDamping = 2f;
    private float heightMulti;
    public FengCustomInputs inputManager;
    public static int invertY = 1;
    public static bool isCheating;
    public static bool isPausing;
    public static bool isTyping;
    public float justHit;
    public int lastScore;
    public static int level;
    private Vector3 lockCameraPosition;
    private GameObject lockTarget;
    public GameObject main_object;
    public float maximumX = 360f;
    public float maximumY = 60f;
    public float minimumX = -360f;
    public float minimumY = -60f;
    private bool needSetHUD;
    private float R;
    private float rotationY;
    public int score;
    public static float sensitivityMulti = 0.5f;
    public static string singleCharacter;
    public static STEREO_3D_TYPE stereoType;
    private Transform target;
    public Texture texture;
    public float timer;
    public int titanNum = 0x20;
    public static bool triggerAutoLock;
    private Vector3 verticalHeightOffset = Vector3.zero;
    private float verticalRotationOffset;
    private float xSpeed = -3f;
    private float ySpeed = -0.8f;
    private int index = 0;
    public static CAMERA_TYPE cameraold;
    public float rotationSpeed = 0.06f;
    public float normalSpeed = 175;
    public float highSpeed = 2f;
    private static bool isTPSfix = false;
    private void Awake()
    {
        isTyping = false;
        isPausing = false;
        base.name = "MainCamera";
    }
    public static void setTPSfixOn()
    {
        isTPSfix = true;
    }
    public GameObject findNearestTitan()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("titan");
        GameObject obj2 = null;
        float num = this.closestDistance = float.PositiveInfinity;
        Vector3 position = this.main_object.transform.position;
        foreach (GameObject obj3 in objArray)
        {
            Vector3 vector2 = obj3.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position - position;
            float magnitude = vector2.magnitude;
            if ((magnitude < num) && ((obj3.GetComponent<TITAN>() == null) || !obj3.GetComponent<TITAN>().hasDie))
            {
                obj2 = obj3;
                num = magnitude;
                this.closestDistance = num;
            }
        }
        return obj2;
    }

    public void Revert()
    {
        if (cameraMode == CAMERA_TYPE.FLY)
            cameraMode = cameraold;
    }
    public void flashBlind()
    {
        GameObject.Find("flash").GetComponent<UISprite>().alpha = 1f;
        this.flashDuration = 2f;
    }

    private int getReverse()
    {
        return invertY;
    }

    private float getSensitivityMulti()
    {
        return (sensitivityMulti * 2f);
    }

    private void LateUpdate()
    {
        if ((this.titanNum > 0) && !this.gameOver)
        {
            this.timer += Time.deltaTime;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (gametype == GAMETYPE.SINGLE)
        {
            GameObject obj2 = GameObject.FindGameObjectWithTag("playerRespawn");
            this.setMainObject((GameObject) UnityEngine.Object.Instantiate(Resources.Load(singleCharacter.ToUpper()), obj2.transform.position, obj2.transform.rotation));
            GameObject obj3 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("UI_IN_GAME"));
            obj3.name = "UI_IN_GAME";
            obj3.SetActive(true);
            NGUITools.SetActive(obj3.GetComponent<UIReferArray>().panels[0], true);
            NGUITools.SetActive(obj3.GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(obj3.GetComponent<UIReferArray>().panels[2], false);
            this.setHUDposition();
        }
    }

    private void reset()
    {
        if (gametype == GAMETYPE.SINGLE)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    public void setHUDposition()
    {
        GameObject.Find("Flare").transform.localPosition = new Vector3(-Screen.width * 0.5f, -Screen.height * 0.5f, 0f);
        GameObject obj2 = GameObject.Find("LabelInfoBottomRight");
        obj2.transform.localPosition = new Vector3(Screen.width * 0.5f, -Screen.height * 0.5f, 0f);
        obj2.GetComponent<UILabel>().text = "Pause : " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[15] + " ";
        GameObject.Find("LabelInfoTopCenter").transform.localPosition = new Vector3(0f, Screen.height * 0.5f, 0f);
        GameObject.Find("LabelInfoTopRight").transform.localPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        GameObject.Find("LabelInfoTopLeft").transform.localPosition = new Vector3(-Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        GameObject.Find("Chatroom").transform.localPosition = new Vector3(-Screen.width * 0.5f, -Screen.height * 0.5f, 0f);
        if ((this.main_object != null) && (this.main_object.GetComponent<HERO>() != null))
        {
            if (gametype == GAMETYPE.SINGLE)
            {
                this.main_object.GetComponent<HERO>().setSkillHUDPosition();
            }
            else if ((this.main_object.networkView != null) && this.main_object.networkView.isMine)
            {
                this.main_object.GetComponent<HERO>().setSkillHUDPosition();
            }
        }
        if (stereoType == STEREO_3D_TYPE.SIDE_BY_SIDE)
        {
            base.gameObject.GetComponent<Camera>().aspect = Screen.width / Screen.height;
        }
    }

    public GameObject setMainObject(GameObject obj)
    {
        this.main_object = obj;
        if (obj == null)
        {
            this.head = null;
            this.distanceMulti = this.heightMulti = 1f;
            return obj;
        }
        if (this.main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head") != null)
        {
            this.head = this.main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
            this.distanceMulti = (this.head != null) ? (Vector3.Distance(this.head.transform.position, this.main_object.transform.position) * 0.2f) : 1f;
            this.heightMulti = (this.head != null) ? (Vector3.Distance(this.head.transform.position, this.main_object.transform.position) * 0.33f) : 1f;
            base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            return obj;
        }
        this.head = null;
        this.distanceMulti = this.heightMulti = 1f;
        if (!(cameraMode == CAMERA_TYPE.FLY))
        {
            base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        return obj;
    }

    private void shakeUpdate()
    {
        if (this.duration > 0f)
        {
            this.duration -= Time.deltaTime;
            if (this.flip)
            {
                Transform transform = base.gameObject.transform;
                transform.position += (Vector3) (Vector3.up * this.R);
            }
            else
            {
                Transform transform2 = base.gameObject.transform;
                transform2.position -= (Vector3) (Vector3.up * this.R);
            }
            this.flip = !this.flip;
            this.R *= this.decay;
        }
    }

    private void Start()
    {
        isPausing = false;
        if (level == 2)
        {
            this.titanNum = 0x12;
        }
        else if (level == 1)
        {
            this.titanNum = 15;
        }
        else if (level == 0)
        {
            this.titanNum = 0x20;
        }
        sensitivityMulti = PlayerPrefs.GetFloat("MouseSensitivity");
        invertY = PlayerPrefs.GetInt("invertMouseY");
        if (PlayerPrefs.HasKey("GameQuality"))
        {
            if (PlayerPrefs.GetFloat("GameQuality") > 0.5f)
            {
                base.GetComponent<TiltShift>().enabled = true;
            }
            else
            {
                base.GetComponent<TiltShift>().enabled = false;
            }
        }
        else
        {
            base.GetComponent<TiltShift>().enabled = true;
        }
        this.inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
    }

    public void startShake(float R, float duration, float decay = 0.95f)
    {
        if (this.duration < duration)
        {
            this.R = R;
            this.duration = duration;
            this.decay = decay;
        }
    }

    private void Update()
    {
        if (gametype == GAMETYPE.STOP)
        {
            Screen.showCursor = true;
            Screen.lockCursor = false;
        }
        else
        {
            if (this.flashDuration > 0f)
            {
                this.flashDuration -= Time.deltaTime;
                if (this.flashDuration <= 0f)
                {
                    this.flashDuration = 0f;
                }
                GameObject.Find("flash").GetComponent<UISprite>().alpha = this.flashDuration * 0.5f;
            }
            if (!isTyping)
            {
                if ((gametype != GAMETYPE.SINGLE) && this.gameOver)
                {
                    if (Network.isServer)
                    {
                        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                        if (this.inputManager.isInputDown[10])
                        {
                            index++;
                            if (index >= players.Length) index = 0;
                            this.setMainObject(players[index]);

                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha0))
                        {
                            if (cameraMode == CAMERA_TYPE.FLY)
                            {
                                cameraMode = cameraold;
                                this.setMainObject(players[index]);
                            }
                            else
                            {
                                cameraold = cameraMode;
                                cameraMode = CAMERA_TYPE.FLY;
                                setMainObject(null);
                            }
                        }
                        else if (this.inputManager.isInputDown[19])
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "1")
                                {
                                    this.setMainObject(obj2);
                                    index = 1;
                                    break;
                                }
                            }
                        }
                        else if (this.inputManager.isInputDown[20])
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "2")
                                {
                                    this.setMainObject(obj2);
                                    index = 2;
                                    break;
                                }
                            }
                        }
                        else if (this.inputManager.isInputDown[21])
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "3")
                                {
                                    this.setMainObject(obj2);
                                    index = 3;
                                    break;
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha4))
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "4")
                                {
                                    this.setMainObject(obj2);
                                    index = 4;
                                    break;
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha5))
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "5")
                                {
                                    this.setMainObject(obj2);
                                    index = 5;
                                    break;
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha6))
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "6")
                                {
                                    this.setMainObject(obj2);
                                    index = 6;
                                    break;
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha7))
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "7")
                                {
                                    this.setMainObject(obj2);
                                    index = 7;
                                    break;
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha8))
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "8")
                                {
                                    this.setMainObject(obj2);
                                    index = 8;
                                    break;
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha9))
                        {
                            foreach (GameObject obj2 in players)
                            {
                                if (obj2.networkView.owner.ToString() == "9")
                                {
                                    this.setMainObject(obj2);
                                    index = 9;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                        if (this.inputManager.isInputDown[10])
                        {
                            index++;
                            if (index >= players.Length) index = 0;
                            this.setMainObject(players[index]);

                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha0))
                        {
                            if (cameraMode == CAMERA_TYPE.FLY)
                            {
                                cameraMode = cameraold;
                                this.setMainObject(players[0]);
                            }
                            else
                            {
                                cameraold = cameraMode;
                                cameraMode = CAMERA_TYPE.FLY;
                                setMainObject(null);
                            }
                        }
                        else if (this.inputManager.isInputDown[19])
                        {
                            this.setMainObject(players[0]);
                        }
                        else if (this.inputManager.isInputDown[20])
                        {
                            this.setMainObject(players[1]);
                        }
                        else if (this.inputManager.isInputDown[21])
                        {
                            this.setMainObject(players[2]);
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha4))
                        {
                            this.setMainObject(players[3]);
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha5))
                        {
                            this.setMainObject(players[4]);
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha6))
                        {
                            this.setMainObject(players[5]);
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha7))
                        {
                            this.setMainObject(players[6]);
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha8))
                        {
                            this.setMainObject(players[7]);
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha9))
                        {
                            this.setMainObject(players[8]);
                        }
                    }
                }
                if (this.inputManager.isInputDown[15])
                {
                    if (isPausing)
                    {
                        return;
                    }
                    isPausing = !isPausing;
                    if (isPausing)
                    {
                        if (gametype == GAMETYPE.SINGLE)
                        {
                            Time.timeScale = 0f;
                        }
                        GameObject obj2 = GameObject.Find("UI_IN_GAME");
                        NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[0], false);
                        NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[1], true);
                        NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[2], false);
                        GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().showKeyMap();
                        GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = true;
                        Screen.showCursor = true;
                        Screen.lockCursor = false;
                    }
                }
                if (this.needSetHUD)
                {
                    this.needSetHUD = false;
                    this.setHUDposition();
                }
                if (this.inputManager.isInputDown[0x11])
                {
                    Screen.fullScreen = !Screen.fullScreen; 
                    if (Screen.fullScreen)
                    {
                        Screen.SetResolution(960, 600, false);
                    }
                    else
                    {
                        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                    }
                    this.needSetHUD = true;
                }
                if (!isPausing)
                {
                    if (this.inputManager.isInputDown[14])
                    {
                        this.reset();
                    }
                    if (this.main_object != null)
                    {
                        RaycastHit hit;
                        if (this.inputManager.isInputDown[13])
                        {
                            if (cameraMode == CAMERA_TYPE.ORIGINAL)
                            {
                                cameraMode = CAMERA_TYPE.WOW;
                                Screen.lockCursor = false;
                            }
                            else if (cameraMode == CAMERA_TYPE.WOW)
                            {
                                cameraMode = CAMERA_TYPE.TPS;
                                Screen.lockCursor = true;
                            }
                            else if (cameraMode == CAMERA_TYPE.TPS)
                            {
                                cameraMode = CAMERA_TYPE.ORIGINAL;
                                Screen.lockCursor = false;
                            }
                            else if (cameraMode == CAMERA_TYPE.FLY)
                            {
                                cameraMode = CAMERA_TYPE.ORIGINAL;
                                Screen.lockCursor = false;
                            }
                            this.verticalRotationOffset = 0f;
                        }
                        if (this.inputManager.isInputDown[0x10])
                        {
                            Screen.showCursor = !Screen.showCursor;
                        }
                        if (this.inputManager.isInputDown[9])
                        {
                            triggerAutoLock = !triggerAutoLock;
                            if (triggerAutoLock)
                            {
                                this.lockTarget = this.findNearestTitan();
                                if (this.closestDistance >= 150f)
                                {
                                    this.lockTarget = null;
                                    triggerAutoLock = false;
                                }
                            }
                        }
                        float y = base.transform.eulerAngles.y;
                        float from = base.transform.position.y;
                        float to = this.main_object.transform.position.y + this.height;
                        from = Mathf.Lerp(from, to, this.heightDamping * Time.deltaTime);
                        Quaternion quaternion = Quaternion.Euler(0f, y, 0f);
                        this.distanceOffsetMulti = (200f - base.camera.fieldOfView) / 150f;
                        float z = base.transform.eulerAngles.z;
                        if (cameraMode == CAMERA_TYPE.WOW)
                        {
                            if (Input.GetKey(KeyCode.Mouse1))
                            {
                                base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y - 50f, base.transform.eulerAngles.z), (((Mathf.Abs(Input.GetAxis("Mouse X")) <= 1f) ? Input.GetAxis("Mouse X") : Mathf.Pow(Input.GetAxis("Mouse X") * this.getSensitivityMulti(), 3f)) * this.xSpeed) * this.getSensitivityMulti());
                                y = base.transform.eulerAngles.y;
                                quaternion = Quaternion.Euler(0f, y, 0f);
                                Transform transform1 = base.transform;
                                transform1.position += (Vector3) ((((Vector3.up * Input.GetAxis("Mouse Y")) * this.ySpeed) * this.getSensitivityMulti()) * this.getReverse());
                            }
                            base.transform.position = (this.head == null) ? this.main_object.transform.position : this.head.transform.position;
                            Transform transform2 = base.transform;
                            transform2.position -= (Vector3) ((((quaternion * Vector3.forward) * this.distance) * this.distanceMulti) * this.distanceOffsetMulti);
                            Transform transform3 = base.transform;
                            transform3.position += (Vector3) ((Vector3.up * 3f) * this.heightMulti);
                            if (Input.GetKey(KeyCode.Mouse1))
                            {
                                this.verticalRotationOffset += ((Input.GetAxis("Mouse Y") * this.ySpeed) * this.getSensitivityMulti()) * this.getReverse();
                                this.verticalRotationOffset = Mathf.Min(12f, this.verticalRotationOffset);
                                this.verticalRotationOffset = Mathf.Max(-2.9f, this.verticalRotationOffset);
                                this.verticalHeightOffset = new Vector3(0f, this.verticalRotationOffset, 0f);
                            }
                            Transform transform4 = base.transform;
                            transform4.position += this.verticalHeightOffset;
                            if (this.head != null)
                            {
                                base.transform.LookAt(this.head.transform);
                            }
                            else
                            {
                                base.transform.LookAt(this.main_object.transform);
                            }
                            base.transform.localEulerAngles = new Vector3((base.transform.eulerAngles.x - 15f) + this.verticalRotationOffset, base.transform.eulerAngles.y, z);
                        }
                        else if (cameraMode == CAMERA_TYPE.ORIGINAL)
                        {
                            float num5;
                            if (Input.mousePosition.x < (Screen.width * 0.4f))
                            {
                                base.transform.position = (this.head == null) ? this.main_object.transform.position : this.head.transform.position;
                                Transform transform5 = base.transform;
                                transform5.position -= (Vector3) ((((quaternion * Vector3.forward) * this.distance) * this.distanceMulti) * this.distanceOffsetMulti);
                                Transform transform6 = base.transform;
                                transform6.position += (Vector3) (((Vector3.up * 3f) * this.heightMulti) * this.distanceOffsetMulti);
                                if (this.head != null)
                                {
                                    base.transform.LookAt(this.head.transform);
                                }
                                else
                                {
                                    base.transform.LookAt(this.main_object.transform);
                                }
                                base.transform.localEulerAngles = new Vector3((base.transform.eulerAngles.x - 10f) + (((80f * ((Screen.height * 0.6f) - Input.mousePosition.y)) / ((float) Screen.height)) * 0.5f), base.transform.eulerAngles.y, z);
                                num5 = (((Screen.width * 0.4f) - Input.mousePosition.x) / ((float) Screen.width)) * 0.4f;
                                num5 = (num5 <= 1f) ? num5 : 1f;
                                base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y - 50f, base.transform.eulerAngles.z), (num5 * 25f) * this.getSensitivityMulti());
                            }
                            else if (Input.mousePosition.x > (Screen.width * 0.6f))
                            {
                                base.transform.position = (this.head == null) ? this.main_object.transform.position : this.head.transform.position;
                                Transform transform7 = base.transform;
                                transform7.position -= (Vector3) ((((quaternion * Vector3.forward) * this.distance) * this.distanceMulti) * this.distanceOffsetMulti);
                                Transform transform8 = base.transform;
                                transform8.position += (Vector3) (((Vector3.up * 3f) * this.heightMulti) * this.distanceOffsetMulti);
                                if (this.head != null)
                                {
                                    base.transform.LookAt(this.head.transform);
                                }
                                else
                                {
                                    base.transform.LookAt(this.main_object.transform);
                                }
                                base.transform.localEulerAngles = new Vector3((base.transform.eulerAngles.x - 10f) + (((80f * ((Screen.height * 0.6f) - Input.mousePosition.y)) / ((float) Screen.height)) * 0.5f), base.transform.eulerAngles.y, z);
                                num5 = ((Input.mousePosition.x - (Screen.width * 0.6f)) / ((float) Screen.width)) * 0.6f;
                                num5 = (num5 <= 1f) ? num5 : 1f;
                                base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y - 50f, base.transform.eulerAngles.z), (-num5 * 25f) * this.getSensitivityMulti());
                            }
                            else
                            {
                                base.transform.position = (this.head == null) ? this.main_object.transform.position : this.head.transform.position;
                                Transform transform9 = base.transform;
                                transform9.position -= (Vector3) ((((quaternion * Vector3.forward) * this.distance) * this.distanceMulti) * this.distanceOffsetMulti);
                                Transform transform10 = base.transform;
                                transform10.position += (Vector3) (((Vector3.up * 3f) * this.heightMulti) * this.distanceOffsetMulti);
                                if (this.head != null)
                                {
                                    base.transform.LookAt(this.head.transform);
                                }
                                else
                                {
                                    base.transform.LookAt(this.main_object.transform);
                                }
                                base.transform.localEulerAngles = new Vector3((base.transform.eulerAngles.x - 10f) + (((80f * ((Screen.height * 0.6f) - Input.mousePosition.y)) / ((float) Screen.height)) * 0.5f), base.transform.eulerAngles.y, z);
                            }
                        }
                        else if (cameraMode == CAMERA_TYPE.TPS)
                        {
                            if (!this.inputManager.menuOn)
                            {
                                Screen.lockCursor = true;
                            }
                            base.transform.position = (this.head == null) ? this.main_object.transform.position : this.head.transform.position;
                            Transform transform11 = base.transform;
                            transform11.position += (Vector3) (Vector3.up * 3f);
                            float num7 = base.transform.localEulerAngles.y + ((Input.GetAxis("Mouse X") * 2.5f) * this.getSensitivityMulti());
                            this.rotationY += ((Input.GetAxis("Mouse Y") * 2.5f) * this.getSensitivityMulti()) * this.getReverse();
                            if (!isTPSfix)
                            {
                                this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
                                this.rotationY = Mathf.Max(this.rotationY, -50f + (this.heightMulti * 2f));
                                this.rotationY = Mathf.Min(this.rotationY, 30f);
                            }
                            else
                            {
                                this.rotationY = Mathf.Clamp(this.rotationY, -90f, 90f);
                            } 
                            base.transform.localEulerAngles = new Vector3(-this.rotationY, num7, z);
                            quaternion = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f);
                            Transform transform12 = base.transform;
                            transform12.position -= (Vector3) ((((quaternion * Vector3.forward) * this.distance) * this.distanceMulti) * this.distanceOffsetMulti);
                            Transform transform13 = base.transform;
                            transform13.position += (Vector3) ((((-Vector3.up * this.rotationY) * 0.1f) * ((float) Math.Pow((double) this.heightMulti, 1.1))) * this.distanceOffsetMulti);
                        }
                        if (triggerAutoLock && (this.lockTarget != null))
                        {
                            Transform transform = this.lockTarget.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                            Vector3 vector = transform.position - ((this.head == null) ? this.main_object.transform.position : this.head.transform.position);
                            vector.Normalize();
                            this.lockCameraPosition = (this.head == null) ? this.main_object.transform.position : this.head.transform.position;
                            this.lockCameraPosition -= (Vector3) (((vector * this.distance) * this.distanceMulti) * this.distanceOffsetMulti);
                            this.lockCameraPosition += (Vector3) (((Vector3.up * 3f) * this.heightMulti) * this.distanceOffsetMulti);
                            base.transform.position = Vector3.Lerp(base.transform.position, this.lockCameraPosition, Time.deltaTime * 4f);
                            if (this.head != null)
                            {
                                base.transform.LookAt((Vector3) ((this.head.transform.position * 0.8f) + (transform.position * 0.2f)));
                            }
                            else
                            {
                                base.transform.LookAt((Vector3) ((this.main_object.transform.position * 0.8f) + (transform.position * 0.2f)));
                            }
                            base.transform.localEulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, z);
                            Vector2 vector2 = base.camera.WorldToScreenPoint(transform.position - ((Vector3) (transform.forward * this.lockTarget.transform.localScale.x)));
                            GameObject.Find("locker").transform.localPosition = new Vector3(vector2.x - (Screen.width * 0.5f), vector2.y - (Screen.height * 0.5f), 0f);
                            if ((this.lockTarget.GetComponent<TITAN>() != null) && this.lockTarget.GetComponent<TITAN>().hasDie)
                            {
                                this.lockTarget = null;
                            }
                        }
                        else
                        {
                            GameObject.Find("locker").transform.localPosition = new Vector3(0f, (-Screen.height * 0.5f) - 50f, 0f);
                        }
                        Vector3 end = (this.head == null) ? this.main_object.transform.position : this.head.transform.position;
                        Vector3 vector39 = ((this.head == null) ? this.main_object.transform.position : this.head.transform.position) - base.transform.position;
                        Vector3 normalized = vector39.normalized;
                        end -= (Vector3) ((this.distance * normalized) * this.distanceMulti);
                        LayerMask mask = ((int) 1) << LayerMask.NameToLayer("Ground");
                        LayerMask mask2 = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
                        LayerMask mask3 = mask | mask2;
                        if (!(cameraMode == CAMERA_TYPE.FLY))
                        {
                            if (this.head != null)
                            {
                                if (Physics.Linecast(this.head.transform.position, end, out hit, (int)mask))
                                {
                                    base.transform.position = hit.point;
                                }
                                else if (Physics.Linecast(this.head.transform.position - ((Vector3)((normalized * this.distanceMulti) * 3f)), end, out hit, (int)mask2))
                                {
                                    base.transform.position = hit.point;
                                }
                                Debug.DrawLine(this.head.transform.position - ((Vector3)((normalized * this.distanceMulti) * 3f)), end, Color.red);
                            }
                            else if (Physics.Linecast(this.main_object.transform.position + Vector3.up, end, out hit, (int)mask3))
                            {
                                base.transform.position = hit.point;
                            }
                        }
                        this.shakeUpdate();
                    }
                    if (cameraMode == CAMERA_TYPE.FLY)
                    {
                        Vector3 dp = Vector3.zero;
                        if (this.inputManager.isInput[0]) dp.z = 1;
                        if (this.inputManager.isInput[1]) dp.z = -1;
                        if (this.inputManager.isInput[2]) dp.x = -1;
                        if (this.inputManager.isInput[3]) dp.x = 1;
                        if (this.inputManager.isInput[10]) dp.y = -1;
                        if (Input.GetKey(KeyCode.Space)) dp.y = 1;
                        var speed = normalSpeed * (Input.GetKey(KeyCode.LeftShift) ? highSpeed : 1);

                        dp.Normalize();
                        dp *= speed * Time.deltaTime;
                        camera.transform.Translate(dp.x, dp.y, dp.z, Space.Self);

                        float rotY = rotationSpeed * Input.GetAxis("Mouse X");
                        float rotX = -rotationSpeed * Input.GetAxis("Mouse Y");

                        float toTop = Vector3.Angle(camera.transform.forward, Vector3.up);
                        if (rotX < 0)
                            rotX = -Mathf.Min(Mathf.Abs(rotX), Mathf.Max(0, toTop - 20));
                        else
                            rotX = Mathf.Min(Mathf.Abs(rotX), Mathf.Max(0, 160 - toTop));
                        camera.transform.RotateAround(Vector3.up, rotY);
                        camera.transform.RotateAround(camera.transform.right, rotX);

                    }
                }
            }
        }
    }

    public enum RotationAxes
    {
        MouseXAndY,
        MouseX,
        MouseY
    }
}

