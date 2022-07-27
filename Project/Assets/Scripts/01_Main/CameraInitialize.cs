using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraInitialize : MonoBehaviour
{
    public float intensity = 45f;
    public float focusDistance = .1f;

    DepthOfField depthOfField;
    Bloom bloom;

    private void Awake()
    {
        depthOfField = GetComponent<PostProcessVolume>().sharedProfile.GetSetting<DepthOfField>();
        bloom = GetComponent<PostProcessVolume>().sharedProfile.GetSetting<Bloom>();
    }

    private void Update()
    {
        depthOfField.focusDistance.value = focusDistance;
        bloom.intensity.value = intensity;
    }
}
