using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

/* 
 *
 *
 *
 *
 *
 *
 *
 *
*/

public class DataAccess : SingletonComponent<DataAccess>
{
    private const string CONNECT_TABLE = "SampleDb.db";

    private const string SELECT_SQL = "select {0} from {1}";
    private const string INSERT_SQL = "insert into {0} {1}";
    private const string UPDATE_SQL = "update {0} set";
    private const string DELETE_SQL = "delete from {0}";
    private const string ORDER_BY_SQL = "order by {0}";

    private StringBuilder ExecutetSql { get; set; }
    private Dictionary<Type, string> SelectType { get; set; }
    private StringBuilder WhereSql { get; set; }
    private StringBuilder JoinSql { get; set; }
    private Dictionary<string, string> PrimarySql { get; set; }
    private string[] OrderByColumns { get; set; }
    private Type BaseType;
    private DataTable SelectTable { get; set; }

    #region 初期化

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DataAccess()
    {
        ExecutetSql = new StringBuilder();
        SelectType = new Dictionary<Type, string>();
        WhereSql = new StringBuilder();
        JoinSql = new StringBuilder();
        PrimarySql = new Dictionary<string, string>();
        OrderByColumns = new string[] { };
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="type"></param>
    public DataAccess(Type type)
    {
        ExecutetSql = new StringBuilder();
        SelectType = new Dictionary<Type, string>();
        WhereSql = new StringBuilder();
        JoinSql = new StringBuilder();
        PrimarySql = new Dictionary<string, string>();
        OrderByColumns = new string[] { };
        this.BaseType = type;
        this.AddSelectType(type);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(Type type)
    {
        ExecutetSql = new StringBuilder();
        SelectType = new Dictionary<Type, string>();
        WhereSql = new StringBuilder();
        JoinSql = new StringBuilder();
        PrimarySql = new Dictionary<string, string>();
        OrderByColumns = new string[] { };
        this.BaseType = type;
        this.AddSelectType(type);
    }

    #endregion

    #region 条件設定

    /// <summary>
    /// 条件文設定
    /// </summary>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <param name="andOr"></param>
    public void AddCondition(string column, object value, Type type, bool equal)
    {
        if (value == null) return;
        this.AddOperator(true);
        DataAccessAttribute attribute = this.GetDataAccessAttribute(type);
        AddConditionValue(column, attribute, value.ToString(), equal);
    }

    /// <summary>
    /// 条件文(in)設定
    /// </summary>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <param name="andOr"></param>
    public void AddInCondition(string column, string[] values, Type type, bool equal = true)
    {
        if (values == null) return;
        this.AddOperator(true);
        DataAccessAttribute attribute = this.GetDataAccessAttribute(type);
        AddInConditionValue(column, attribute, values, equal);
    }

    /// <summary>
    /// 条件文追加設定
    /// </summary>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <param name="andOr"></param>
    public void AddCondition(string column, object value, Type type, bool isAnd, bool equal)
    {
        if (value == null) return;
        this.AddOperator(isAnd);
        DataAccessAttribute attribute = this.GetDataAccessAttribute(type);
        AddConditionValue(column, attribute, value.ToString(), equal);
    }

    /// <summary>
    /// 条件文(in)追加設定
    /// </summary>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <param name="andOr"></param>
    public void AddInCondition(string column, string[] values, Type type, bool isAnd, bool equal = true)
    {
        if (values == null) return;
        this.AddOperator(isAnd);
        DataAccessAttribute attribute = this.GetDataAccessAttribute(type);
        AddInConditionValue(column, attribute, values, equal);
    }

    /// <summary>
    /// 条件値設定
    /// </summary>
    /// <param name="column"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    /// <param name="equal"></param>
    private void AddConditionValue(string column, DataAccessAttribute attribute, string value, bool equal)
    {
        // エスケープ処理
        if (value.Contains("'"))
        {
            value = value.Replace("'", "''");
        }

        if (equal)
        {
            WhereSql.AppendLine(string.Format("{0}.{1} = '{2}'", attribute.TableName, column, value.ToString()));
        }
        else
        {
            WhereSql.AppendLine(string.Format("{0}.{1} != '{2}'", attribute.TableName, column, value.ToString()));
        }
    }

    /// <summary>
    /// 条件値(in)設定
    /// </summary>
    /// <param name="column"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    private void AddInConditionValue(string column, DataAccessAttribute attribute, string[] values, bool equal)
    {
        // エスケープ
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i].Contains("'"))
            {
                values[i] = values[i].Replace("'", "''");
            }
        }

