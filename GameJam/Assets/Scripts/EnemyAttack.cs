using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int cooldown = 1;
    [SerializeField] public int damage = 1;
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] public float maxHealth = 5f;
    [SerializeField] private float curHealth = 5f;
    [SerializeField] private float pointValue = 25f;

    private AnimationManager _animator;
    private Transform _myTransform;
    private GameObject _player;
    private List<GameObject> _toKill;
    private GameObject healthBar;
    private Transform health;
    private float maxHeight;
    private float timer = 0;

    private bool _canAttack = true, _storedAttack = false, _doingAttack = false;

    
    public GameObject spawnObject;

    private void Awake()
    {
        _animator = GetComponent<AnimationManager>();
        _myTransform = this.gameObject.GetComponent<Transform>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _toKill = new List<GameObject>();
    }

    private void Start()
    {
        healthBar = GameObject.Instantiate((GameObject)Resources.Load("Healthbar"), _myTransform.position + (new Vector3(0, 0.5f, 0)), _myTransform.rotation);
        healthBar.transform.SetParent(_myTransform);
        health = healthBar.transform.GetChild(0);
        maxHeight = health.localScale.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckHitPlayer(collision.collider);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckHitPlayer(other);
    }

    private void CheckHitPlayer(Collider2D collision)
    {
        if (timer <= 0 && collision.gameObject.CompareTag("Player"))
        {
            // TODO: Animate to attack
            Attack();
            collision.gameObject.GetComponent<PlayerCombat>().HitPlayer(damage);
            timer = cooldown;
        }
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer--;
        }
        
        if(_canAttack && _storedAttack)
        {
            _storedAttack = false;
            Attack();
        }
    }

    private void LateUpdate()
    {
        spawnObject.GetComponent<EnemyManager>().KillEnemies(_toKill);
        _toKill.Clear();
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        _canAttack = true;
        StopAttack();
    }
    public void StopAttack()
    {
        _canAttack = true;
        _doingAttack = false;
        StopAttackAnim();
    }
    private void StopAttackAnim()
    {
        _animator.DoAttack(_doingAttack);
    }
    private void Attack()
    {
        if (_canAttack)
        {
            _canAttack = false;
            _doingAttack = true;
            _animator.DoAttack(_doingAttack);
            StartCoroutine(ResetAttack());
        }
        else
            _storedAttack = true;
    }

    public void HitEnemy(float damage, Vector3 atkPos, GameObject attacker)
    {
        curHealth -= damage;
        if (attacker == _player)
        {
            AudioManager.instance.Play("Hit");
            if (_player.GetComponent<PlayerCombat>().GetAttackBoost())
            {
                curHealth = 0;
            }
        }
        if (curHealth <= 0 && !_toKill.Contains(this.gameObject))
        {
            if (attacker == _player)
            {
                attacker.GetComponent<ScoreManager>().AddPoints(pointValue);
                AudioManager.instance.Play("Pickup");
            }
            curHealth = 0;
            _toKill.Add(this.gameObject);
        }
        UpdateHealthbar();
    }

    public void SetHealth(float health)
    {
        maxHealth = health;
        curHealth = health;
        UpdateHealthbar();
    }

    public void UpdateHealthbar()
    {
        health.localScale = new Vector3(curHealth / maxHealth, maxHeight, 1);
    }
}
