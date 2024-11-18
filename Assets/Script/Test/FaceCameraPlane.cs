using System;
using UnityEngine;

public class FaceCameraPlane : MonoBehaviour
{
    public GameObject child;
    private void FixedUpdate() 
    {
        CustomFacing();
    }

    public void CustomFacing()
    {
        // perpendicular
        child.transform.localRotation = Quaternion.Euler(0, 0, Vector3.Dot(transform.forward, Vector3.forward) * 90);
    }
}
