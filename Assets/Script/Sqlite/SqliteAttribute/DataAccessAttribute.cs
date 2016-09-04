using System;

public class DataAccessAttribute : Attribute
{
    public string TableName { get; set; }

    public DataAccessAttribute(string tableName)
    {
        this.TableName = tableName;
    }
}
