/// <summary>
/// Table class of PartyTable
/// PartyTableのテーブルクラス
/// </summary>
[DataAccess("PartyTable")]
public class PartyTable : TableBase
{
    [DataProperty(5, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, true)]
    public string PartyId { get; set; }

    [DataProperty(128, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, false)]
    public string PartyName { get; set; }
}
