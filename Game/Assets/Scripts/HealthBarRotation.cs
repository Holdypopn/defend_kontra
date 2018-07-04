using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarRotation : MonoBehaviour
{
    void Start()
    {
        Refresh();
    }
	
    /// <summary>
    /// Updates the healthbar direction to the camera
    /// </summary>
	public void Refresh ()
    {
        Vector3 v = Camera.main.transform.position - transform.position;

        v.x = v.z = 0.0f;

        transform.LookAt(Camera.main.transform.position);

        transform.rotation = (Camera.main.transform.rotation); // Take care about camera rotation
    }
}
