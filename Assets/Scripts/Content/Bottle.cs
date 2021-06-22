using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    private Collider2D _collider;
    private GameManager _gameManager;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        _gameManager.CompletedLevel();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _gameManager.CompletedLevel();

            Destroy(this.gameObject);
        }
    }

    public void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
}
