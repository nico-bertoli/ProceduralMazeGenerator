using System;

public class DataCell
{
    public event Action OnWallBuiltOrDestroyed;

    #region ============================================================================================== Public Properties
    public bool IsTopWallActive { get; private set; }
    public bool IsRightWallActive { get; private set; }

    #endregion Public Properties
    #region ============================================================================================== Fields

    /// <summary>
    /// Distance from top border
    /// </summary>
    public readonly short PosM;

    /// <summary>
    /// Distance from left border
    /// </summary>
    public readonly short PosN;

    private readonly DataGrid dataGrid;

    #endregion Fields
    #region ============================================================================================= Public Methods

    //-------------------- Constructors
    public DataCell(DataGrid dataGrid, short posM, short posN)
    {
        PosM = posM;
        PosN = posN;
        this.dataGrid = dataGrid;

        //most of maze generation algorithms start with a grid with walls active
        IsTopWallActive = true;
        IsRightWallActive = true;
    }

    //-------------------- Overrides
    public override string ToString() => "[" + PosM.ToString() + "," + PosN + "]";
    public override int GetHashCode() => (int)(PosM * dataGrid.ColumnsCount + PosN);
    public override bool Equals(object obj)
    {
        DataCell otherCell = obj as DataCell;

        if (otherCell == null || otherCell.PosN != PosN || otherCell.PosM != PosM)
            return false;

        return true;
    }

    //-------------------- Walls
    public void SetTopWallActive(bool value)
    {
        if (IsTopWallActive == value)
            return;

        IsTopWallActive = value;
        OnWallBuiltOrDestroyed?.Invoke();
    }

    public void SetRightWallActive(bool value)
    {
        if (IsRightWallActive == value)
            return;

        IsRightWallActive = value;
        OnWallBuiltOrDestroyed?.Invoke();
    }
    #endregion Public Methods
}
