using UnityEngine;

[ExecuteAlways]
public class EvenlySpacedTransforms : MonoBehaviour
{
    [Header("Reference (left object)")]
    public Transform reference;

    [Header("Objects placed to the right")]
    public Transform[] objects;

    [Header("Layout")]
    [Min(0f)]
    public float spacing = 2f;

    public Vector3 direction = Vector3.right;

    private void OnValidate()
    {
        Arrange();
    }

    public void Arrange()
    {
        if (reference == null || objects == null)
            return;

        if (direction == Vector3.zero)
            return;

        Vector3 dir = direction.normalized;

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null)
                continue;

            objects[i].position =
                reference.position + dir * spacing * (i + 1);
        }
    }
}
