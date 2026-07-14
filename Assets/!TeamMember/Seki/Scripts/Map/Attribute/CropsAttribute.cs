using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropsAttribute : IAttributeTile {
    public eAttribute AttributeType => eAttribute.Crops;

    public void OnEnterTile(HexTileData tile, CharacterBase character = null) {
        throw new System.NotImplementedException();
    }

    public void OnTickTile(HexTileData tile) {
        throw new System.NotImplementedException();
    }

    public void OnTurnEndOnTile(HexTileData tile, CharacterBase character = null) {
        throw new System.NotImplementedException();
    }
}
