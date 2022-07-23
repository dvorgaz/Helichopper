using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryMovement : GroundMovement
{
    [SerializeField] private Animator anim;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();

        if(anim != null)
        {
            anim.SetFloat("MoveSpeed", agent.desiredVelocity.magnitude / agent.speed);
        }
    }

    public void OnDeath()
    {
        enabled = false;
        agent.enabled = false;
        rb.isKinematic = false;
        rb.angularVelocity = Random.insideUnitSphere * 20.0f;

        if (anim != null)
        {
            anim.SetFloat("MoveSpeed", 0);
        }
    }
}
