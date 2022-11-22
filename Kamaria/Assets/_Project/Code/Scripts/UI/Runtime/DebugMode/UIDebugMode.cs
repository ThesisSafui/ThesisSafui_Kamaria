using System;
using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.DebugMode
{
    public sealed class UIDebugMode : MonoBehaviour
    {
        [Serializable]
        private sealed class ButtonKeyStone
        {
            [SerializeField] private Button button;
            [SerializeField] private KeyStones keyStones;

            public Button Button => button;
            public KeyStones KeyStones => keyStones;
        }
        
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private List<ButtonKeyStone> buttonKeyStones;
        [SerializeField] private Button reset;
        [SerializeField] private GameManagerFarming gameManagerFarming;
        
        private void OnEnable()
        {
            if (reset != null)
            {
                reset.onClick.AddListener(ResetSpawn);
            }
            
            for (int i = 0; i < buttonKeyStones.Count; i++)
            {
                int index = i;
                buttonKeyStones[i].Button.onClick.AddListener(delegate
                {
                    UseKeyStone(buttonKeyStones[index].Button, buttonKeyStones[index].KeyStones);
                });
            }
        }

        private void ResetSpawn()
        {
            gameManagerFarming.ResetDebugMode();
        }

        private void OnDisable()
        {
            if (reset != null)
            {
                reset.onClick.RemoveListener(ResetSpawn);
            }
            
            for (int i = 0; i < buttonKeyStones.Count; i++)
            {
                buttonKeyStones[i].Button.onClick.RemoveAllListeners();
            }
        }

        private void UseKeyStone(Button button, KeyStones keyStones)
        {
            for (int i = 0; i < buttonKeyStones.Count; i++)
            {
                if (buttonKeyStones[i].KeyStones == keyStones)
                {
                    buttonKeyStones[i].Button.interactable = false;
                    continue;
                }

                buttonKeyStones[i].Button.interactable = true;
            }
            
            playerData.Info.DeviceCompartment.SlotWeapon.Info.UsedKeyStone = keyStones;
        }
    }
}