using UnityEngine;
/******************************************************************************
 * Project: UmgebungsSimulation
 * File: SkyboxDayNightCycle.cs
 * Version: 1.01
 * Autor:  Franz Mörike (FM);
 * 
 * 
 * These coded instructions, statements, and computer programs contain
 * proprietary information of the author and are protected by Federal
 * copyright law. They may not be disclosed to third parties or copied
 * or duplicated in any form, in whole or in part, without the prior
 * written consent of the author.
 * 
 * NOTE:
 * 
 * The basic coding structure and algorithms are from Unterricht_GenerischeWelten!
 * That code-base was used, improved and adjusted to my usage.
 * 
 * ChangeLog
 * ----------------------------
 *  21.11.2021  created && added comments
 *  22.11.2021  added color
 *  
 *****************************************************************************/
public class SkyboxDayNightCycle : MonoBehaviour
{
    [SerializeField]
    private bool pauseDayNightRotation;
    [SerializeField]
    private bool isDay;
    [SerializeField]
    [Range(0.1f, 5f)]
    private float timeScale;
    [SerializeField]
    [Range(0f, 24f)]
    private float currentTime;
    [SerializeField]
    [Range(-180f, 180f)]
    private float startOffsetRotationY = -30f;
    [SerializeField]
    [Range(0.5f, 3f)]
    private float sunScalingFactor = 1f;
    [SerializeField]
    private Gradient sunColor;
    [SerializeField]
    [Range(0.5f, 3f)]
    private float moonScalingFactor = 1f;
    [SerializeField]
    private Gradient moonColor;
    [SerializeField]
    private bool fixedDayTime;

    private Transform sunRef;
    private Transform moonRef;
    private Light sunLight;
    private Light moonLight;
    private float sunIntensity;
    private float moonIntensity;

    public float CurrentTime { get => currentTime; set => currentTime = value; }
    public float TimeScale { get => timeScale; set => timeScale = value; }

    private void Awake()
    {
        //Assign moon and sun
        sunRef = gameObject.transform.GetChild(0).transform.GetComponent<Transform>();
        moonRef = gameObject.transform.GetChild(1).transform.GetComponent<Transform>();
        sunLight = sunRef.GetComponent<Light>();
        moonLight = moonRef.GetComponent<Light>();
        //Debug.Log(sunRef.name + moonRef.name);
    }

    private void Update()
    {
        isDay = sunRef.transform.rotation.eulerAngles.x <= 180f;
        if (!pauseDayNightRotation)
        {
            UpdateTime();
            UpdateSun();
            UpdateMoon();
        }
    }

    private void UpdateTime()
    {
        if (fixedDayTime)
        {
            //mid of day
            currentTime = 6;
        }
        else
        {
            currentTime += Time.deltaTime * TimeScale;
            if (currentTime >= 24)
            {
                currentTime = 0;
            }
        }

    }

    /// <summary>
    /// Updated the suns color, light intensity and rotation accordingly
    /// </summary>
    private void UpdateSun()
    {
        //updating rotation
        //time between 0 and 1
        float currentTimeMapped = currentTime / 24;
        float rotationX = currentTimeMapped * 360f;
        sunRef.transform.localRotation = Quaternion.Euler(new Vector3(rotationX, startOffsetRotationY, 0f));
        //updating sun intensity | Turn off light at night
        sunIntensity = sunScalingFactor * Mathf.Clamp01(Vector3.Dot(-sunLight.transform.forward, Vector3.up));
        sunLight.intensity = sunIntensity;
        //updating color
        sunLight.color = sunColor.Evaluate(sunIntensity);
    }

    /// <summary>
    /// Updated the moons color, light intensity and rotation accordingly
    /// </summary>
    private void UpdateMoon()
    {
        //updating rotation
        //time between 0 and 1
        float currentTimeMapped = currentTime / 24;
        float rotationX = 180f + currentTimeMapped * 360f;
        moonRef.transform.localRotation = Quaternion.Euler(new Vector3(rotationX, startOffsetRotationY, 0f));
        //updating moon intensity | Turn off light at day
        moonIntensity = moonScalingFactor * Mathf.Clamp01(Vector3.Dot(-moonLight.transform.forward, Vector3.up));
        moonLight.intensity = moonIntensity;
        //updating color
        moonLight.color = moonColor.Evaluate(moonIntensity);
    }
}
