using UnityEngine;
using System.Collections.Generic;

public class ButtonLayout : MonoBehaviour
{
    public enum ArcDirection
    {
        Right,
        Top,
        Left,
        Bottom
    }

    [Header("References")]
    public Transform centerButton;

    [Tooltip("Buttons are paired by index with Labels.")]
    public List<Transform> outerButtons = new List<Transform>();

    [Tooltip("Labels are paired by index with Buttons.")]
    public List<Transform> labels = new List<Transform>();

    [Header("Layout")]
    public ArcDirection direction = ArcDirection.Top;

    [Range(0f, 360f)]
    public float spanAngle = 180f;

    [Header("Radii")]
    public float buttonRadius = 2f;
    public float labelRadius = 3f;

    private float GetDirectionAngle()
    {
        switch (direction)
        {
            case ArcDirection.Right: return 0f;
            case ArcDirection.Top: return 90f;
            case ArcDirection.Left: return 180f;
            case ArcDirection.Bottom: return 270f;
            default: return 90f;
        }
    }

    [ContextMenu("Update Layout")]
    public void UpdateLayout()
    {
        if (centerButton == null)
            return;

        int count = outerButtons.Count;
        if (count == 0)
            return;

        Vector3 centerPos = centerButton.position;

        float centerAngle = GetDirectionAngle();
        float startAngle = centerAngle + spanAngle * 0.5f;
        float endAngle = centerAngle - spanAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Transform button = outerButtons[i];
            if (button == null)
                continue;

            float t = (count == 1) ? 0.5f : (float)i / (count - 1);
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            button.position = new Vector3(
                centerPos.x + dir.x * buttonRadius,
                centerPos.y + dir.y * buttonRadius,
                button.position.z
            );

            if (i < labels.Count && labels[i] != null)
            {
                Transform label = labels[i];

                label.position = new Vector3(
                    centerPos.x + dir.x * labelRadius,
                    centerPos.y + dir.y * labelRadius,
                    label.position.z
                );
            }
        }
    }

    private void OnValidate()
    {
        UpdateLayout();
    }

    private void Start()
    {
        UpdateLayout();
    }
}
