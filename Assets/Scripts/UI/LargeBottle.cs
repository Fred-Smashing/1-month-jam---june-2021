using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeBottle : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private ParticleSystem _particleSystem;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        StopAnimation();
    }

    public void PlayAnimation()
    {
        _animator.speed = 1;
        _animator.Play("pop_cap");
    }

    public void StartParticleEffect()
    {
        _particleSystem.Play();
    }

    public void StopAnimation()
    {
        _animator.speed = 0;
    }
}
