/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnjoyGameClub.TextLifeFramework.Core.Animation.LiteMotion.Editor
{
    [CustomPropertyDrawer(typeof(LiteMotionBehaviour), true)]
    public class LiteMotionBehaviourDrawer : PropertyDrawer
    {
        private Dictionary<object, float> _dictionary = new Dictionary<object, float>();
        private Dictionary<object, bool> _dictionaryFolder = new Dictionary<object, bool>();

        private Dictionary<Object, UnityEditor.SerializedObject> _reorderableListMap =
            new Dictionary<Object, UnityEditor.SerializedObject>();

        private float _propertyHeight;
        private const float WIDTH_OFFSET = 10;
        private const float X_OFFSET = 10;
        private const float PROPERTY_X_OFFSET = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            var currentObject = property.objectReferenceValue;
            if (currentObject == null)
            {
                return;
            }

            var behaviourSerializedObject = GetSerializedObject(currentObject);
            behaviourSerializedObject.Update();
            _dictionary.TryAdd(currentObject, 0);
            _dictionaryFolder.TryAdd(currentObject, true);
            _dictionary[currentObject] = 0;
            _dictionaryFolder[currentObject] = EditorGUI.Foldout(position, _dictionaryFolder[currentObject], label);
            position.y += EditorGUIUtility.singleLineHeight + 1;
            if (_dictionaryFolder[currentObject])
            {
                var behaviour = currentObject as LiteMotionBehaviour;
                DataType dataType = behaviour.dataType;
                CharacterBehaviourType behaviourType = behaviour.behaviourType;

                // Data type.
                SerializedProperty dataTypeProperty = behaviourSerializedObject.FindProperty("dataType");
                SerializedProperty characterBehaviourProperty =
                    behaviourSerializedObject.FindProperty("behaviourType");
                SerializedProperty linearValueProperty = behaviourSerializedObject.FindProperty("intensity");
                SerializedProperty randomValueProperty = behaviourSerializedObject.FindProperty("Range");
                SerializedProperty characterTimeOffsetProperty =
                    behaviourSerializedObject.FindProperty("characterTimeOffset");
                SerializedProperty pivotProperty = behaviourSerializedObject.FindProperty("Pivot");
                DrawProperty(ref position, characterBehaviourProperty, currentObject);
                DrawProperty(ref position, dataTypeProperty, currentObject);

                if (dataType is DataType.Random &&
                    behaviourType is not (CharacterBehaviourType.Color or CharacterBehaviourType.Alpha))
                {
                    DrawProperty(ref position, randomValueProperty,
                        currentObject);
                }
                else if (behaviourType is not (CharacterBehaviourType.Color or CharacterBehaviourType.Alpha))
                {
                    DrawProperty(ref position, linearValueProperty, currentObject);
                }


                DrawProperty(ref position, characterTimeOffsetProperty, currentObject);

                if (behaviourType is CharacterBehaviourType.RotateX or CharacterBehaviourType.RotateY
                    or CharacterBehaviourType.RotateZ or CharacterBehaviourType.ScaleX or CharacterBehaviourType.ScaleY)
                {
                    DrawProperty(ref position, pivotProperty, currentObject);
                }

                if (dataType is DataType.Curve)
                {
                    if (behaviourType is CharacterBehaviourType.Color)
                    {
                        DrawProperty(ref position, behaviourSerializedObject.FindProperty("gradient"),
                            currentObject);
                    }
                    else
                    {
                        DrawProperty(ref position, behaviourSerializedObject.FindProperty("animationCurve"),
                            currentObject);
                    }
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            behaviourSerializedObject.ApplyModifiedProperties();
        }


        private void DrawProperty(ref Rect position, SerializedProperty property, object obj)
        {
            float dataTypePropertyHeight = EditorGUI.GetPropertyHeight(property, true);
            EditorGUI.PropertyField(new Rect(position.x + X_OFFSET, position.y,
                    position.width - WIDTH_OFFSET - PROPERTY_X_OFFSET,
                    dataTypePropertyHeight),
                property,
                true);
            position.y += dataTypePropertyHeight + 2;
            _dictionary[obj] += dataTypePropertyHeight + 2;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var currentObject = property.objectReferenceValue;
            float height = EditorGUIUtility.singleLineHeight; // 标题的高度
            if (currentObject == null)
            {
                return height; // 额外留出 Null 提示的高度
            }

            _dictionaryFolder.TryAdd(currentObject, true);
            if (currentObject == null || !_dictionaryFolder[currentObject])
            {
                return height; // 额外留出 Null 提示的高度
            }

            _dictionary.TryAdd(currentObject, 0);
            height += _dictionary[currentObject];
            return height;
        }

        private UnityEditor.SerializedObject GetSerializedObject(Object obj)
        {
            UnityEditor.SerializedObject serializedObject;
            if (!_reorderableListMap.TryGetValue(obj, out serializedObject))
            {
                serializedObject = new UnityEditor.SerializedObject(obj);
                _reorderableListMap.Add(obj, serializedObject);
            }

            return serializedObject;
        }
    }
}