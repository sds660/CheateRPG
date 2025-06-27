using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour
{
    
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] public float maxHealth = 100;
    [SerializeField] private float curHealth = 100;
    [SerializeField] private LayerMask _enemyLayers;

    private MouseInput _mouseInput;
    private AnimationManager _animator;
    private RectTransform _healthBar;
    private FlashSprite _flash;
    private bool _isDead = false;
    private bool _canAttack = true, _storedAttack = false, _doingAttack = false, _godMode = false, _attackBoost = false;
    private float startingHealthWidth;
    private float startingHealthHeight;
    public float attackRange = 0.5f;

    private void Awake()
    {
        _mouseInput = new MouseInput();
        _animator = GetComponent<AnimationManager>();
        
        TryGetComponent(out _flash);
        
        _healthBar = GameObject.Find("Player UI/HealthBar/Health").GetComponent<RectTransform>();
        startingHealthWidth = _healthBar.rect.width;
        startingHealthHeight = _healthBar.rect.height;
    }
    private void Start()
    {
        _mouseInput.Controls.Attack.performed += ctx => Attack();
    }
    private void Update()
    {
        // stored attack makes it easier to spam attacks b/c it records
        // a previous attack input even if attack is not possible at that moment
        // to use when next attack is available
        if(_canAttack && _storedAttack)
        {
            _storedAttack = false;
            Attack();
        }
    }
    private void OnEnable()
    {
        _mouseInput.Enable();
    }
    private void OnDisable()
    {
        _mouseInput.Disable();
    }
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        _canAttack = true;
        StopAttack();
    }

    public bool GetGodMode()
    {
        return _godMode;
    }

    public void ToggleGodMode()
    {
        _godMode = !_godMode;
    }
    public bool GetAttackBoost()
    {
        return _attackBoost;
    }

    public void ToggleAttackBoost()
    {
        _attackBoost = !_attackBoost;
    }
    private void Attack()
    {
        if (_canAttack)
        {
            _canAttack = false;
            _doingAttack = true;
            _animator.DoAttack(_doingAttack);
            CheckHit();
            StartCoroutine(ResetAttack());
        }
        else
            _storedAttack = true;
    }
    public void CheckHit()
    {
        var hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, _enemyLayers);
        if (hitEnemy != null)
        {
            hitEnemy.GetComponent<EnemyAttack>().HitEnemy(damage, transform.position, this.gameObject);
        }
    }
    public void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    private void StopAttackAnim()
    {
        _animator.DoAttack(_doingAttack);
    }
    public void StopAttack()
    {
        _canAttack = true;
        _doingAttack = false;
        StopAttackAnim();
    }

    public void HitPlayer(float damage)
    {
        if (_godMode || _flash.GetInProgress()) return;
        
        curHealth -= damage;
        
        _flash.DoFlash(GetComponent<SpriteRenderer>(), 5, 0.05f, true);
        AudioManager.instance.Play("Hit");
        
        if (curHealth < 0 && !_isDead)
        {
            _isDead = true;
            curHealth = 0;
            AudioManager.instance.Play("SlowBeat");
            StartCoroutine(Death());
        }
        UpdateHealthbar();
    }

    private IEnumerator Death()
    {
        gameObject.transform.Rotate(0, 0, 90);
        enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Animator>().enabled = false;
        _animator.enabled = false;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddHealth(float heal)
    {
        curHealth += heal;
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
        UpdateHealthbar();
    }

    public void UpdateHealthbar()
    {
        float deltaHealth = curHealth / maxHealth;
        _healthBar.sizeDelta = new Vector2(startingHealthWidth * deltaHealth, startingHealthHeight);
    }
}