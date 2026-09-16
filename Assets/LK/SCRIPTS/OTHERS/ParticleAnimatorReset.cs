using UnityEngine;

public class ParticleAnimatorReset : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Animator animator;

    private void Awake()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (particleSystem == null || animator == null)
            return;

        // Detect when the particle system has just started playing
        if (particleSystem.isPlaying && !wasPlaying)
        {
            ResetAnimator();
        }

        wasPlaying = particleSystem.isPlaying;
    }

    private bool wasPlaying;

    private void ResetAnimator()
    {
        animator.Rebind();
        animator.Update(0f);
        animator.Play(0, 0, 0f);
        animator.Update(0f);
    }
}