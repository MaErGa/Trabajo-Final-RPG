using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadConfig : MonoBehaviour
{
    public float scaleAtRuntime;

    void Awake()
    {
        scaleAtRuntime = Time.timeScale;
        Time.timeScale = 1;
    }
}
// Script para que no empiece pausado