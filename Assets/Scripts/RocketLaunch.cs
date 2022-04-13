using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class RocketLaunch : MonoBehaviour {
    //public TextMeshProUGUI fuelText;
    public Text fuelText;
    public TextMeshProUGUI rocketHText;
    public TMP_Dropdown matDrop;
    public TMP_Dropdown fuelDrop;
    public Transform rocketBody;
    public AudioSource rocketAudio;
    private float fuel;
    private Rigidbody rb;
    private float mDot;
    private float thrust;
    private float TSFC;
    private float fuelTime;
    private int inLaunch = 0;
    private float cd = 0;
    private bool changed = false;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        matDrop.value = 1;
        fuelDrop.value = 1;
        SetRocket();
    }

    private void FixedUpdate() {
        rb.AddForce(new Vector3(0, -Mathf.Pow(Physics.gravity.y, 2), 0));
    }

    private void Update() {
        cd += Time.deltaTime;
        if(cd > 0.5f) {
            rocketHText.text = $"Rocket Height: {(int) this.gameObject.transform.localPosition.y / 10}m";
            cd = 0;
        }
    }

    public void LaunchRocket() {
        rocketAudio.Play();
        rb.freezeRotation = false;
        // coke bottle density: 1026kg/m^3 area: 0.085m^2 v: 10m/s pressure: 101.325kpa area: 0.0008617m
        float density = 0;
        float area = 0.085f;
        float exitV = 5;
        float exitP = 101.325f;
        float exitA = 0.0008617f;
        float mW = 0;
        float fW = float.Parse(fuelText.text);
        float weight = 0;

        if(matDrop.value == 0) { // plastic
            mW = 1.0f;
        }
        else if(matDrop.value == 1) { // Titanium
            mW = 164469.0f;
        }
        else if(matDrop.value == 2) { // Diamond
            mW = 128115.0f;
        }
        else if(matDrop.value == 3) { // Aluminum
            mW = 98550.0f;
        }
        else if(matDrop.value == 4) { // Osmium
            mW = 821250.0f;
        }

        if(fuelDrop.value == 0) { // Coca Cola
            density = 1026.0f;
        }
        else if(fuelDrop.value == 1) { // Liquid hydrogen
            density = 71f;
        }
        else if(fuelDrop.value == 2) { // Liquid oxygen
            density = 1141f;
        }
        else if(fuelDrop.value == 3) { // Gasoline
            density = 760f;
        }
        else if(fuelDrop.value == 4) { // Alcohol
            density = 785.027f;
        }
        else if(fuelDrop.value == 5) { // Kerosene
            density = 820f;
        }

        weight = mW + fW;

        if(inLaunch == 0) {
            StartCoroutine(Launch(density, area, exitV, exitP, exitA, weight));
            StopCoroutine(Launch(density, area, exitV, exitP, exitA, weight));
        }
    }

    public void SetRocket() {
        rb.freezeRotation = true;
        if(matDrop.value == 0) { // plastic
            rocketBody.transform.localScale = new Vector3(2f, 7f, 2f);
            this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        else { // everything else
            rocketBody.transform.localScale = new Vector3(10f, 35f, 10f);
            this.gameObject.transform.localPosition = new Vector3(0, 28f, 0);
        }
    }

    public void ResetRocket() {
        rocketAudio.Stop();
        rb.freezeRotation = true;
        GameObject rocket = GameObject.Find("Rocket");
        if(matDrop.value == 0) { // plastic
            rocketBody.transform.localScale = new Vector3(2f, 7f, 2f);
            this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        else { // everything else
            rocketBody.transform.localScale = new Vector3(10f, 35f, 10f);
            this.gameObject.transform.localPosition = new Vector3(0, 28f, 0);
        }
        rocket.transform.localRotation = Quaternion.Euler(Vector3.zero);
        rocket.GetComponent<Rigidbody>().velocity = Vector3.zero;
        rocket.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        StopAllCoroutines();
        inLaunch = 0;
        print("average kg of fuel is 408233.133kg with titanium and liquid hydrogen");
    }

    private IEnumerator Launch(float density, float area, float exitVelocity, float exitPressure, float exitArea, float weight) {
        inLaunch = 1;
        rb.mass = weight;
        fuel = float.Parse(fuelText.text);
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
