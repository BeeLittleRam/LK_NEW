using UnityEngine;

namespace HutongGames
{
    public class AnimatorUtils
    {
        public static bool IsPlaying(Animator anim, int animLayer, string stateName) =>
            anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName) &&
            anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f;
    }
}