//Author: Yasiru Karunawansa
//Purpose: controls the basic behaviour of any vehicle. The parent class for Human and Zombie classes.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour {
    private Vector3 vehiclePosition;
    private Vector3 acceleration;
    private Vector3 direction;
    public Vector3 velocity;
    public Vector3 futureEvadeDistance;
    public Vector3 futurePursueDistance;
    
    private GameObject[] obstacles;
    private Vehicle[] allVehicles;
    public List<GameObject> vehicleObjects;
    private Manager managerScript;
    // Floats
    private float radius = 20f;
    private float safeDistance = 20f;
    public float mass;
    public float maxSpeed;

    public abstract void calcSteeringForces();
    // Use this for initialization
    public void Start () {
        vehiclePosition = transform.position;
        managerScript = GameObject.Find("Main Camera").GetComponent<Manager>();
        obstacles = managerScript.obstacles;
        allVehicles = FindObjectsOfType<Vehicle>();


    }
	
	// Update is called once per frame
	public void Update () {
     
        Wrap();
        calcSteeringForces();
        velocity += acceleration * Time.deltaTime;
    
        vehiclePosition += velocity * Time.deltaTime;
        direction = velocity.normalized;
  
        transform.position = vehiclePosition;
        acceleration = Vector3.zero;
   
        Rotate();

        foreach(GameObject obstacle in obstacles)
        {
            Vector3 seekForce = ObstacleAvoidance(obstacle);
            Vector3 ultimateForce = seekForce;
            ultimateForce = ultimateForce * maxSpeed;
            ApplyForce(ultimateForce);

        }
 


    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;

    }


    public void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        acceleration += friction;
    }

    /// <summary>
    /// move towards a certain position or object
    /// </summary>
    public Vector3 Seek(Vector3 targetPosition)
    {
        // Step 1: Find DV (desired velocity)
        // TargetPos - CurrentPos
        Vector3 desiredVelocity = targetPosition - vehiclePosition;

        // Step 2: Scale vel to max speed
        // desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Step 3:  Calculate seeking steering force
        Vector3 seekingForce = desiredVelocity - velocity;

        // Step 4: Return force
        return seekingForce;
    }
    public Vector3 Seek(GameObject target)
    {
        return Seek(target.transform.position);
    }

    /// <summary>
    /// move away from a certain position or object
    /// </summary>
    public Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity = vehiclePosition - targetPos;
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;
        Vector3 fleeingForce = desiredVelocity - velocity;
        return fleeingForce;
    }
    public Vector3 Flee(GameObject target)
    {
        return Flee(target.transform.position);
    }

    /// <summary>
    /// keep vehicles within bounds
    /// </summary>
    private void Wrap()
    {
        if (vehiclePosition.x > 480 || vehiclePosition.x < 20 || vehiclePosition.z > 480 || vehiclePosition.z > 480 || vehiclePosition.z < 20)
        {
            Vector3 seekForce = Seek(new Vector3 (250,0,250));
            Vector3 ultimateForce = seekForce;
            ultimateForce = ultimateForce * maxSpeed;
            ApplyForce(ultimateForce);
        }
    }

    /// <summary>
    /// Rotate object towrds the direction it is facing
    /// </summary>
    void Rotate()
    {
      gameObject.transform.forward = direction;
      gameObject.transform.position = vehiclePosition;
    }

    /// <summary>
    /// avoid tree obstacles
    /// </summary>
    protected Vector3 ObstacleAvoidance(GameObject obstacle)
    {
        // Info needed for obstacle avoidance
        Vector3 vecToCenter = obstacle.transform.position - vehiclePosition;
        float dotForward = Vector3.Dot(vecToCenter, transform.forward);
        float dotRight = Vector3.Dot(vecToCenter, transform.right);
        float radiiSum = obstacle.GetComponent<Obstacle>().radius + radius;

        // Step 1: Are there objects in front of me?  
        // If obstacle is behind, ignore, no need to steer - exit method
        // Compare dot forward < 0
        if (dotForward < 0)
        {
            return Vector3.zero;
        }

        // Step 2: Are the obstacles close enough to me?  
        // Do they fit within my "safe" distance
        // If the distance > safe, exit method
        if (vecToCenter.magnitude > safeDistance)
        {
            return Vector3.zero;
        }

        // Step 3:  Check radii sum against distance on one axis
        // Check dot right, 
        // If dot right is > radii sum, exit method
        if (radiiSum < Mathf.Abs(dotRight))
        {
            return Vector3.zero;
        }

        // NOW WE HAVE TO STEER!  
        // The only way to get to this code is if the obstacle is in my path
        // Determine if obstacle is to my left or right
        // Desired velocity in opposite direction * max speed
        Vector3 desiredVelocity;

        if (dotRight < 0)        // Left
        {
            desiredVelocity = transform.right * maxSpeed;
        }
        else                    // Right
        {
            desiredVelocity = -transform.right * maxSpeed;
        }

        // Debug line to obstacle
        // Helpful to see which obstacle(s) a vehicle is attempting to maneuver around
        Debug.DrawLine(transform.position, obstacle.transform.position, Color.green);

        // Return steering force
        Vector3 steeringForce = desiredVelocity - velocity;
        return steeringForce;
    }

    /// <summary>
    /// sperate from other vehicles depending on the closest objects and weighing them to avoid clipping through
    /// </summary>
    public Vector3 Separate(GameObject[] vehicles)
    {
        Vector3 sumForce = Vector3.zero;
        float dist = radius * radius*2;

        //Loop through each vehicle
        foreach (GameObject vehicle in vehicles)
        {
            if (vehicle == null)
            {
                continue;
            }
            else
            {
                Vector3 distBetween = transform.position - vehicle.transform.position;
                if (distBetween.sqrMagnitude == 0)
                    continue;

                else if (distBetween.sqrMagnitude < dist)
                {
                    Vector3 seperation = distBetween;
                    float weight = 1 / distBetween.sqrMagnitude;
                    sumForce += (seperation * weight*3);
                }
            }
        
        }
        return sumForce;
    }

    /// <summary>
    /// seek a random position on a circles projected forward to simulate wandering
    /// </summary>
    public Vector3 Wander()
  {
      float circleRad = 20;
      float angle = Random.Range(0, 360);

      Vector3 pos = transform.position;
      Vector3 circleCenter = pos + (transform.forward*10);



      Vector3 displacement = new Vector3(-1,0,-1);
  
      displacement.x = circleCenter.x+Mathf.Cos(angle) *circleRad ;
      displacement.z = circleCenter.z+Mathf.Sin(angle)*circleRad ;


      return displacement;

          

    }

    /// <summary>
    /// pursue class that seeks a distance ahead of the gam object being pursued
    /// </summary>
    protected Vector3 Pursue(Vehicle target)
    {
    
       futureEvadeDistance  = target.transform.position + (target.velocity * 3);
       Vector3 seekForce = Seek(futureEvadeDistance);
       return seekForce;
    }

    /// <summary>
    /// evade class that flees a distance ahead of the gam object pursuing
    /// </summary>
    protected Vector3 Evade(Vehicle target)
    {
       futurePursueDistance = target.transform.position + (target.velocity * 3);
        Vector3 fleeForce = Flee(futurePursueDistance);
        return fleeForce;
    }


   


}
