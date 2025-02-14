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
            
            SetDefaultValues(newKeyProp, newValueProp, newIndex);

            // Forcing Unity inspector refresh
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            SceneView.RepaintAll(); // Inspector force repaint

            GUI.FocusControl(null);
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    private void SetDefaultValues(SerializedProperty keyProp, SerializedProperty valueProp, int index)
    {
        // Using the property type to figure out the right defaults
        if (keyProp.propertyType == SerializedPropertyType.String)
        {
            keyProp.stringValue = "NewKey" + index;
        }
        else if (keyProp.propertyType == SerializedPropertyType.Integer)
        {
            keyProp.intValue = index;
        }
        else if (keyProp.propertyType == SerializedPropertyType.Float)
        {
            keyProp.floatValue = 0.0f;
        }

        // Default value handling for value types
        if (valueProp.propertyType == SerializedPropertyType.Integer)
        {
            valueProp.intValue = 0;
        }
        else if (valueProp.propertyType == SerializedPropertyType.Float)
        {
            valueProp.floatValue = 0.0f;
        }
        else if (valueProp.propertyType == SerializedPropertyType.String)
        {
            valueProp.stringValue = "NewValue" + index;
        }
        else if (valueProp.propertyType == SerializedPropertyType.Boolean)
        {
            valueProp.boolValue = false;
        }
        else
        {
            // Handling reference types or custom objects (null by default for now)
            valueProp.objectReferenceValue = null;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        return EditorGUIUtility.singleLineHeight * (keysProperty.arraySize + 2);
    }
}