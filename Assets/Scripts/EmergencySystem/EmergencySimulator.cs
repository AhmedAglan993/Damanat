using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencySimulator : MonoBehaviour
{
    public float simulationInterval = 10f; // Time between alarms
    public bool autoStart = true;

    public List<EmergencyRoomTrigger> allRooms = new();
    private Coroutine simulationRoutine;

    void Start()
    {
        // Collect all room triggers in the scene
        allRooms.AddRange(FindObjectsOfType<EmergencyRoomTrigger>());

        if (autoStart)
            StartSimulation();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SimulateTrigger();
            print("Ddddd");
        }
    }
    [ContextMenu("Start Simulation")]
    public void StartSimulation()
    {
        if (simulationRoutine == null)
            simulationRoutine = StartCoroutine(SimulateAlarms());
    }

    [ContextMenu("Stop Simulation")]
    public void StopSimulation()
    {
        if (simulationRoutine != null)
        {
            StopCoroutine(simulationRoutine);
            simulationRoutine = null;
        }
    }

    IEnumerator SimulateAlarms()
    {
        yield return new WaitForSeconds(2);

        while (true)
        {
            if (allRooms.Count == 0)
            {
                Debug.LogWarning("[EmergencySimulator] No EmergencyRoomTriggers found.");
                yield break;
            }

            // Randomly select a room
            SimulateTrigger();

            yield return new WaitForSeconds(simulationInterval);
        }
    }

    private void SimulateTrigger()
    {
        EmergencyRoomTrigger randomRoom = allRooms[Random.Range(0, allRooms.Count)];

        // Trigger the alarm
        if (randomRoom != null)
        {
            Debug.Log($"[Simulator] Triggering alarm in {randomRoom.roomName}");
            randomRoom.TriggerAlarm();
        }
    }
}
