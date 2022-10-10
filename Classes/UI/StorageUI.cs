using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine;
using SandBox.View.Map;
using Storage.Classes.UI;
using TaleWorlds.ScreenSystem;

namespace Storage.Classes
{
    public class StorageUI : MapView
    {
        private StorageVM _ds;
        private GauntletLayer _layer;

        public StorageUI()
        {
            base.CreateLayout();
        }

        public void PushScreen(StorageVM vm)
        {
            if(_layer != null)
            {
                MapScreen.Instance.RemoveLayer(_layer);
            }
            _layer = new GauntletLayer(580);
            _ds = vm;
            _layer.LoadMovie("TestStorage", _ds);
            _layer.InputRestrictions.SetInputRestrictions();
            MapScreen.Instance.AddLayer(_layer);
            ScreenManager.TrySetFocus(_layer);
        }

        public void Close()
        {
            MapScreen.Instance.RemoveLayer(_layer);
        }
    }
}
