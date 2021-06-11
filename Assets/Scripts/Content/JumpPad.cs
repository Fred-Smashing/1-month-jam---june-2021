using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float padForce = 5f;
    [SerializeField] private Vector2 forceDirection = Vector2.up;

    private List<Collision2D> collisionList = new List<Collision2D>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Touched Pad");
            if (!collisionList.Contains(collision))
            {
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(forceDirection * padForce, ForceMode2D.Impulse);
                collisionList.Add(collision);
            }
        }
    }

    private void FixedUpdate()
    {
        collisionList.Clear();
    }
}
