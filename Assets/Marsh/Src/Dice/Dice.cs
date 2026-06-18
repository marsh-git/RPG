using UnityEngine;

public class Dice : MonoBehaviour {
    private Rigidbody rb;
    private bool isStopped = false;
    private int result = 0;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (!isStopped && rb.velocity.magnitude < 0.05f && rb.angularVelocity.magnitude < 0.05f) {
            isStopped = true;
            result = GetTopFace();
        }
    }

    int GetTopFace() {
        DiceFace[] faces = GetComponentsInChildren<DiceFace>();
        DiceFace top = null;
        float maxY = -999f;

        foreach (var f in faces) {
            float y = f.transform.up.y;
            if (y > maxY) {
                maxY = y;
                top = f;
            }
        }
        return top.faceValue;
    }

    public int GetResult() => result;
    public bool IsStopped() => isStopped;
}
