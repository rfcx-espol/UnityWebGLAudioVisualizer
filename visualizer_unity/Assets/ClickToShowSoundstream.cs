using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ClickToShowSoundstream : MonoBehaviour {

    public ForestInput forest;
    public RadioMutex mutex;

   

    // Use this for initialization
    void Start () {
        timer = 0;
        w_controller = forest.waveCanvas.GetComponent<WavesController>();

    }
	
	// Update is called once per frame
	void Update () {
        if (forest.IsActive)
        {
            timer += Time.deltaTime;
            if (timer >= wait_for_load && loading)
            {
                loading = false;
                w_controller.doStart();
            }
        }
	}

    void OnMouseDown()
    {
        if (!forest.IsActive)
        {
            forest.EnableVisualizer(mutex.station);
            loading = true;
            timer = 0;
            mutex.do_set_station();          
        }
    }

    float timer;
    bool loading;
    float wait_for_load = 3.0f;
    WavesController w_controller;
}
