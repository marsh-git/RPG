using UnityEngine;

public class Dice : MonoBehaviour {
    private Rigidbody rb;
    private bool isStopped = false;
    private int result = 0;
    private float stableTime = 0f;

    private float lifeTime = 0f;
    public float maxLifeTime = 7.5f;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }


    void Update() {

        lifeTime += Time.deltaTime;
        if (lifeTime >= maxLifeTime) {
            Destroy(gameObject);
            return;
        }

        if (isStopped) return;        
        result = GetTopFace();

        bool almostStopped =
            rb.velocity.magnitude < 0.1f &&
            rb.angularVelocity.magnitude < 0.1f;

        if (almostStopped) {

            stableTime += Time.deltaTime;

            // 0.5秒以上静止 → 本当に止まった
            if (stableTime > 0.5f) {
                isStopped = true;
                result = GetTopFace();
            }
        }
        else {
            // 動いている間はリセット
            stableTime = 0f;
        }
    }


    int GetTopFace() {
        DiceFace[] faces = GetComponentsInChildren<DiceFace>();

        DiceFace top = null;
        float maxDot = -999f;

        foreach (var f in faces) {
            float dot = Vector3.Dot(f.transform.forward, Vector3.up);
            if (dot > maxDot) {
                maxDot = dot;
                top = f;
            }
        }
        return top.faceValue;
    }

    public int GetResult() => result;
    public bool IsStopped() => isStopped;
}
