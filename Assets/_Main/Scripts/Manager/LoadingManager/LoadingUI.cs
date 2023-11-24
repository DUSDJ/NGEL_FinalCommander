using UnityEditor;
using UnityEngine;

namespace Assets._Main.Scripts.Manager.LoadingManager
{
    public class LoadingUI : ScriptableObject
    {
        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
        }
    }
}