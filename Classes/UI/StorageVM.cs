using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Storage.Classes.UI
{
    public class StorageVM : ViewModel
    {
        private string _title;
        private string _buttonText;
        private Action _buttonAction;

        public string Title
        {
            get => this._title;
            set
            {
                if (!(this._title != value))
                    return;
                this._title = value;
                this.OnPropertyChanged(nameof(Title));
            }
        }

        public string ButtonText
        {
            get => this._buttonText;
            set
            {
                if (!(this._buttonText != value))
                    return;
                this._buttonText = value;
                this.OnPropertyChanged(nameof(ButtonText));
            }
        }

        public Action ButtonAction
        {
            get => this._buttonAction;
            set
            {
                if (!(this._buttonAction != value))
                    return;

                this._buttonAction = value;
                this.OnPropertyChanged(nameof(ButtonAction));
            }
        }

        public StorageVM(string Title,
            string ButtonText,
            Action ButtonAction)
        {
            this.Title = Title;
            this.ButtonText = ButtonText;
            this.ButtonAction = ButtonAction;
        }

        public void OnButtonClicked() => ButtonAction();
    }
}
