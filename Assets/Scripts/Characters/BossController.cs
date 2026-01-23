using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float movementSpeed = 4f;

    [Header("Teleport Points")]
    public Transform[] groundTeleportPoints;
    public Transform sanctuaryPoint;
    public Transform[] allTeleportPoints;

    [Header("Phase 2 Floating")]
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;
    private Vector3 sanctuaryBasePos;

    [Header("Attack - Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float timeBetweenShots = 0.7f;

    [Header("Summon")]
    public GameObject minionPrefab;
    public Transform[] minionSpawnPoints;
    public GameObject shieldVisual;

    private Damageable damageable;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D bossCollider;

    private bool isDead = false;
    private bool isInvincibleAction = false;
    private int waveCount = 0;
    private List<GameObject> activeMinions = new List<GameObject>();
    private bool lastAttackWasMelee = false;
    private bool isActive = false;
    private float defaultGravity;

    private enum BossState 
    { 
        Idle, 
        Chasing, 
        MeleeAttack, 
        Shooting, 
        Phase2_Shielded, 
        Phase2_Vulnerable, 
        Summoning 
    }
    private BossState currentState = BossState.Idle;
    private enum BossPhase 
    { 
        Phase1, 
        Phase2, 
        Phase3 
    }
    private BossPhase currentPhase = BossPhase.Phase1;

    void Awake()
    {
        damageable = GetComponent<Damageable>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<Collider2D>();

        defaultGravity = rb.gravityScale;
        if (shieldVisual) shieldVisual.SetActive(false);
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        damageable.OnPlayerDied += Die;
    }

    void Update()
    {

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
            else
            {
                return;
            }
        }
        if (isDead || !isActive) return;

        CheckPhaseTransition();
        FacePlayer();

        if (currentState == BossState.Chasing)
        {
            MoveTowardsPlayer();
        }

        if (currentState == BossState.Phase2_Shielded)
        {
            HandleHover();
            CheckMinions();
        }
    }

    public void ActivateBoss()
    {
        if (isActive) return;

        isActive = true;

        Debug.Log("The Dark Mage woke up!");

        StartCoroutine(MainCombatLoop());
    }

    private void FacePlayer()
    {
        if (player.position.x < transform.position.x) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleHover()
    {
        float newY = sanctuaryBasePos.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = new Vector3(sanctuaryBasePos.x, newY, sanctuaryBasePos.z);
    }

    IEnumerator MainCombatLoop()
    {
        yield return new WaitForSeconds(1f);
        while (!isDead)
        {
            if (currentState == BossState.Phase2_Shielded || currentState == BossState.Phase2_Vulnerable)
            {
                yield return null;
                continue;
            }

            Debug.Log($"[BOSS] Fõciklus - Fázis: {currentPhase}");

            // Phase 1
            if (currentPhase == BossPhase.Phase1)
            {
                if (lastAttackWasMelee)
                {
                    yield return StartCoroutine(RangedAttackRoutine());
                    lastAttackWasMelee = false;
                }
                else
                {
                    yield return StartCoroutine(MeleeAttackRoutine());
                    lastAttackWasMelee = true;
                }
            }
            // Phase 3
            else if (currentPhase == BossPhase.Phase3)
            {
                yield return StartCoroutine(TeleportToRandomPoint());

                int rng = Random.Range(0, 3);
                Debug.Log($"[BOSS] Phase 3 Akció: {rng} (0=Melee, 1=Ranged, 2=Summon)");

                if (rng == 0)
                {

                    yield return StartCoroutine(MeleeAttackRoutinePhase3());
                }
                else if (rng == 1)
                {
                    yield return StartCoroutine(RangedAttackRoutinePhase3());
                }
                else
                {
                    yield return StartCoroutine(SummonActionRoutine());
                }

                yield return new WaitForSeconds(0.5f);
            }

            float cooldown = (currentPhase == BossPhase.Phase1) ? 2f : 1f;
            yield return new WaitForSeconds(cooldown);
        }
    }

    IEnumerator MeleeAttackRoutine()
    {
        Transform targetPoint = GetClosestGroundPoint();
        yield return StartCoroutine(TeleportToPoint(targetPoint));
        yield return StartCoroutine(MeleeLogic());
    }

    IEnumerator RangedAttackRoutine()
    {
        yield return StartCoroutine(TeleportToPoint(GetRandomGroundPoint()));
        yield return StartCoroutine(RangedLogic());
    }

    IEnumerator MeleeAttackRoutinePhase3()
    {
        if (Mathf.Abs(transform.position.y - player.position.y) > 2.5f)
        {
            Transform targetPoint = GetClosestGroundPoint();
            yield return StartCoroutine(TeleportToPoint(targetPoint));
        }
        yield return StartCoroutine(MeleeLogic());
    }

    IEnumerator RangedAttackRoutinePhase3()
    {
        yield return StartCoroutine(RangedLogic());
    }

    IEnumerator SummonActionRoutine()
    {
        currentState = BossState.Summoning;
        rb.gravityScale = defaultGravity;

        animator.SetBool("isAttacking", true);

        animator.SetTrigger("CastSpell");
        yield return new WaitForSeconds(0.3f);

        if (minionSpawnPoints.Length > 0 && activeMinions.Count < 3)
        {
            Transform spawnPos = minionSpawnPoints[Random.Range(0, minionSpawnPoints.Length)];
            GameObject minion = Instantiate(minionPrefab, spawnPos.position, Quaternion.identity);
            activeMinions.Add(minion);
        }

        yield return new WaitForSeconds(0.2f);

        animator.SetBool("isAttacking", false);
        currentState = BossState.Idle;
    }

    IEnumerator MeleeLogic()
    {
        currentState = BossState.Chasing;
        animator.SetBool("isMoving", true);

        float chaseTimer = 8f;
        bool reachedPlayer = false;

        while (chaseTimer > 0)
        {
            chaseTimer -= Time.deltaTime;
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < 4f) 
            { 
                reachedPlayer = true; 
                break; 
            }
            yield return null;
        }

        currentState = BossState.MeleeAttack;
        animator.SetBool("isMoving", false);

        if (reachedPlayer)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            if (bossCollider != null) bossCollider.enabled = false;

            animator.SetBool("isAttacking", true);
            animator.SetTrigger("MeleeAttack1");
            yield return new WaitForSeconds(0.5f);

            animator.SetTrigger("MeleeAttack2");

            animator.SetBool("isAttacking", false);

            if (bossCollider != null) bossCollider.enabled = true;
            rb.gravityScale = defaultGravity;
        }
        currentState = BossState.Idle;
    }

    IEnumerator RangedLogic()
    {
        currentState = BossState.Shooting;
        isInvincibleAction = true;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        if (bossCollider != null) bossCollider.enabled = false;

        animator.SetBool("isAttacking", true);
        animator.SetTrigger("CastSpell");

        for (int i = 0; i < 3; i++)
        {
            ShootFireball();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        animator.SetBool("isAttacking", false);
        if (bossCollider != null) bossCollider.enabled = true;
        rb.gravityScale = defaultGravity;

        isInvincibleAction = false;
        currentState = BossState.Idle;
    }

    // PHASE 2
    IEnumerator Phase2Routine()
    {
        Debug.LogWarning("[BOSS] FÁZISVÁLTÁS: Phase 2 indul!");
        currentPhase = BossPhase.Phase2;

        yield return StartCoroutine(TeleportToPoint(sanctuaryPoint, true));

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        sanctuaryBasePos = transform.position;

        waveCount = 0;

        while (waveCount < 2)
        {
            Debug.Log($"[BOSS] Phase 2 - Hullám {waveCount + 1}/2");

            isInvincibleAction = true;
            if (bossCollider != null) bossCollider.enabled = false;

            currentState = BossState.Phase2_Shielded;

            SpawnMinions(2);

            while (activeMinions.Count > 0)
            {
                yield return null;
            }

            Debug.Log("[BOSS] Minionok halottak - Pajzs LE, Kábult állapot!");

            if (shieldVisual) shieldVisual.SetActive(false);

            isInvincibleAction = false;
            currentState = BossState.Phase2_Vulnerable;

            if (bossCollider != null) bossCollider.enabled = true;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = defaultGravity;

            animator.SetTrigger("Stunned");

            yield return new WaitForSeconds(3f);

            if (waveCount < 1)
            {
                yield return StartCoroutine(TeleportToPoint(sanctuaryPoint, true));

                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f;
                sanctuaryBasePos = transform.position;
            }

            waveCount++;
        }

        Debug.LogWarning("[BOSS] FÁZISVÁLTÁS: Phase 3 indul!");

        rb.gravityScale = defaultGravity;
        currentPhase = BossPhase.Phase3;
        currentState = BossState.Idle;
    }

    void CheckMinions() 
    { 
        activeMinions.RemoveAll(x => x == null); 
    }
    void MoveTowardsPlayer()
    {
        Vector2 target = new Vector2(player.position.x, transform.position.y);
        if (player.position.x < transform.position.x) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);
        transform.position = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
    }
    IEnumerator TeleportToPoint(Transform targetPoint, bool withShield = false)
    {
        if (targetPoint == null) yield break;

        rb.gravityScale = 0f;
        if (bossCollider != null) bossCollider.enabled = false;

        animator.SetTrigger("TeleportOut");
        yield return new WaitForSeconds(0.5f);

        spriteRenderer.enabled = false;
        transform.position = targetPoint.position;

        yield return new WaitForSeconds(0.2f);

        if (withShield && shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }

        spriteRenderer.enabled = true;
        animator.SetTrigger("TeleportIn");
        yield return new WaitForSeconds(0.5f);

        if (!isInvincibleAction && currentState != BossState.Phase2_Shielded)
        {
            if (bossCollider != null) bossCollider.enabled = true;
            rb.gravityScale = defaultGravity;
        }
    }

    void ShootFireball()
    {
        if (projectilePrefab != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var script = proj.GetComponent<SimpleProjectile>();
            if (script) script.SetDirection((player.position - firePoint.position).normalized);
        }
    }
    void SpawnMinions(int count)
    {
        activeMinions.Clear();
        for (int i = 0; i < count; i++)
        {
            if (minionSpawnPoints.Length > 0)
            {
                Transform spawnPos = minionSpawnPoints[i % minionSpawnPoints.Length];
                GameObject minion = Instantiate(minionPrefab, spawnPos.position, Quaternion.identity);
                activeMinions.Add(minion);
            }
        }
    }
    void CheckPhaseTransition()
    {
        if (currentPhase == BossPhase.Phase1)
        {
            float hpPercent = (float)damageable.Health / (float)damageable.MaxHealth;
            if (hpPercent < 0.6f)
            {
                if (currentState == BossState.Idle || currentState == BossState.Chasing)
                {
                    StartCoroutine(Phase2Routine());
                }
            }
        }
    }
    Transform GetClosestGroundPoint()
    {
        if (groundTeleportPoints.Length == 0) return transform;

        if (player == null)
        {
            return transform;
        }

        Transform bestTarget = groundTeleportPoints[0]; float closestDistanceSqr = Mathf.Infinity;
        foreach (Transform potentialTarget in groundTeleportPoints)
        {
            float dSqrToTarget = (potentialTarget.position - player.position).sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) { closestDistanceSqr = dSqrToTarget; bestTarget = potentialTarget; }
        }
        return bestTarget;
    }
    Transform GetRandomGroundPoint() 
    { 
        if (groundTeleportPoints.Length == 0) return transform; 
        return groundTeleportPoints[Random.Range(0, groundTeleportPoints.Length)]; 
    }

    IEnumerator TeleportToRandomPoint() 
    { 
        if (allTeleportPoints.Length > 0) 
            yield return StartCoroutine(TeleportToPoint(allTeleportPoints[Random.Range(0, allTeleportPoints.Length)])); 
    }

    void Die()
    {
        isDead = true;
        StopAllCoroutines();
        Debug.Log("[BOSS] MEGHALTAM!");
        rb.gravityScale = defaultGravity;
        Destroy(gameObject, 3f);
    }
}