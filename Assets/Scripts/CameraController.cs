using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;
    private Vector3 scalar = new Vector3(0.5f, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = offset + Vector3.Scale(player.transform.position, Vector3.forward);
        transform.position = offset + Vector3.Scale(player.transform.position, scalar);
    }
}
