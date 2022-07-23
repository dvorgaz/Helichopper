using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class ShootingAI : MonoBehaviour
{
    private Health health;    
    private WeaponLauncher weapon;

    private GameObject target;
    private Health targetHealth;
    private bool isTargetInLOS = false;

    public bool IsTargetValid
    {
        get { return target != null && target.activeInHierarchy && isTargetInLOS; }
    }

    [SerializeField] private float detectionRange;
    [SerializeField] private float losRangeOffset = 5.0f;
    [SerializeField] private float losSizeOffset = 20.0f;
    [SerializeField] private Vector2 tickDelayRange = new Vector2(1, 1);
    [SerializeField] private Vector2 attackDelayRange = new Vector2(1, 1);
    [SerializeField] private float attackAxisLength;

    private bool isAttacking = false;
    private bool horizontalAttack;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        weapon = GetComponentInChildren<WeaponLauncher>();

        StartCoroutine(ThinkCoroutine());

        horizontalAttack = Random.Range(0, 2) > 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!health.Alive)
            return;

        if(IsTargetValid && weapon != null)
        {
            if(targetHealth.Alive && !isAttacking)
            {
                StartCoroutine(AttackCoroutine(target.transform.position));
                //weapon.Launch(target.transform.position);
            }
        }
    }

    void FindTarget()
    {
        isTargetInLOS = false;
        target = GameObject.FindWithTag("Player");

        if (target != null)
        {
            targetHealth = target.GetComponent<Health>();
            if(targetHealth != null && targetHealth.Alive)
            {
                isTargetInLOS = IsInLineOfSight(target.transform.position);
            }
        }
    }

    private bool IsInLineOfSight(Vector3 pos)
    {
        RaycastHit hitInfo;
        Vector3 delta = pos - weapon.transform.position;
        Vector3 dir = delta.normalized;
        float dist = delta.magnitude;

        if (dist > detectionRange)
            return false;

        if (Physics.Raycast(weapon.transform.position + dir * losRangeOffset, dir, out hitInfo, dist, LayerMask.GetMask("Default", "Vehicle"), QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.distance < dist - losSizeOffset)
                return false;
        }

        return true;
    }

    IEnumerator ThinkCoroutine()
    {
        while (true)
        {
            if (!IsTargetValid)
            {
                FindTarget();
            }
            else
            {
                isTargetInLOS = IsInLineOfSight(target.transform.position);
            }

            yield return new WaitForSeconds(Random.Range(tickDelayRange.x, tickDelayRange.y));
        }
    }

    IEnumerator AttackCoroutine(Vector3 targetPoint)
    {
        isAttacking = true;

        Vector3 toTarget = targetPoint - transform.position;

        Vector3 crossAxis = horizontalAttack ? Vector3.up : Vector3.Cross(toTarget, Vector3.up);
        horizontalAttack = !horizontalAttack;
        //Vector3 crossAxis = Random.Range(0, 2) > 0 ? Vector3.up : Vector3.Cross(toTarget, Vector3.up);
        //Vector3 crossAxis = Vector3.up;
        Vector3 attackAxis = Vector3.Cross(toTarget, crossAxis).normalized * (attackAxisLength / 2.0f);

        //Vector3 startPoint = targetPoint + attackAxis;
        //Vector3 endPoint = targetPoint - attackAxis;

        float attackDuration = 2.0f;

        for(float t = 0; t < attackDuration; t += Time.deltaTime)
        {
            Vector3 startPoint = target.transform.position + attackAxis;
            Vector3 endPoint = target.transform.position - attackAxis;
            weapon.Fire(Vector3.Lerp(startPoint, endPoint, t / attackDuration));
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(attackDelayRange.x, attackDelayRange.y));

        isAttacking = false;
    }

    public void OnDeath()
    {
        StopAllCoroutines();
    }
}
