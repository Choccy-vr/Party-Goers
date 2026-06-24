using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class UIHapticFeedback : MonoBehaviour
{
    [SerializeField] HapticImpulsePlayer controllerHaptics;
    [Range(0f, 1f)]
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float duration = 0.1f;

    public void onUIHover()
    {
        controllerHaptics.SendHapticImpulse(amplitude, duration);
    }
}
