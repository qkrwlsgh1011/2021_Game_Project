using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyPerfect;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    public FadeEffect fade;

    [SerializeField]
    public GameManager timepast;

    [SerializeField]
    private Inventory inven;

    Animator characterAnimator;
    public Animator bowAnimator;
    CharacterController characterController;
    Camera _camera;
    GameObject nearObject;
    Common_WanderScript wanderScript;
    PlayerAllyMonsterList allyMonsterList;
    public SaveNLoad theSaveNLoad;

    public float speed = 5f;
    public float runSpeed = 8f;
    float finalSpeed;
    bool run;

    bool toggleCameraRotation;
    public float cameraSmoothness = 10f;

    public GameObject[] weapons;
    List<string> weaponList = new List<string>();
    public bool isEquipSword = false;
    public bool isEquipBow = false;
    bool isAttackDelay = false;
    public float closeAttackRange = 2f;
    public float wideAttackRange = 5f;
    public float closeAttackDamage = 10f;
    public float wideAttackDamage = 5f;

    public float hunger = 100f;
    private float foodTimer;
    private float hungerTimer;
    public float foodTimerValue = 180f;
    public float hungerTimerValue = 5f;


    public float fatigue = 0f;
    private float fatigueTimer;
    public float fatigueMax = 100f;
    private float fatigueTimerValue = 5f;

    public float tameAndFeedRange = 2f;

    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        _camera = Camera.main;
        characterController = GetComponent<CharacterController>();
        wanderScript = GetComponent<Common_WanderScript>();
        allyMonsterList = GetComponent<PlayerAllyMonsterList>();

        foodTimer = foodTimerValue;
        hungerTimer = hungerTimerValue;

        fatigueTimer = fatigueTimerValue;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
            toggleCameraRotation = true;    //둘러보기 활성화 
        else
            toggleCameraRotation = false;   //둘러보기 비활성화

        if (Input.GetKey(KeyCode.LeftShift))
            run = true;
        else
            run = false;

        SetMovement();

        if (Input.GetButtonDown("Interaction"))
        {
            InteractItem();
        }

        if (Input.GetButtonDown("Tame and Feed"))
        {
            TameAndFeed();
        }

        wanderScript.checkPrey();

        if (Input.GetMouseButton(0) && (isEquipSword || isEquipBow))
        {
            BeginAttackAnim();

            if (!isAttackDelay)
            {
                isAttackDelay = true;
                Attack();
                StartCoroutine(CountAttackDelay());
            }
        }
        else if (!isAttackDelay)
        {
            StopAttackAnim();
        }

        if (!isAttackDelay && (Input.GetButtonDown("Swap1") || Input.GetButtonDown("Swap2")))
        {
            SwapWeapon();
        }

        foodTimer = Mathf.MoveTowards(foodTimer, 0f, Time.deltaTime);
        if (foodTimer == 0f)
            HungerDecrease();

        fatigue = Mathf.MoveTowards(fatigue, fatigueMax, Time.deltaTime);
        if (fatigue == fatigueMax)
            IsFatigue();
    }

    private void LateUpdate()
    {
        if (toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * cameraSmoothness);
        }

        if (Input.GetKeyDown(KeyCode.K))
            theSaveNLoad.LoadData();
    }

    void SetMovement()
    {
        finalSpeed = (run) ? runSpeed : speed;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        characterController.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);

        float percent = ((run) ? 1 : 0.5f) * moveDirection.magnitude;
        characterAnimator.SetFloat("move", percent, 0.1f, Time.deltaTime);
    }

    void BeginAttackAnim()
    {
        if (isEquipSword)   //칼을 들고 있을 때
        {
            characterAnimator.SetBool("sword_attack", true); //칼 attack 애니메이션 실행
        }
        else if (isEquipBow)    //활을 들고 있을 때
        {
            characterAnimator.SetBool("bow_attack", true);   //활 charge 애니메이션 실행
            bowAnimator.SetBool("bow_attack", true);
        }
    }

    void StopAttackAnim()
    {
        if (isEquipSword)
        {
            characterAnimator.SetBool("sword_attack", false);
        }
        else if (isEquipBow)
        {
            characterAnimator.SetBool("bow_attack", false);
            bowAnimator.SetBool("bow_attack", false);
        }
    }

    void Attack()
    {
        var attackRange = isEquipSword == true ? closeAttackRange : wideAttackRange;

        if (wanderScript.primaryPrey != null)
        {
            var angle = Vector3.Angle(transform.forward, wanderScript.primaryPrey.transform.position - transform.position);
            var distance = Vector3.Distance(transform.position, wanderScript.primaryPrey.transform.position);
            if (angle <= 90f && distance <= attackRange)
            {
                wanderScript.attackTarget = wanderScript.primaryPrey;
                wanderScript.CurrentState = Common_WanderScript.WanderState.Attack;

                if (isEquipSword)
                    wanderScript.attackTarget.TakeDamage(closeAttackDamage);
                else if (isEquipBow)
                    wanderScript.attackTarget.TakeDamage(wideAttackDamage);
            }
        }
    }

    IEnumerator CountAttackDelay()
    {
        if (isEquipSword)
            yield return new WaitForSeconds(1.2f);
        else if (isEquipBow)
            yield return new WaitForSeconds(0.65f);

        isAttackDelay = false;
        wanderScript.attackTarget = null;
        wanderScript.CurrentState = Common_WanderScript.WanderState.Idle;
    }

    void SwapWeapon()
    {
        if (Input.GetButtonDown("Swap1") && weaponList.Contains("Sword"))
        {
            if (!isEquipSword)
            {
                isEquipSword = true;
                isEquipBow = false;
                weapons[0].SetActive(true);
                weapons[1].SetActive(false);
                characterAnimator.SetBool("sword", true);
                characterAnimator.SetBool("bow", false);
            }
            else
            {
                isEquipSword = false;
                isEquipBow = false;
                weapons[0].SetActive(false);
                weapons[1].SetActive(false);
                characterAnimator.SetBool("sword", false);
                characterAnimator.SetBool("bow", false);
            }
        }
        else if (Input.GetButtonDown("Swap2") && weaponList.Contains("Bow"))
        {
            if (!isEquipBow)
            {
                isEquipSword = false;
                isEquipBow = true;
                weapons[0].SetActive(false);
                weapons[1].SetActive(true);
                characterAnimator.SetBool("sword", false);
                characterAnimator.SetBool("bow", true);
            }
            else
            {
                isEquipSword = false;
                isEquipBow = false;
                weapons[0].SetActive(false);
                weapons[1].SetActive(false);
                characterAnimator.SetBool("sword", false);
                characterAnimator.SetBool("bow", false);
            }
        }
    }

    public void HungerIncrease(float amount)
    {
        hunger += amount;
        foodTimer = foodTimerValue;
        hungerTimer = hungerTimerValue;
    }

    void HungerDecrease()
    {
        hungerTimer = Mathf.MoveTowards(hungerTimer, 0f, Time.deltaTime);
        if (hungerTimer == 0f)
        {
            hungerTimer = hungerTimerValue;
            hunger = Mathf.MoveTowards(hunger, 0f, 10f);
            IsHunger();
        }
    }

    void IsHunger()
    {
        if (hunger <= 10f)
            wanderScript.TakeDamage(wanderScript.stats.toughness / 10);
    }

    void IsFatigue()
    {
        fatigueTimer = Mathf.MoveTowards(fatigueTimer, 0f, Time.deltaTime);
        if (fatigueTimer == 0f)
        {
            fatigueTimer = fatigueTimerValue;
            wanderScript.TakeDamage(wanderScript.stats.toughness / 10);
        }
    }

    void InteractItem()
    {
        if (nearObject != null)
        {
            if (nearObject.tag == "weapon")
            {
                ItemList item = nearObject.GetComponent<ItemList>();

                if (item.value == 0 && !(weaponList.Contains("Sword")))
                    weaponList.Add("Sword");
                else if (item.value == 1 && !(weaponList.Contains("Bow")))
                    weaponList.Add("Bow");

                Destroy(nearObject);
            }

            if (nearObject.tag == "House")
            {
                ItemList item = nearObject.GetComponent<ItemList>();
                if (item.value == 100)
                {
                    fade.OnFade();
                    timepast.ChangeDays();
                    fatigue = 0f;
                }
            }

            if (nearObject.tag == "Item")
            {
                inven.Acquireitem(nearObject.transform.GetComponent<ItemPickUp>().item);
                Destroy(nearObject);
            }
        }
        else
            return;
    }

    void TameAndFeed()
    {
        if (wanderScript.primaryPrey != null)
        {
            var angle = Vector3.Angle(transform.forward, wanderScript.primaryPrey.transform.position - transform.position);
            var distance = Vector3.Distance(transform.position, wanderScript.primaryPrey.transform.position);
            if (angle <= 90f && distance <= tameAndFeedRange && inven.FindFeedItem())
            {
                inven.UseFeedItem();
                if (wanderScript.primaryPrey.gameObject.GetComponent<AllyMonsterControl>().enabled == false)
                {
                    allyMonsterList.addMonster(wanderScript.primaryPrey.species + "_Ally");
                    wanderScript.primaryPrey.gameObject.GetComponent<AllyMonsterControl>().enabled = true;
                }
                else
                {
                    wanderScript.primaryPrey.gameObject.GetComponent<AllyMonsterControl>().HungerIncrease();
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "weapon" || other.tag == "House" || other.tag == "Item")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "weapon" || other.tag == "House" || other.tag == "Item")
            nearObject = null;
    }
}
