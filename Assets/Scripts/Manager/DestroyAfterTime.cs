using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{

    [SerializeField] private float timeToDestroy = 4f;

    private void Start() => Invoke(nameof(Destroy), timeToDestroy);

    private void Destroy() => Destroy(gameObject);

}