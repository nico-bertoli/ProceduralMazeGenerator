using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a game object representation of a DataGrid showing maze generation (slower genration)
/// </summary>
public class LiveGenerationGrid : AbsGridObj {
    public override IEnumerator Init(DataGrid _grid) {
        yield return StartCoroutine(base.Init(_grid));
        yield return StartCoroutine(setCellsActive(true));
        SetWallsMeshActive(true);
        OnInitCompleted();
    }
}
