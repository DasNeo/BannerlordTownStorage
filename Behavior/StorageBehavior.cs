using Storage.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Storage
{
    internal class StorageBehavior : CampaignBehaviorBase
    {
        public Storages Rosters = new Storages();
        
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTickEvent);
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
                Campaign.Current.GameMenuManager.SetNextMenu("player_town_storage_list");
            }), false, 2, true);

            campaignGameStarter.AddGameMenuOption("player_town_storage_list", "player_town_storage_purchase", "Buy Storage", new GameMenuOption.OnConditionDelegate((args) =>
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
                Campaign.Current.GameMenuManager.ExitToLast();
                //Campaign.Current.GameMenuManager.RefreshMenuOptions(Campaign.Current.CurrentMenuContext);
            }), false, 0, false);

            campaignGameStarter.AddGameMenuOption("player_town_storage_list", "player_town_storage_sell", "Sell Storage", new GameMenuOption.OnConditionDelegate((args) =>
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
                Rosters.BuyStorage(Settlement.CurrentSettlement);
                Campaign.Current.GameMenuManager.ExitToLast();
                //Campaign.Current.GameMenuManager.RefreshMenuOptions(Campaign.Current.CurrentMenuContext);
            }), false, 0, false);

            campaignGameStarter.AddGameMenuOption("player_town_storage_list", "player_town_storage_enter", "Enter Storage", new GameMenuOption.OnConditionDelegate((args) =>
            {
                Settlement settlement = Settlement.CurrentSettlement;
                if(!Rosters.Contains(settlement))
                    return false; // storage hasn't been bought in this settlement


                bool isAtWar = settlement?.OwnerClan.IsAtWarWith(Hero.MainHero.MapFaction) ?? false;
                args.Tooltip = (isAtWar) ? new TextObject("You are currently in war with this faction and can't access the storage.") : new TextObject("");
                args.IsEnabled = !isAtWar;
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                return true;
            }), new GameMenuOption.OnConsequenceDelegate((args) =>
            {
                InventoryManager.OpenScreenAsStash(Rosters[Settlement.CurrentSettlement].Roster);
            }), false, 0, true);



            
        }
    }
}
