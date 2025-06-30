#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif
using UnityEngine;

public class RecorderControllerScript : MonoBehaviour
{
#if UNITY_EDITOR
    RecorderController recorderController;

    void Start()
    {
        // Setup the recorder controller
        var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(controllerSettings);

        // Create a movie recorder
        var movieRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        movieRecorder.name = "MyVideoRecorder";
        movieRecorder.Enabled = true;

        // Output settings
        movieRecorder.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
        movieRecorder.VideoBitRateMode = VideoBitrateMode.Medium;
        movieRecorder.OutputFile = "Recordings/MyVideo_" + DefaultWildcard.Take;

        // Set image input from Game View
        movieRecorder.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = 1920,
            OutputHeight = 1080
        };

        // Set the recording mode (manual)
        controllerSettings.SetRecordModeToManual();
        controllerSettings.AddRecorderSettings(movieRecorder);

        // Start recording
        recorderController.PrepareRecording();
        recorderController.StartRecording();
    }

    void Update()
    {
        // Example: stop after 5 seconds
        if (recorderController.IsRecording() && Time.time > 5f)
        {
            recorderController.StopRecording();
            Debug.Log("Recording stopped.");
        }
    }
#endif
}
