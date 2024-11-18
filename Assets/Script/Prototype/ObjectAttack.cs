
using System.Collections;
using Game;
using Unity.Netcode;
using UnityEngine;

public class ObjectAttack : NetworkBehaviour
{
    private ObjectInfoPacked objectInfoPacked;

    private void Awake() 
    {
        objectInfoPacked = GetComponent<ObjectInfoPacked>();
    }
    private void FixedUpdate() 
    {
        if (!IsOwner) return;
        Attack();
    }

    private void Attack()
    {
        if (Input.GetKey(KeyCode.J))
        {
            ServerAttackRpc();
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        
    }

    private float attackCooldown = 3f;
    private bool canAttack = true;
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }

    [Rpc(SendTo.Server)]
    public void ServerAttackRpc()
    {
        if (canAttack)
        {
            canAttack = false;
            StartCoroutine(ResetAttack());
            StartCoroutine(CheckAttacking());
            objectInfoPacked.Animator.SetBool("Attack", true);
        }
    }

    private float defaultMoveSpeed;
    public IEnumerator CheckAttacking()
    {
        defaultMoveSpeed = objectInfoPacked.ObjectMovable.MoveSpeed;
        objectInfoPacked.MoveSpeed.Value *= 0.1f;
        while (objectInfoPacked.Animator.GetBool("Attack"))
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        objectInfoPacked.MoveSpeed.Value = defaultMoveSpeed;
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}