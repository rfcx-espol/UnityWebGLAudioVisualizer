﻿// ---------------------------------------
// Spectrum Visualizer code by Bad Raccoon
// (C)opyRight 2017/2018 By :
// Bad Raccoon / Creepy Cat / Barking Dog 
// ---------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Text;

public class OutputVis : MonoBehaviour {
	public GameObject[] cubes;
	public Color barColor;
	public float sizePower = 20;

	public enum axisStrech {dx, dy, dz, dyAndDz, all};
	public axisStrech stretchAxis=axisStrech.dy;

	public enum channelColour {red, green, blue, all};
	public channelColour currentChannel= channelColour.red;

	private float currentRed;
	private float currentGreen;
	private float currentBlue;

	public float colorPower = 12;

    public BrowserCommunication comm;
    public Text debugText;
    public float updateTime;

    bool is_running;

    public void do_start()
    {
        is_running = true;
    }

    void Start(){
		currentRed = barColor.r;
		currentGreen = barColor.g;
		currentBlue = barColor.b;
	}

	void Update () {

        if (!is_running) return;

        //float[] spects = AudioListener.GetSpectrumData (1024, 0, FFTWindow.Rectangular);
        float[] spects;

        if (_update_timer <= 0)
        {
            spects = comm.GetSamples();
            _update_timer = updateTime;
            //StringBuilder str = new StringBuilder();

            //for (int i = 0; i < 10; i++)
            //{
            //    str.Append(spects[i] + "\n");
            //}

            //debugText.text = str.ToString();

            for (int i = 0; i < cubes.Length; i++)
            {

                // Save the old size
                Vector3 previousScale = cubes[i].transform.localScale;

                // The new size
                if (stretchAxis == axisStrech.dx)
                {
                    previousScale.x = spects[i] * sizePower;
                }

                if (stretchAxis == axisStrech.dy)
                {
                    previousScale.y = spects[i] * sizePower;
                }

                if (stretchAxis == axisStrech.dz)
                {
                    previousScale.z = spects[i] * sizePower;
                }

                if (stretchAxis == axisStrech.dyAndDz)
                {
                    previousScale.y = spects[i] * sizePower;
                    previousScale.z = spects[i] * sizePower;
                }

                if (stretchAxis == axisStrech.all)
                {
                    previousScale.x = spects[i] * sizePower;
                    previousScale.y = spects[i] * sizePower;
                    previousScale.z = spects[i] * sizePower;
                }

                // Reset size
                cubes[i].transform.localScale = previousScale;

                //if (i == 0) {
                //	Debug.Log (spects [i]);
                //}

                // Colour change
                if (currentChannel == channelColour.red)
                {
                    barColor.r = currentRed + spects[i] * colorPower;
                }

                if (currentChannel == channelColour.green)
                {
                    barColor.g = currentGreen + spects[i] * colorPower;
                }

                if (currentChannel == channelColour.blue)
                {
                    barColor.b = currentBlue + spects[i] * colorPower;
                }

                if (currentChannel == channelColour.all)
                {
                    barColor.b = currentBlue + (spects[i] * colorPower);
                    barColor.g = currentGreen + (spects[i] * colorPower);
                    barColor.r = currentRed + (spects[i] * colorPower);
                }

                cubes[i].GetComponent<Renderer>().material.color = barColor;
            }
        }
        else
        {
            _update_timer -= Time.deltaTime;
        }
    }

    float _update_timer;
}