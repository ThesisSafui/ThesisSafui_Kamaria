using Kamaria.Item.Weapon;
using UnityEditor;
using UnityEngine;

namespace Kamaria.Item.GUIEditor
{
    [CustomEditor(typeof(ItemWeaponSwordManager))]
    public class GUIItemWeaponSwordManager : Editor
    {
        private float spaceMain = 5;
        
        public override void OnInspectorGUI()
        {
            ItemWeaponSwordManager itemWeaponSwordManager = (ItemWeaponSwordManager)target;

            DrawDefaultInspector();

            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (1)"))
            {
                itemWeaponSwordManager.UnlockSkillComponent1Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (2)"))
            {
                itemWeaponSwordManager.UnlockSkillComponent2Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (3)"))
            {
                itemWeaponSwordManager.UnlockSkillComponent3Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Evolution"))
            {
                itemWeaponSwordManager.EvolutionEditor();
            }
        }
    }
}