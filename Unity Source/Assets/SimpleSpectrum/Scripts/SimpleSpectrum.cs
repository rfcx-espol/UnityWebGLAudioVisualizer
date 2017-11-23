using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

using System; //REMOVE THISSSSS

public class SimpleSpectrum : MonoBehaviour {
    /// <summary>
    /// Enables or disables the processing and display of spectrum data. 
    /// </summary>
	public bool isEnabled = true;

    #region SAMPLING PROPERTIES
    /// <summary>
    /// The AudioSource to take data from. Can be null if useListenerInstead is true.
    /// </summary>
	public AudioSource audioSource;
    /// <summary>
    /// If true, take audio data from the AudioListener instead.
    /// </summary>
	public bool useListenerInstead = false;
    /// <summary>
    /// The audio channel to use when sampling.
    /// </summary>
	public int sampleChannel = 0;
    /// <summary>
    /// The number of samples to use when sampling. Must be a power of two.
    /// </summary>
	public int numSamples = 256;
    /// <summary>
    /// The FFTWindow to use when sampling.
    /// </summary>
    public FFTWindow windowUsed = FFTWindow.BlackmanHarris;
    /// <summary>
    /// If true, audio data is scaled logarithmically.
    /// </summary>
    public bool useLogarithmicFrequency = true;
    /// <summary>
    /// If true, the values of the spectrum are multiplied based on their frequency, to keep the values proportionate.
    /// </summary>
    public bool multiplyByFrequency = true;
    /// <summary>
    /// Determines what percentage of the full frequency range to use (1 being the full range, reducing the value towards 0 cuts off high frequencies).
    /// This can be useful when using MP3 files or audio with missing high frequencies.
    /// </summary>
    public float highFrequencyTrim = 1;
    /// <summary>
    /// When useLogarithmicFrequency is false, this value stretches the spectrum data onto the bars.
    /// </summary>
    public float linearSampleStretch = 1;
    #endregion 

    #region BAR PROPERTIES
    /// <summary>
    /// The amount of bars to use.
    /// </summary>
	public int barAmount = 32;
    /// <summary>
    /// Stretches the values of the bars.
    /// </summary>
    public float barYScale = 50;
    /// <summary>
    /// Sets a minimum scale for the bars; they will never go below this scale.
    /// This value is also used when isEnabled is false.
    /// </summary>
    public float barMinYScale = 0.1f;
    /// <summary>
    /// The prefab of bar to use when building.
    /// Refer to the documentation to use a custom prefab.
    /// </summary>
	public GameObject barPrefab;
    /// <summary>
    /// Stretches the bars sideways. 
    /// </summary>
    public float barXScale = 1;
    /// <summary>
    /// Increases the spacing between bars.
    /// </summary>
    public float barXSpacing = 0;
    /// <summary>
    /// Bends the Spectrum using a given angle.
    /// </summary>
    public float barCurveAngle = 0;
    /// <summary>
    /// Rotates the Spectrum inwards or outwards. Especially useful when using barCurveAngle.
    /// </summary>
    public float barXRotation = 0;
    /// <summary>
    /// The amount of dampening used when the new scale is higher than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
	[Range(0,1)]
	public float attackDamp = 0.25f;
    /// <summary>
    /// The amount of dampening used when the new scale is lower than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
	[Range(0,1)]
    public float decayDamp = 0.3f;
    #endregion

    #region COLOR PROPERTIES
    /// <summary>
    /// Determines whether to apply a color gradient on the bars, or just use colorMin as a solid color.
    /// </summary>
	public bool useColorGradient = false;
    /// <summary>
    /// The minimum (low value) color if useColorGradient is true, else the solid color to use.
    /// </summary>
	public Color colorMin = Color.black;
    /// <summary>
    /// The maximum (high value) color if useColorGradient is true.
    /// </summary>
    public Color colorMax = Color.white;
    /// <summary>
    /// The curve that determines the interpolation between colorMin and colorMax.
    /// </summary>
    public AnimationCurve colorValueCurve;
    /// <summary>
    /// The amount of dampening used when the new color value is higher than the existing color value. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    public float colorAttackDamp = 1;
    /// <summary>
    /// The amount of dampening used when the new color value is lower than the existing color value. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    public float colorDecayDamp = 1;
    #endregion

