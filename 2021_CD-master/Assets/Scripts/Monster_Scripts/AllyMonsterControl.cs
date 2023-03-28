using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using PolyPerfect;

public class AllyMonsterControl : MonoBehaviour
{
    private Animal_WanderScript wanderScript;

    public GameObject player;
    private PlayerAllyMonsterList allyMonsterList;
    public float rangeFromPlayer = 5f;
    private bool outOfRange;

    public float hunger = 100f;
    private float feedTimer;
    private float hungerTimer;
    public float feedTimerValue = 180f;
    public float hungerTimerValue = 5f;

    public GameObject item;
    private float itemTimer;
    public float itemTimerValue = 180f;

    public float breedTimer;
    public float breedTimerValue = 360f;
    public float breedRange = 3f;

    private void Awake()
    {
        wanderScript = GetComponent<Animal_WanderScript>();
        allyMonsterList = player.GetComponent<PlayerAllyMonsterList>();
    }

    private void OnDisable()
    {
        allyMonsterList.deleteMonster(wanderScript.species);
    }

    // Start is called before the first frame update
    void Start()
    {
        wanderScript.species += "_Ally";
        wanderScript.constainedToWanderZone = true;
        wanderScript.nonAgressiveTowards.Add("Human");
        wanderScript.nonAgressiveTowards.Add(wanderScript.species);

        outOfRange = false;

        feedTimer = feedTimerValue;
        hungerTimer = hungerTimerValue;

        itemTimer = itemTimerValue;

        breedTimer = breedTimerValue;
    }

    // Update is called once per frame
    void Update()
    {
        AddAllyMonster();

        feedTimer = Mathf.MoveTowards(feedTimer, 0f, Time.deltaTime);
        if (feedTimer == 0f)
            HungerDecrease();

        if (Vector3.Distance(transform.position, player.transform.position) > rangeFromPlayer)
        {
            outOfRange = true;
            wanderScript.enabled = false;
            FollowPlayer();
        }
        else if (outOfRange)
        {
            outOfRange = false;
            wanderScript.enabled = true;
            wanderScript.SetState(Animal_WanderScript.WanderState.Idle);
            wanderScript.UpdateAI();
        }

        itemTimer = Mathf.MoveTowards(itemTimer, 0f, Time.deltaTime);
        if (itemTimer == 0f)
            ProduceItem();

        foreach (Common_WanderScript other in Common_WanderScript.AllAnimals)
        {
            if(wanderScript != other && wanderScript.species == other.species && Vector3.Distance(transform.position, other.transform.position) <= breedRange
                && transform.localScale.Equals(new Vector3(1.0f, 1.0f, 1.0f)) && other.transform.localScale.Equals(new Vector3(1.0f, 1.0f, 1.0f)))
                breedTimer = Mathf.MoveTowards(breedTimer, 0f, Time.deltaTime);
            if (breedTimer == 0f)
            {
                Breed(other);
                break;
            }
        }
    }

    void AddAllyMonster()
    {
        foreach (string species in allyMonsterList.list)
        {
            if (species != wanderScript.species && !wanderScript.nonAgressiveTowards.Contains(species))
            {
                wanderScript.nonAgressiveTowards.Add(species);
            }
        }
    }

    void FollowPlayer()
    {
        var targetPosition = player.transform.position;

        MovementState moveState = null;
        var maxSpeed = 0f;
        foreach (var state in wanderScript.movementStates)
        {
            var stateSpeed = state.moveSpeed;
            if (stateSpeed > maxSpeed)
            {
                moveState = state;
                maxSpeed = stateSpeed;
            }
        }

        var turnSpeed = moveState.turnSpeed;
        var moveSpeed = maxSpeed;
        wanderScript.ClearAnimatorBools();
        wanderScript.TrySetBool(moveState.animationBool, true);

        if (wanderScript.navMeshAgent)
        {
            wanderScript.navMeshAgent.destination = targetPosition;
            wanderScript.navMeshAgent.speed = moveSpeed;
            wanderScript.navMeshAgent.angularSpeed = turnSpeed;
        }
        else
            wanderScript.characterController.SimpleMove(moveSpeed * Vector3.ProjectOnPlane(targetPosition - transform.position, Vector3.up).normalized);
    }

    public void HungerIncrease()
    {
        hunger = 100f;
        feedTimer = feedTimerValue;
        hungerTimer = hungerTimerValue;

        if (transform.localScale.Equals(new Vector3(1.0f, 1.0f, 1.0f)))
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
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
        if(hunger <= 10f && wanderScript.CurrentState != Animal_WanderScript.WanderState.Dead)
            wanderScript.TakeDamage(wanderScript.stats.toughness / 10);
    }

    void ProduceItem()
    {
        if (wanderScript.animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.LookOut"))
        {
            Instantiate(item, transform.position, Quaternion.identity);
            itemTimer = itemTimerValue;
        }
    }

    void Breed(Common_WanderScript other)
    {
        if (wanderScript.animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Sleeping"))
        {
            GameObject childAnimal = Instantiate(gameObject, transform.position, Quaternion.identity);
            childAnimal.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            childAnimal.GetComponent<AllyMonsterControl>().enabled = true;
            breedTimer = breedTimerValue;
            other.GetComponent<AllyMonsterControl>().breedTimer = other.GetComponent<AllyMonsterControl>().breedTimerValue;
        }
    }
}
