
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

	private float attackCooldown = 3f;
	private bool canAttack = true;
	public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
	public Coroutine stuck;

	[Rpc(SendTo.Server)]
	public void ServerAttackRpc()
	{
		if (canAttack)
		{
			canAttack = false;
			StartCoroutine(ResetAttack());
			objectInfoPacked.Animator.SetBool("Attack", true);
			stuck = StartCoroutine(CheckAttacking());
		}
	}
	private float defaultMoveSpeed;
	public IEnumerator CheckAttacking()
	{
		defaultMoveSpeed = objectInfoPacked.ObjectMovable.MoveSpeed;
		objectInfoPacked.ObjectMovable.MoveSpeedNetVar.Value *= 0.1f;
		
		do
		{
			yield return new WaitForSeconds(Time.fixedDeltaTime);	
		}
		while (objectInfoPacked.Animator.GetBool("Attack"));
		
		objectInfoPacked.ObjectMovable.MoveSpeedNetVar.Value = defaultMoveSpeed;
	}

	private IEnumerator ResetAttack()
	{
		yield return new WaitForSeconds(attackCooldown);
		canAttack = true;
	}
}