    float lograithmicAmplitudePower = 2, multiplyByFrequencyPower = 1.5f;
    float[] spectrum; 
	Transform[] bars;
    Material[] barMaterials; //optimisation
    float[] oldYScales; //also optimisation
    float[] oldColorValues; //...optimisation
    int materialValId;

	float logFreqMultiplier, highestLogFreq; //multiplier to ensure that the log-ified frequencies stretch to the highest record in the array.

	void Start () {
		if(audioSource == null && !useListenerInstead){
			Debug.LogError ("An audio source has not been assigned. Please assign a reference to a source, or set useAudioListener instead.");
		}

        RebuildSpectrum();
	}

    /// <summary>
    /// Rebuilds this instance of Spectrum, applying any changes.
    /// </summary>
    public void RebuildSpectrum()
    {
        isEnabled = false;	//just in case

        //clear all the existing children
        int childs = transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        numSamples = Mathf.ClosestPowerOfTwo(numSamples);
        
        //initialise arrays
        spectrum = new float[numSamples];
        bars = new Transform[barAmount];
        barMaterials = new Material[barAmount];
        oldYScales = new float[barAmount];
        oldColorValues = new float[barAmount];


        float spectrumLength = barAmount * (1 + barXSpacing);
        float midPoint = spectrumLength / 2;

        //spectrum bending calculations
        float curveAngleRads = 0, curveRadius = 0, halfwayAngleR = 0, halfwayAngleD = 0;
        Vector3 curveCentreVector = Vector3.zero;
        if (barCurveAngle > 0)
        {
            curveAngleRads = (barCurveAngle / 360) * (2 * Mathf.PI);
            curveRadius = spectrumLength / curveAngleRads;

            halfwayAngleR = curveAngleRads / 2;
            halfwayAngleD = barCurveAngle / 2;
            curveCentreVector = new Vector3(0, 0, 1 * curveRadius);
            curveCentreVector = curveCentreVector * -1;
        }

        for (int i = 0; i < barAmount; i++)
        {
            GameObject barClone = Instantiate(barPrefab, transform, false) as GameObject; //create the bars and assign the parent
            //barClone.name = i.ToString();
            barClone.transform.localScale = new Vector3(barXScale, barMinYScale, 1);

            if (barCurveAngle > 0) //apply spectrum bending
            {
                float position = ((float)i / barAmount);
                float thisBarAngleR = (position * curveAngleRads) - halfwayAngleR;
                float thisBarAngleD = (position * barCurveAngle) - halfwayAngleD;
                barClone.transform.localPosition = new Vector3(Mathf.Sin(thisBarAngleR) * curveRadius, 0, Mathf.Cos(thisBarAngleR) * curveRadius) + curveCentreVector;
                barClone.transform.localRotation = Quaternion.Euler(barXRotation, thisBarAngleD, 0);
            }
            else //standard positioning
            {
                barClone.transform.localPosition = new Vector3(i * (1 + barXSpacing) - midPoint, 0, 0);
            }

            bars[i] = barClone.transform;
            barMaterials[i] = barClone.transform.GetChild(0).GetComponent<Renderer>().material;

            int color1Id = Shader.PropertyToID("_Color1"), color2Id = Shader.PropertyToID("_Color2");
            barMaterials[i].SetColor(color1Id, colorMin);
            barMaterials[i].SetColor(color2Id, colorMax);
        }

        materialValId = Shader.PropertyToID("_Val");

        highestLogFreq = Mathf.Log(barAmount + 1, 2); //gets the highest possible logged frequency, used to calculate which sample of the spectrum to use for a bar
        logFreqMultiplier = numSamples / highestLogFreq;

        isEnabled = true;
    }

