using System;

/// <summary>
/// Attribute to set the property to
/// プロパティに設定する属性
/// </summary>
public class DataPropertyAttribute : Attribute
{
    /// <summary>
    /// Data types that are stored in the database
    /// データベースで格納しているデータ型
    /// </summary>
    public enum SQLITE_TYPE
    {
        /// <summary>
        /// PropertyType is string
        /// プロパティのタイプがstring
        /// </summary>
        TEXT,

        /// <summary>
        /// Type of property is a numeric type(int,float,double)
        /// プロパティのタイプが数値型(int,float,double)
        /// </summary>
        NUMERIC,

        /// <summary>
        /// PropertyType is bool
        /// プロパティのタイプがbool
        /// </summary>
        BOOLEAN,

        /// <summary>
        /// PropertyType is List <string>. Data to be stored is a comma ( , ) separator
        /// プロパティのタイプがList<string> 。格納されるデータがカンマ(,)区切り
        /// </summary>
        ARRAY,

        /// <summary>
        /// PropertyType is DateTime
        /// プロパティのタイプがDateTime
        /// </summary>
        DATETIME,
    }

    /// <summary>
    /// MaxLength
    /// 最大文字数
    /// </summary>
    public int MaxLength { get; set; }

    /// <summary>
    /// Data types that are stored in the database
    /// データベースで格納しているデータ型
    /// </summary>
    public SQLITE_TYPE SqliteType { get; set; }

    /// <summary>
    /// NULL permission settings (NUll = true, Not Null = false)
    /// NULL許可設定 (NUll = true, Not Null = false)
    /// </summary>
    public bool NullOk { get; set; }

    /// <summary>
    /// Primary Key Settings (PrimaryKey = true, Not PrimaryKey = false)
    /// プライマリキー設定 (PrimaryKey = true, Not PrimaryKey = false)
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// constructor
    /// コンストラクタ
    /// </summary>
    /// <param name="maxLength"></param>
    /// <param name="sqliteType"></param>
    /// <param name="nullOk"></param>
    /// <param name="isPrimaryKey"></param>
    public DataPropertyAttribute(int maxLength, SQLITE_TYPE sqliteType, bool nullOk, bool isPrimaryKey)
    {
        this.MaxLength = maxLength;
        this.SqliteType = sqliteType;
        this.NullOk = nullOk;
        this.IsPrimaryKey = isPrimaryKey;
    }
}
