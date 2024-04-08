using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GyroscopeControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
{
    if (SystemInfo.supportsGyroscope)
    {
        Input.gyro.enabled = true;
        Debug.Log("Gyroscope enabled.");
    }
    else
    {
        Debug.Log("Gyroscope not supported.");
    }
}

    // Update is called once per frame
    void Update()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Debug.Log("Gyro attitude: " + Input.gyro.attitude);
            Debug.Log("Gyro rotation rate: " + Input.gyro.rotationRate);

            // Obtenez les données du gyroscope
            Quaternion gyroRotation = Input.gyro.attitude;
            
            // Convertissez l'orientation du gyroscope du repère de Unity
            gyroRotation = Quaternion.Euler(90f, 0f, 0f) * (new Quaternion(-gyroRotation.x, -gyroRotation.y, gyroRotation.z, gyroRotation.w));
            
            // Ajustez la rotation de la caméra
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, gyroRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
