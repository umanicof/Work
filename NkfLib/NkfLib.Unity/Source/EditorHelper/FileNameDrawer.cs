/// 自動実装プロパティに [field : SerializeField] を付けるだけでインスペクタに表示する仕組み
/// https://spi8823.hatenablog.com/entry/2020/03/14/005001
using UnityEditor;
using UnityEngine;

namespace NkfLib.Unity
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerializeField))]
    public class FieldNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.isArray)  //arrayでダメな理由は知らん
            {
                label.text = property.displayName;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }
    }
#endif
}