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
        public ItemRoster Roster = new ItemRoster();

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("StorageRoster", ref Roster);
        }

        public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddGameMenus(campaignGameStarter);
        }

        protected void AddGameMenus(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption("town", "player_town_storage", "Enter Storage", new GameMenuOption.OnConditionDelegate((args) =>
            {
                Settlement settlement = Settlement.CurrentSettlement;
                bool isAtWar = settlement?.OwnerClan.IsAtWarWith(Hero.MainHero.MapFaction) ?? false;
                args.Tooltip = (isAtWar) ? new TextObject("You are currently in war with this faction and can't access the storage.") : new TextObject("");
                args.IsEnabled = !isAtWar;
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                return true;
            }), new GameMenuOption.OnConsequenceDelegate((args) =>
            {
                //roster.Add(new ItemRosterElement(Items.All.FirstOrDefault(), 20));
                InventoryManager.OpenScreenAsStash(Roster);
            }), false, 2, true);
        }


    }
}
