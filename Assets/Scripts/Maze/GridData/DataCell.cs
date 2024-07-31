using System;

public class DataCell
{
    public event Action OnWallBuiltOrDestroyed;
    public bool IsTopWallActive
    {
        get => isTopWallActive;
        set
        {
            isTopWallActive = value;
            OnWallBuiltOrDestroyed?.Invoke();
        }
    }
    
    public bool IsRightWallActive
    {
        get => isRightWallActive;
        set
        {
            isRightWallActive = value;
            OnWallBuiltOrDestroyed?.Invoke();
        }
    }
    #region ============================================================================================== Public Fields
    
    /// <summary>
    /// Distance from top border
    /// </summary>
    public int PosM { get; }
    
    /// <summary>
    /// Distance from left border
    /// </summary>
    public int PosN { get; }

    #endregion Public Fields
    #region ============================================================================================= Private Fields
    
    /// <summary>
    /// Grid object this cell is associated to
    /// </summary>
    private DataGrid grid;

    private bool isTopWallActive;
    private bool isRightWallActive;

    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public DataCell(DataGrid grid, int posM, int posN)
    {
        PosM = posM;
        PosN = posN;
        this.grid = grid;

        //most of maze generation algorithms start with a grid with walls active
        SetAllWallsActive(true);   
    }
    
    public override string ToString() => "[" + PosM.ToString() + "," + PosN + "]";
    public override int GetHashCode() => PosM * grid.ColumnsCount + PosN;
    public override bool Equals(object obj) {
        DataCell otherCell = obj as DataCell;
        
        if (otherCell == null || otherCell.PosN != PosN || otherCell.PosM != PosM)
            return false;
        
        return true;
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods
    /// <summary>
    /// Enables/disables all walls
    /// </summary>
    /// <param name="_active"></param>
    private void SetAllWallsActive(bool setActive)
    {
        IsTopWallActive = setActive;
        IsRightWallActive = setActive;
    }
    #endregion Private Methods
}
