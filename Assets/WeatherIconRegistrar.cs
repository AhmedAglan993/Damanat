using UnityEngine;

public class WeatherIconRegistrar : MonoBehaviour
{
    [System.Serializable]
    public class ConditionIconPair
    {
        public string condition;
        public Sprite icon;
    }

    [Header("Condition to Icon Mapping")]
    public ConditionIconPair[] mappings;

    private void Awake()
    {
        foreach (var pair in mappings)
        {
            if (!string.IsNullOrEmpty(pair.condition) && pair.icon != null)
            {
                WeatherIconLibrary.Register(pair.condition, pair.icon);
            }
        }
    }
}
