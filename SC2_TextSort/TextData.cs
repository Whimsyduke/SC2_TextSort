using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SC2_TextSort
{
    public class TranslateDataObject : PropertyTools.Observable
    {
        private string id;
        private string zh_CN;
        private string en_US;

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                this.SetValue(ref this.id, value, () => this.Id);
            }
        }

        public string Zh_CN
        {
            get
            {
                return zh_CN;
            }

            set
            {
                this.SetValue(ref this.zh_CN, value, () => this.Zh_CN);
            }
        }

        public string En_US
        {
            get
            {
                return en_US;
            }

            set
            {
                this.SetValue(ref this.en_US, value, () => this.En_US);
            }
        }
    }
}
