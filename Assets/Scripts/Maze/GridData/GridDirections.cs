using static DataGrid;
using System.Collections.Generic;
using System;
using System.Linq;

public static class GridDirections
{
    public enum Directions { Up, Right, Down, Left };
    public static Directions GetInverseDirection(Directions direction)
    {
        switch (direction)
        {
            case Directions.Up: return Directions.Down;
            case Directions.Down: return Directions.Up;
            case Directions.Left: return Directions.Right;
            case Directions.Right: return Directions.Left;
            default: throw new Exception($"{direction} not recognized");
        }
    }

    public static List<Directions> GetAllDirections() => ((Directions[])Enum.GetValues(typeof(Directions))).ToList();
}
