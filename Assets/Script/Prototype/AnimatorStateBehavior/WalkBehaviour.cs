using UnityEngine;

public class WalkBehaviour : StateMachineBehaviour 
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Walk", true);
    }
}