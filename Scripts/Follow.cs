//Author: Yasiru Karunawansa
//Purpose: controls the behaviour of a path follower. Inherits from the Vehicle class.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : Vehicle
{
    public Material DebugWhite;
    List<Vector3> points = new List<Vector3>();
    int startIndex;
    public GameObject[] pathObjects;
    // Use this for initialization
    void Start()
    {
        base.Start();
        startIndex = 0;
     
        foreach(GameObject pathObj in pathObjects)
        {
            points.Add(pathObj.transform.position);
        }
        Debug.Log(points.Count);

    }

    // Update is called once per frame
    void Update()
    {
        
        base.Update();
    }
    public override void calcSteeringForces()
    {
        float distance = Vector3.Distance(transform.position,points[startIndex]);
        if (distance < 2)
        {
            startIndex++;
        }
        else
        {
            Vector3 seekForce = Seek(points[startIndex]);
            Vector3 ultimateForce = seekForce;
            ultimateForce = ultimateForce * maxSpeed;
            ApplyForce(ultimateForce);
        }

        if (startIndex == 4)
        {
            startIndex = 0;
        }
    
    }
    void OnRenderObject()
    {

            DebugWhite.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(points[0]);
            GL.Vertex(points[1]);
            GL.End();

            DebugWhite.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(points[1]);
            GL.Vertex(points[2]);
            GL.End();

            DebugWhite.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(points[2]);
            GL.Vertex(points[3]);
            GL.End();

            DebugWhite.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(points[3]);
            GL.Vertex(points[0]);
            GL.End();

    }
}
