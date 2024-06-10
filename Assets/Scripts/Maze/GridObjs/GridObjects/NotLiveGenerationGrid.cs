using System.Collections;

public class NotLiveGenerationGrid : AbsGridObj {
    public override IEnumerator Init(DataGrid grid) {
        yield return StartCoroutine(base.Init(grid));
        yield return StartCoroutine(GenerateChunks());
        OnInitCompleted();
    }
}
