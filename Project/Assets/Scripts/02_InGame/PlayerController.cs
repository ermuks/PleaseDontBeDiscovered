using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private float speed = 5f;
    private float run = 1.8f;
    private float walk = .7f;
    private float jump = 11f;
    private float currentDistance = 2f;

    [SerializeField]
    private float normalDistance = 2f;
    [SerializeField]
    private float runDistance = 3.5f;
    [SerializeField]
    private Vector3 offset = new Vector3(.8f, 3.6f, 0);

    private Transform camParent;
    private Transform cam;

    private GameObject aliveObject;

    private CharacterController controller;
    private Animator anim;

    private float rememberAngleX, rememberAngleY;
    private float currentAngleX, currentAngleY;
    private float targetAngleX, targetAngleY;
    private float gravityY;

    float sensitivity = 3f;
    float negativeSensitivity => sensitivity * 3f;

    private Vector3 moveDir;
    private float horizontal, vertical;

    private float jumpTimer = .4f;
    private float jumpDelay = .4f;

    private float airTimer = .0f;

    private bool isAround = false;

    private bool punching = false;

    private bool isDead = false;
    private bool isWatcher = false;
    private bool isMurder = false;
    private int playerIndex = 0;
    private Transform targetPlayer;

    private bool isWorking = false;

    private float slowTimer = 3.0f;
    private float slowEndTime = 3.0f;
    private bool isSlow = false;

    private bool killable = false;
    private float killCooldown = .0f;
    private float killTimer = .0f;

    private bool isFallingDamage = false;
    private bool isRunable = false;

    private bool isWater = false;

    private bool canBreath = true;
    private float breathHoldTimer = .0f;
    private float breathHoldMaximum = 20f;

    public Vector3 hitNormal;
    public float hitAngle;

    private void Awake()
    {
        speed = (float)PhotonNetwork.CurrentRoom.CustomProperties["moveSpeed"];
        isMurder = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"];
        killCooldown = (float)PhotonNetwork.CurrentRoom.CustomProperties["killCooldown"];
        isFallingDamage = (bool)PhotonNetwork.CurrentRoom.CustomProperties["fallingDamage"];
        isRunable = (bool)PhotonNetwork.CurrentRoom.CustomProperties["runable"];

        killTimer = killCooldown - 10;

        camParent = Camera.main.transform.parent;
        cam = Camera.main.transform;
        aliveObject = transform.Find("AliveObject").gameObject;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        EventManager.AddEvent("Player :: Die", (p) =>
        {
            EventManager.SendEvent("Player :: WorkEnd");
            EventManager.SendEvent("InGameUI :: CloseInventory");
            if (p[0].GetType() == typeof(Player))
            {
                SetDie((Player)p[0]);
            }
            else if (p[0].GetType() == typeof(DieMessage))
            {
                SetDie();
            }
        });
        EventManager.AddEvent("Player :: VoteDie", (p) =>
        {
            EventManager.SendEvent("Player :: WorkEnd");
            EventManager.SendEvent("Player :: SetWatching");
            SetDie();
        });
        EventManager.AddEvent("Player :: WorkStart", (p) =>
        {
            WorkMessage msg = (WorkMessage)p[0];
            switch (msg)
            {
                case WorkMessage.Treezone:
                    anim.SetBool("isWorkTree", true);
                    break;
                case WorkMessage.WaterZone:
                    anim.SetBool("isWorkWater", true);
                    break;
                case WorkMessage.FishZone:
                    anim.SetBool("isWorkFish", true);
                    break;
                default:
                    break;
            }
            isWorking = true;
        });



        EventManager.AddEvent("Player :: WorkSuccess", (p) =>
        {
            isWorking = false;
            WorkMessage msg = (WorkMessage)p[0];
            Collider col = (Collider)p[1];
            switch (msg)
            {
                case WorkMessage.Treezone:
                    anim.SetBool("isWorkTree", false);
                    EventManager.SendEvent("Inventory :: AddItem", "0007", Random.Range(1, 3));
                    break;
                case WorkMessage.WaterZone:
                    anim.SetBool("isWorkWater", false);
                    EventManager.SendEvent("Inventory :: Change", "0002", "0003");
                    break;
                case WorkMessage.FishZone:
                    anim.SetBool("isWorkFish", false);
                    EventManager.SendEvent("Inventory :: AddItem", "0000", 1);
                    break;
                case WorkMessage.Inventory:
                    EventManager.SendEvent("InGameUI :: OpenInventory", col);
                    isWorking = true;
                    break;
                case WorkMessage.CampFire:
                    EventManager.SendEvent("Inventory :: Change", "0000", "0001");
                    break;
                default:
                    break;
            }
        });
        EventManager.AddEvent("Player :: WorkEnd", (p) =>
        {
            anim.SetBool("isWorkTree", false);
            anim.SetBool("isWorkWater", false);
            anim.SetBool("isWorkFish", false);
            isWorking = false;
            EventManager.SendEvent("InGameUI :: WorkEnd");
        });
        EventManager.AddEvent("Player :: SetWatching", (p) => isWatcher = true);
        EventManager.AddEvent("Player :: PlayerDieToWatch", (p) =>
        {
            NextPlayer(true);
            EventManager.SendEvent("Player :: WorkEnd");
            aliveObject.SetActive(false);
        });
        EventManager.AddEvent("Player :: CanBreath", (p) =>
        {
            canBreath = true;
        });
        EventManager.AddEvent("Player :: CantBreath", (p) =>
        {
            canBreath = false;
        });
        EventManager.AddEvent("Player :: EndGame", (p) =>
        {
            GameObject spawnArea;
            if ((bool)p[0] == isMurder) {
                spawnArea =
                isMurder ?
                GameObject.FindGameObjectWithTag("MurdererSpawnZone") :
                GameObject.FindGameObjectWithTag("PlayerSpawnZone");
                Bounds bounds = spawnArea.GetComponent<Collider>().bounds;

                float x = bounds.extents.x;
                float y = bounds.extents.y;
                float z = bounds.extents.z;
                controller.enabled = false;
                transform.position = bounds.center + new Vector3(Random.Range(-x, x), Random.Range(-y, y), Random.Range(-z, z));
                controller.enabled = true;
            }
        });
    }

    private void Start()
    {
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["startItem"])
        {
            ItemManager.SetItemRandom();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            isWorking = false;
        }
        if ((bool)EventManager.GetData("InGameUI >> VoteUIActive")) return;
        if ((bool)EventManager.GetData("InGameData >> FinishVoteAnimationPlaying"))
        {
            anim.SetBool("Forward", false);
            anim.SetBool("Back", false);
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);

            anim.SetFloat("Speed", 0);
            CameraPositionVoteEnding();
            return;
        }
        if (!isDead)
        {
            if (isWorking)
            {
                horizontal = .0f;
                vertical = .0f;
                moveDir = Vector3.zero;
                anim.SetBool("Forward", false);
                anim.SetBool("Back", false);
                anim.SetBool("Right", false);
                anim.SetBool("Left", false);

                anim.SetFloat("Speed", 0);
                WorkingCamera();
            }
            else
            {
                CameraRotate();
                PlayerMove();
                PlayerAttack();
            }
            if (!isMurder)
            {
                BreathTimer();
            }
            PlayerSlow();
            PlayerJumpTimer();
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

    private void BreathTimer()
    {
        if (canBreath)
        {
            if (breathHoldTimer >= breathHoldMaximum) breathHoldTimer = breathHoldMaximum;
            breathHoldTimer -= Time.deltaTime;
            if (breathHoldTimer <= 0) breathHoldTimer = 0;
        }
        else
        {
            breathHoldTimer += Time.deltaTime;
            if (breathHoldTimer >= breathHoldMaximum)
            {
                EventManager.SendEvent("Player :: BreathDamage", Time.deltaTime * .07f);
                breathHoldTimer = breathHoldMaximum;
            }
            EventManager.SendEvent("Refresh Breath", breathHoldTimer / breathHoldMaximum);
        }
    }

    private void CameraPositionVoteEnding()
    {
        Transform voteEnding = (Transform)EventManager.GetData("InGameUI >> VoteEndingPosition");
        camParent.position = voteEnding.position;
        camParent.rotation = voteEnding.rotation;
        cam.localRotation = Quaternion.identity;
        cam.localPosition = Vector3.one;
    }

    private void KillCooldownUpdate()
    {
        killTimer += Time.deltaTime;
        EventManager.SendEvent("InGameUI :: SetKillCooldown", killTimer, killCooldown, killTimer >= killCooldown);
        if (killTimer >= killCooldown)
        {
            killable = true;
        }
    }

    private void PlayerJumpTimer()
    {
        jumpTimer += Time.deltaTime;
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

    private void NextPlayer(bool auto = false)
    {
        if (auto || Input.GetKeyDown(Settings.instance.GetKey(KeySettings.WatchNextPlayer)))
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        ReFind:
            if (++playerIndex >= players.Length) playerIndex = 0;
            if (SetTargetWatch(players))
            {
                goto ReFind;
            }
        }
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.WatchPrevPlayer)))
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        ReFind:
            if (--playerIndex < 0) playerIndex = players.Length - 1;
            if (SetTargetWatch(players))
            {
                goto ReFind;
            }
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


        while (targetPlayer == null)
        {
            NextPlayer();
        }
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
        cam.localRotation = Quaternion.identity;
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
        //~(1 << gameObject.layer), QueryTriggerInteraction.Ignore

        bool isRun = Input.GetKey(Settings.instance.GetKey(KeySettings.RunKey)) && isMurder;
        bool isWalk = Input.GetKey(Settings.instance.GetKey(KeySettings.WalkKey));
        bool forward = Input.GetKey(Settings.instance.GetKey(KeySettings.ForwardKey));
        bool backward = Input.GetKey(Settings.instance.GetKey(KeySettings.BackwardKey));
        bool right = Input.GetKey(Settings.instance.GetKey(KeySettings.RightKey));
        bool left = Input.GetKey(Settings.instance.GetKey(KeySettings.LeftKey));

        if (right == left)
        {
            if (horizontal > 0)
            {
                if (horizontal - delta * negativeSensitivity < 0)
                {
                    horizontal = 0;
                }
                else
                {
                    horizontal -= delta * negativeSensitivity;
                }
            }
            else
            {
                if (horizontal + delta * negativeSensitivity > 0)
                {
                    horizontal = 0;
                }
                else
                {
                    horizontal += delta * negativeSensitivity;
                }
            }
        }
        if (forward == backward)
        {
            if (vertical > 0)
            {
                if (vertical - delta * negativeSensitivity <= 0)
                {
                    vertical = 0;
                }
                else
                {
                    vertical -= delta * negativeSensitivity;
                }
            }
            else
            {
                if (vertical + delta * negativeSensitivity >= 0)
                {
                    vertical = 0;
                }
                else
                {
                    vertical += delta * negativeSensitivity;
                }
            }
        }

        hitAngle = Vector3.Angle(Vector3.up, hitNormal);
        if (Physics.CapsuleCast(point1, point2, controller.radius, Vector3.down, .02f, ~(1 << gameObject.layer), QueryTriggerInteraction.Ignore))
        //if ()//Physics.Raycast(controller.bounds.center, Vector3.down, controller.bounds.extents.y + .42f, ~(1 << gameObject.layer), QueryTriggerInteraction.Ignore))
        {
            if (hitAngle <= controller.slopeLimit)
            {
                if (forward) vertical += delta * sensitivity;
                if (backward) vertical -= delta * sensitivity;
                if (right) horizontal += delta * sensitivity;
                if (left) horizontal -= delta * sensitivity;

                horizontal = Mathf.Clamp(horizontal, -.7f, .7f);
                vertical = Mathf.Clamp(vertical, -.5f, 1f);
                anim.SetBool("Air", false);

                if (gravityY < -10f && isFallingDamage)
                {
                    EventManager.SendEvent("InGameUI :: Hurt");
                    EventManager.SendEvent("Player :: FallingDamage", gravityY + 10f);
                    slowTimer = .0f;
                }
                gravityY = .0f;
                if (!isWater && Input.GetKey(Settings.instance.GetKey(KeySettings.JumpKey)) && jumpTimer >= jumpDelay)
                {
                    jumpTimer = .0f;
                    anim.SetBool("Air", true);
                    gravityY = jump;
                }

                moveDir = Quaternion.Euler(0, isAround ? rememberAngleY : currentAngleY, 0) * new Vector3(horizontal, 0, vertical);
                moveDir *= isSlow ? .5f : 1f;

                if (moveDir.magnitude > 1) moveDir.Normalize();
                if (isRun) moveDir *= run;
                if (isWalk) moveDir *= walk;

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
                if (!isWater && Input.GetKey(Settings.instance.GetKey(KeySettings.JumpKey)) && jumpTimer >= jumpDelay)
                {
                    jumpTimer = .0f;
                    anim.SetBool("Air", true);
                    gravityY = jump * (1f - hitNormal.y);
                    moveDir.x += (1f - hitNormal.y) * hitNormal.x * jump / 3f;
                    moveDir.z += (1f - hitNormal.y) * hitNormal.z * jump / 3f;
                }
                moveDir.x = Mathf.Lerp(moveDir.x, (1f - hitNormal.y) * hitNormal.x * speed / 2f, Time.deltaTime * 12f);
                moveDir.z = Mathf.Lerp(moveDir.z, (1f - hitNormal.y) * hitNormal.z * speed / 2f, Time.deltaTime * 12f);
            }
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
                anim.SetBool("PunchLeft", true);
            }
            else
            {
                anim.SetBool("PunchRight", true);
            }
            SetPunch();
        }
    }

    private void PunchEnd()
    {
        anim.SetBool("PunchLeft", false);
        anim.SetBool("PunchRight", false);
    }

    private void HitEnd()
    {
        anim.SetBool("Hit", false);
    }

    public void SetPunch()
    {
        if (killable)
        {
            RaycastHit[] hits = Physics.BoxCastAll(transform.position + controller.center, Vector3.one * .5f, Quaternion.Euler(currentAngleX, currentAngleY, 0) * Vector3.forward, Quaternion.identity, 2.5f, 1 << gameObject.layer, QueryTriggerInteraction.Ignore);
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
                            EventManager.SendEvent("PUN :: Hit", GetComponent<PhotonView>().Owner, view.Owner);
                            break;
                        }
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
        EventManager.SendEvent("InGameUI :: SetDie", player);
    }

    public void SetDie()
    {
        isDead = true;
        anim.SetBool("isDead", true);
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
        if (other.gameObject == GetComponent<PlayerData>().ReportArea) return;
        EventManager.SendEvent("InGameUI :: TriggerEnter", other);
        if (other.CompareTag("DeepWater"))
        {
            isWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EventManager.SendEvent("InGameUI :: TriggerExit", other);
        if (other.CompareTag("DeepWater"))
        {
            Debug.Log(other.name, other.gameObject);
            isWater = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }
}