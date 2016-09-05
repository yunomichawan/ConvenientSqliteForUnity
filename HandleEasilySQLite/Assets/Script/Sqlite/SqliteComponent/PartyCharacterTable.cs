/// <summary>
/// Table class of PartyCharacterTable
/// PartyCharacterTableのテーブルクラス
/// </summary>
[DataAccess("PartyCharacterTable")]
public class PartyCharacterTable : TableBase
{
    [DataProperty(5, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, true)]
    public string PartyId { get; set; }

    [DataProperty(5, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, true)]
    public string CharacterId { get; set; }
}
