using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;

namespace Storage.Classes
{
    internal class Storages : IList<Storage>
    {
        /// <summary>
        /// One-time price for a storage.
        /// </summary>
        public const int PurchasePrice = 500;
        /// <summary>
        /// Daily rent price for each storage
        /// </summary>
        public const int RentPrice = 200;
        /// <summary>
        /// <para>Multiplies the PurchasePrice by the Multiplier with each Purchase.</para>
        /// Formular: (int)Math.Round(PurchasePrice * (StorageCount * PurchasePriceMultiplier));
        /// </summary>
        public const double PurchasePriceMultiplier = 1;

        /// <summary>
        /// Calculates the purchase price for the next storage taking <see cref="PurchasePriceMultiplier"/> into account
        /// </summary>
        public int NextPurchasePrice
        {
            get
            {
                double multiplier = (StorageCount+1) * PurchasePriceMultiplier;
                return (int)Math.Round(PurchasePrice * multiplier);
            }
        }

        public CampaignGameStarter cgs { get; set; }

        private List<Storage> storages = new List<Storage>();

        public int StorageCount
        {
            get => storages.Count;
        }

        public bool BuyStorage(Settlement settlement)
        {
            if(Hero.MainHero.Gold >= NextPurchasePrice
                && !Contains(settlement))
            {
                GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, NextPurchasePrice);
                Add(new Storage()
                {
                    ID = StorageCount,
                    Roster = new TaleWorlds.CampaignSystem.Roster.ItemRoster(),
                    Settlement = settlement
                });
                return true;
            }
            return false;
        }

        #region IList Implementation
        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public Storage this[int index] 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

        public Storage this[Settlement settlement] 
        { 
            get 
            {
                return storages.FirstOrDefault(r => r.Settlement == settlement);
            }
            set
            {
                var stor = storages.FirstOrDefault(r => r.Settlement == settlement);
                if(value is Storage)
                {
                    stor = (Storage)value;
                } else
                {
                    throw new ArgumentException("Value is not a Storage object");
                }
            }
        }

        public void Add(Storage value)
        {
            storages.Add((Storage)value);
        }

        public bool Contains(Storage value)
        {
            var stor = storages.FirstOrDefault(r => r.Settlement == value.Settlement);
            return (stor is null) ? false : true;
        }

        public bool Contains(Settlement value)
        {
            var stor = storages.FirstOrDefault(r => r.Settlement == value);
            return (stor is null) ? false : true;
        }

        public void Clear()
        {
            storages.Clear();
        }

        public int IndexOf(Storage value)
        {
            return storages.FirstOrDefault(r => r.Settlement == value.Settlement).ID;
        }

        public void Insert(int index, Storage value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Storage value)
        {
            var storId = storages.FirstOrDefault(r => r.Settlement == value.Settlement);
            return storages.Remove(storId);
        }

        public bool Remove(Settlement value)
        {
            var storId = storages.FirstOrDefault(r => r.Settlement == value);
            return storages.Remove(storId);
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Storage[] array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator<Storage> IEnumerable<Storage>.GetEnumerator() => storages.GetEnumerator();
        public IEnumerator GetEnumerator() => storages.GetEnumerator();
        #endregion
    }
}
