using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleSpectrum))]
[CanEditMultipleObjects]
public class SimpleSpectrumEditor : Editor {
	SerializedProperty  propertyWindow;

    SerializedProperty propertyEnabled;

	SerializedProperty propertyAudioSource;
	SerializedProperty propertyUseListenerInstead;

	SerializedProperty propertyAttackDamp;
	SerializedProperty propertyDecayDamp;

    SerializedProperty propertyNumSamples;
    SerializedProperty propertySampleChannel;
    SerializedProperty propertyUseLogFreq;
    SerializedProperty propertyLinearSampleStretch;
    SerializedProperty propertyMultiplyByFreq;
    SerializedProperty propertyHighFreqTrim;

    SerializedProperty propertyBarPrefab;
    SerializedProperty propertyBarAmount;
    SerializedProperty propertyBarYScale;
    SerializedProperty propertyBarMinYScale;
    SerializedProperty propertyBarXScale;
    SerializedProperty propertyBarXSpacing;
    SerializedProperty propertyBarPositionAngle;
    SerializedProperty propertyBarXRotation;

	SerializedProperty propertyUseColorGradient;
	SerializedProperty propertyColorMin;
	SerializedProperty propertyColorMax;
    SerializedProperty propertyColorCurve;
    SerializedProperty propertyColorAttackDamp;
    SerializedProperty propertyColorDecayDamp;

	bool foldoutSpectrumOpen = true;
    bool foldoutSamplingOpen = true;
	bool foldoutBarsOpen = true;
	bool foldoutColorsOpen = true;


    /*void NotifyPropertyChanged()
    {
        if (Application.isPlaying)
        {
            Debug.Log("Something's changed, rebuilding spectrum");
            ((SimpleSpectrum)target).RebuildSpectrum();
        }
	}*/

	void OnEnable(){
        propertyEnabled = serializedObject.FindProperty("isEnabled");

		propertyAudioSource = serializedObject.FindProperty ("audioSource");
		propertyUseListenerInstead = serializedObject.FindProperty ("useListenerInstead");
		propertyAttackDamp = serializedObject.FindProperty ("attackDamp");
		propertyDecayDamp = serializedObject.FindProperty ("decayDamp");

        propertyNumSamples = serializedObject.FindProperty("numSamples");
        propertySampleChannel = serializedObject.FindProperty("sampleChannel");
        propertyUseLogFreq = serializedObject.FindProperty("useLogarithmicFrequency");
        propertyLinearSampleStretch = serializedObject.FindProperty("linearSampleStretch");
        propertyMultiplyByFreq = serializedObject.FindProperty("multiplyByFrequency");
        propertyHighFreqTrim = serializedObject.FindProperty("highFrequencyTrim");
        propertyWindow = serializedObject.FindProperty("windowUsed");

        propertyBarPrefab = serializedObject.FindProperty("barPrefab");
        propertyBarAmount = serializedObject.FindProperty("barAmount");
        propertyBarYScale = serializedObject.FindProperty("barYScale");
        propertyBarMinYScale = serializedObject.FindProperty("barMinYScale");
        propertyBarXScale = serializedObject.FindProperty("barXScale");
        propertyBarXSpacing = serializedObject.FindProperty("barXSpacing");
        propertyBarPositionAngle = serializedObject.FindProperty("barCurveAngle");
        propertyBarXRotation = serializedObject.FindProperty("barXRotation");

        propertyUseColorGradient = serializedObject.FindProperty("useColorGradient");
        propertyColorMin = serializedObject.FindProperty("colorMin");
        propertyColorMax = serializedObject.FindProperty("colorMax");
        propertyColorCurve = serializedObject.FindProperty("colorValueCurve");
        propertyColorAttackDamp = serializedObject.FindProperty("colorAttackDamp");
        propertyColorDecayDamp = serializedObject.FindProperty("colorDecayDamp");
	}


