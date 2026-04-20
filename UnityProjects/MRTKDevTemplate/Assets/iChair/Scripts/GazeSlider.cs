using MixedReality.Toolkit.UX;
using UnityEngine;

public class GazeSlider : MonoBehaviour
{
    [SerializeField]
    protected Slider slider = null;
    [SerializeField]
    protected float sensitivity = 0.5f;

    public float Value => slider.Value;

    public virtual void ResetState()
    {
        slider.Value = 0.5f;
    }

    protected virtual void Step(float scale)
    {
        slider.Value = Mathf.Clamp(slider.Value + scale * Time.deltaTime, slider.MinValue, slider.MaxValue);
    }

    public virtual void StepUp()
    {
        Step(sensitivity);
    }

    public virtual void StepDown()
    {
        Step(-sensitivity);
    }
}
