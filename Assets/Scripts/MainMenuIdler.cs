using UnityEngine;
using System.Collections;

public class MainMenuIdler : MonoBehaviour
{
    [SerializeField]
    public float rotRate = 30.0f;

    [SerializeField]
    private Vector3 rotSpeed = Vector3.up;

    // Before rendering each frame..
    void Update()
    {
        transform.Rotate(rotSpeed * rotRate * Time.deltaTime);

        if (transform.rotation.z >= 10 || transform.rotation.z <= -10)
        {
            rotSpeed = -rotSpeed;
        }
    }
}
