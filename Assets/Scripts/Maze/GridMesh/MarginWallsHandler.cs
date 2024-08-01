using UnityEngine;

public class MarginWallsHandler : MonoBehaviour
{
    //[SerializeField] private GameObject marginWallPrefab;
    
    [SerializeField] private WallObject leftMargin;
    [SerializeField] private WallObject bottomMargin;

    private void Start() => EnableMargins(false);

    public void InitMargins(DataGrid dataGrid, float wallsWidth)
    {       
        float positionAdjustment = 0.5f - wallsWidth / 2f;
        
        leftMargin.SetPosition(-0.5f,-dataGrid.RowsCount / 2f  + positionAdjustment);
        bottomMargin.SetPosition(dataGrid.ColumnsCount / 2f  - positionAdjustment - wallsWidth, -dataGrid.RowsCount + 0.5f);

        bottomMargin.SetWidth(wallsWidth);
        leftMargin.SetWidth(wallsWidth);

        bottomMargin.SetLength(dataGrid.ColumnsCount);
        leftMargin.SetLength(dataGrid.RowsCount);

        bottomMargin.transform.rotation = Quaternion.Euler(Vector3.zero);
        leftMargin.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        bottomMargin.gameObject.SetActive(true);
        leftMargin.gameObject.SetActive(true);
    }

    public void SetWallsWidth(DataGrid dataGrid, float width)
    {
        if (leftMargin != null)
        {
            leftMargin.SetWidth(width);
            leftMargin.SetLength(dataGrid.RowsCount);
        }
        
        if (bottomMargin != null)
        {
            bottomMargin.SetWidth(width);
            bottomMargin.SetLength(dataGrid.ColumnsCount);
        }
    }
    
    public void EnableMargins(bool enable)
    {
        leftMargin.gameObject.SetActive(enable);
        bottomMargin.gameObject.SetActive(enable);   
    }
}
