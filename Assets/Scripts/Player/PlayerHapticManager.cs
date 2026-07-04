using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class PlayerHapticManager : MonoBehaviour
{
    [SerializeField] HapticImpulsePlayer leftControllerHaptics;
    [SerializeField] HapticImpulsePlayer rightControllerHaptics;

    public void sendHaptic(float amplitude, float frequency, float duration)
    {
        leftControllerHaptics.SendHapticImpulse(amplitude, duration, frequency);
        rightControllerHaptics.SendHapticImpulse(amplitude, duration, frequency);
    }

}
