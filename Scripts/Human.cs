//Author: Yasiru Karunawansa
//Purpose: controls the behaviour of a Human. Inherits from the Vehicle class.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Vehicle {


    public bool isDebug;

    public Material DebugWhite;
    public Material DebugBlack;
    public Material DebugRed;

    public GameObject debug1;
    public GameObject debugSphere;
    private GameObject[] zombies;
    private GameObject[] humans;

    public Vector3 evadeForce;
    public Vector3 fleeForce;
    private Vector3 psgPos;

    private Manager managerScript;
    // Use this for initialization
    void Start () {
        base.Start();
        debugSphere = Instantiate(debug1, this.transform.position, Quaternion.identity);
        debugSphere.SetActive(isDebug);
        managerScript = GameObject.Find("Main Camera").GetComponent<Manager>();
        isDebug = false;

	}
	
	// Update is called once per frame
	void Update () {

        humans = managerScript.humans;
        zombies = managerScript.zombies;

        Align();
        Cohesion();
  
        Vector3 seperateSeek = base.Separate(humans);
        Vector3 ultimateForce = seperateSeek;
        ultimateForce = ultimateForce * base.maxSpeed;
        base.ApplyForce(ultimateForce);

        DrawSpheres();
        base.Update();
        if (Input.GetKeyDown(KeyCode.D))//flip debugLines on input
        {
            isDebug = !isDebug;
        }

    }

    /// <summary>
    /// wander or evade nearest zombie depending on positioning
    /// </summary>
    public override void calcSteeringForces()
    {
     

        foreach (GameObject zombie in zombies)
        {

            float distance = Vector3.Distance(this.transform.position, zombie.transform.position);
            Vehicle vehicleScript = zombie.GetComponent<Vehicle>();
            if (distance < 10)
            {
        
                Vector3 ultimateForce;
                evadeForce = base.Evade(vehicleScript);
                ultimateForce = evadeForce;
                ultimateForce = ultimateForce * base.maxSpeed;
                base.ApplyForce(ultimateForce);
      


            }

            else if(transform.position.x < 480 || transform.position.x > 20 || transform.position.z < 480  || transform.position.z > 20)
          {
             
            Vector3 wanderForce = base.Wander();
            Vector3 ultimateForce;      
            Vector3 seekForce = base.Seek(wanderForce);
            ultimateForce = seekForce;
            base.ApplyForce(ultimateForce);
           
          }

        }
    }

    /// <summary>
    /// Draw debug lines 
    /// </summary>
    void OnRenderObject()
    {
      

        if (isDebug == true) {
            DebugBlack.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(this.transform.position);
            GL.Vertex(this.transform.forward * 5 + transform.position);
            GL.End();

            DebugRed.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(this.transform.position);
            GL.Vertex(this.transform.right * 5 + transform.position);
            GL.End();

            DebugRed.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(this.transform.position);
            GL.Vertex(debugSphere.transform.position);
            GL.End();

        }
    

    }

    /// <summary>
    /// align human object to the average direction of the horde
    /// </summary>
    void Align()
    {
        Vector3 desiredVelocity = managerScript.humanSumFwd.normalized * maxSpeed;
        Vector3 steeringForce = desiredVelocity - velocity;
        ApplyForce(steeringForce);

    }

    /// <summary>
    /// seek the center of the horde to make sure the horde stays together
    /// </summary>
    void Cohesion()
    {
        Vector3 ultimateForce;
        Vector3 seekForce;
        Vector3 centerPoint = managerScript.humanCenteroid;
        seekForce = Seek(centerPoint);
        ultimateForce = seekForce;
        base.ApplyForce(ultimateForce);
    }

    /// <summary>
    /// draw debug spheres to show future positions
    /// </summary>
    void DrawSpheres()
    {

        debugSphere.SetActive(isDebug);
        debugSphere.transform.position = transform.position + transform.forward * 40;
         
    }



}
