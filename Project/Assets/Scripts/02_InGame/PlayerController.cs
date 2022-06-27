using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private float speed = 5f;
    private float run = 2.3f;
    private float jump = 9f;
    private float currentDistance = 3.5f;
    [SerializeField]
    private float normalDistance = 3.5f;
    [SerializeField]
    private float runDistance = 5.6f;
    [SerializeField]
    private Vector3 offset = new Vector3(1f, 4f, 0);

    private Transform camParent;
    private Transform cam;

    private CharacterController controller;
    private Animator anim;

    private float rememberAngleX, rememberAngleY;
    private float currentAngleX, currentAngleY;
    private float targetAngleX, targetAngleY;
    private float gravityY;
    private Vector3 moveDir;
    private float horizontal, veritcal;

    private float airTimer = .0f;

    private bool isAround = false;

    private bool punching = false;

    private bool isDead = false;
    private bool isWatcher = false;
    private bool isMurder = false;
    private int playerIndex = 0;
    private Transform targetPlayer;

    private bool workabled = false;
    private bool isWorking = false;

    private float slowTimer = 3.0f;
    private float slowEndTime = 3.0f;
    private bool isSlow = false;

    private bool killable = true;
    private float killCooldown = .0f;
    private float killTimer = .0f;

    private void Awake()
    {
        speed = (float)PhotonNetwork.CurrentRoom.CustomProperties["moveSpeed"];
        isMurder = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"];
        killCooldown = (float)PhotonNetwork.CurrentRoom.CustomProperties["killCooldown"];
        camParent = Camera.main.transform.parent;
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        EventManager.AddEvent("Player :: Die", (p) =>
        {
            anim.SetBool("isWorkTree", false);
            anim.SetBool("isWorkWater", false);
            anim.SetBool("isWorkFish", false);
            isWorking = false;
            EventManager.SendEvent("InGameUI :: WorkEnd");
            if (p[0].GetType() == typeof(Player))
            {
                SetDie((Player)p[0]);
            }
            else if(p[0].GetType() == typeof(DieMessage))
            {
                SetDie((DieMessage)p[0]);
            }
        });
        EventManager.AddEvent("Player :: WorkStart", (p) =>
        {
            WorkMessage msg = (WorkMessage)p[0];
            switch (msg)
            {
                case WorkMessage.Treezone:
                    anim.SetBool("isWorkTree", true);
                    anim.SetTrigger("SetWork");
                    break;
                case WorkMessage.WaterZone:
                    anim.SetBool("isWorkWater", true);
                    anim.SetTrigger("SetWork");
                    break;
                case WorkMessage.FishZone:
                    anim.SetBool("isWorkFish", true);
                    anim.SetTrigger("SetWork");
                    break;
                default:
                    break;
            }
            isWorking = true;
        });



        EventManager.AddEvent("Player :: WorkSuccess", (p) =>
        {
            WorkMessage msg = (WorkMessage)p[0];
            switch (msg)
            {
                case WorkMessage.Treezone:
                    anim.SetBool("isWorkTree", false);
                    EventManager.SendEvent("Inventory :: AddItem", "0007", Random.Range(1, 3));
                    break;
                case WorkMessage.WaterZone:
                    anim.SetBool("isWorkWater", false);
                    EventManager.SendEvent("Inventory :: Change", "0002", "0003", false);
                    break;
                case WorkMessage.FishZone:
                    anim.SetBool("isWorkFish", false);
                    EventManager.SendEvent("Inventory :: AddItem", "0000", 1);
                    break;
                default:
                    break;
            }
            isWorking = false;
        });
        EventManager.AddEvent("Player :: WorkEnd", (p) =>
        {
            anim.SetBool("isWorkTree", false);
            anim.SetBool("isWorkWater", false);
            anim.SetBool("isWorkFish", false);
            isWorking = false;
        });
    }

    private void Start()
    {
        EventManager.SendEvent("Inventory :: AddItem", "0003", 3);
    }

    void Update()
    {
        if (!isDead)
        {
            if (isWorking)
            {
                WorkingCamera();
            }
            else
            {
                CameraRotate();
                PlayerSlow();
                PlayerMove();
                PlayerAttack();
            }
        }
        else
        {
            if (isWatcher)
            {
                WatcherCameraRotate();
                NextPlayer();
            }
            else
            {
                PlayerDieCamera();
                NextPlayer();
            }
        }
        if (isMurder)
        {
            if (!killable)
            {
                KillCooldownUpdate();
            }
        }
    }

    private void KillCooldownUpdate()
    {
        killTimer += Time.deltaTime;
        EventManager.SendEvent("InGameUI :: SetKillCooldown", killTimer, killTimer >= killCooldown);
        if (killTimer >= killCooldown)
        {
            killable = true;
        }
    }

    private void PlayerSlow()
    {
        slowTimer += Time.deltaTime;
        if (slowTimer >= slowEndTime)
        {
            slowTimer = slowEndTime;
        }
        isSlow = slowTimer < slowEndTime;
    }

    private void WorkingCamera() 
    {
        Quaternion targetAngle = Quaternion.Euler(40, currentAngleY, 0);
        Vector3 targetPosition = transform.position + Quaternion.Euler(0, currentAngleY, 0) * new Vector3(0, 3, 0);
        Vector3 targetCamPosition = -Vector3.forward * 7;

        float sensitivity = Time.deltaTime * 3;

        camParent.rotation = Quaternion.Lerp(camParent.rotation, targetAngle, sensitivity);
        camParent.position = Vector3.Lerp(camParent.position, targetPosition, sensitivity);

        cam.localPosition = Vector3.Lerp(cam.localPosition, targetCamPosition, sensitivity);

    }

    private void NextPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.WatchNextPlayer)))
        {
        ReFind:
            if (++playerIndex >= players.Length) playerIndex = 0;
            if (SetTargetWatch(players)) goto ReFind;
        }
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.WatchPreviewPlayer)))
        {
        ReFind:
            if (--playerIndex < 0) playerIndex = players.Length - 1;
            if (SetTargetWatch(players)) goto ReFind;
        }
    }

    private bool SetTargetWatch(GameObject[] players)
    {
        if (!isWatcher)
        {
            EventManager.SendEvent("InGameUI :: WatchUI");
            isWatcher = true;
        }
        targetPlayer = players[playerIndex].transform;
        EventManager.SendEvent("InGameUI :: WatchPlayer", targetPlayer);
        return (bool)targetPlayer.GetComponent<PhotonView>().Owner.CustomProperties["isDead"];
    }

    private void WatcherCameraRotate()
    {
        targetAngleX -= Input.GetAxis("Mouse Y");
        targetAngleX = Mathf.Clamp(targetAngleX, -90, 90);

        targetAngleY += Input.GetAxis("Mouse X");

        currentAngleX = Mathf.Lerp(currentAngleX, targetAngleX, Time.deltaTime * 30f);
        currentAngleY = Mathf.Lerp(currentAngleY, targetAngleY, Time.deltaTime * 30f);

        if (targetPlayer == null) NextPlayer();
        camParent.position = targetPlayer.position + Quaternion.Euler(0, currentAngleY, 0) * offset;
        cam.localRotation = Quaternion.identity;
        cam.localPosition = -Vector3.forward * currentDistance;

        camParent.rotation = Quaternion.Euler(currentAngleX, currentAngleY, 0);
    }

    private void PlayerDieCamera()
    {
        cam.position = Vector3.Lerp(cam.position, transform.position + Vector3.up * 20f, Time.deltaTime * 3f);
        cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(90, 0, 0), Time.deltaTime * 3f);
    }

    private void CameraRotate()
    {
        bool isRun = Input.GetKey(Settings.instance.GetKey(KeySettings.RunKey));
        Debug.DrawLine(camParent.position, camParent.position + -cam.forward * (isRun ? runDistance : normalDistance), Color.red);
        if (Physics.Raycast(camParent.position, -cam.forward, out RaycastHit hit, isRun ? runDistance : normalDistance, ~(1 << gameObject.layer), QueryTriggerInteraction.Ignore))
        {
            currentDistance = hit.distance;
        }
        else
        {
            if (isRun)
            {
                currentDistance = Mathf.Lerp(currentDistance, runDistance, Time.deltaTime * 30f);
            }
            else
            {
                currentDistance = Mathf.Lerp(currentDistance, normalDistance, Time.deltaTime * 30f);
            }
        }

        if (!isAround && Input.GetKey(Settings.instance.GetKey(KeySettings.AroundKey)))
        {
            rememberAngleX = targetAngleX;
            rememberAngleY = targetAngleY;
        }
        
        if (isAround && !Input.GetKey(Settings.instance.GetKey(KeySettings.AroundKey)))
        {
            targetAngleX = rememberAngleX;
            targetAngleY = rememberAngleY % 360;
        }
        isAround = Input.GetKey(Settings.instance.GetKey(KeySettings.AroundKey));

        targetAngleX -= Input.GetAxis("Mouse Y");
        targetAngleX = Mathf.Clamp(targetAngleX, -90, 90);
        
        targetAngleY += Input.GetAxis("Mouse X");

        currentAngleX = Mathf.Lerp(currentAngleX, targetAngleX, Time.deltaTime * 30f);
        currentAngleY = Mathf.Lerp(currentAngleY, targetAngleY, Time.deltaTime * 30f);

        camParent.position = transform.position + Quaternion.Euler(0, currentAngleY, 0) * offset;
        cam.localPosition = -Vector3.forward * currentDistance;

        camParent.rotation = Quaternion.Euler(currentAngleX, currentAngleY, 0);
        //cam.position = transform.position + Quaternion.Euler(currentAngleX, currentAngleY, 0) * -Vector3.forward * distance + Quaternion.Euler(targetAngleX, targetAngleY, 0) * offset;
        transform.rotation = Quaternion.Euler(0, isAround ? rememberAngleY : currentAngleY, 0);
    }

    private void PlayerMove()
    {
        float delta = Time.deltaTime;
        Vector3 point1 = controller.bounds.center + Vector3.up * (controller.height - controller.radius * 2) / 2;
        Vector3 point2 = controller.bounds.center - Vector3.up * (controller.height - controller.radius * 2) / 2;
        if (controller.isGrounded || Physics.CapsuleCast(point1, point2, controller.radius, Vector3.down, .02f, ~(1 << gameObject.layer)))
        {
            float sensitivity = 3f;

            bool isRun = Input.GetKey(Settings.instance.GetKey(KeySettings.RunKey)) && isMurder;
            bool forward = Input.GetKey(Settings.instance.GetKey(KeySettings.ForwardKey));
            bool backward = Input.GetKey(Settings.instance.GetKey(KeySettings.BackwardKey));
            bool right = Input.GetKey(Settings.instance.GetKey(KeySettings.RightKey));
            bool left = Input.GetKey(Settings.instance.GetKey(KeySettings.LeftKey));

            if (forward) veritcal += delta * sensitivity;
            if (backward) veritcal -= delta * sensitivity;
            if (right) horizontal += delta * sensitivity;
            if (left) horizontal -= delta * sensitivity;

            if (right == left)
            {
                if (horizontal > 0)
                {
                    if (horizontal - delta * sensitivity < 0)
                    {
                        horizontal = 0;
                    }
                    else
                    {
                        horizontal -= delta * sensitivity;
                    }
                }
                else
                {
                    if (horizontal + delta * sensitivity > 0)
                    {
                        horizontal = 0;
                    }
                    else
                    {
                        horizontal += delta * sensitivity;
                    }
                }
            }
            if (forward == backward)
            {
                if (veritcal > 0)
                {
                    if (veritcal - delta * sensitivity <= 0)
                    {
                        veritcal = 0;
                    }
                    else
                    {
                        veritcal -= delta * sensitivity;
                    }
                }
                else
                {
                    if (veritcal + delta * sensitivity >= 0)
                    {
                        veritcal = 0;
                    }
                    else
                    {
                        veritcal += delta * sensitivity;
                    }
                }
            }

            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
            veritcal = Mathf.Clamp(veritcal, -1f, 1f);
            if (gravityY < -6f)
            {
                Debug.Log(gravityY + 6f);
            }
            gravityY = .0f;
            anim.SetBool("Air", false);

            if (Input.GetKey(KeyCode.Space))
            {
                anim.SetBool("Air", true);
                gravityY = jump;
            }

            moveDir = Quaternion.Euler(0, isAround ? rememberAngleY : currentAngleY, 0) * new Vector3(horizontal, 0, veritcal);
            moveDir *= isSlow ? .5f : 1f;

            if (moveDir.magnitude > 1) moveDir.Normalize();
            if (isRun) moveDir *= run;

            float angle = Vector3.Angle(transform.forward, moveDir);
            angle *= horizontal < 0 ? -1 : 1;

            bool moveForward, moveBackward, moveRight, moveLeft;

            moveForward = CheckAngleInRange(angle, -45f, 45f);
            moveBackward = angle < -135f || angle >= 135f;
            moveRight = CheckAngleInRange(angle, 45f, 135f);
            moveLeft = CheckAngleInRange(angle, -135f, -45f);

            anim.SetBool("Forward", moveForward);
            anim.SetBool("Back", moveBackward);
            anim.SetBool("Right", moveRight);
            anim.SetBool("Left", moveLeft);

            anim.SetFloat("Speed", moveDir.magnitude);

            moveDir *= speed;
            airTimer = .0f;
        }
        else
        {
            airTimer += delta;
        }


        gravityY += Physics.gravity.y * delta;
        moveDir.y = gravityY;

        controller.Move(moveDir * delta);

        if (airTimer > .8f)
        {
            if (!anim.GetBool("Air"))
            {
                anim.SetBool("Air", true);
            }
        }
    }

    private void PlayerAttack()
    {
        if (Input.GetMouseButtonDown(0) && !punching)
        {
            StartPunch();
            if (Random.Range(0f, 100f) < 50f)
            {
                anim.SetTrigger("PunchLeft");
            }
            else
            {
                anim.SetTrigger("PunchRight");
            }
        }
    }

    public void SetPunch()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position + controller.center, Vector3.one * .5f, Quaternion.Euler(currentAngleX, currentAngleY, 0) * Vector3.forward, Quaternion.identity, 2.5f, 1 << gameObject.layer);
        foreach (var item in hits)
        {
            if (item.collider.gameObject.CompareTag("Player"))
            {
                PhotonView view = item.collider.GetComponent<PhotonView>();
                if (!view.IsMine)
                {
                    if (!(bool)view.Owner.CustomProperties["isMurder"] && !(bool)view.Owner.CustomProperties["isDead"])
                    {
                        killable = false;
                        killTimer = .0f;
                        view.RPC("Hit", view.Owner, GetComponent<PhotonView>().Owner);
                        break;
                    }
                }
            }
        }
    }
    public void StartPunch() => punching = true;
    public void EndPunch() => punching = false;

    public void SetDie(Player player)
    {
        isDead = true;
        anim.SetBool("isDead", true);
        anim.SetTrigger("Die");
        EventManager.SendEvent("InGameUI :: SetDie", player);
    }

    public void SetDie(DieMessage msg)
    {
        isDead = true;
        anim.SetBool("isDead", true);
        anim.SetTrigger("Die");
        EventManager.SendEvent("InGameUI :: Die", msg);
    }

    public void NoDie()
    {
        isDead = false;
    }

    private bool CheckAngleInRange(float value, float min, float max)
    {
        return value >= min && value < max;
    }

    private void OnTriggerEnter(Collider other)
    {
        EventManager.SendEvent("InGameUI :: TriggerEnter", other);
    }

    private void OnTriggerExit(Collider other)
    {
        EventManager.SendEvent("InGameUI :: TriggerExit", other);
    }
}