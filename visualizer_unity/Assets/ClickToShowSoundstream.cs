using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ClickToShowSoundstream : MonoBehaviour {

    public ForestInput forest;
    public string pathToAudio;

    [DllImport("__Internal")]
    private static extern void SendAudioPath(string str);

    // Use this for initialization
    void Start () {
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (forest.IsActive)
        {
            timer += Time.deltaTime;
            if (timer >= wait_for_load && loading)
            {
                loading = false;
                forest.EnableWaves();
                forest.spectrum.do_start();
                forest.data.do_start();
            }
        }
	}

    void OnMouseDown()
    {
        if (!forest.IsActive)
        {
            SendAudioPath(pathToAudio);
            loading = true;
            timer = 0;          
        }
    }

    float timer;
    bool loading;
    float wait_for_load = 3.0f;
}
