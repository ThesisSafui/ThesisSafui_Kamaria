using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Player.Data.DeviceCompartmen;
using UnityEditor;
using UnityEngine;

namespace Kamaria.Player.GUIEditor
{
    [CustomEditor(typeof(EditorDeviceCompartmentManager))]
    public sealed class GUIDeviceCompartmentManager : Editor
    {
        private float spaceMain = 5;
        private BaseItem currentWeapon;

        public override void OnInspectorGUI()
        {
            EditorDeviceCompartmentManager deviceCompartmentManager = (EditorDeviceCompartmentManager)target;
            
            DrawDefaultInspector();
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Equip (Sword)"))
            {
                if (deviceCompartmentManager.PlayerData.Info.DeviceCompartment.SlotWeapon.IsEmpty)
                {
                    var weaponSword =
                        deviceCompartmentManager.PlayerData.Info.Inventory.InventoryWeapon.Weapon.Find(x =>
                            x.WeaponType == WeaponTypes.Sword);
                    deviceCompartmentManager.Equip(weaponSword);
                    currentWeapon = weaponSword;
                }
            }
            if (GUILayout.Button("Equip (Knuckle)"))
            {
                if (deviceCompartmentManager.PlayerData.Info.DeviceCompartment.SlotWeapon.IsEmpty)
                {
                    var weaponKnuckle =
                        deviceCompartmentManager.PlayerData.Info.Inventory.InventoryWeapon.Weapon.Find(x =>
                            x.WeaponType == WeaponTypes.Knuckle);
                    deviceCompartmentManager.Equip(weaponKnuckle);
                    currentWeapon = weaponKnuckle;
                }
            }
            if (GUILayout.Button("Equip (Gun)"))
            {
                if (deviceCompartmentManager.PlayerData.Info.DeviceCompartment.SlotWeapon.IsEmpty)
                {
                    var weaponGun =
                        deviceCompartmentManager.PlayerData.Info.Inventory.InventoryWeapon.Weapon.Find(x =>
                            x.WeaponType == WeaponTypes.Gun);
                    deviceCompartmentManager.Equip(weaponGun);
                    currentWeapon = weaponGun;
                }
            }
            
            EditorGUILayout.Space(spaceMain);
            if (GUILayout.Button("Remove"))
            {
                if (!deviceCompartmentManager.PlayerData.Info.DeviceCompartment.SlotWeapon.IsEmpty)
                {
                    deviceCompartmentManager.Remove(currentWeapon);
                }
            }
        }
    }
}