using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelBound : MonoBehaviour
{
    [SerializeField] private float scaling = .2222f;

    private void OnDrawGizmos()
    {
        var cam = GetComponent<Camera>();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3((1920 * scaling) * cam.orthographicSize, (1080 * scaling) * cam.orthographicSize, 1));
    }
}
