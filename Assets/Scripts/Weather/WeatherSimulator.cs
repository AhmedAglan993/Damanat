using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeatherData
{
    public float temperature;
    public float precipitation;
    public float humidity;
    public float windSpeed;
    public float aqi;
    public string condition;
    public string carbonFootprint;
    public string viewDistance;   // e.g., "10km"
}

public class WeatherSimulator : MonoBehaviour
{
    public WeatherData currentWeather = new();
    public List<WeatherData> weeklyForecast = new();

    [Header("Simulation Settings")]
    public float updateInterval = 5f;

    private readonly string[] conditions = { "Clear", "Rainy", "Cloudy", "Foggy", "Stormy" };

    void Start()
    {
        InvokeRepeating(nameof(GenerateRandomWeather), 0f, updateInterval);
    }

    void GenerateRandomWeather()
    {
        currentWeather = new WeatherData
        {
            temperature = UnityEngine.Random.Range(-5f, 35f),
            precipitation = UnityEngine.Random.Range(0f, 100f),
            humidity = UnityEngine.Random.Range(30f, 90f),
            windSpeed = UnityEngine.Random.Range(5f, 25f),
            aqi = UnityEngine.Random.Range(10f, 100f),
            condition = conditions[UnityEngine.Random.Range(0, conditions.Length)],
            carbonFootprint = $"{UnityEngine.Random.Range(15f, 25f):F1}g CO₂/km²",
            viewDistance = $"{UnityEngine.Random.Range(5f, 20f):F0} km"
        };

        // Simulate 7-day forecast
        weeklyForecast.Clear();
        for (int i = 0; i < 7; i++)
        {
            weeklyForecast.Add(new WeatherData
            {
                temperature = UnityEngine.Random.Range(-5f, 35f),
                precipitation = UnityEngine.Random.Range(0f, 100f),
                humidity = UnityEngine.Random.Range(30f, 90f),
                windSpeed = UnityEngine.Random.Range(5f, 25f),
                aqi = UnityEngine.Random.Range(10f, 100f),
                condition = conditions[UnityEngine.Random.Range(0, conditions.Length)],
                carbonFootprint = $"{UnityEngine.Random.Range(15f, 25f):F1}g CO₂/km²",
                viewDistance = $"{UnityEngine.Random.Range(5f, 20f):F0} km"
            });

        }
        GetComponent<WeatherPanelUIController>().UpdateWeatherUI(currentWeather, weeklyForecast);

        Debug.Log("Weather updated!");
    }
}
