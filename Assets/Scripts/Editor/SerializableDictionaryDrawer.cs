using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // EditorGUI.LabelField(postion, label.text, "Serializble Dictionary is not implemented in UI yet");
        
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        SerializedProperty valuesProperty = property.FindPropertyRelative("values");
        if (keysProperty == null || valuesProperty == null)
        {
            EditorGUI.LabelField(position, label.text, "Keys and Values are required.");
            return;
        }
        
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, 
            EditorGUIUtility.singleLineHeight), property.isExpanded, label);
        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;
        float lineHeight = EditorGUIUtility.singleLineHeight + 2;
        Rect fieldRect = new Rect(position.x, position.y + lineHeight, position.width, EditorGUIUtility.singleLineHeight);

        for (int i = 0; i < keysProperty.arraySize; i++)
        {
            SerializedProperty keyProp = keysProperty.GetArrayElementAtIndex(i);
            SerializedProperty valueProp = valuesProperty.GetArrayElementAtIndex(i);

            float halfWidth = (position.width - 40) / 2;
            Rect keyRect = new Rect(fieldRect.x, fieldRect.y + (i * lineHeight), halfWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(fieldRect.x + halfWidth + 5, fieldRect.y + (i * lineHeight), halfWidth, EditorGUIUtility.singleLineHeight);
            Rect removeRect = new Rect(fieldRect.x + position.width - 20, fieldRect.y + (i * lineHeight), 20, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
            
            //Remove button
            if (GUI.Button(removeRect, "X"))
            {
                keysProperty.DeleteArrayElementAtIndex(i);
                valuesProperty.DeleteArrayElementAtIndex(i);
            }
        }
        
        //Add Button
        Rect buttonRect = new Rect(fieldRect.x, fieldRect.y + (keysProperty.arraySize * lineHeight), position.width, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(buttonRect, "Add Entry"))
        {
            keysProperty.arraySize++;
            valuesProperty.arraySize++;
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        return EditorGUIUtility.singleLineHeight * (keysProperty.arraySize + 2);
    }
}
