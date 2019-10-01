using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using JsonFormats;
using VrScene;
using UnityEngine.SceneManagement;

namespace MergeScene
{
    public class MapManager : MonoBehaviour
    {
        public static string[] WorldList;
        public InputField MapNameInputField;
        CommunicationManager CommunicationManager;
        List<MapParent> MapParents = new List<MapParent> { };

        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
        }

        private void Start()
        {
            StartCoroutine("FetchMap");
        }

        public void OnClickSubmitButton()
        {
            StartCoroutine("UploadMergeData");
        }
  
        IEnumerator  UploadMergeData()
        {
            MergeData data = new MergeData();
            List<MergeMap> maps = new List<MergeMap> { };
            this.MapParents.ForEach(map =>
            {
                MergeMap d = new MergeMap();
                d.x = (int)map.currentDiff.x;
                d.y = (int)map.currentDiff.y;
                d.map_id = map.MapId;
                d.rotate = map.currentRotate;
                maps.Add(d);
            });
            data.merge_maps = maps;
            data.name = MapNameInputField.text;
            var uploadMergeTask = CommunicationManager.uploadMergeAsync(data);
            yield return new WaitUntil(() => uploadMergeTask.IsCompleted);
            SceneManager.LoadScene("Title");
        }

        IEnumerator FetchMap()
        {
            for(int i = 0; i < WorldList.Length; i++)
            {
                var fetchBlocksTask = CommunicationManager.fetchMapBlocksAsync(WorldList[i]);
                var fetchColorRulesTask = CommunicationManager.fetchColorsAsync(WorldList[i]);
                yield return new WaitUntil(() => fetchBlocksTask.IsCompleted); // 通信中の場合次のフレームに処理を引き継ぐ
                yield return new WaitUntil(() => fetchColorRulesTask.IsCompleted);

                Object mapParentPrefab = (GameObject)Resources.Load("MapParent");
                MapParent map = (Instantiate(mapParentPrefab, new Vector3(0f,10f, 0f), Quaternion.identity) as GameObject).GetComponent<MapParent>();
                map.MapId = WorldList[i];
                map.AddBlock(fetchBlocksTask.Result);
                map.ApplyColorRules(fetchColorRulesTask.Result);
                map.Move(48*i, 0);
                this.MapParents.Add(map);
            }
        }
    }
}
