using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 500.0f;
    public TrackLayoutGenerator trackGenerator;

    void LateUpdate()
    {
        Vector3 cameraPosition = Vector3.Scale(this.transform.position, Vector3.forward);

        if (cameraPosition.magnitude > threshold)
        {
            for (int scene = 0; scene < SceneManager.sceneCount; scene++)
            {
                foreach (GameObject g in SceneManager.GetSceneAt(scene).GetRootGameObjects())
                {
                    if (g.tag == "Ground")
                    {
                        continue;
                    }
                    g.transform.position -= cameraPosition;
                }
            }

            Vector3 originDelta = Vector3.zero - cameraPosition;
            trackGenerator.UpdateSpawnOrigin(originDelta);
            GameManager.Instance.distanceBase += (int) threshold;
            //Debug.Log("recentering: origin delta = " + originDelta);
        }
    }
}
