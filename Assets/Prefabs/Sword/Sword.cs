using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GameObject.FindGameObjectWithTag("PickableSword").GetComponent<MeshRenderer>();
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
