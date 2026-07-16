using UnityEngine;

[System.Serializable]
public struct EventChoiceData
{
    [Header("何を与えるか、何をするか")]
    public bool addStatus;
    public bool addRelic;
    public bool takeDamage;
    public bool takeHeal;

    [Header("キャラクターに与えるステータス補正(何もなければ0)")]
    [SerializeField] public CharacterStatus status;

    [Header("付与するレリック")]
    [SerializeField] RelicDataBase relicData;

    [Header("HP回復、ダメージ")]
    public int damage;
    public int heal;

    [Header("次のページへ飛ぶかどうか")]
    public bool nextPage;

    /// <summary>
    /// イベントスイッチによって行うイベントを実行
    /// </summary>
    public void SwichEvent()
    {
        if (addStatus) PlayerBase.instance.AddPermanentStatus(status);

        if (addRelic) PlayerBase.instance.AddRelic(relicData);

        if(takeDamage) PlayerBase.instance.TakeDamage(damage);

        if(takeHeal) PlayerBase.instance.Heal(heal);

        return;
    }
}
