using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Example
/// Table operation class of sqlite
/// Sqliteテーブルデータ操作クラス
/// </summary>
public class SqliteTableService
{
    /// <summary>
    /// Select Example
    /// キャラクターデータを取得
    /// </summary>
    /// <returns></returns>
    public static List<CharacterData> GetCharacterDataList()
    {
        // SelectExample1
        // 1.Initialized with the table you want to select
        // 1. 取得したいテーブルで初期化
        DataAccess.Instance.Init(typeof(CharacterData));
        // 2.Run the select statement that is automatically generated in accordance with the table. It gets the data .
        // 2.テーブルに合わせて自動生成されたselect文を実行し、データを取得します。
        List<CharacterData> characterDataList = DataAccess.Instance.GetDataList<CharacterData>();

        // SelectExample2
        // 1.Initialized with the table you want to select
        // 1. 取得したいテーブルで初期化
        DataAccess.Instance.Init(typeof(CharacterData));
        DataTable dataTable = DataAccess.Instance.GetDataTable();
        List<CharacterData> characterDataList2 = DataBinding<CharacterData>.DataTableToObjectList(dataTable);

        // SelectExample3
        // Run the SQL statement that was created .
        // 作成されたSQL文を実行します。
        List<CharacterData> characterDataList3 = DataAccess.Instance.GetDataList<CharacterData>("select * from CharacterTable");

        // SelectExample4
        // Run the SQL statement that was created .
        // 作成されたSQL文を実行します。
        DataTable dataTable2 = DataAccess.Instance.GetDataTable("select CharacterId,Name from CharacterTable");
        List<CharacterData> characterDataList4 = DataBinding<CharacterData>.DataTableToObjectList(dataTable2);

        return characterDataList;
    }

    /// <summary>
    /// Get the character data that brute string to PartyId
    /// パーティーテーブルに紐づくキャラクターデータを取得
    /// </summary>
    /// <param name="partyId"></param>
    /// <returns></returns>
    public static List<CharacterData> GetPartyCharacterList(string partyId)
    {
        DataAccess dataAccess = DataAccess.Instance;
        // 最初にセレクトしたいテーブルで初期化
        dataAccess.Init(typeof(PartyTable));
        
        // It performs an internal table joins in PartyId
        // PartyIdでテーブルの内部結合を行う
        dataAccess.AddJoinTable(typeof(PartyTable), typeof(PartyCharacterTable), "PartyId");
        
        // It performs an internal table joins in CharacterId
        // CharacterIdでテーブルの内部結合を行う
        dataAccess.AddJoinTable(typeof(PartyCharacterTable), typeof(CharacterData), "CharacterId", "CharacterId");

        // Set condition
        // 条件を指定する
        dataAccess.AddCondition("PartyId", partyId, typeof(PartyTable), true);
        
        // Get the bound table
        // 結合したテーブルを取得
        DataTable dataTable = dataAccess.GetDataTable();

        // Get the PartyTable from the combined table
        // 結合されたテーブルからPartyTableを取得
        List<PartyTable> partyTableList = DataBinding<PartyTable>.DataTableToObjectList(dataTable);

        // Get the PartyTable from the combined table
        // 結合されたテーブルからCharacterDataを取得
        List<CharacterData> characterDataList = DataBinding<CharacterData>.DataTableToObjectList(dataTable);

        return characterDataList;
    }

    /// <summary>
    /// Get to sort the master data
    /// マスターデータをソートして取得
    /// </summary>
    /// <param name="masterCategory"></param>
    /// <returns></returns>
    public static List<MasterData> GetMasterDataList(string masterCategory)
    {
        DataAccess dataAccess = DataAccess.Instance;
        // Init
        dataAccess.Init(typeof(MasterData));
        dataAccess.AddCondition("MasterCategory", masterCategory, typeof(MasterData), true);
        
        // Set the sort item
        // ソート項目を設定
        dataAccess.AddOrderByColumns("Seq", typeof(MasterData), true);
        dataAccess.AddOrderByColumns("MasterCategory", typeof(MasterData), false);

        return dataAccess.GetDataList<MasterData>();
    }

    /// <summary>
    /// Insert Example
    /// キャラクターデータを登録
    /// </summary>
    /// <param name="characterData"></param>
    public static void InsertCharacterData(CharacterData characterData)
    {
        DataAccess.Instance.Init(typeof(CharacterData));
        // Create Numbering CharacterId
        // CharacterIdを採番
        characterData.CharacterId = DataAccess.Instance.GetAssignNumber("CharacterId");

        // Insert SQL will be issued on the basis of the argument
        // 引数に基づきインサートSQL発行
        DataAccess.Instance.Insert(characterData);
    }

    /// <summary>
    /// Update Example
    /// キャラクターデータを更新
    /// </summary>
    /// <param name="characterData"></param>
    public static void UpdateCharacterData(CharacterData characterData)
    {
        // Init
        DataAccess.Instance.Init(typeof(CharacterData));
        // Update SQL is issued on the basis of the argument . (NULL values ​​are not updated .)
        // 引数に基づきアップデートSQL発行。NULL値は更新されません。
        DataAccess.Instance.UpdateSql(characterData);
    }

    /// <summary>
    /// Delete Example
    /// キャラクターデータを削除
    /// </summary>
    /// <param name="characterData"></param>
    public static void DeleteCharacterData(CharacterData characterData)
    {
        // Init
        DataAccess.Instance.Init(typeof(CharacterData));
        // Issue the Delete SQL based on the arguments .
        // 引数に基づきデリートSQL発行
        DataAccess.Instance.Delete(characterData);
    }

    /// <summary>
    /// Delete Master Data . Ensure that the data is not deleted
    /// マスターデータを削除。削除されないこを確認。
    /// </summary>
    /// <param name="masterData"></param>
    public static void DeleteMasterData(MasterData masterData)
    {
        DataAccess.Instance.Init(typeof(MasterData));
        // Please check the log that was not deleted .
        // 削除されないことをログで確認してください。
        DataAccess.Instance.Delete(masterData);
    }
}
