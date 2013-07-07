using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AssetBrowser
{
    class ScopedListViewUpdate : IDisposable
    {
        ListView control;

        public ScopedListViewUpdate( ListView control )
        {
            this.control = control;

            control.BeginUpdate();
        }

        public void Dispose()
        {
            control.EndUpdate();
        }
    }
}
