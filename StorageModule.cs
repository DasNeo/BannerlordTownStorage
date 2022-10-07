using SandBox.Missions.MissionLogics.Arena;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Storage
{
    public class StorageModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            InformationManager.DisplayMessage(new InformationMessage("Storage Loaded", Color.FromUint(14703633U)));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            this.AddBehaviours(gameInitializer);
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            base.OnCampaignStart(game, starterObject);
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

            this.AddMissionBehaviours(mission);
        }

        private void AddBehaviours(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddBehavior(new StorageBehavior());
        }

        private void AddMissionBehaviours(Mission mission)
        {
            
        }
    }
}
