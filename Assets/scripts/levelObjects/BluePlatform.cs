using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class BluePlatform : MonoBehaviour
{
    public int speed = 5;
    public List<Vector3> waypoints = new List<Vector3>();
    int currentWaypoint = 0;
    bool vorw�rts = true;
    void Start()
    {
        foreach (Transform child in transform)
        {
            waypoints.Add(child.transform.position);
        }
    }

    void Update()
    {
        if (currentWaypoint < waypoints.Count && vorw�rts)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint], Time.deltaTime * speed);

            if (transform.position == waypoints[currentWaypoint])
            {
                currentWaypoint++;
                Console.WriteLine(currentWaypoint);
            }
        }
        if (currentWaypoint == waypoints.Count)
        {
            vorw�rts = false;
            currentWaypoint--;
            Console.WriteLine(currentWaypoint);
        }
        if (currentWaypoint > waypoints.Count && !vorw�rts)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint], Time.deltaTime * speed);

            if (transform.position == waypoints[currentWaypoint])
            {
                currentWaypoint--;
                Console.WriteLine(currentWaypoint);
            }
        }
        if (currentWaypoint == 0)
        {
            vorw�rts = true;
            Console.WriteLine(currentWaypoint);
        }



    }
}
