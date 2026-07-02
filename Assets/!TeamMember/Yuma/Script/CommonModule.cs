using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 汎用関数
/// </summary>
public class CommonModule{

    /// <summary>
    /// リストが空か判定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <returns></returns>
    public static bool IsEmpty<T>(List<T> _list) {
        //短絡評価なので大丈夫
        return _list == null || _list.Count <= 0;
    }
    public static bool IsEmpty<T>(T[] _array) {
        //短絡評価なので大丈夫
        return _array == null || _array.Length <= 0;
    }

    /// <summary>
    /// リストに対して有効なインデックスか判定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <param name="_index"></param>
    /// <returns></returns>
    public static bool IsEnableIndex<T>(List<T> _list, int _index) {
        //空
        if (IsEmpty(_list)) return false;

        return _index >= 0 && _list.Count > _index;
    }
    public static bool IsEnableIndex<T>(T[] _array, int _index) {
        //空
        if (IsEmpty(_array)) return false;

        return _index >= 0 && _array.Length > _index;
    }
    public static async UniTask WaitTask(List<UniTask> _taskList) {
        //終了したタスクをリストから除き、リストが空になるまで待つ
        while (true) {
            for (int i = _taskList.Count - 1; i >= 0; i--) {
                //タスクが終了したらリストから抜く
                if (!_taskList[i].Status.IsCompleted()) continue;

                _taskList.RemoveAt(i);
            }
            //もしリストが空ならループを抜ける
            if (IsEmpty(_taskList)) break;
            //1フレーム待つ
            await UniTask.DelayFrame(1);
        }
    }

    public static void InitializeList<T>(ref List<T> _list, int capacity = -1) {
        if (_list == null) {
            if (capacity < 1) {
                _list = new List<T>();
            }
            else {
                _list = new List<T>(capacity);
            }
        }
        else {
            if (_list.Capacity < capacity)
                _list.Capacity = capacity;

            _list.Clear();
        }
    }

    /// <summary>
    /// 一つのオブジェクトのレイヤーをすべて変更
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    public static void SetLayerRecursively(GameObject obj, int layer) {
        if (obj == null) return;

        obj.layer = layer;

        foreach (Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public static void SetLayerCollision(int _targetLayer, int _ignoreLayer,bool _ignore) {
        Physics2D.IgnoreLayerCollision(_targetLayer, _ignoreLayer, _ignore);
    }

}
