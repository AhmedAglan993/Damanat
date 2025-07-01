using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherPanelUIController : MonoBehaviour
{
    [Header("AQI")]
    [SerializeField] private Image aqiRing;
    [SerializeField] private TMP_Text aqiText;
    [SerializeField] private TMP_Text aqiLevel;
    [SerializeField] private TMP_Text weatherCondition;
    [SerializeField] private TMP_Text carbonFootprint;
    [SerializeField] private TMP_Text viewDistance;

    [Header("Current Weather")]
    [SerializeField] private TMP_Text currentTempText;
    [SerializeField] private TMP_Text precipitationText;
    [SerializeField] private TMP_Text humidityText;
    [SerializeField] private TMP_Text windText;
    [SerializeField] private Image currentWeatherIcon;

    [Header("Forecast (7 Days)")]
    [SerializeField] private List<TMP_Text> dayLabels;
    [SerializeField] private List<TMP_Text> rangeLabels;
    [SerializeField] private List<Image> forecastIcons;

    [Header("Graph")]
    [SerializeField] private WeatherGraphRenderer graphRenderer;

    private readonly string[] days = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

    public void UpdateWeatherUI(WeatherData current, List<WeatherData> forecast)
    {
        if (current == null || forecast == null || forecast.Count < 7)
        {
            Debug.LogWarning("Weather data is incomplete.");
            return;
        }

        // AQI Section
        float aqiNormalized = current.aqi / 100f;
        aqiRing.fillAmount = aqiNormalized;
        aqiText.text = $"{current.aqi:F0}%";
        aqiLevel.text = GetAQILevel(current.aqi);
        weatherCondition.text = current.condition;
        carbonFootprint.text = current.carbonFootprint;
        viewDistance.text = current.viewDistance;

        // Current Stats
        currentTempText.text = $"{current.temperature:F0}°C";
        precipitationText.text = $"{current.precipitation:F0}%";
        humidityText.text = $"{current.humidity:F0}%";
        windText.text = $"{current.windSpeed:F0}km/h";
        currentWeatherIcon.sprite = WeatherIconLibrary.GetIcon(current.condition);

        // Forecast
        for (int i = 0; i < 7; i++)
        {
            var day = forecast[i];
            dayLabels[i].text = days[i];
            rangeLabels[i].text = $"{day.temperature - 2:F0}° - {day.temperature + 2:F0}°"; // Simulated range
            forecastIcons[i].sprite = WeatherIconLibrary.GetIcon(day.condition);
        }

        graphRenderer.SetForecast(forecast);
        graphRenderer.SwitchMode("Wind");

       
    }

    private string GetAQILevel(float aqi)
    {
        if (aqi < 25) return "Good";
        if (aqi < 50) return "Moderate";
        return "Unhealthy";
    }
}
public static class WeatherIconLibrary
{
    private static Dictionary<string, Sprite> iconMap = new();

    public static void Register(string condition, Sprite sprite)
    {
        iconMap[condition] = sprite;
    }

    public static Sprite GetIcon(string condition)
    {
        if (iconMap.TryGetValue(condition, out var icon)) return icon;
        return null;
    }
}
