using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using JsonFormats;
using VrScene;

namespace MergeScene
{
    public class MapManager : MonoBehaviour
    {
        public static string[] WorldList;
        CommunicationManager CommunicationManager;
        List<MapParent> mapParent;

        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
        }

        private void Start()
        {
            StartCoroutine("FetchMap");
        }

        IEnumerator FetchMap()
        {
            for(int i = 0; i < WorldList.Length; i++)
            {
                //GameObject parent = new GameObject("Map" + i.ToString());
                var fetchBlocksTask = CommunicationManager.fetchMapBlocksAsync(WorldList[i]);
                var fetchColorRulesTask = CommunicationManager.fetchColorsAsync(WorldList[i]);
                yield return new WaitUntil(() => fetchBlocksTask.IsCompleted); // 通信中の場合次のフレームに処理を引き継ぐ
                yield return new WaitUntil(() => fetchColorRulesTask.IsCompleted);

                Object mapParentPrefab = (GameObject)Resources.Load("MapParent");
                MapParent map = (Instantiate(mapParentPrefab, new Vector3(0f,10f, 0f), Quaternion.identity) as GameObject).GetComponent<MapParent>();
                map.AddBlock(fetchBlocksTask.Result);
                map.ApplyColorRules(fetchColorRulesTask.Result);
                map.Move(48*i, 0);
            }
        }


        
    }
}
