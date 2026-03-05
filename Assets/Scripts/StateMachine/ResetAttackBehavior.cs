using UnityEngine;

public class ResetAttackBehavior : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController player = animator.GetComponent<PlayerController>();

        if (player != null)
        { 
            player.isAttacking = false;
        }
    }
}