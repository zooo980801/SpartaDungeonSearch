using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum Faction
{
    Ally,
    Enemy
}

public enum AIState
{
    Idle,
    Wandering,
    Attacking,
    Fleeing
}

public class NPC : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    public float safeDistance;
    private AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    [Header("진영 설정")]
    public Faction faction = Faction.Enemy;


    private float playerDistance;

    public float fieldOfView = 120f;

    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }


    private void Update()
    {
        if (faction == Faction.Ally)
        {
            AllyUpdate();
            return;
        }

        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        animator.SetBool("Moving", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
            case AIState.Fleeing:
                FleeingUpdate();
                break;
        }
    }


    private void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
            case AIState.Fleeing:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
        }

        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        // ✅ 타겟이 감지 범위 내에 있으면 공격 상태로 진입
        Transform target = FindNearestTarget();
        if (target != null && Vector3.Distance(transform.position, target.position) < detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }


    void AttackingUpdate()
    {
        Transform target = FindNearestTarget();

        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > attackDistance || !IsInFieldOfView(target))
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("Attack");

                IDamagable damagable = target.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.TakePhysicalDamage(damage);
                }
            }
        }
    }
    bool IsInFieldOfView(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        return angle < fieldOfView * 0.5f;
    }


    void FleeingUpdate()
    {
        if (agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(GetFleeLocation());
        }
        else
        {
            SetState(AIState.Wandering);
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle)
        {
            return;
        }
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5f;
    }

    Vector3 GetFleeLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (GetDestinationAngle(hit.position) > 90 || playerDistance < safeDistance)
        {

            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    float GetDestinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - CharacterManager.Instance.Player.transform.position, transform.position + targetPos);
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health -= damageAmount;

        Debug.Log($"[NPC] {gameObject.name} 체력 감소: -{damageAmount}, 남은 체력: {health}");
        if (health <= 0)
            Die();

        StartCoroutine(DamageFlash());
    }

    void Die()
    {
        for (int x = 0; x < dropOnDeath.Length; x++)
        {
            Instantiate(dropOnDeath[x].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);

        yield return new WaitForSeconds(0.1f);
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = Color.white;
    }

    void AllyUpdate()
    {
        // 플레이어 따라가기
        Transform player = CharacterManager.Instance.Player.transform;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > 3f)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
        }

        // 근처 적 NPC 탐색 및 공격
        NPC targetEnemy = FindNearestEnemy();
        if (targetEnemy != null)
        {
            float dist = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (dist < attackDistance)
            {
                if (Time.time - lastAttackTime > attackRate)
                {
                    lastAttackTime = Time.time;

                    if (targetEnemy.faction != this.faction)
                    {
                        targetEnemy.TakePhysicalDamage(damage);
                        animator.SetTrigger("Attack");
                    }
                }

                agent.isStopped = true;
            }

            else
            {
                agent.isStopped = false;
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
    }

    NPC FindNearestEnemy()
    {
        float closest = Mathf.Infinity;
        NPC nearest = null;

        foreach (NPC npc in FindObjectsOfType<NPC>())
        {
            if (npc == this || npc.faction == this.faction) continue;

            float dist = Vector3.Distance(transform.position, npc.transform.position);
            if (dist < closest)
            {
                closest = dist;
                nearest = npc;
            }
        }

        return nearest;
    }

    Transform FindNearestTarget()
    {
        Transform nearest = null;
        float closest = Mathf.Infinity;

        foreach (NPC npc in FindObjectsOfType<NPC>())
        {
            if (npc == this || npc.faction == this.faction) continue;

            float dist = Vector3.Distance(transform.position, npc.transform.position);
            if (dist < closest)
            {
                closest = dist;
                nearest = npc.transform;
            }
        }

        // 플레이어도 포함
        Transform player = CharacterManager.Instance.Player.transform;
        if (this.faction == Faction.Enemy)
        {
            float playerDist = Vector3.Distance(transform.position, player.position);
            if (playerDist < closest)
            {
                nearest = player;
            }
        }

        return nearest;
    }




}
