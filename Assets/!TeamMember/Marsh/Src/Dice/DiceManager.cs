using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour {
    [SerializeField] private Dice dicePrefab;
    [SerializeField] private Transform spawnPoint;

    private List<Dice> dices = new List<Dice>();

    void Update() {
        // 左クリックでサイコロを振る
        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(RollDice(1)); // 1個だけ投げる例
        }
    }

    public IEnumerator RollDice(int count) {
        dices.Clear();

        for (int i = 0; i < count; i++) {
            Dice d = Instantiate(dicePrefab, spawnPoint.position, Random.rotation);
            Rigidbody rb = d.GetComponent<Rigidbody>();

            // 投げる力
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

        // 結果表示
        foreach (var d in dices) {
            Debug.Log("出目: " + d.GetResult());
        }
    }
}
