using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Sqlite Example
/// </summary>
public class SqliteTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // selelct all CharacterData
        // キャラクターを全て取得
        List<CharacterData> characterDataList = SqliteTableService.GetCharacterDataList();

        // select party CharacterData
        // パーティ(PartyId = 00001)に参加しているキャラクターを取得
        List<CharacterData> partyCharacterList = SqliteTableService.GetPartyCharacterList("00001");

        // Get the AI category of master table
        // マスターテーブルのAIカテゴリを取得
        List<MasterData> masterDataList = SqliteTableService.GetMasterDataList("AiCategory");

        // insert characterTable
        // キャラクターの登録
        CharacterData characterData = new CharacterData();
        characterData.Name = "InsertCharacter";
        characterData.Level = 1;
        characterData.Hp = 5000;
        characterData.Attack = 100;
        characterData.Deffence = 100;
        characterData.Speed = 55.5f;
        SqliteTableService.InsertCharacterData(characterData);

        // update characterTable
        // キャラクターの更新
        CharacterData updateCharacterData = characterDataList[0];
        updateCharacterData.Level++;
        updateCharacterData.Hp += 100;
        updateCharacterData.Attack += 5;
        SqliteTableService.UpdateCharacterData(updateCharacterData);

        // delete characterTable
        // キャラクターの削除
        CharacterData deleteCharacterData = characterDataList[1];
        SqliteTableService.DeleteCharacterData(deleteCharacterData);

        // Delete Master Data . Ensure that the data is not deleted
        // マスターデータを削除。削除されないこを確認。
        MasterData masterData = masterDataList[0];
        SqliteTableService.DeleteMasterData(masterData);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
