using UnityEngine;

[System.Serializable]
public struct EventChoiceData
{
    [Header("何を与えるか、何をするか(checkを入れるとイベントが発生)")]
    public bool addStatus;
    public bool addRelic;
    public bool takeDamage;
    public bool takeHeal;

    [Header("キャラクターに与えるステータス補正(何もなければ0)")]
    [SerializeField] public CharacterStatus[] status;

    [Header("付与するレリック")]
    [SerializeField] RelicDataBase[] relicData;

    [Header("HP回復、ダメージ")]
    public int damage;
    public int heal;

    [Header("次のページへ飛ぶかどうか")]
    public bool nextPage;

    [Header("ランダム要素(ランダム要素を入れるか")]
    public bool addRandom;

    /// <summary>
    /// イベントスイッチによって行うイベントを実行
    /// </summary>
    public void SwichEvent()
    {
        if (addStatus && status != null)
        {
            if (addRandom)
            {
                int rand = Random.Range(0, status.Length);
                PlayerBase.instance.AddPermanentStatus(status[rand]);
            }
            else
                PlayerBase.instance.AddPermanentStatus(status[0]);
        }

        if (addRelic && relicData != null)
        {
            if (addRandom)
            {
                int rand = Random.Range(0, relicData.Length);
                PlayerBase.instance.AddRelic(relicData[rand]);
            }
            else
                PlayerBase.instance.AddRelic(relicData[0]);
        }

        if(takeDamage) PlayerBase.instance.TakeDamage(damage);

        if(takeHeal) PlayerBase.instance.Heal(heal);

        return;
    }
}
