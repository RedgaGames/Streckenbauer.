using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the persistence and destruction of skid trails.
/// </summary>
public class SkidTrail : MonoBehaviour
{
    [SerializeField] private float _persistTime = 5f;

    private IEnumerator Start()
    {
        while (true)
        {
            yield return null;

            if (transform.parent == null || transform.parent.parent == null)
            {
                Destroy(gameObject, _persistTime);
                yield break; // Exit the coroutine after initiating destruction
            }
        }
    }
}
