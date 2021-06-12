using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderEnable : MonoBehaviour
{
    [SerializeField] private GameObject renderObject;

    private void Awake()
    {
        renderObject.SetActive(true);
    }
}
