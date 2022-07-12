[System.Serializable]
public class InventoryInfo
{
    public CellInfo[] cells;
}

[System.Serializable]
public class CellInfo
{
    public string itemCode;
    public int itemCount;
}


