using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private bool _gotHit = false;
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void Hit()
    {
        _gotHit = true;
        _animator.SetBool("GotHit", _gotHit);
        Invoke("ResetHit", 0.2f);
    }
    public void DoAttack(bool atk)
    {
        _animator.SetBool("isAttacking", atk);
    }
    private void ResetHit()
    {
        _gotHit = false;
        _animator.SetBool("GotHit", _gotHit);
    }
    public void Death()
    {
        _animator.SetBool("Dead", true);
    }
    public void FaceDown(bool facingDown)
    {
        _animator.SetBool("facingDown", facingDown);
    }
    public void Moving(bool isMoving)
    {
        _animator.SetBool("isMoving", isMoving);
    }
}