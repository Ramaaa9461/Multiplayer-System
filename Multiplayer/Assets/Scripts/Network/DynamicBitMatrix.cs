using System.Collections;
using System.Collections.Generic;

public class DynamicBitMatrix
{
    private List<BitArray> matrix;
    private int columns;

    public DynamicBitMatrix(int columns)
    {
        this.columns = columns;
        matrix = new List<BitArray>();
    }

    public bool Get(int row, int column)
    {
        if (row < matrix.Count)
        {
            return matrix[row][column];
        }
        return false;
    }

    public void Set(int row, int column, bool value)
    {
        while (row >= matrix.Count)
        {
            matrix.Add(new BitArray(columns));
        }
        matrix[row][column] = value;
    }

    public void ClearRow(int row)
    {
        if (row < matrix.Count)
        {
            matrix[row].SetAll(false);
        }
    }

    public int Rows => matrix.Count;
    public int Columns => columns;
}