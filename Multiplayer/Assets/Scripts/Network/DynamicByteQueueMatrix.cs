using System.Collections;
using System.Collections.Generic;

public class DynamicByteQueueMatrix
{
    private List<Dictionary<MessageType, Queue<byte[]>>> matrix;

    public DynamicByteQueueMatrix()
    {
        matrix = new List<Dictionary<MessageType, Queue<byte[]>>>();
    }

    public Queue<byte[]> Get(int row, MessageType column)
    {
        if (row < matrix.Count && matrix[row].ContainsKey(column))
        {
            return matrix[row][column];
        }
        return new Queue<byte[]>();
    }

    public void Set(int row, MessageType column, Queue<byte[]> value)
    {
        while (row >= matrix.Count)
        {
            matrix.Add(new Dictionary<MessageType, Queue<byte[]>>());
        }
        matrix[row][column] = value;
    }

    public void ClearRow(int row)
    {
        if (row < matrix.Count)
        {
            matrix[row].Clear();
        }
    }

    public int Rows => matrix.Count;
}

public class DynamicFloatQueueMatrix
{
    private List<Dictionary<MessageType, Queue<float>>> matrix;

    public DynamicFloatQueueMatrix()
    {
        matrix = new List<Dictionary<MessageType, Queue<float>>>();
    }

    public Queue<float> Get(int row, MessageType column)
    {
        if (row < matrix.Count && matrix[row].ContainsKey(column))
        {
            return matrix[row][column];
        }
        return new Queue<float>();
    }

    public void Set(int row, MessageType column, Queue<float> value)
    {
        while (row >= matrix.Count)
        {
            matrix.Add(new Dictionary<MessageType, Queue<float>>());
        }
        matrix[row][column] = value;
    }

    public void ClearRow(int row)
    {
        if (row < matrix.Count)
        {
            matrix[row].Clear();
        }
    }

    public int Rows => matrix.Count;
}
