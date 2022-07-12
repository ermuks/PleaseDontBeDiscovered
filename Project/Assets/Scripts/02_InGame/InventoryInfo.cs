[System.Serializable]
public class InventoryInfo
{
    public int index;
    public CellInfo[] cells;
}

[System.Serializable]
public class CellInfo
{
    public string itemCode;
    public int itemCount;
}


