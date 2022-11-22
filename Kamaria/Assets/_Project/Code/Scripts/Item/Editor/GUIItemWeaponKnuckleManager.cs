using Kamaria.Item.Weapon;
using UnityEditor;
using UnityEngine;

namespace Kamaria.Item.GUIEditor
{
    [CustomEditor(typeof(ItemWeaponKnuckleManager))]
    public class GUIItemWeaponKnuckleManager : Editor
    {
        private float spaceMain = 5;
        
        public override void OnInspectorGUI()
        {
            ItemWeaponKnuckleManager itemWeaponKnuckleManager = (ItemWeaponKnuckleManager)target;

            DrawDefaultInspector();

            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (1)"))
            {
                itemWeaponKnuckleManager.UnlockSkillComponent1Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (2)"))
            {
                itemWeaponKnuckleManager.UnlockSkillComponent2Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (3)"))
            {
                itemWeaponKnuckleManager.UnlockSkillComponent3Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Evolution"))
            {
                itemWeaponKnuckleManager.EvolutionEditor();
            }
        }
    }
}