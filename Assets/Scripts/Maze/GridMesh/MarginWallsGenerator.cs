using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarginWallsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject marginWallPrefab;
    
    private WallObject leftMargin;
    private WallObject bottomMargin;

    public void InitMargins(DataGrid dataGrid, float wallsWidth) {
        
        leftMargin = Instantiate(marginWallPrefab).GetComponent<WallObject>();
        bottomMargin = Instantiate(marginWallPrefab).GetOrAddComponent<WallObject>();

        leftMargin.gameObject.name = "Left margin";
        bottomMargin.gameObject.name = "Bottom margin";
        
        float positionAdjustment = 0.5f - wallsWidth / 2f;
        
        leftMargin.SetPosition(-0.5f,-dataGrid.RowsCount / 2f  + positionAdjustment);
        bottomMargin.SetPosition(dataGrid.ColumnsCount / 2f  - positionAdjustment - wallsWidth, -dataGrid.RowsCount + 0.5f);

        bottomMargin.SetWidth(wallsWidth);
        leftMargin.SetWidth(wallsWidth);
        bottomMargin.SetLength(dataGrid.ColumnsCount);
        leftMargin.SetLength(dataGrid.RowsCount);

        leftMargin.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        leftMargin.transform.parent = bottomMargin.transform.parent = transform;
        bottomMargin.gameObject.SetActive(true);
        leftMargin.gameObject.SetActive(true);
        leftMargin.SetMeshActive(true);
        bottomMargin.SetMeshActive(true);
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
    
    public void Reset()
    {
        if(leftMargin != null)
            Destroy(leftMargin.gameObject);
        
        if(bottomMargin != null)
            Destroy(bottomMargin.gameObject);
    }
}
