using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineController : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    // Expect exactly 2 transforms here (Key, Second layer button)
    public List<Transform> targetKeys = new List<Transform>(2);

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Vector3 a = targetKeys[0].position;
        Vector3 b = targetKeys[1].position;

        _lineRenderer.SetPosition(0, a);
        _lineRenderer.SetPosition(1, b);
    }

    private void Update()
    {

    }
}
