using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class RocketLaunch : MonoBehaviour {
    public TextMeshProUGUI fuelText;
    private float fuel = 1f;
    private Rigidbody rb;
    private float mDot;
    private float thrust;
    private float TSFC;
    private float fuelTime;
    private int inLaunch = 0;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        rb.AddForce(new Vector3(0, -Mathf.Pow(Physics.gravity.y, 2), 0));
    }

    public void LaunchCokeBottle() {
        if(inLaunch == 0) { // coke bottle density: 1026kg/m^3 area: 0.085m^2 v: 10m/s pressure: 101.325kpa area: 0.0008617m
            StartCoroutine(Launch(1026f, 0.085f, 5, 101.325f, 0.0008617f, 5f));
            StopCoroutine(Launch(1026f, 0.085f, 5, 101.325f, 0.0008617f, 5f));
        }
    }

    private IEnumerator Launch(float density, float area, float exitVelocity, float exitPressure, float exitArea, float weight) {
        inLaunch = 1;
        rb.mass = weight;
        //F=mdot*ve+(pe-pa)Ae
        //mdot=density*V*A kg/m^3 m/s m^2
        //pa=14.7psi
        mDot = density * fuel * area;
        thrust = mDot * exitVelocity + (exitPressure * 101.325f) * exitArea;
        TSFC = mDot / thrust; // fuel burned per hour
        //float fuelTime = System.Convert.ToInt32(fuelText.text) / TSFC; Put this in when launch buttons are setup
        fuelTime = (fuel / 250) / TSFC;

        float timePassed = 0;
        while(timePassed <= fuelTime * 60) {
            rb.AddRelativeForce(new Vector3(0, thrust, 0), ForceMode.Force);

            timePassed += Time.deltaTime;

            print($"mdot: {mDot}, thrust: {thrust}, fuel consumption: {TSFC} fuel time: {fuelTime} hours {fuelTime * 60} seconds, timePassed: {timePassed} inLaunch: {inLaunch}");

            if(timePassed >= fuelTime * 60) {
                inLaunch = 2;
            }

            yield return null;
        }
    }
}
