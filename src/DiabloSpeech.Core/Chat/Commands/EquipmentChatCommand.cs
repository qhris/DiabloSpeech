// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloInterfaceAPI;
using DiabloSpeech.Extensions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DiabloSpeech.Core.Chat.Commands
{
    [CommandAlias("equipment", "item")]
    class EquipmentChatCommand : IChatCommand
    {
        const string ServerName = "DiabloInterfaceItems";

        public async Task Process(IChatWriter chat, ChatCommandData data)
        {
            if (data.Arguments.Count == 0)
            {
                chat.SendMessage($"Usage: !{data.CommandAlias} [item]. Possible values: helm, armor, weapon, shield, weapon2, shield2, belt, gloves, boots, ring(s), and amulet.");
                return;
            }

            // Resolve valid item names.
            string itemName = data.Arguments[0]?.ToLowerInvariant();
            ItemRequest.Slot? itemSlot = ResolveItemSlot(itemName);
            if (!itemSlot.HasValue)
            {
                chat.SendMessage("Invalid item name.");
                return;
            }

            // Attempt to get a response.
            ItemResponse response;
            try { response = await QueryItem(itemSlot.Value); }
            catch (TimeoutException)
            {
                chat.SendMessage("Failed to get a response from item server!");
                return;
            }

            if (!response.IsValid)
            {
                chat.SendMessage("Invalid request!");
            }
            else if (!response.Success)
            {
                chat.SendMessage($"{itemName.CapitalizeFirst()} not equipped!");
            }
            else if (response.Items.Count > 0)
            {
                foreach (var item in response.Items)
                {
                    var itemBuilder = new StringBuilder(item.ItemName);
                    if (item.Properties.Count > 0)
                    {
                        itemBuilder.Append(": ");
                        itemBuilder.Append(string.Join(", ", item.Properties));
                    }

                    chat.SendMessage(itemBuilder.ToString().Replace("\n", ""));
                }
            }
        }

        async Task<ItemResponse> QueryItem(ItemRequest.Slot itemSlot)
        {
            using (var connection = new DiabloInterfaceConnection(ServerName))
            {
                return await connection.RequestAsync(new ItemRequest(itemSlot));
            }
        }

        public ItemRequest.Slot? ResolveItemSlot(string slot)
        {
            switch (slot.ToLowerInvariant())
            {
                case "helm":    return ItemRequest.Slot.Helm;
                case "armor":   return ItemRequest.Slot.Armor;
                case "weapon":  return ItemRequest.Slot.Weapon;
                case "shield":  return ItemRequest.Slot.Shield;
                case "weapon2": return ItemRequest.Slot.WeaponSwap;
                case "shield2": return ItemRequest.Slot.ShieldSwap;
                case "belt":    return ItemRequest.Slot.Belt;
                case "gloves":  return ItemRequest.Slot.Gloves;
                case "boots":   return ItemRequest.Slot.Boots;
                case "ring":    return ItemRequest.Slot.Rings;
                case "rings":   return ItemRequest.Slot.Rings;
                case "amulet":  return ItemRequest.Slot.Amulet;
                default: return null;
            }
        }
    }
}
