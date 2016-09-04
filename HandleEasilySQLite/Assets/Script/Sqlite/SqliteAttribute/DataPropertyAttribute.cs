using System;

public class DataPropertyAttribute : Attribute
{
    public enum SQLITE_TYPE
    {
        TEXT,
        NUMERIC,
        BOOLEAN,
        ARRAY,
        DATETIME,
    }

    public int MaxLength { get; set; }
    public SQLITE_TYPE SqliteType { get; set; }
    public bool NullOk { get; set; }
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="maxLength">最大文字数</param>
    /// <param name="sqliteType">データ型</param>
    /// <param name="nullOk">NULL許容</param>
    /// <param name="isPrimaryKey">プライマリキー</param>
    public DataPropertyAttribute(int maxLength, SQLITE_TYPE sqliteType, bool nullOk, bool isPrimaryKey)
    {
        this.MaxLength = maxLength;
        this.SqliteType = sqliteType;
        this.NullOk = nullOk;
        this.IsPrimaryKey = isPrimaryKey;
    }
}
