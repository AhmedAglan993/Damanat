using System;

[Serializable]
public class UserPreferences
{
    public bool useHoverToOpenMenu = true;
    public bool enableHotspotsByDefault = true;
    public bool autoFocusOnSelection = true;
    public string defaultFloor = "Ground Floor";
    public bool filterCriticalAlertsOnly = false;

    public ThemeMode theme = ThemeMode.Auto;
    public CameraAngle defaultCameraAngle = CameraAngle.BirdEye;
    public bool playSoundOnAlerts = true;
    public MovementType navigationMode = MovementType.WASD;
}

public enum ThemeMode { Light, Dark, Auto }
public enum CameraAngle { BirdEye, FloorView, Isometric, FreeOrbit }
public enum MovementType { WASD, Orbit, ClickToMove }
