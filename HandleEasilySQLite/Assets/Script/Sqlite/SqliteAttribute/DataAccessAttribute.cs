using System;

/// <summary>
/// テーブル名を定義する属性
/// Attributes that define the table name
/// </summary>
public class DataAccessAttribute : Attribute
{
    public string TableName { get; set; }

    public DataAccessAttribute(string tableName)
    {
        this.TableName = tableName;
    }
}
