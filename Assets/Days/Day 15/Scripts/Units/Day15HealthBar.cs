using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Day15HealthBar : MonoBehaviour
{
    public Slider slider;
    public float updateSpeedSeconds = 0.03f;
    public float positionOffset = 20.0f;

    private Day15Health health;

    private Camera cam;
    public Vector3 initialScale;
    private float maxDistance = 40f;
    private float minDistance = 3f;

    private void Awake()
    {
        cam = Camera.main;
        slider = GetComponent<Slider>();
    }

    public void SetHealth(Day15Health health)
    {
        this.health = health;
        health.OnHealthPctChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float pct)
    {
        StartCoroutine(ChangeToPct(pct));
    }

    private IEnumerator ChangeToPct(float pct)
    {
        float preChangePct = slider.value;
        //float elapsed = 0f;

        //while(elapsed < updateSpeedSeconds)
        //{
        //    elapsed += Time.deltaTime;
        //    slider.value = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
        //    yield return null;
        //}

        slider.value = pct;
        yield return null;
    }

    private void LateUpdate()
    {
        cam = Camera.main;
        Plane plane = new Plane(cam.transform.forward, cam.transform.position);
        float dist = Mathf.Clamp(plane.GetDistanceToPoint(health.transform.position), minDistance, maxDistance);
        if (!dist.Equals(minDistance))
        {
            transform.localScale = initialScale * Mathf.Lerp(0.2f, 1.0f, 1 - (dist/maxDistance));
        }
        else
        {
            transform.localScale = Vector3.zero;
        }

        transform.position = cam.WorldToScreenPoint(health.transform.position + Vector3.up * positionOffset);
    }

    private void OnDestroy()
    {
        health.OnHealthPctChanged -= HandleHealthChanged;
    }
}
