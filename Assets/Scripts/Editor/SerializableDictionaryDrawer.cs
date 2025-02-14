using UnityEngine;
using UnityEditor;
using System;

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

        // Foldout to toggle dictionary visibility
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

        // Add Entry Button functionality
        Rect buttonRect = new Rect(fieldRect.x, fieldRect.y + (dictSize * lineHeight), position.width, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(buttonRect, "Add Entry"))
        {
            // Ensure we properly expand the dictionary and update properties
            keysProperty.arraySize++;
            valuesProperty.arraySize++;

            // Apply property modifications before updating because Unity can be a little slow with updates
            property.serializedObject.ApplyModifiedProperties();

            // Refetch the updated properties after increasing the array size
            keysProperty = property.FindPropertyRelative("keys");
            valuesProperty = property.FindPropertyRelative("values");

            int newIndex = keysProperty.arraySize - 1;
            SerializedProperty newKeyProp = keysProperty.GetArrayElementAtIndex(newIndex);
            SerializedProperty newValueProp = valuesProperty.GetArrayElementAtIndex(newIndex);

            if (newKeyProp == null || newValueProp == null)
            {
                Debug.LogError("Failed to retrieve new array elements! SerializedProperty may not have updated correctly.");
                return;
            }

            // Set default values based on the type of the key and value
            SetDefaultValues(newKeyProp, newValueProp, newIndex);

            // Reapply changes and refresh the inspector
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            SceneView.RepaintAll(); // Refreshing the inspector to properly display the new entry

            GUI.FocusControl(null);
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    private void SetDefaultValues(SerializedProperty keyProp, SerializedProperty valueProp, int index)
    {
        // Geting the type of key and value from the dictionary
        Type keyType = keyProp.propertyType == SerializedPropertyType.String ? typeof(string) : typeof(object);
        Type valueType = valueProp.propertyType == SerializedPropertyType.Integer ? typeof(int) : typeof(object);

        if (keyType == typeof(string))
        {
            keyProp.stringValue = "NewKey" + index;  // Default key name
        }
        else if (keyType == typeof(int))
        {
            keyProp.intValue = index;  // Default key value
        }

        // Seting default based on value type
        if (valueType == typeof(int))
        {
            valueProp.intValue = 0;  // Default int value
        }
        else if (valueType == typeof(float))
        {
            valueProp.floatValue = 0.0f;  // Default float value
        }
        else if (valueType == typeof(string))
        {
            valueProp.stringValue = "DefaultValue";  // Default string value
        }
        else if (valueType == typeof(bool))
        {
            valueProp.boolValue = false;  // Default bool value
        }
        else
        {
            // If it's a custom type, maybe I add something to handle that here
            // valueProp.objectReferenceValue = null; 
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        return EditorGUIUtility.singleLineHeight * (keysProperty.arraySize + 2);
    }
}