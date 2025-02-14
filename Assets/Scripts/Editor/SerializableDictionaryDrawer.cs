using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        SerializedProperty valuesProperty = property.FindPropertyRelative("values");

        if (keysProperty == null || valuesProperty == null)
        {
            EditorGUI.LabelField(position, label.text, "Error: Keys and Values are missing.");
            return;
        }

        // expanding the property if it's not collapsed
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded, label, true);

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;
        float lineHeight = EditorGUIUtility.singleLineHeight + 2;
        Rect fieldRect = new Rect(position.x, position.y + lineHeight, position.width, EditorGUIUtility.singleLineHeight);

        int dictSize = keysProperty.arraySize;
        float halfWidth = (position.width - 50) / 2;

        // drawing the key-value pairs
        for (int i = 0; i < dictSize; i++)
        {
            SerializedProperty keyProp = keysProperty.GetArrayElementAtIndex(i);
            SerializedProperty valueProp = valuesProperty.GetArrayElementAtIndex(i);

            Rect keyRect = new Rect(fieldRect.x, fieldRect.y + (i * lineHeight), halfWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(fieldRect.x + halfWidth + 5, fieldRect.y + (i * lineHeight), halfWidth, EditorGUIUtility.singleLineHeight);
            Rect removeRect = new Rect(fieldRect.x + position.width - 25, fieldRect.y + (i * lineHeight), 20, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);

            // Remove entry button functionality
            if (GUI.Button(removeRect, "X"))
            {
                keysProperty.DeleteArrayElementAtIndex(i);
                valuesProperty.DeleteArrayElementAtIndex(i);
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                GUI.FocusControl(null);
                return;
            }
        }

        // Add Entry Button fucntionality
        Rect buttonRect = new Rect(fieldRect.x, fieldRect.y + (dictSize * lineHeight), position.width, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(buttonRect, "Add Entry"))
        {
            // Ensure we properly expand the dictionary and update properties
            keysProperty.arraySize++;
            valuesProperty.arraySize++;

            // applying property modifications before updating because unity stoopid
            property.serializedObject.ApplyModifiedProperties();

            // refetching keys and values right after increasing array size 
            keysProperty = property.FindPropertyRelative("keys");
            valuesProperty = property.FindPropertyRelative("values");

            // ensuring new entry is initialized
            int newIndex = keysProperty.arraySize - 1;
            SerializedProperty newKeyProp = keysProperty.GetArrayElementAtIndex(newIndex);
            SerializedProperty newValueProp = valuesProperty.GetArrayElementAtIndex(newIndex);

            if (newKeyProp == null || newValueProp == null)
            {
                Debug.LogError("Failed to retrieve new array elements! SerializedProperty may not have updated correctly.");
                return;
            }

            newKeyProp.stringValue = "NewKey" + newIndex;
            newValueProp.intValue = 0;

            //reapplying and updating
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            SceneView.RepaintAll();  // refreshing inspector to properly apply changes...ahh unity ._.

            GUI.FocusControl(null);
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
    
    //conclusion of my first proper official tool I guess
}