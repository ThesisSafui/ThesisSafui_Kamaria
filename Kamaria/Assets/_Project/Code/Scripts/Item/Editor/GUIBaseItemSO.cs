using UnityEditor;
using UnityEngine;

namespace Kamaria.Item.GUIEditor
{
    [CustomEditor(typeof(BaseItemSO))]
    public class GUIBaseItemSO : Editor
    {
        private float spaceMain = 5;
        private float spaceSub = 0;
        private float boxHeight = 25;
        
        public override void OnInspectorGUI()
        {
            BaseItemSO baseItemSO = (BaseItemSO)target;

            DrawDefaultInspector();

            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Change value to default", GUILayout.Height(boxHeight+5)))
            {
                baseItemSO.SetValueDefault();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Add count", GUILayout.Height(boxHeight)))
            {
                baseItemSO.AddCount();
            }
            EditorGUILayout.Space(spaceSub);
            if (GUILayout.Button("Reset count", GUILayout.Height(boxHeight)))
            {
                baseItemSO.ResetCount();
            }
            EditorGUILayout.Space(spaceSub);
            if (GUILayout.Button("Reset item", GUILayout.Height(boxHeight)))
            {
                baseItemSO.ResetItem();
            }

            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Set item key stone", GUILayout.Height(boxHeight), GUILayout.Width(300)))
            {
                baseItemSO.SetItemKeyStone();
            }
            EditorGUILayout.Space(spaceSub);
            if (GUILayout.Button("Set item craft material", GUILayout.Height(boxHeight), GUILayout.Width(300)))
            {
                baseItemSO.SetItemCraftMaterial();
            }
            EditorGUILayout.Space(spaceSub);
            if (GUILayout.Button("Set item potion", GUILayout.Height(boxHeight), GUILayout.Width(300)))
            {
                baseItemSO.SetItemPotion();
            }
            EditorGUILayout.Space(spaceSub);
            if (GUILayout.Button("Set item support", GUILayout.Height(boxHeight), GUILayout.Width(300)))
            {
                baseItemSO.SetItemSupport();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Set item weapon sword", GUILayout.Height(boxHeight), GUILayout.Width(300)))
            {
                baseItemSO.SetItemWeaponSword();
            }
            EditorGUILayout.Space(spaceSub);
            if (GUILayout.Button("Set item weapon knuckle", GUILayout.Height(boxHeight), GUILayout.Width(300)))
            {
                baseItemSO.SetItemWeaponKnuckle();
            }
            EditorGUILayout.Space(spaceSub);
            if (GUILayout.Button("Set item weapon gun", GUILayout.Height(boxHeight), GUILayout.Width(300)))
            {
                baseItemSO.SetItemWeaponGun();
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Weapon refresh index create", GUILayout.Height(boxHeight)))
            {
                baseItemSO.RefreshIndexItemWeapon();
            }
            EditorGUILayout.Space(spaceSub);
            if (GUILayout.Button("General refresh index create", GUILayout.Height(boxHeight)))
            {
                baseItemSO.RefreshIndexItemGeneral();
            }
        }
    }
}