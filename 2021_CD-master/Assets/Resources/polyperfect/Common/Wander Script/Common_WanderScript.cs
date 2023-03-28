using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PolyPerfect
{
    public class Common_WanderScript : MonoBehaviour
    {

        [SerializeField] public string species = "NA";

        [SerializeField, Tooltip("This specific animal stats asset, create a new one from the asset menu under (LowPolyAnimals/NewAnimalStats)")]
        public AIStats stats;

        // [SerializeField, Tooltip("How dominent this animal is in the food chain, agressive animals will attack less dominant animals.")]
        public int dominance = 1;
        public int originalDominance = 0;

        [SerializeField, Tooltip("How far this animal can sense a predator.")]
        public float awareness = 30f;

        [SerializeField, Tooltip("How far this animal can sense it's prey.")]
        public float scent = 30f;

        public float originalScent = 0f;

        // [SerializeField, Tooltip("How many seconds this animal can run for before it gets tired.")]
        public float stamina = 10f;

        // [SerializeField, Tooltip("How much this damage this animal does to another animal.")]
        public float power = 10f;

        // [SerializeField, Tooltip("How much health this animal has.")]
        public float toughness = 5f;

        // [SerializeField, Tooltip("Chance of this animal attacking another animal."), Range(0f, 100f)]
        public float aggression = 0f;
        public float originalAggression = 0f;

        // [SerializeField, Tooltip("How quickly the animal does damage to another animal (every 'attackSpeed' seconds will cause 'power' amount of damage).")]
        public float attackSpeed = 0.5f;

        // [SerializeField, Tooltip("If true, this animal will attack other animals of the same specices.")]
        public bool territorial = false;

        // [SerializeField, Tooltip("Stealthy animals can't be detected by other animals.")]
        public bool stealthy = false;

        public static List<Common_WanderScript> allAnimals = new List<Common_WanderScript>();

        public GameObject overImage;

        public static List<Common_WanderScript> AllAnimals
        {
            get { return allAnimals; }
        }

        public enum WanderState
        {
            Idle,
            Wander,
            Chase,
            Evade,
            Attack,
            Feast,
            Dead
        }

        public WanderState CurrentState;
        public Common_WanderScript primaryPrey;
        public Common_WanderScript primaryPursuer;
        public Common_WanderScript attackTarget;

        public CharacterController characterController;
        public NavMeshAgent navMeshAgent;
        public bool forceUpdate = false;

        private void Awake()
        {
            //Assign the stats to variables
            dominance = stats.dominance;
            toughness = stats.toughness;
            power = stats.power;
            stamina = stats.stamina;
            territorial = stats.territorial;
            aggression = stats.agression;
            attackSpeed = stats.attackSpeed;
            stealthy = stats.stealthy;
            scent = stats.scent;

            characterController = GetComponent<CharacterController>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        void OnEnable()
        {
            allAnimals.Add(this);
        }

        void OnDisable()
        {
            allAnimals.Remove(this);
        }

        public void TakeDamage(float damage)
        {
            toughness -= damage;
            if (toughness <= 0f)
                Die();
        }

        public virtual void Die()
        {
            Time.timeScale = 0;
            overImage.SetActive(true);
        }

        public void checkPrey()
        {
            var closestDistance = scent;
            primaryPrey = null;

            foreach (var animal in allAnimals)
            {
                if (animal == this || animal.CurrentState == WanderState.Dead)
                    continue;

                var distance = Vector3.Distance(transform.position, animal.transform.position);
                if (distance > closestDistance)
                    continue;

                closestDistance = distance;
                primaryPrey = animal;
                if (CurrentState != WanderState.Attack)
                    CurrentState = WanderState.Chase;
            }
        }

    }
}