using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a game object representation of a DataGrid hiding maze generation (faster genration)
/// </summary>
public class NotLiveGenerationGrid : AbsGridObj {
    public override IEnumerator Init(DataGrid _grid) {
        yield return StartCoroutine(base.Init(_grid));
        yield return StartCoroutine(CombineMeshes());
        OnInitCompleted();
    }
}
