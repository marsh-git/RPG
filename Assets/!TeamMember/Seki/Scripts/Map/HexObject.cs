using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileObject : MonoBehaviour {

    /// <summary>
    /// 座標のセットアップ
    /// </summary>
    /// <param name="setPosition"></param>
    public void Setup(Vector3 setPosition) {
        Vector3 position = transform.position;
        position.x = setPosition.x;
        position.y = 0;
        position.z = setPosition.z;
        transform.position = position;
    }
}
