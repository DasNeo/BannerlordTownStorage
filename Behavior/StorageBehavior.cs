using Storage.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Storage
{
    internal class StorageBehavior : CampaignBehaviorBase
    {
        public Storages Rosters = new Storages();

        internal bool isInStorageInventory = false;
        
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTickEvent);
            CampaignEvents.PlayerInventoryExchangeEvent.AddNonSerializedListener(this, PlayerInventoryExchangeEvent);

            Rosters.Roster.RosterUpdatedEvent += Roster_RosterUpdatedEvent;
            //InventoryManager.InventoryLogic.InventoryListener = new InventoryLogic(null);
        }

        private void Roster_RosterUpdatedEvent(ItemRosterElement item, int count)
        {
            if(isInStorageInventory && count > 0)
            {
                if(Rosters.Capacity < Rosters.Roster.Count)
                {
                    //var transCommand = TransferCommand.Transfer(count, InventoryLogic.InventorySide.OtherInventory, InventoryLogic.InventorySide.PlayerInventory,
                    //    item, EquipmentIndex.None, EquipmentIndex.None, Hero.MainHero.CharacterObject, false);
                    //InventoryManager.InventoryLogic.AddTransferCommand(transCommand);
                    //InventoryManager.InventoryLogic.AddTransferCommand(new TransferCommand() { FromSide = InventoryLogic.InventorySide.OtherInventory, ToSide });
                    var newItem = new ItemRosterElement(item.EquipmentElement, count);
                    Hero.MainHero.PartyBelongedTo?.ItemRoster.Add(newItem);

                    var oldItem = Rosters.Roster.FirstOrDefault(r => r.IsEqualTo(item));
                    var oldItemIndex = Rosters.Roster.FindIndexOfElement(item.EquipmentElement);
                    if (oldItem.Amount+1 == count)
                        Rosters.Roster.AddToCounts(oldItem.EquipmentElement, -(oldItem.Amount+1));
                    else
                    {
                        oldItem.Amount -= count;
                    }
                    
                    //Rosters.Roster.(item);
                }
            }
        }

        private void PlayerInventoryExchangeEvent(List<(ItemRosterElement, int)> arg1, List<(ItemRosterElement, int)> arg2, bool arg3)
        {
            if(isInStorageInventory)
            {
                if(arg2.Count > Rosters.Capacity)
                {

                }
            }
            isInStorageInventory = false;
        }

        private void DailyTickEvent()
        {
            if(Storages.RentPrice > 0)
                Hero.MainHero.ChangeHeroGold(-Storages.RentPrice);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("StorageRosters", ref Rosters);
        }

        public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddGameMenus(campaignGameStarter);
            Rosters.cgs = campaignGameStarter;
        }

        protected void AddGameMenus(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption("town", "player_town_storage", "Storage", new GameMenuOption.OnConditionDelegate((args) =>
            {
                args.IsEnabled = true;
                args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                return true;
            }), new GameMenuOption.OnConsequenceDelegate((args) =>
            {
                GameMenu.SwitchToMenu("player_town_storage_actions");
            }), false, 2, true);

            campaignGameStarter.AddGameMenu("player_town_storage_actions", "Storage", (args) =>
            {
                args.MenuTitle = new TextObject("Storage");
            });

            campaignGameStarter.AddGameMenuOption("player_town_storage_actions", "player_town_storage_purchase", "Manage Storages", new GameMenuOption.OnConditionDelegate((args) =>
            {
                return true;
            }), new GameMenuOption.OnConsequenceDelegate((args) =>
            {
                StorageUI ui = new StorageUI();
                ui.PushScreen(new Classes.UI.StorageVM("Testing Stuff here", "Click me", () =>
                {
                    ui.Close();
                }));
            }), false, 0, false);

            campaignGameStarter.AddGameMenuOption("player_town_storage_actions", "player_town_storage_purchase", "Buy Storage", new GameMenuOption.OnConditionDelegate((args) =>
            {
                Settlement settlement = Settlement.CurrentSettlement;
                bool hasPurchasedStorage = Rosters.Contains(settlement);

                if (hasPurchasedStorage)
                    return false; // return false if they have already purchased this storage to disable this option.

                string RentPriceText = (Storages.RentPrice > 0) ? $" and {Storages.RentPrice} daily." : ".";
                bool isEnabled = true;

                args.Tooltip = new TextObject($"Buying this Storage costs {Rosters.NextPurchasePrice}{RentPriceText}");

                if (Hero.MainHero.Gold < Rosters.NextPurchasePrice)
                    isEnabled = false; // Player doesn't have enought money to buy it so we disable it.

                args.IsEnabled = isEnabled;
                args.optionLeaveType = GameMenuOption.LeaveType.Manage;
                return true;
            }), new GameMenuOption.OnConsequenceDelegate((args) =>
            {
                Rosters.BuyStorage(Settlement.CurrentSettlement);
                GameMenu.SwitchToMenu("player_town_storage_actions");
                //Campaign.Current.GameMenuManager.RefreshMenuOptions(Campaign.Current.CurrentMenuContext);
            }), false, 1, false);

            campaignGameStarter.AddGameMenuOption("player_town_storage_actions", "player_town_storage_sell", "Sell Storage", new GameMenuOption.OnConditionDelegate((args) =>
            {
                Settlement settlement = Settlement.CurrentSettlement;
                bool hasPurchasedStorage = Rosters.Contains(settlement);

                if (!hasPurchasedStorage)
                    return false; // return false if they have already purchased this storage to disable this option.

                args.IsEnabled = true;
                args.optionLeaveType = GameMenuOption.LeaveType.Manage;
                return true;
            }), new GameMenuOption.OnConsequenceDelegate((args) =>
            {
                Rosters.SellStorage(Settlement.CurrentSettlement);
                GameMenu.SwitchToMenu("player_town_storage_actions");
                //Campaign.Current.GameMenuManager.RefreshMenuOptions(Campaign.Current.CurrentMenuContext);
            }), false, 1, false);

            campaignGameStarter.AddGameMenuOption("player_town_storage_actions", "player_town_storage_enter", "Enter Storage", new GameMenuOption.OnConditionDelegate((args) =>
            {
                Settlement settlement = Settlement.CurrentSettlement;
                if (!Rosters.Contains(settlement))
                    return false; // storage hasn't been bought in this settlement


                bool isAtWar = settlement?.OwnerClan.IsAtWarWith(Hero.MainHero.MapFaction) ?? false;
                args.Tooltip = (isAtWar) ? new TextObject("You are currently in war with this faction and can't access the storage.") : new TextObject("");
                args.IsEnabled = !isAtWar;
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                return true;
            }), new GameMenuOption.OnConsequenceDelegate((args) =>
            {
                isInStorageInventory = true;
                InventoryManager.OpenScreenAsStash(Rosters.Roster);
            }), false, 2, true);

            campaignGameStarter.AddGameMenuOption("player_town_storage_actions", "player_town_storage_exit", "Leave", new GameMenuOption.OnConditionDelegate((args) =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                return true;
            }), (args) =>
            {
                GameMenu.ExitToLast();
            }, false, -1, true);
        }
    }
}
