using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace Storage.Classes
{
    internal class Storage
    {
        public int ID { get; set; }
        public Settlement Settlement { get; set; }
        public int Capacity { get; set; }
    }
}
