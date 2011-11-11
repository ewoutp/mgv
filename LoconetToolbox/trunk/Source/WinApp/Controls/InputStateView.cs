﻿/*
Loconet toolbox
Copyright (C) 2010 Modelspoorgroep Venlo, Ewout Prangsma

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using LocoNetToolBox.Protocol;

namespace LocoNetToolBox.WinApp.Controls
{
    public partial class InputStateView : UserControl
    {
        private readonly Dictionary<int, InputItem> items = new Dictionary<int, InputItem>();

        /// <summary>
        /// Default ctor
        /// </summary>
        public InputStateView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Process the given response.
        /// </summary>
        internal void ProcessResponse(Response value)
        {
            var inpRep = value as InputReport;
            var swRep = value as SwitchReport;
            if (inpRep != null)
            {
                var item = GetItem(inpRep.Address, true);
                item.ForeColor = inpRep.SensorLevel ? Color.Red : Color.Green;
                item.Text += inpRep.SensorLevel ? "-On" : "-Off";
            }
            else if (swRep != null)
            {
                var item = GetItem(swRep.Address, false);
                item.ForeColor = swRep.SensorLevel ? Color.Red : Color.Green;
                item.Text += swRep.SensorLevel ? "-On" : "-Off";
            }
        }

        /// <summary>
        /// Gets or creates an item for the given address.
        /// </summary>
        private InputItem GetItem(int address, bool isFeedback)
        {
            InputItem item;
            var key = InputItem.GetKey(address, isFeedback);
            if (!items.TryGetValue(key, out item))
            {
                item = new InputItem(address, isFeedback);
                items.Add(key, item);

                lvInputs.BeginUpdate();
                lvInputs.Items.Clear();
                lvInputs.Items.AddRange(items.Values.ToArray());
                lvInputs.EndUpdate();
            }
            return item;
        }

        /// <summary>
        /// Input for a specific address
        /// </summary>
        private class InputItem : ListViewItem
        {
            private readonly string textPrefix;

            /// <summary>
            /// Default ctor
            /// </summary>
            internal InputItem(int address, bool isFeedback)
            {
                textPrefix = string.Format("{0}{1}", isFeedback ? "F" : "S", address + 1);
            }

            /// <summary>
            /// Merge text.
            /// </summary>
            public new string Text
            {
                get { return textPrefix; }
                set { base.Text = value; }
            }

            /// <summary>
            /// Indexing key.
            /// </summary>
            internal static int GetKey(int address, bool isFeedback)
            {
                return isFeedback ? address : -address; 
            }
        }
    }
}
