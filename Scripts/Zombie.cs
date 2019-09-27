//Author: Yasiru Karunawansa
//Purpose: controls the behaviour of a Zombie. Inherits from the Vehicle class.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Vehicle
{
    private Vector3 debugLinePos;

    public Material DebugWhite;
    public Material DebugBlack;
    public Material DebugRed;
    public GameObject debug2;
    public GameObject debugSphere;
    public bool isDebug;
    private float tracker;
    private GameObject humantoSeek;
    public GameObject zombie;
    private GameObject[] humans;
    private GameObject[] zombies;
    private Manager managerScript;
    private SphereCollider humanCollider;
    private SphereCollider zombieCollider;
    // Use this for initialization
    void Start()
    {
        base.Start();
        isDebug = false;
        debugSphere = Instantiate(debug2, this.transform.position, Quaternion.identity);
        debugSphere.SetActive(isDebug);
        managerScript = GameObject.Find("Main Camera").GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        tracker = 500;
        humans = managerScript.humans;
        zombies = managerScript.zombies;

        CollisionDetection();

        Vector3 seperateSeek = base.Separate(zombies);
        Vector3 ultimateForce = seperateSeek;
        ultimateForce = ultimateForce * base.maxSpeed;
        base.ApplyForce(ultimateForce);
        DrawSpheres();
        base.Update();

        if (Input.GetKeyDown(KeyCode.D))
        {
            isDebug = !isDebug;
        }


    }

    /// <summary>
    /// move towards a human depending on distance
    /// </summary>
    public override void calcSteeringForces()
    {
       
        Vector3 ultimateForce;

        if (humans.Length != 0) {
            foreach (GameObject human in humans) {

              
                float distance = Vector3.Distance(this.transform.position, human.transform.position);
                Vehicle vehicleScript = human.GetComponent<Vehicle>();
                if (distance < tracker)
                {
                    tracker = distance;
                    humantoSeek = human;
                    debugLinePos = humantoSeek.transform.position;
                    Vector3 seekForce = base.Pursue(vehicleScript);
                    ultimateForce = seekForce;
                    ultimateForce = ultimateForce * base.maxSpeed;
                    base.ApplyForce(ultimateForce);

                }

                else if(distance > tracker)
                {
                        continue;
                }
            }
     
         
        
        }
        else
        {
            Vector3 wanderForce = base.Wander();
            Vector3 seekForce = base.Seek(wanderForce);
            ultimateForce = seekForce;
            base.ApplyForce(ultimateForce);
        }
    }

    /// <summary>
    /// Detect collisions between humans and zombies to spawn more zombies
    /// </summary>
    public void CollisionDetection()
    {
        foreach (GameObject human in humans)
        {
            if (human != null)
            {
                humanCollider = human.GetComponent<SphereCollider>();
                zombieCollider = this.GetComponent<SphereCollider>();
                if (Vector3.Distance(human.transform.position, this.transform.position) - 5 < (humanCollider.radius + zombieCollider.radius))
                {

                    Vector3 spawnPos = human.transform.position;

                    Destroy(human);
                    Instantiate(zombie, spawnPos, Quaternion.identity);
                }
            }
            else
            {
                continue;
            }
        }
    }


    /// <summary>
    /// Draw debug lines 
    /// </summary>
    void OnRenderObject()
    {
        if (isDebug==true) {
            DebugWhite.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(this.transform.position);
            GL.Vertex(debugLinePos);
            GL.End();

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
        }

    }
    /// <summary>
    /// draw debug spheres to show future positions
    /// </summary>
    void DrawSpheres()
    {

        debugSphere.SetActive(isDebug);
        debugSphere.transform.position = transform.position + transform.forward * 40;

    }

    void Align()
    {
        Vector3 desiredVelocity = managerScript.zombieSumFwd.normalized * maxSpeed;
        Vector3 steeringForce = desiredVelocity - velocity;
        ApplyForce(steeringForce);
     }

    void Cohesion()
    {
        Vector3 ultimateForce;
        Vector3 seekForce;
        Vector3 centerPoint = managerScript.zombieCenteroid;
        seekForce = Seek(centerPoint);
        ultimateForce = seekForce;
        base.ApplyForce(ultimateForce);
    }
}
