using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerExit : MonoBehaviour
{
    public float delay = 1f;

    public delegate void ExitAction();
    public static event ExitAction OnChunkExited;

    private bool exited = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!exited)
            {
                exited = true;
                OnChunkExited();
                StartCoroutine(WaitAndDeactivate());
            }
        }
    }

    IEnumerator WaitAndDeactivate()
    {
        yield return new WaitForSeconds(delay);
        Destroy(transform.root.gameObject);
    }
}
