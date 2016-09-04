using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SqliteTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // セレクト方法その1
        // 1.セレクトしたいテーブルで初期化
        DataAccess.Instance.Init(typeof(CharacterData));
        // 2.自動生成されるセレクト文を実行しデータを取得
        List<CharacterData> characterDataList = DataAccess.Instance.GetDataList<CharacterData>(true);

        // セレクト方法その2
        DataAccess.Instance.Init(typeof(CharacterData));
        DataTable dataTable = DataAccess.Instance.GetDataTable(true);
        List<CharacterData> characterDataList2 = DataBinding<CharacterData>.DataTableToObjectList(dataTable);

        // セレクト方法その3
        List<CharacterData> characterDataList3 = DataAccess.Instance.GetDataList<CharacterData>("select * from CharacterTable");

        // セレクト方法その4
        // 
        DataTable dataTable2 = DataAccess.Instance.GetDataTable("select * from CharacterTable");
        List<CharacterData> characterDataList4 = DataBinding<CharacterData>.DataTableToObjectList(dataTable2);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
