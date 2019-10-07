using UnityEngine;

namespace SocialConnector
{
    public class URLShare : MonoBehaviour
    {
        public string URL = "hoge";
        public void ShareURL()
        {
            SocialConnector.Share(URL, null, null);
        }
    }
}