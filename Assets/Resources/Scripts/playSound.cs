using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playSound : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void playSoundOnPress() {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
    }

    public void loadScene()
    {
        DontDestroyOnLoad(GameObject.Find("LMHeadMountedRig"));
        DontDestroyOnLoad(GameObject.Find("Directional Light"));
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("ButtonsPanel"));
        DontDestroyOnLoad(GameObject.Find("ClientObject"));
        DontDestroyOnLoad(GameObject.Find("FixedObject"));
        DontDestroyOnLoad(GameObject.Find("ConvertionObject"));
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
        DontDestroyOnLoad(GameObject.Find("Plane"));
        DontDestroyOnLoad(GameObject.Find("saveFixedCubes"));
        DontDestroyOnLoad(GameObject.Find("saveFixedAxes"));
        DontDestroyOnLoad(GameObject.Find("Sliders"));

        SceneManager.LoadScene("SerialRobot");
    }
}
