#if UNITY_EDITOR

using UnityEditor;

using UnityEngine;

namespace DefaultNamespace
{
[CustomEditor(typeof(TestSomething))]
public class TestSomethingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestSomething testSomething = (TestSomething)this.target;
        if (GUILayout.Button("切换装备")) testSomething.ChangeEquipment();
        if (GUILayout.Button("设置 倍速")) testSomething.SetTimeScale();
        if (GUILayout.Button("播放动画")) testSomething.PlayAnime();
        // Test
         if( GUILayout.Button("Test")) testSomething.Test();
    }
}
}
#endif