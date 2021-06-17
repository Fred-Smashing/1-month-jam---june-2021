using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float padForce = 5f;
    [SerializeField] private Vector2 forceDirection = Vector2.up;

    private List<GameObject> collisionList = new List<GameObject>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collisionGameObject = collision.gameObject;

        if (collisionGameObject.CompareTag("Player"))
        {
            if (!collisionList.Contains(collisionGameObject))
            {
                Rigidbody2D affectedBody = collisionGameObject.GetComponent<Rigidbody2D>();

                var momentum = Mathf.Abs(collisionGameObject.GetComponent<PlayerController>().velocityLastFrame.y * 0.4f);

                collisionGameObject.GetComponent<Rigidbody2D>().AddForce(forceDirection * (padForce + momentum), ForceMode2D.Impulse);
                collisionList.Add(collisionGameObject);

                //Debug.Log(string.Format("Added force to body {0} with a force of {1}", affectedBody, padForce + momentum));
            }
        }
    }

    private void FixedUpdate()
    {
        if (collisionList.Count > 0)
        {
            StartCoroutine(clearList());
        }
    }

    private IEnumerator clearList()
    {
        yield return new WaitForFixedUpdate();
        collisionList.Clear();
    }
}
