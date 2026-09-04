using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttributeFactory{
    private static Dictionary<eAttribute, Func<IAttributeTile>> _attributeRegistry = null;

    /// <summary>
    /// 初期化処理
    /// 登録はここで行う
    /// </summary>
    public static void Initialize() {
        _attributeRegistry = new();
        // 何もない
        Register(eAttribute.Invalid,       () => new NoneAttribute());
        // イベントクラス
        Register(eAttribute.Event,      () => new EventAttribute());
        // 作物クラス
        Register(eAttribute.Crops,      () => new CropsAttribute());
        // 街クラス
        Register(eAttribute.Town,       () => new TownAttribute());
        // 敵の前哨基地クラス
        Register(eAttribute.Outpost,    () => new OutpostAttribute());
        // ショップクラス
        Register(eAttribute.Shop,       () => new ShopAttribute());
        // キャンプ地クラス
        Register(eAttribute.Camp,       () => new CampAttribute());
        // 移動不可能クラス
        Register(eAttribute.CannotMove, () => new CannotMoveAttribute());
    }
    /// <summary>
    /// 属性クラスの登録
    /// </summary>
    /// <param name="type"></param>
    /// <param name="attributeFunc"></param>
    private static void Register(eAttribute attirbute, Func<IAttributeTile> attributeFunc) {
        _attributeRegistry[attirbute] = attributeFunc;
    }
    /// <summary>
    /// 属性クラスの生成
    /// </summary>
    /// <param name="attirbute"></param>
    /// <returns></returns>
    public static IAttributeTile Create(eAttribute attirbute) {
        return _attributeRegistry.TryGetValue(attirbute, out var attributeFunc) ? attributeFunc() : null;
    }
}