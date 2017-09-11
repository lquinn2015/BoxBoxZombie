using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State { Idle, Chasing, Attacking};
    State currState; 

    NavMeshAgent pathfinder;
    Transform target;
    Material skinMaterial;
    Color normalColor;

    LivingEntity targetEntity;

    float strikingDistance = .5f;
    float timeBetweenAttacks = 1;
    float nextAttack;

    float damage = 1;

    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    // Use this for initialization
	protected override void Start ()
    {
        base.Start();

        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;

            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;
            hasTarget = true;
            currState = State.Chasing;

            skinMaterial = GetComponent<Renderer>().material;
            normalColor = skinMaterial.color;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currState = State.Idle;
    }

	// Update is called once per frame
	void Update ()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttack)
            {
                float sqrDistance = (target.position - transform.position).sqrMagnitude;
                if (sqrDistance < Mathf.Pow(strikingDistance + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttack = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
	}

    IEnumerator Attack()
    {
        currState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;
        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;


        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = 4 * (-percent * percent + percent);
            transform.position = Vector3.Lerp(startPos, attackPos, interpolation);
            yield return null;
        }

        skinMaterial.color = normalColor;
        currState = State.Chasing;
        pathfinder.enabled = true;
    }


    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;
     
        while (hasTarget)
        {
            if (currState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + strikingDistance/2);
                if (!dead)
                {
                    
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);  //This must always happen or unity will fucking crash
        }
    }

}
