#if UNITY_EDITOR
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class SerializeVisualizer : Editor
{
    public override void OnInspectorGUI()
    {
        // オブジェクトに更新をかける
        serializedObject.Update();
        // ターゲットとなるオブジェクトをMonobehaviourとして取得
        var targetObject = target as MonoBehaviour;
        // ターゲットとなるオブジェクトの型情報を取得
        var type = targetObject.GetType();

        // m_Scriptを先に表示(アタッチしているスクリプト自身)
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(targetObject), typeof(MonoBehaviour), false);
        }

        // クラスに定義されているフィールドを順番に描画
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // 変数がReadOnlyAttributeを継承しているかを確認
            var attr = field.GetCustomAttribute<ReadOnlySerializeFieldAttribute>();
            // 変数のメモがあればそれをもらう
            var note = attr?.Note;
            var noteColor = attr?.NoteColor;
            // キャストできたかを調べる
            var isReadOnly = attr != null;
            // publicかSerializeField属性がついているかを調べる
            var isSerialized = field.IsPublic || field.GetCustomAttribute<SerializeField>() != null;

            // 描画対象(SerializeField or ReadOnlySerializeField)なら
            if (isSerialized || isReadOnly)
            {
                var prop = serializedObject.FindProperty(field.Name);
                // Unity標準の描画が用意されていればそれを使う
                if (prop != null)
                {
                    // ただし、ReadOnlyなら編集を不可能にする
                    DrawFieldWithNote(prop, note, noteColor, isReadOnly);
                }
                // privateなど、Unity標準で用意されていない場合
                else if (isReadOnly)
                {
                    // まずは値を取得
                    object value = field.GetValue(targetObject);
                    // 編集を不可能にしたまま表示
                    using (new EditorGUI.DisabledScope(true))
                    {
                        GUI.enabled = true;
                        DrawLabelWithNote(field.Name, value, note, noteColor);
                        GUI.enabled = true;
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// SerializedProperty用のフィールド＋Note描画
    /// </summary>
    private void DrawFieldWithNote(SerializedProperty prop, string note, Color? noteColor, bool isReadOnly)
    {
        EditorGUILayout.BeginHorizontal();

        if (isReadOnly)
            BrightReadOnlyDrawer.Draw(prop);
        else
            EditorGUILayout.PropertyField(prop, true);

        if (!string.IsNullOrEmpty(note))
        {
            var prevColor = GUI.color;
            GUI.color = noteColor ?? Color.white;
            EditorGUILayout.LabelField(note, EditorStyles.boldLabel, GUILayout.MaxWidth(75));
            GUI.color = prevColor;
        }

        EditorGUILayout.EndHorizontal();
    }


    /// <summary>
    /// 非SerializedField用のLabel＋Note描画
    /// </summary>
    private void DrawLabelWithNote(string fieldName, object value, string note, Color? noteColor)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(fieldName, value != null ? value.ToString() : "null");
        if (!string.IsNullOrEmpty(note))
        {
            var prevColor = GUI.color;
            GUI.color = noteColor ?? Color.white;
            EditorGUILayout.LabelField(note, EditorStyles.boldLabel, GUILayout.MaxWidth(75));
            GUI.color = prevColor;
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// ReadOnlyな変数を明るいまま描画するためのクラス
    /// </summary>
    public static class BrightReadOnlyDrawer
    {
        /// <summary>
        /// 通常の描画だと暗くなってしまうので、明るく描画するためのメソッド
        /// </summary>
        public static void Draw(SerializedProperty prop)
        {
            string displayName = prop.displayName;
            string valueStr = GetValueAsString(prop);
            EditorGUILayout.LabelField(displayName, valueStr);
        }

        /// <summary>
        /// SerializePropertyのバリューを文字列に直して返すメソッド
        /// </summary>
        private static string GetValueAsString(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue.ToString();
                case SerializedPropertyType.Float:
                    return prop.floatValue.ToString("0.###");
                case SerializedPropertyType.Boolean:
                    return prop.boolValue ? "True" : "False";
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value.ToString();
                case SerializedPropertyType.Color:
                    return prop.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue ? prop.objectReferenceValue.name : "None";
                case SerializedPropertyType.Enum:
                    return prop.enumDisplayNames[prop.enumValueIndex];
                case SerializedPropertyType.Rect:
                    return prop.rectValue.ToString();
                case SerializedPropertyType.Bounds:
                    return prop.boundsValue.ToString();
                case SerializedPropertyType.Quaternion:
                    Quaternion q = prop.quaternionValue;
                    return $"({q.x:F2}, {q.y:F2}, {q.z:F2}, {q.w:F2})";
                case SerializedPropertyType.AnimationCurve:
                    return $"Curve ({prop.animationCurveValue.length} keys)";
                default:
                    return "(Unsupported Type)";
            }
        }

        private static Vector4 QuaternionToVector4(Quaternion q)
            => new Vector4(q.x, q.y, q.z, q.w);
    }

}
#endif
