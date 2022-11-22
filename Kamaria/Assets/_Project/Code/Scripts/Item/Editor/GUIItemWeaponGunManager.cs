using Kamaria.Item.Weapon;
using UnityEditor;
using UnityEngine;

namespace Kamaria.Item.GUIEditor
{
    [CustomEditor(typeof(ItemWeaponGunManager))]
    public sealed class GUIItemWeaponGunManager : Editor
    {
        private float spaceMain = 5;
        
        public override void OnInspectorGUI()
        {
            ItemWeaponGunManager itemWeaponGunManager = (ItemWeaponGunManager)target;

            DrawDefaultInspector();

            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (1)"))
            {
                itemWeaponGunManager.UnlockSkillComponent1Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (2)"))
            {
                itemWeaponGunManager.UnlockSkillComponent2Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Unlock Skill Component (3)"))
            {
                itemWeaponGunManager.UnlockSkillComponent3Editor();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Evolution"))
            {
                itemWeaponGunManager.EvolutionEditor();
            }
        }
    }
}