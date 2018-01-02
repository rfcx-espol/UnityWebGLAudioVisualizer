using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestInput : MonoBehaviour {

    public SpectrumVis spectrum;
    public OutputVis data;
    

    public bool IsActive{ get { return active; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Cancel"))
            DisableWaves();
	}

    void DisableWaves()
    {
        active = false;
        spectrum.gameObject.SetActive(false);
        data.gameObject.SetActive(false);
    }

    public void EnableWaves()
    {
        active = true;
        spectrum.gameObject.SetActive(true);
        data.gameObject.SetActive(true);
    }

    bool active;
}