        if (equal)
        {
            WhereSql.AppendLine(string.Format("{0}.{1} in ({2})", attribute.TableName, column, string.Format("'{0}'", string.Join("','", values))));
        }
        else
        {
            WhereSql.AppendLine(string.Format("{0}.{1} not in ({2})", attribute.TableName, column, string.Format("'{0}'", string.Join("','", values))));
        }
    }

    /// <summary>
    /// And Or 設定
    /// </summary>
    /// <param name="andOr"></param>
    private void AddOperator(bool isAnd)
    {
        if (string.IsNullOrEmpty(WhereSql.ToString()))
        {
            WhereSql.AppendLine("where");
        }
        else
        {
            if (isAnd)
            {
                WhereSql.AppendLine("and");
            }
            else
            {
                WhereSql.AppendLine("or");
            }
        }
    }

    #endregion

    #region テーブル結合

    /// <summary>
    /// 結合テーブル追加
    /// And結合のみ
    /// </summary>
    /// <param name="baseType"></param>
    /// <param name="joinColumns">同名列</param>
    /// <param name="isInner"></param>
    public void AddJoinTable(Type baseType, Type joinType, string[] joinColumns, bool isInner = true)
    {
        DataAccessAttribute attribite = this.GetDataAccessAttribute(joinType);
        string tableName = this.GetDataAccessAttribute(baseType).TableName;
        string joinTableType = (isInner) ? "inner" : "left";
        StringBuilder joinConditions = new StringBuilder();

        for (int i = 0; i < joinColumns.Length; i++)
        {
            if (i.Equals(0)) joinConditions.Append(" and ");
            joinConditions.Append(string.Format("{0}.{1} = {2}.{3}", tableName, joinColumns[i], attribite.TableName, joinColumns[i]));
        }

        JoinSql.AppendLine(string.Format("{0} join {1} on {2}", joinTableType, attribite.TableName, joinConditions.ToString()));

        this.AddSelectType(joinType);
    }

    /// <summary>
    /// 結合テーブル追加
    /// And結合のみ
    /// </summary>
    /// <param name="baseType"></param>
    /// <param name="joinColumns">同名列</param>
    /// <param name="isInner"></param>
    public void AddJoinTable(Type baseType, Type joinType, string joinColumn, bool isInner = true)
    {
        this.AddJoin(baseType, joinType, joinColumn, joinColumn, isInner);
    }

    /// <summary>
    /// 結合テーブル追加
    /// And結合のみ
    /// </summary>
    /// <param name="baseType"></param>
    /// <param name="joinType"></param>
    /// <param name="baseColumn"></param>
    /// <param name="joinColumn"></param>
    /// <param name="isInner"></param>
    public void AddJoinTable(Type baseType, Type joinType, string baseColumn, string joinColumn, bool isInner = true)
    {
        this.AddJoin(baseType, joinType, baseColumn, joinColumn, isInner);
    }

    /// <summary>
    /// 結合テーブル追加(private)
    /// And結合のみ
    /// </summary>
    /// <param name="baseType"></param>
    /// <param name="joinType"></param>
    /// <param name="baseColumn"></param>
    /// <param name="joinColumn"></param>
    /// <param name="isInner"></param>
    private void AddJoin(Type baseType, Type joinType, string baseColumn, string joinColumn, bool isInner = true)
    {
        string joinTableName = this.GetDataAccessAttribute(joinType).TableName;
        string baseTableName = this.GetDataAccessAttribute(baseType).TableName;
        string joinTableType = (isInner) ? "inner" : "left";
        StringBuilder joinConditions = new StringBuilder();
        joinConditions.Append(string.Format("{0}.{1} = {2}.{3}", baseTableName, baseColumn, joinTableName, joinColumn));
        JoinSql.AppendLine(string.Format("{0} join {1} on {2}", joinTableType, joinTableName, joinConditions.ToString()));

        this.AddSelectType(joinType);
    }

    /// <summary>
    /// 結合時セレクト列を回収
    /// </summary>
    /// <param name="type"></param>
    private void AddSelectType(Type type)
    {
        if (!this.SelectType.ContainsKey(type))
        {
            this.SelectType.Add(type, this.GetSqliteColumn(type));
        }
    }

    #endregion

    #region ソート設定

    /// <summary>
    /// ソート項目追加
    /// </summary>
    public void AddOrderByColumns(string column, Type type)
    {
        // 下記の処理をやるよりだったら素直にList<T>を使った方が利口ではないだろうか？
        string[] baseArray = this.OrderByColumns;

        string[] newArray = new string[this.OrderByColumns.Length + 1];

        //配列の要素をコピーする
        Array.Copy(baseArray, newArray, Math.Min(baseArray.Length, newArray.Length));
        //できた配列をintArrayに戻す
        this.OrderByColumns = newArray;

        this.OrderByColumns[this.OrderByColumns.Length - 1] = string.Format("{0}.{1}", this.GetDataAccessAttribute(type).TableName, column);
    }

    /// <summary>
    /// ソート項目を設定(ソート対象の列が全て同じ場合のみ)
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="type"></param>
    public void SetOrderByColumns(string[] columns, Type type)
    {
        string[] orderColumns = new string[columns.Length];
        string tableName = this.GetDataAccessAttribute(type).TableName;

        for (int i = 0; i < columns.Length; i++)
        {
            orderColumns[i] = string.Format("{0}.{1}", tableName, columns[i]);
        }
        this.OrderByColumns = orderColumns;
    }

    #endregion

    #region セレクト文発行

    /// <summary>
    /// 手動生成セレクトSQL文発行。複雑なSQLを発行する際に使用することを想定
    /// </summary>
    /// <returns></returns>
    public List<T> GetDataList<T>(string sql)
    {
        DataTable dataTable = new DataTable();
        try
        {
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            dataTable = sqliteDataBase.ExecuteQuery(sql);
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + sql);
        }
        return DataBinding<T>.DataTableToObjectList(dataTable);
    }

    /// <summary>
    /// セレクトSQL文発行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<T> GetDataList<T>()
    {
        try
        {
            this.CreateSelectSql();
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            this.SelectTable = sqliteDataBase.ExecuteQuery(this.ExecutetSql.ToString());
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
        }
        finally
        {
            Debug.Log(this.ExecutetSql.ToString());
            this.Init(this.BaseType);
        }

        return DataBinding<T>.DataTableToObjectList(this.SelectTable);
    }

    /// <summary>
    /// セレクトSQL文発行
    /// </summary>
    /// <returns></returns>
    public DataTable GetDataTable()
    {
        try
        {
            this.CreateSelectSql();
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            this.SelectTable = sqliteDataBase.ExecuteQuery(this.ExecutetSql.ToString());
            Debug.Log(this.ExecutetSql.ToString());
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
        }
        finally
        {
            this.Init(this.BaseType);
        }

        return this.SelectTable;
    }

    /// <summary>
    /// セレクトSQL文発行
    /// </summary>
    /// <returns></returns>
    public DataTable GetDataTable(string sql)
    {
        try
        {
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            this.SelectTable = sqliteDataBase.ExecuteQuery(sql);
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + sql);
        }
        finally
        {
            this.Init(this.BaseType);
        }

        return this.SelectTable;
    }

    /// <summary>
    /// セレクトSQL文発行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetDataFirst<T>()
    {
        try
        {
            this.CreateSelectSql();
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            Debug.Log(ExecutetSql.ToString());
            this.SelectTable = sqliteDataBase.ExecuteQuery(this.ExecutetSql.ToString());

            if (this.SelectTable.Rows.Count.Equals(0))
            {
                return default(T);
            }

            T data = (T)Activator.CreateInstance(typeof(T), new object[] { });
            DataBinding<T>.DataRowToObject(this.SelectTable.Rows[0], data);

            return data;
        }
        catch (SqliteException ex)
        {
            Debug.LogError(ex.Message + " " + this.ExecutetSql.ToString());
        }
        finally
        {
            this.Init(this.BaseType);
        }

        return default(T);
    }

    /// <summary>
    /// 対象列から重複せずにデータを取得
    /// </summary>
    /// <param name="column"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public string[] GetGroupDataArray(string column, Type type)
    {
        try
        {
            string groupSql = "select {0} from {1} group by {2}";
            this.ExecutetSql.AppendLine(string.Format(groupSql, column, this.GetDataAccessAttribute(type).TableName, column));
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            this.SelectTable = sqliteDataBase.ExecuteQuery(this.ExecutetSql.ToString());

            if (SelectTable.Rows.Count.Equals(0))
            {
                return new string[] { };
            }

            string[] dataArray = new string[this.SelectTable.Rows.Count];

            for (int i = 0; i < this.SelectTable.Rows.Count; i++)
            {
                dataArray[i] = this.SelectTable.Rows[i][column].ToString();
            }

            return dataArray;
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
        }

        return new string[] { };
    }

    /// <summary>
    /// ユニークデータ存在チェック
    /// </summary>
    /// <param name="component"></param>
    /// <returns>存在:true、非存在:false</returns>
    public bool ChkPrimaryData(object component)
    {
        try
        {
            string countColumn = "Count";
            string countSql = "select count(*) as {0} from {1}";
            this.ExecutetSql.AppendLine(string.Format(countSql, countColumn, this.GetDataAccessAttribute(component.GetType()).TableName));

            this.GetPrimaryKeyValue(component);
            this.CreatePrimaryWhere(component.GetType());

            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            this.SelectTable = sqliteDataBase.ExecuteQuery(this.ExecutetSql.ToString());

            int count = int.Parse(this.SelectTable.Rows[0][countColumn].ToString());
            return !count.Equals(0);
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
        }

        return false;
    }

    /// <summary>
    /// セレクトSQL文生成
    /// </summary>
    private void CreateSelectSql()
    {
        this.ExecutetSql = new StringBuilder();
        DataAccessAttribute attribute = this.GetDataAccessAttribute(this.BaseType);
        ExecutetSql.AppendLine(string.Format(SELECT_SQL, this.CreateSelectColumn(), attribute.TableName));
        ExecutetSql.AppendLine(this.JoinSql.ToString());
        ExecutetSql.AppendLine(this.WhereSql.ToString());

        if (!this.OrderByColumns.Length.Equals(0))
        {
            ExecutetSql.AppendLine(string.Format(ORDER_BY_SQL, string.Join(",", this.OrderByColumns)));
        }
    }

    /// <summary>
    /// セレクトSQL列文を生成
    /// </summary>
    /// <returns></returns>
    private string CreateSelectColumn()
    {
        bool firstTable = true;
        StringBuilder stringBuilder = new StringBuilder();

        foreach (Type type in this.SelectType.Keys)
        {
            if (firstTable)
            {
                firstTable = false;
            }
            else
            {
                stringBuilder.Append(",");
            }

            stringBuilder.AppendLine(this.SelectType[type]);
        }

        return stringBuilder.ToString();
    }

    #endregion

    #region インサート文発行

    /// <summary>
    /// インサートSQL発行
    /// </summary>
    /// <param name="component"></param>
    public void Insert(object component)
    {
        try
        {
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            this.CreateInsertSql(component);
            sqliteDataBase.ExecuteNonQuery(this.ExecutetSql.ToString());
        }
        catch (SqliteException ex)
        {
            Debug.LogError(ex.Message + " " + this.ExecutetSql.ToString());
        }
    }

    /// <summary>
    /// インサートSQLを複数発行
    /// </summary>
    /// <param name="componentList"></param>
    public void InsertMulti<T>(List<T> componentList)
    {
        try
        {
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            foreach (object component in componentList)
            {
                try
                {
                    this.CreateInsertSql(component);
                    sqliteDataBase.ExecuteNonQuery(this.ExecutetSql.ToString());
                }
                catch (SqliteException ex)
                {
                    Debug.LogError(ex.Message);
                    break;
                }
            }
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
        }
    }

    /// <summary>
    /// インサートSQL文生成
    /// </summary>
    /// <param name="component"></param>
    private void CreateInsertSql(object component)
    {
        this.ExecutetSql = new StringBuilder();
        string sql = string.Format(INSERT_SQL, this.GetDataAccessAttribute(component.GetType()).TableName, this.GetInsertUpdateSql(component, true));
        this.ExecutetSql.AppendLine(sql);
        //this.ExecutetSql.AppendLine(this.GetDateFormat());
        //this.ExecutetSql.AppendLine(this.GetDateFormat());
        Debug.Log(this.ExecutetSql.ToString());
    }

    #endregion

    #region アップデート文発行

    /// <summary>
    /// アップデートSQL発行
    /// MonoBehaviour
    /// </summary>
    public void UpdateSql(object component)
    {
        SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
        // トランザクション開始
        sqliteDataBase.TransactionStart();
        try
        {
            this.CreateUpdateSql(component);
            sqliteDataBase.ExecuteNonQueryEx(this.ExecutetSql.ToString());
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
            // ロールバック
            sqliteDataBase.TransactionRollBack();
        }
        finally
        {
            this.Init(component.GetType());
        }
        // コミット
        sqliteDataBase.TransactionCommit();
    }

    /// <summary>
    /// アップデートSQLを複数発行。プライマリキー指定以外不可。
    /// </summary>
    /// <param name="componentList"></param>
    public void UpdateMulti<T>(List<T> componentList)
    {
        SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
        // トランザクション開始
        sqliteDataBase.TransactionStart();
        this.WhereSql = new StringBuilder();
        try
        {
            foreach (object component in componentList)
            {
                try
                {
                    this.CreateUpdateSql(component);
                    sqliteDataBase.ExecuteNonQueryEx(this.ExecutetSql.ToString());
                    this.Init(component.GetType());
                }
                catch (SqliteException ex)
                {
                    Debug.LogError(ex.Message);
                    break;
                }
            }
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
            // ロールバック
            sqliteDataBase.TransactionRollBack();
        }
        finally
        {
            this.Init(componentList[0].GetType());
        }
        // コミット
        sqliteDataBase.TransactionCommit();
    }

    /// <summary>
    /// アップデートSQL文生成
    /// </summary>
    /// <param name="component"></param>
    private void CreateUpdateSql(object component)
    {
        this.ExecutetSql = new StringBuilder();
        string tableName = this.GetDataAccessAttribute(component.GetType()).TableName;
        string sql = string.Format(UPDATE_SQL, tableName);
        this.ExecutetSql.AppendLine(sql);
        this.ExecutetSql.AppendLine(this.GetInsertUpdateSql(component, false));
        this.CreateDeleteUpdateWhere(tableName, component);
        Debug.Log(this.ExecutetSql.ToString());
    }

    #endregion

    #region デリート文発行

    /// <summary>
    /// デリートSQL文発行。
    /// </summary>
    /// <param name="component"></param>
    public void Delete(object component)
    {
        SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
        sqliteDataBase.TransactionStart();
        try
        {
            this.CreateDeleteSql(component);
            sqliteDataBase.ExecuteNonQueryEx(this.ExecutetSql.ToString());
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
            sqliteDataBase.TransactionRollBack();
        }
        finally
        {
            this.Init(component.GetType());
        }
        sqliteDataBase.TransactionCommit();
    }

    /// <summary>
    /// デリートSQLを複数発行。プライマリキー指定以外不可。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="componentList"></param>
    public void DeleteMulti<T>(List<T> componentList)
    {
        SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
        // トランザクション開始
        sqliteDataBase.TransactionStart();
        this.WhereSql = new StringBuilder();
        try
        {
            foreach (object component in componentList)
            {
                try
                {
                    this.CreateDeleteSql(component);
                    sqliteDataBase.ExecuteNonQueryEx(this.ExecutetSql.ToString());
                    this.Init(component.GetType());
                }
                catch (SqliteException ex)
                {
                    Debug.LogError(ex.Message);
                    break;
                }
            }
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
            // ロールバック
            sqliteDataBase.TransactionRollBack();
        }
        finally
        {
            this.Init(componentList[0].GetType());
        }
        // コミット
        sqliteDataBase.TransactionCommit();
    }

    /// <summary>
    /// デリートSQL文生成
    /// </summary>
    /// <param name="component"></param>
    private void CreateDeleteSql(object component)
    {
        this.ExecutetSql = new StringBuilder();
        string tableName = this.GetDataAccessAttribute(component.GetType()).TableName;
        string sql = string.Format(DELETE_SQL, tableName);
        this.ExecutetSql.AppendLine(sql);

        this.GetPrimaryKeyValue(component);
        this.CreateDeleteUpdateWhere(tableName, component);
        Debug.Log(this.ExecutetSql.ToString());
    }

    #endregion

    #region 採番

    /// <summary>
    /// 対象列を採番
    /// </summary>
    /// <returns></returns>
    public string GetAssignNumber(string column)
    {
        string countColumn = "Count";
        string countSql = "select max(cast({0} as interger)) as " + countColumn + " from {1}";
        this.ExecutetSql.Append(string.Format(countSql, column, this.GetDataAccessAttribute(this.BaseType).TableName));
        SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
        this.SelectTable = sqliteDataBase.ExecuteQuery(this.ExecutetSql.ToString());
        PropertyInfo propertyInfo = this.BaseType.GetProperty(column);
        if (propertyInfo == null)
        {
            Debug.Log("not found assign column");
            return string.Empty;
        }
        DataPropertyAttribute attribute = this.GetDataPropertyAttribute(propertyInfo);

        if (this.SelectTable.Rows.Count.Equals(0))
        {
            return this.GetAssignNumberFormat(attribute.MaxLength, "1");
        }
        else
        {
            int no = int.Parse(this.SelectTable.Rows[0][countColumn].ToString()) + 1;
            return this.GetAssignNumberFormat(attribute.MaxLength, no.ToString());
        }
    }

    /// <summary>
    /// 採番番号を形成
    /// </summary>
    /// <param name="maxLength"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    private string GetAssignNumberFormat(int maxLength, string number)
    {
        while (true)
        {
            if (!number.Length.Equals(maxLength))
            {
                number = "0" + number;
            }
            else
            {
                break;
            }
        }

        return number;
    }

    #endregion

    #region 条件生成

    /// <summary>
    /// アップデート、デリート文の条件生成を共通化。
    /// </summary>
    /// <param name="tableName"></param>
    private void CreateDeleteUpdateWhere(string tableName, object componet)
    {
        if (!this.WhereSql.ToString().Equals(string.Empty))
        {
            if (!this.JoinSql.ToString().Equals(string.Empty))
            {
                this.ExecutetSql.AppendLine(string.Format("from {0}", tableName));
                this.ExecutetSql.AppendLine(this.JoinSql.ToString());
            }
            this.ExecutetSql.AppendLine(this.WhereSql.ToString());
        }
        else
        {
            // プライマリキーより条件文作成
            CreatePrimaryWhere(componet.GetType());
        }
    }

    /// <summary>
    /// プライマリキーより条件文作成
    /// </summary>
    /// <param name="componet"></param>
    private void CreatePrimaryWhere(Type type)
    {
        foreach (string key in PrimarySql.Keys)
        {
            this.AddCondition(key, PrimarySql[key], type, true);
        }
        this.ExecutetSql.AppendLine(this.WhereSql.ToString());
    }

    #endregion

    /// <summary>
    /// 登録、更新用SQL中身生成。プライマリキー回収。
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    private string GetInsertUpdateSql(object component, bool isInsert)
    {
        StringBuilder stringBuilder = new StringBuilder();
        StringBuilder propertyBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        string insertSql = " ({0}) values ({1})";
        string updateSql = "{0} = '{1}'";
        bool firstProperty = true;
        Type type = component.GetType();
        PropertyInfo[] propertyInfoArray = type.GetProperties();
        foreach (PropertyInfo propertyInfo in propertyInfoArray)
        {
            if (propertyInfo.DeclaringType.Equals(type))
            {
                DataPropertyAttribute attribute = this.GetDataPropertyAttribute(propertyInfo);
                if (attribute != null)
                {
                    object value = this.GetSqliteTypeValue(attribute, propertyInfo, component);

                    // プライマリキー取得
                    if (attribute.IsPrimaryKey && !isInsert)
                    {
                        this.AddPrimaryKey(value, propertyInfo.Name);
                    }

                    if (value == null && !isInsert) continue;

                    if (firstProperty)
                    {
                        firstProperty = false;
                    }
                    else
                    {
                        stringBuilder.Append(",");
                        propertyBuilder.Append(",");
                    }

                    // 登録
                    if (isInsert)
                    {
                        stringBuilder.AppendLine(string.Format("'{0}'", (value == null) ? "" : value.ToString()));
                        propertyBuilder.AppendLine(propertyInfo.Name);
                    }
                    // 更新
                    else
                    {
                        stringBuilder.AppendLine(string.Format(updateSql, propertyInfo.Name, value.ToString()));
                    }
                }
            }
        }

        if (isInsert)
        {
            propertyBuilder.AppendLine(",CreateDate");
            propertyBuilder.AppendLine(",UpdateDate");
            stringBuilder.AppendLine(string.Format(",'{0}'", this.GetDateFormat(DateTime.Now)));
            stringBuilder.AppendLine(string.Format(",'{0}'", this.GetDateFormat(DateTime.Now)));
            return string.Format(insertSql, propertyBuilder.ToString(), stringBuilder.ToString());
        }
        else
        {
            stringBuilder.AppendLine(string.Format(",UpdateDate = '{0}'", this.GetDateFormat(DateTime.Now)));
            return stringBuilder.ToString();
        }
    }

    private object GetSqliteTypeValue(DataPropertyAttribute attribute, PropertyInfo propertyInfo, object component)
    {
        object value = null;
        switch (attribute.SqliteType)
        {
            case DataPropertyAttribute.SQLITE_TYPE.TEXT:
            case DataPropertyAttribute.SQLITE_TYPE.NUMERIC:
                value = propertyInfo.GetValue(component, null);
                break;
            case DataPropertyAttribute.SQLITE_TYPE.BOOLEAN:
                bool flg = (bool)propertyInfo.GetValue(component, null);
                value = (flg) ? "1" : "0";
                break;
            case DataPropertyAttribute.SQLITE_TYPE.ARRAY:
                if (propertyInfo.PropertyType.Equals(typeof(List<string>)))
                {
                    List<string> listArray = (List<string>)propertyInfo.GetValue(component, null);
                    if (listArray == null)
                    {
                        value = null;
                    }
                    else
                    {
                        value = string.Join(",", listArray.ToArray());
                    }
                }
                break;
            case DataPropertyAttribute.SQLITE_TYPE.DATETIME:
                if (propertyInfo.PropertyType.Equals(typeof(DateTime)))
                {
                    value = this.GetDateFormat((DateTime)propertyInfo.GetValue(component, null));
                }
                break;
            default:
                break;
        }

        if (value != null)
        {
            // 'をエスケープする
            if (value.ToString().Contains("'"))
            {
                value = value.ToString().Replace("'", "''");
            }
        }

        return value;
    }

    /// <summary>
    /// テーブル操作SQL発行。
    /// MonoBehaviour
    /// </summary>
    public void SqlExecuteNonQuery(string sql)
    {
        try
        {
            SqliteDatabase sqliteDataBase = new SqliteDatabase(CONNECT_TABLE);
            sqliteDataBase.ExecuteNonQuery(sql);
        }
        catch (SqliteException ex)
        {
            Debug.Log(ex.Message + " " + this.ExecutetSql.ToString());
        }
    }

    #region プライマリーキー、値、SQLite列回収

    /// <summary>
    /// プライマリキーと値を回収
    /// </summary>
    private void GetPrimaryKeyValue(object component)
    {
        Type type = component.GetType();
        PropertyInfo[] propertyInfoArray = type.GetProperties();
        foreach (PropertyInfo propertyInfo in propertyInfoArray)
        {
            if (propertyInfo.DeclaringType.Equals(type))
            {
                DataPropertyAttribute attribute = this.GetDataPropertyAttribute(propertyInfo);
                if (attribute != null)
                {
                    if (attribute.IsPrimaryKey)
                    {
                        this.AddPrimaryKey(propertyInfo.GetValue(component, null), propertyInfo.Name);
                    }
                }
            }
        }
    }

    /// <summary>
    /// プライマリキーを追加
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    private void AddPrimaryKey(object obj, string properyName)
    {
        if (obj != null)
        {
            if (!this.PrimarySql.ContainsKey(properyName))
            {
                this.PrimarySql.Add(properyName, obj.ToString());
            }
        }
        else
        {
            if (!this.PrimarySql.ContainsKey(properyName))
            {
                this.PrimarySql.Add(properyName, string.Empty);
            }
        }

    }

    /// <summary>
    /// SQLiteに含まれる列を取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetSqliteColumn(Type type)
    {
        StringBuilder stringBuilder = new StringBuilder();
        bool first = true;
        PropertyInfo[] propertyInfoArray = type.GetProperties();
        string tableName = this.GetDataAccessAttribute(type).TableName;
        string format = "{0}.{1}";

        foreach (PropertyInfo propertyInfo in propertyInfoArray)
        {
            DataPropertyAttribute attribute = this.GetDataPropertyAttribute(propertyInfo);

            if (attribute != null)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.AppendLine(string.Format(format, tableName, propertyInfo.Name));
            }
        }

        return stringBuilder.ToString();
    }

    #endregion

    /// <summary>
    /// 登録更新日付のファーマット
    /// </summary>
    /// <returns></returns>
    private string GetDateFormat(DateTime dateTime)
    {
        return dateTime.ToString("yyyy/MM/dd hh:mm");
    }

    #region 属性取得

    /// <summary>
    /// 接続属性取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private DataAccessAttribute GetDataAccessAttribute(Type type)
    {
        return (DataAccessAttribute)Attribute.GetCustomAttribute(type, typeof(DataAccessAttribute));
    }

    /// <summary>
    /// Sqliteデータ属性取得
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    private DataPropertyAttribute GetDataPropertyAttribute(PropertyInfo propertyInfo)
    {
        return (DataPropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(DataPropertyAttribute));
    }

    #endregion
}
