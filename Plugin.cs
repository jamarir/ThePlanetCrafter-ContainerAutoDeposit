using System;
using UnityEngine.InputSystem;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SpaceCraft;

namespace TPCMod
{
    // When clicking the left MoveAll button along-side with LCTRL, deposit any item from the player's inventory to the container's if the container has at least one instance of the player's item to deposit.
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInProcess("Planet Crafter.exe")]
    public class Plugin : BaseUnityPlugin
    {
        const string PLUGIN_GUID = "dev.jamarir.theplanetcrafter.containerautodeposit";
        const string PLUGIN_NAME = "The Planet Crafter Plugin ContainerAutoDeposit";
        const string PLUGIN_VERSION = "1.0.0";
        internal static new ManualLogSource Log;

        private void Awake()
        {
            Log = base.Logger;
            Log.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
            (new Harmony(PLUGIN_GUID)).PatchAll();
        }

        [HarmonyPatch]
        class Patch
        {
            public static bool performAutoDeposit = false;

            // Prefix Hook the "SpaceCraft.InventoriesHandler.TransferAllItems()" function to check whether LCTRL + left MoveAll button is pressed.
            [HarmonyPrefix]
            [HarmonyPatch(typeof(SpaceCraft.InventoriesHandler), "TransferAllItems")]
            static void Prefix_SpaceCraftInventoriesHandlerTransferAllItems(SpaceCraft.InventoriesHandler __instance, Inventory fromInventory, Inventory toInventory)
            {
                // Checking if the MoveAll button pressed is associated with the left inventory (the player's).
                DataConfig.UiType openedUi = Managers.GetManager<WindowsHandler>().GetOpenedUi();
                var window = (UiWindowContainer)Managers.GetManager<WindowsHandler>().GetWindowViaUiId(openedUi);
                var _inventoryLeft = (Inventory)AccessTools.Field(window.GetType(), "_inventoryLeft").GetValue(window);

                // Toggle the patch trigger accordingly.
                if (Keyboard.current.leftCtrlKey.isPressed && fromInventory == _inventoryLeft)
                {
                    Patch.performAutoDeposit = true;
                }
                //Log.LogInfo($"trigger: {Patch.performAutoDeposit}");
            }

            // The method transfering stuff is "SpaceCraft.InventoriesHandler.TransferAllItemsServerRpc()". The exact condition looked after is: "if (inventoryById2.AddItem(worldObject))", after which the item is added into the destination container. So we'll patch that AddItem() conditional result (prefix) if we're in a patch scenario.
            [HarmonyPrefix]
            [HarmonyPatch(typeof(SpaceCraft.Inventory), "AddItem")]
            static bool Prefix_SpaceCraftInventoryAddItem(SpaceCraft.Inventory __instance, WorldObject worldObject)
            {
                // If we're in a patching scenario, we'll add the condition that the player's item must be in container's inventory.
                if (Patch.performAutoDeposit)
                {
                    // For each item in the destination container, we check if its WorlObject's Group Id (e.g. Magnesium, ice, Silicon, Cobalt, Titanium, etc.) is identical to the player's item.
                    //  __instance is the destination container, and worldObject is the player's item to be deposited here.
                    foreach (WorldObject containerWorldObject in __instance.GetInsideWorldObjects())
                    {
                        // If we've at least one match, store the player's item.
                        if (containerWorldObject.GetGroup().GetId() == worldObject.GetGroup().GetId())
                        {
                            // Execute the original call
                            return true;
                        }
                    }
                    // Otherwise, we don't deposit the item, then skipping the original call.
                    return false;
                }
                // If we aren't in a patching scenario, we call the original method.
                return true;
            }

            // After any TransferAllItems patch, we make sure to disable the patch trigger to avoid edge cases.
            [HarmonyPostfix]
            [HarmonyPatch(typeof(SpaceCraft.InventoriesHandler), "TransferAllItems")]
            static void Postfix_SpaceCraftInventoriesHandlerTransferAllItems()
            {
                Patch.performAutoDeposit = false;
            }
        }
    }
}