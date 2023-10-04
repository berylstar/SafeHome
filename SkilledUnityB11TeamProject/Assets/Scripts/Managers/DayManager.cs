using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayManager : MonoBehaviour
{
    [Header("Date")]
    public int day = 0;
    [Range(0.0f, 1.0f)] public float time;
    public bool isNight = false;

    [Header("Setting")]
    public float fullDayLength;
    private float timeRate;
    public Vector3 noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;
    public Material sunSkybox;
    public AudioClip dayBGM;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;
    public Material moonSkybox;
    public AudioClip nightBGM;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    [Header("UI")]
    public TextMeshProUGUI textDay;

    private float dayStart = 0.3f;
    private float nightStart = 0.7f;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = dayStart;
        TalkingMyself(day);
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        if (isNight && !(time <= dayStart || nightStart <= time))
        {
            day += 1;

            sun.gameObject.SetActive(true);
            moon.gameObject.SetActive(false);
            isNight = false;
            SoundManager.ChangeBackGroundMusic(dayBGM);
            TalkingMyself(day);
            
            if (day <= 5) textDay.text = day.ToString();
            else textDay.text = "????";
        }
        else if (!isNight && (time <= dayStart || nightStart <= time))
        {
            sun.gameObject.SetActive(false);
            moon.gameObject.SetActive(true);
            isNight = true;
            SoundManager.ChangeBackGroundMusic(nightBGM);
        }

        RenderSettings.skybox = isNight ? moonSkybox : sunSkybox;

        if (isNight) UpdateLighting(moon, moonColor, moonIntensity);
        else UpdateLighting(sun, sunColor, sunIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    private void UpdateLighting(Light lightSource, Gradient colorGradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * 4.0f * noon;
        lightSource.color = colorGradient.Evaluate(time);
        lightSource.intensity = intensity;
    }

    private void TalkingMyself(int _day)
    {
        string conv = _day switch
        {
            1 => "�� ���� ���� ���������ϴ�...\n��ų�� �� �����̴�.",
            2 => "5�Ͽ� �������� ����� �ߴٴ� �ҽ��� �Դ�.\n���� �� �����ϰ� �ؾ���...",
            3 => "������ ���ؼ� ���� ���Ѿ� �ϴµ�.\n������ �������� ����..",
            4 => "�����̸� �������� �� �� �ִ�.\n��������...",
            5 => "������ 4�� �ƴ� 5���̴�..\n������ �Դ�...",
            6 => "�� �������� �ƹ� ������ ���°���...\n���� ���� �Ͼ �ɱ�?",
            _ => "",
        };

        UIManager.PopupText(conv);
    }
}
