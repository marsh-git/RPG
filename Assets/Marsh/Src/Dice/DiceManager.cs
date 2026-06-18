using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceFace : MonoBehaviour {
    public int faceValue; // この面の数字（1〜6）
}


public class DiceManager : MonoBehaviour {
    [SerializeField] private Dice dicePrefab;
    [SerializeField] private Transform spawnPoint;

    private List<Dice> dices = new List<Dice>();

    public IEnumerator RollDice(int count, System.Action<Dictionary<int, int>, int> onComplete) {
        dices.Clear();

        // サイコロ生成 & 投げる
        for (int i = 0; i < count; i++) {
            Dice d = Instantiate(dicePrefab, spawnPoint.position, Random.rotation);
            Rigidbody rb = d.GetComponent<Rigidbody>();

            // DEMEO風：ランダム方向に投げる
            Vector3 force = new Vector3(
                Random.Range(-2f, 2f),
                5f,
                Random.Range(3f, 6f)
            );

            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

            dices.Add(d);
        }

        // 全サイコロが止まるまで待つ
        yield return new WaitUntil(() => dices.TrueForAll(d => d.IsStopped()));

        // 出目集計
        Dictionary<int, int> counts = new Dictionary<int, int>();
        int total = 0;

        foreach (var d in dices) {
            int r = d.GetResult();
            total += r;

            if (!counts.ContainsKey(r)) counts[r] = 0;
            counts[r]++;
        }

        onComplete?.Invoke(counts, total);
    }
}
