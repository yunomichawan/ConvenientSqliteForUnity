[DataAccess("CharacterTable")]
public class CharacterData : TableBase
{
    [DataProperty(5, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, true)]
    public string CharacterId { get; set; }

    [DataProperty(128, DataPropertyAttribute.SQLITE_TYPE.TEXT, false, false)]
    public string Name { get; set; }

    [DataProperty(3, DataPropertyAttribute.SQLITE_TYPE.NUMERIC, false, true)]
    public int Level { get; set; }

    [DataProperty(6, DataPropertyAttribute.SQLITE_TYPE.NUMERIC, false, false)]
    public int Hp { get; set; }

    [DataProperty(6, DataPropertyAttribute.SQLITE_TYPE.NUMERIC, false, false)]
    public int Attack { get; set; }

    [DataProperty(6, DataPropertyAttribute.SQLITE_TYPE.NUMERIC, false, false)]
    public int Deffence { get; set; }

    [DataProperty(5, DataPropertyAttribute.SQLITE_TYPE.NUMERIC, false, false)]
    public float Speed { get; set; }
}