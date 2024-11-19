
using UnityEngine;

public class AttackSMB : StateMachineBehaviour
{
	
	private void OnEnable() 
	{
		Debug.Log("AttackSMB OnEnable");
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetBool("Attack", false);
	}
}