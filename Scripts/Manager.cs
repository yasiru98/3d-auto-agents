//Author: Yasiru Karunawansa
//Purpose: keeps track of obstacles, humans and zombies. Also draws the GUI.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public Material DebugOrange;

    public GameObject zombie;
    public GameObject human;
    public GameObject debugCube;
    private GameObject debug;
    public GameObject[] zombies;
    public GameObject[] humans;
    public GameObject[] obstacles;

    public bool isDebug;

    public Vector3 humanSumFwd;
    public Vector3 humanCenteroid;
    public Vector3 zombieSumFwd;
    public Vector3 zombieCenteroid;

    public Camera[] cameras;
    // Current camera index
    private int currentCameraIndex;

    // Use this for initialization
    void Start () {
        currentCameraIndex = 0;
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }
        //if any cameras were added to the controller, enable the first one
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }


        obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        debug = Instantiate(debugCube, this.transform.position, Quaternion.identity);
        isDebug = false;
        debug.SetActive(isDebug);
    }
	
	// Update is called once per frame
	void Update () {
        // Press the 'C' key to cycle through cameras in the array
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentCameraIndex==0)
            {
                currentCameraIndex++;
                cameras[currentCameraIndex].gameObject.SetActive(true);
            
            }
            else if(currentCameraIndex == 1)
            {
                cameras[currentCameraIndex].gameObject.SetActive(false);
                currentCameraIndex--;
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
          
        }

        DrawDebug();

        
        humans = GameObject.FindGameObjectsWithTag("human");
        zombies = GameObject.FindGameObjectsWithTag("zombie");

        foreach(GameObject human in humans)
        {
            humanSumFwd += human.transform.forward;
            humanCenteroid += human.transform.position;
        }
        humanCenteroid = humanCenteroid / humans.Length;


        foreach (GameObject zombie in zombies)
        {
            zombieSumFwd += zombie.transform.forward;
            zombieCenteroid += zombie.transform.position;
        }
        zombieCenteroid = zombieCenteroid / zombies.Length;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Vector3 pos = new Vector3(Random.Range(20,400), 0, Random.Range(20, 400));

            Instantiate(zombie, pos, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Vector3 pos = new Vector3(Random.Range(20, 400), 0, Random.Range(20, 400));

            Instantiate(human, pos, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.D))//flip debugLines on input
        {
            isDebug = !isDebug;
        }
    }

    /// <summary>
    /// draw debug cube to show the center of the human horde.
    /// </summary>
    void DrawDebug()
    {
       
       debug.SetActive(isDebug);
       debug.transform.position = humanCenteroid;
        
    }

    /// <summary>
    /// draw debug line from the human horde center to their average direction.
    /// </summary>
    void OnRenderObject()
    {


        if (isDebug == true)
        {
            DebugOrange.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(debug.transform.position);
            GL.Vertex(humanSumFwd);
            GL.End();


        }


    }
    /// <summary>
    /// draw GUI info
    /// </summary>
    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "Press 'D' for debug info \n Press 'Z' to add a zombie\n Press 'H' to add a human\n Press 'C' to change camera view");
    }
}