	void Update () {
		if (isEnabled) {

            //sampleChannel = Mathf.Clamp(sampleChannel, 0, 1); //force the channel to be valid

			if(useListenerInstead){
                AudioListener.GetSpectrumData(spectrum, sampleChannel, windowUsed); //get the spectrum data
			}else{
                audioSource.GetSpectrumData(spectrum, sampleChannel, windowUsed); //get the spectrum data
			}


#if UNITY_EDITOR    //allows for editing curve while in play mode, disabled in build for optimisation

                float spectrumLength = bars.Length * (1 + barXSpacing);

                float midPoint = spectrumLength / 2;

                float curveAngleRads = 0, curveRadius = 0, halfwayAngleR = 0, halfwayAngleD = 0;
                Vector3 curveCentreVector = Vector3.zero;
                if (barCurveAngle > 0)
                {
                    curveAngleRads = (barCurveAngle / 360) * (2 * Mathf.PI);
                    curveRadius = spectrumLength / curveAngleRads;

                    halfwayAngleR = curveAngleRads / 2;
                    halfwayAngleD = barCurveAngle / 2;
                    curveCentreVector = new Vector3(0, 0, 1 * curveRadius);
                    curveCentreVector = curveCentreVector * -1;
                }
#endif

			for (int i = 0; i < bars.Length; i++) {
				Transform bar = bars [i];

				float value;
                float trueSampleIndex;

                //GET SAMPLES
				if (useLogarithmicFrequency) {
					//LOGARITHMIC FREQUENCY SAMPLING
                    trueSampleIndex = highFrequencyTrim * (highestLogFreq - Mathf.Log(bars.Length + 1 - i, 2)) * logFreqMultiplier; //gets the index equiv of the logified frequency
                    
                    //^that really needs explaining.
                    //'logarithmic frequencies' just means we want more of the lower frequencies and less of the high ones.
                    //a normal log2 graph will quickly go past 1-5 and spend much more time on stuff above that, but we want the opposite
                    //so by doing log2(max(i)) - log2(max(i) - i), we get a flipped log graph
                    //(make a graph of log2(64)-log2(64-x) to see what I mean)
                    //this isn't finished though, because that graph doesn't actually map the bar index (x) to the spectrum index (y).
                    //logFreqMultiplier stretches the grpah upwards so that the highest value (log2(max(i)))hits the highest frequency.
                    //also 1 gets added to barAmount pretty much everywhere, because without it, the log hits (barAmount-1,max(freq))

                } else {
					//LINEAR (SCALED) FREQUENCY SAMPLING 
                    trueSampleIndex = i * linearSampleStretch;
				}

                //the true sample is usually a decimal, so we need to lerp between the floor and ceiling of it.

                int sampleIndexFloor = Mathf.FloorToInt(trueSampleIndex);
                sampleIndexFloor = Mathf.Clamp(sampleIndexFloor, 0, spectrum.Length - 2); //just keeping it within the spectrum array's range

                float sampleIndexDecimal = trueSampleIndex % 1; //gets the decimal point of the true sample, for lerping

                value = Mathf.SmoothStep(spectrum[sampleIndexFloor], spectrum[sampleIndexFloor + 1], sampleIndexDecimal); //smoothly interpolate between the two samples using the true index's decimal.

                //MANIPULATE & APPLY SAMPLES
                if (multiplyByFrequency) //multiplies the amplitude by the true sample index
                {
                    value = value * (trueSampleIndex+1);
                }

                value = Mathf.Sqrt(value); //compress the amplitude values by sqrt(x)


                //DAMPENING
                //Vector3 oldScale = bar.localScale;
                float oldYScale = oldYScales[i], newYScale;
                if (value * barYScale > oldYScale)
                {
                    newYScale = Mathf.Lerp(oldYScale, Mathf.Max(value * barYScale, barMinYScale), attackDamp);
				} else {
                    newYScale = Mathf.Lerp(oldYScale, Mathf.Max(value * barYScale, barMinYScale), decayDamp);
				}

                bar.localScale = new Vector3(barXScale,newYScale,1);

                oldYScales[i] = newYScale;

                //set colour
                if (useColorGradient)
                {
                    float newColorVal = colorValueCurve.Evaluate(value);
                    float oldColorVal = oldColorValues[i];

                    if (newColorVal > oldColorVal)
                    {
                        if (colorAttackDamp != 1)
                        {
                            newColorVal = Mathf.Lerp(oldColorVal, newColorVal, colorAttackDamp);
                        }
                    }
                    else
                    {
                        if (colorDecayDamp != 1)
                        {
                            newColorVal = Mathf.Lerp(oldColorVal, newColorVal, colorDecayDamp);
                        }
                    }

                    barMaterials[i].SetFloat(materialValId, newColorVal);

                    oldColorValues[i] = newColorVal;
                }

#if UNITY_EDITOR
                //realtime modifications for Editor only
                if (barCurveAngle > 0)
                {
                    float position = ((float)i / bars.Length);
                    float thisBarAngleR = (position * curveAngleRads) - halfwayAngleR;
                    float thisBarAngleD = (position * barCurveAngle) - halfwayAngleD;
                    bar.localRotation = Quaternion.Euler(barXRotation, thisBarAngleD, 0);
                    bar.localPosition = new Vector3(Mathf.Sin(thisBarAngleR) * curveRadius, 0, Mathf.Cos(thisBarAngleR) * curveRadius) + curveCentreVector;
                }
                else
                {
                    bar.localPosition = new Vector3(i * (1 + barXSpacing) - midPoint, 0, 0);
                }
#endif
			}

		}else{ //switched off
			foreach (Transform bar in bars) {
                bar.localScale = Vector3.Lerp(bar.localScale, new Vector3(1, barMinYScale, 1), decayDamp);
			}
		}
	}

