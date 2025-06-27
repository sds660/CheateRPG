using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    [SerializeField] float damage = 0.0001f;
    List<GameObject> _hurtObjects;

    private void Awake()
    {
        _hurtObjects = new List<GameObject>();
    }

    private void Update()
    {
        foreach (GameObject obj in _hurtObjects)
        {
            if (obj.tag == "Player")
            {
                obj.GetComponent<PlayerCombat>().HitPlayer(damage);
            }
            else if (obj.tag == "Enemy")
            {
                obj.GetComponent<EnemyAttack>().HitEnemy(damage, this.transform.position, null);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _hurtObjects.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _hurtObjects.Remove(collision.gameObject);
    }
}