	public override void OnInspectorGUI(){
		serializedObject.Update ();

		EditorGUILayout.LabelField ("A simple audio spectum generator by Sam Boyer.", new GUIStyle{ fontSize = 10});

        EditorGUILayout.PropertyField(propertyEnabled);

		foldoutSpectrumOpen = EditorGUILayout.Foldout (foldoutSpectrumOpen,"Spectrum Settings");
		if(foldoutSpectrumOpen){

			if(!propertyUseListenerInstead.boolValue){
				EditorGUILayout.PropertyField (propertyAudioSource);
			}

            propertyUseListenerInstead.boolValue = EditorGUILayout.Toggle("Use AudioListener instead?", propertyUseListenerInstead.boolValue);

			EditorGUILayout.PropertyField (propertyAttackDamp);
			EditorGUILayout.PropertyField (propertyDecayDamp);


            foldoutSamplingOpen = EditorGUILayout.Foldout (foldoutSamplingOpen,"Sampling Settings");
            if (foldoutSamplingOpen)
            {
                EditorGUILayout.PropertyField(propertyNumSamples);
                propertySampleChannel.intValue = EditorGUILayout.IntField("Sample Channel", propertySampleChannel.intValue);
                //Mathf.Clamp(propertySampleChannel.intValue, 0, 1);

                EditorGUILayout.PropertyField(propertyUseLogFreq);

                if (propertyUseLogFreq.boolValue)
                {
                    propertyHighFreqTrim.floatValue = EditorGUILayout.Slider("High Frequency Trim", propertyHighFreqTrim.floatValue, 0, 1);
                    //EditorGUILayout.PropertyField(propertyHighFreqTrim);
                }
                else
                {
                    EditorGUILayout.PropertyField(propertyLinearSampleStretch);
                }

                EditorGUILayout.PropertyField(propertyMultiplyByFreq); 

                EditorGUILayout.PropertyField(propertyWindow);
            }
		}

		foldoutBarsOpen = EditorGUILayout.Foldout (foldoutBarsOpen,"Bar Settings");
		if (foldoutBarsOpen) {
            EditorGUILayout.PropertyField (propertyBarAmount);
			EditorGUILayout.PropertyField (propertyBarPrefab);
            EditorGUILayout.PropertyField (propertyBarYScale);
            EditorGUILayout.PropertyField(propertyBarMinYScale);
            EditorGUILayout.PropertyField(propertyBarXScale);
            EditorGUILayout.PropertyField(propertyBarXSpacing);
            EditorGUILayout.PropertyField(propertyBarXRotation);
            propertyBarPositionAngle.floatValue = EditorGUILayout.Slider("Specrum Bend Angle", propertyBarPositionAngle.floatValue, 0, 360);

		}

		foldoutColorsOpen = EditorGUILayout.Foldout (foldoutColorsOpen,"Color Settings");
		if (foldoutColorsOpen) {

			EditorGUILayout.PropertyField (propertyUseColorGradient);
			propertyColorMin.colorValue = EditorGUILayout.ColorField(propertyUseColorGradient.boolValue ? "Minimum Color" : "Color", propertyColorMin.colorValue);
			if(propertyUseColorGradient.boolValue){
				propertyColorMax.colorValue = EditorGUILayout.ColorField("Maximum Color", propertyColorMax.colorValue);

                EditorGUILayout.PropertyField(propertyColorCurve);

                EditorGUILayout.PropertyField(propertyColorAttackDamp);
                EditorGUILayout.PropertyField(propertyColorDecayDamp);
			}
		}

		if(GUILayout.Button("Rebuild Spectrum")){
            Rebuild();
		}

		serializedObject.ApplyModifiedProperties ();
	}

    private void Rebuild()
    {
        if (Application.isPlaying)
        {
            ((SimpleSpectrum)target).RebuildSpectrum();
        }
    }

    void OnValidate()
    {
        
    }
}
