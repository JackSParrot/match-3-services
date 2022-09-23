using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor
{
    public class EditorUtilities
    {
        [Serializable]
        private class Language
        {
            public List<LanguajeDictionary.LocalizationEntry> data;
        }

        [Serializable]
        private class Languajes
        {
            public Language English;
            public Language Spanish;
        }

        private static string kLocalizationUrl =
            "https://script.google.com/macros/s/AKfycbx1Zpf-3FQQvfScSDPDePg8Nywm8XAmAmvAZUD9lgPacXdi9quoffFXL3ZeBZBBx09ILg/exec";

        [MenuItem("Tools/Localization/Update")]
        public static void UpdateLocalization()
        {
            Debug.Log("Updating localization");
            UnityWebRequest request = new UnityWebRequest(kLocalizationUrl, "GET", new DownloadHandlerBuffer(), null);
            request.SendWebRequest().completed += operation =>
            {
                Debug.Log("Response received");
                if (request.error != null)
                {
                    Debug.Log("Response received With error " + request.error);
                    return;
                }

                Debug.Log(request.downloadHandler.text);

                var languajes = JsonUtility.FromJson<Languajes>(request.downloadHandler.text);
                var english = languajes.English;
                var spanish = languajes.Spanish;
                System.IO.File.WriteAllText(Application.dataPath + "/Resources/English_file.json",
                    JsonUtility.ToJson(english));
                System.IO.File.WriteAllText(Application.dataPath + "/Resources/Spanish_file.json",
                    JsonUtility.ToJson(spanish));
                AssetDatabase.Refresh();
            };
        }
    }
}