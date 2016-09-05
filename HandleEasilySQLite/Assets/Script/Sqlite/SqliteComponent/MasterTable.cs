/// <summary>
/// Table class of MasterTable . Deletion of the data can not be
/// MasterTableのテーブルクラス。 データの削除はできません
/// </summary>
[DataAccess("MasterTable")]
[DeleteImpossible]
public class MasterData : TableBase
{
    [DataProperty(3, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, true)]
    public string MasterId { get; set; }

    [DataProperty(32, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, true)]
    public string MasterCategory { get; set; }

    [DataProperty(64, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, false)]
    public string MasterName { get; set; }

    [DataProperty(3, DataPropertyAttribute.SQLITE_TYPE.NUMERIC, false, false)]
    public int Seq { get; set; }

    [DataProperty(512, DataPropertyAttribute.SQLITE_TYPE.TEXT, true, false)]
    public string Remarks { get; set; }
}