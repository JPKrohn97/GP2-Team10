using MoreMountains.NiceVibrations;


public static class ManagerVibration
{
    public static bool PreventVibration = false;

    public static void Vibrate(HapticTypes targetHapticType)
    {
        if (!PreventVibration)
        {
            MMVibrationManager.Haptic(targetHapticType);
        }
    }


}

