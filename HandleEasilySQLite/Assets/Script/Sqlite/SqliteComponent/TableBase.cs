using System;

/// <summary>
/// テーブルのベースクラス
/// Base class of table
/// </summary>
public class TableBase
{
    [DataProperty(15, DataPropertyAttribute.SQLITE_TYPE.DATETIME, false, true)]
    public DateTime CreateDate { get; set; }

    [DataProperty(15, DataPropertyAttribute.SQLITE_TYPE.DATETIME, false, true)]
    public DateTime UpdateDate { get; set; }
}

