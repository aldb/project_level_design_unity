using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicAmbiance : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource audioSource;

    void Start()
    {
        audioSource.playOnAwake = true;
        audioSource.loop = true; // Force true value
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
