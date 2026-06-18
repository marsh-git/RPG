using UnityEngine;

public class Dice : MonoBehaviour {
    private Rigidbody rb;
    private bool isStopped = false;
    private int result = 0;

    public bool cheatEnabled = false;
    public int forcedResult = 0;

    private float stableTime = 0f;
    private bool cheatApplied = false; // ★ 誘導は1回だけ

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (isStopped) return;

        // ★ 速度が小さくなってきたら「止まりそう」
        bool almostStopped =
            rb.velocity.magnitude < 0.4f &&
            rb.angularVelocity.magnitude < 0.4f;

        if (almostStopped) {

            stableTime += Time.deltaTime;

            // ★ 誘導は止まる直前の一瞬だけ
            if (!cheatApplied && cheatEnabled && forcedResult != 0 && stableTime > 0.2f) {
                ApplyCheatTorque(forcedResult);
                cheatApplied = true;
            }

            // ★ 完全停止
            if (stableTime > 0.8f) {
                isStopped = true;
                result = GetTopFace();
            }
        }
        else {
            stableTime = 0f;
            cheatApplied = false;
        }
    }

    // ★ 自然な誘導トルク
    void ApplyCheatTorque(int face) {
        DiceFace target = null;
        foreach (var f in GetComponentsInChildren<DiceFace>()) {
            if (f.faceValue == face) {
                target = f;
                break;
            }
        }
        if (target == null) return;

        Vector3 desiredUp = target.transform.forward;

        Vector3 axis = Vector3.Cross(transform.up, desiredUp);
        float angle = Vector3.SignedAngle(transform.up, desiredUp, axis);

        // ★ トルクは極小にする（これが自然さの秘密）
        float torqueStrength = 0.02f;

        rb.AddTorque(axis.normalized * angle * torqueStrength, ForceMode.VelocityChange);
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
