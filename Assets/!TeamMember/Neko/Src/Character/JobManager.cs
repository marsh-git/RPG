using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class JobManager : MonoBehaviour
{
    public static JobManager instance;

    //  初期Jobの番号(固定)
    public readonly int START_JOB = 0;

    //  JobDataを配列で持つ(0番目を初期Job)
    [SerializeField] private JobData[] jobDatas;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// ジョブをデータから取得
    /// </summary>
    /// <param name="Num"></param>
    public JobData GetJobData(int Num)
    {
        return jobDatas[Num];
    }

}
