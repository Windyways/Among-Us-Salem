using Object = UnityEngine.Object;
using UnityEngine;
using System.Collections;

[RegisterInIl2Cpp]
public class SandParticle(IntPtr ptr) : MonoBehaviour(ptr)
{
    public float speed = 10f;
    public float lifetime = 2f;
    private float timer;

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= lifetime) Destroy(gameObject);
    }
}