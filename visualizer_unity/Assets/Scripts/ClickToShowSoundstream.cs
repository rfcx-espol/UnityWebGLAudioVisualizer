using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ClickToShowSoundstream : MonoBehaviour {

    public ForestInput forest;
    public RadioMutex mutex;
    public MeshRenderer[] feedback;

   

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
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    for (int i = 0; i < feedback.Length; i++)
                        feedback[i].material.color = Color.red;
                }
            }
        }
	}

    void OnMouseDown()
    {
        if (!forest.IsActive)
        {
            try
            {
                mutex.do_set_station();
                for (int i = 0; i < feedback.Length; i++)
                    feedback[i].material.color = Color.green;
                forest.EnableVisualizer(mutex.station);
                loading = true;
                timer = 0;
            }
            catch(System.Exception e)
            {
                Debug.LogError("No se pudo conectar al servidor.");
            }
                            
        }
    }

    float timer;
    bool loading;
    float wait_for_load = 3.0f;
    WavesController w_controller;
}
