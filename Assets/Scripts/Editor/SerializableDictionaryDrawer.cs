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

        // Expand the property if it's not collapsed
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

        // Draw key-value pairs
        for (int i = 0; i < dictSize; i++)
        {
            SerializedProperty keyProp = keysProperty.GetArrayElementAtIndex(i);
            SerializedProperty valueProp = valuesProperty.GetArrayElementAtIndex(i);

            Rect keyRect = new Rect(fieldRect.x, fieldRect.y + (i * lineHeight), halfWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(fieldRect.x + halfWidth + 5, fieldRect.y + (i * lineHeight), halfWidth, EditorGUIUtility.singleLineHeight);
            Rect removeRect = new Rect(fieldRect.x + position.width - 25, fieldRect.y + (i * lineHeight), 20, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);

            // Remove entry button
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

        // Add Entry Button
        Rect buttonRect = new Rect(fieldRect.x, fieldRect.y + (dictSize * lineHeight), position.width, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(buttonRect, "Add Entry"))
        {
            // Ensure we properly expand the dictionary and update properties
            keysProperty.arraySize++;
            valuesProperty.arraySize++;

            // Apply property modifications before updating
            property.serializedObject.ApplyModifiedProperties();

            // Re-fetch keys and values after increasing array size
            keysProperty = property.FindPropertyRelative("keys");
            valuesProperty = property.FindPropertyRelative("values");

            // Ensure that the new entry is initialized
            int newIndex = keysProperty.arraySize - 1;
            SerializedProperty newKeyProp = keysProperty.GetArrayElementAtIndex(newIndex);
            SerializedProperty newValueProp = valuesProperty.GetArrayElementAtIndex(newIndex);

            if (newKeyProp == null || newValueProp == null)
            {
                Debug.LogError("Failed to retrieve new array elements! SerializedProperty may not have updated correctly.");
                return;
            }

            newKeyProp.stringValue = "NewKey" + newIndex; // Default key
            newValueProp.intValue = 0;                    // Default value

            // Re-apply and update
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            SceneView.RepaintAll();  // Refresh the inspector to reflect the changes

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
}