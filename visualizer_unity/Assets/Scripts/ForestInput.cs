using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestInput : MonoBehaviour {

    public CanvasGroup waveCanvas;
    public BrowserCommunication comm;
    
    //property IsActive to know if the visualizator is currently active.
    public bool IsActive{ get { return active; } }

	// Use this for initialization
	void Start () {
        w_controller = waveCanvas.GetComponent<WavesController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Cancel"))
            DisableWaves();
        if (Input.GetKeyDown(KeyCode.Tab))
            w_controller.Enlarge();
	}

    //This disables the wave visualization and hides the UICanvas
    void DisableWaves()
    {
        comm.stop_current_station();
        active = false;
        waveCanvas.alpha = 0;
    }

    //This enables the visualizer and shows the UICanvas panel
    public void EnableVisualizer(int station)
    {
        Debug.Log("ENABLED");
        w_controller.setSourceName("Station " + station);
        active = true;
        waveCanvas.alpha = 1;
    }

    bool active;
    WavesController w_controller;
}
