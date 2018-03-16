using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadObj : MonoBehaviour {
    public float RotationSpeed;
    [SerializeField]
    public GameObject[] ToPreload;
	// Use this for initialization
	void Start () {
        foreach (GameObject g in ToPreload)
            g.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotationSpeed);
	}
}