    /// <summary>
    /// Returns a logarithmically scaled and proportionate array of spectrum data from the AudioSource.
    /// </summary>
    /// <param name="source">The AudioSource to take data from.</param>
    /// <param name="spectrumSize">The size of the returned array.</param>
    /// <param name="sampleSize">The size of sample to take from the AudioSource. Must be a power of two.</param>
    /// <param name="windowUsed">The FFTWindow to use when sampling.</param>
    /// <param name="channelUsed">The audio channel to use when sampling.</param>
    /// <returns>A logarithmically scaled and proportionate array of spectrum data from the AudioSource.</returns>
    public static float[] GetLogarithmicSpectrumData(AudioSource source, int spectrumSize, int sampleSize, FFTWindow windowUsed = FFTWindow.BlackmanHarris, int channelUsed = 0)
    {
        float[] spectrum = new float[spectrumSize];

        channelUsed = Mathf.Clamp(channelUsed, 0, 1);
        float[] samples = new float[Mathf.ClosestPowerOfTwo(sampleSize)];
        source.GetSpectrumData(samples, channelUsed, windowUsed);

        float highestLogSampleFreq = Mathf.Log(spectrum.Length + 1, 2); //gets the highest possible logged frequency, used to calculate which sample of the spectrum to use for a bar

        float logSampleFreqMultiplier = sampleSize / highestLogSampleFreq;

        for (int i = 0; i < spectrum.Length; i++) //for each float in the output
        {

            float trueSampleIndex = (highestLogSampleFreq - Mathf.Log(spectrum.Length + 1 - i, 2)) * logSampleFreqMultiplier; //gets the index equiv of the logified frequency

            //the true sample is usually a decimal, so we need to lerp between the floor and ceiling of it.

            int sampleIndexFloor = Mathf.FloorToInt(trueSampleIndex);
            sampleIndexFloor = Mathf.Clamp(sampleIndexFloor, 0, samples.Length - 2); //just keeping it within the spectrum array's range
            float sampleIndexDecimal = trueSampleIndex % 1; //gets the decimal point of the true sample, for lerping

            float value = Mathf.SmoothStep(spectrum[sampleIndexFloor], spectrum[sampleIndexFloor + 1], sampleIndexDecimal); //smoothly interpolate between the two samples using the true index's decimal.

            value = value * trueSampleIndex; //multiply value by its position to make it proportionate;

            value = Mathf.Sqrt(value); //compress the amplitude values by sqrt(x)

            spectrum[i] = value;
        }
        return spectrum;
    }

}
