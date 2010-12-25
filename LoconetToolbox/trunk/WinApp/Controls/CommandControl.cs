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
using System;
using System.Windows.Forms;
using LocoNetToolBox.Model;
using LocoNetToolBox.Protocol;
using LocoNetToolBox.WinApp.Communications;

namespace LocoNetToolBox.WinApp.Controls
{
    public partial class CommandControl : UserControl
    {
        private AsyncLocoBuffer lb;
        private LocoNetState lnState;

        /// <summary>
        /// Default ctor
        /// </summary>
        public CommandControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Connect to locobuffer
        /// </summary>
        internal AsyncLocoBuffer LocoBuffer { set { lb = value; } }
        internal LocoNetState LocoNetState  { set { lnState = value;}}

        /// <summary>
        /// Execute the given request.
        /// </summary>
        private void Execute(Request request)
        {
            lb.BeginRequest(request, e => {
                if (e.HasError)
                {
                    MessageBox.Show(e.Error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);                    
                }
            });
        }

        private void CmdGpOnClick(object sender, EventArgs e)
        {
            Execute(new GlobalPowerOn());
        }

        private void CmdGpOffClick(object sender, EventArgs e)
        {
            Execute(new GlobalPowerOff());
        }

        private void CmdQueryClick(object sender, EventArgs e)
        {
            Execute(new PeerXferRequest1
                        {
                Command = PeerXferRequest.Commands.Read,
                DestinationLow = 0,
               // DestinationHigh = 0,
                SvAddress = 0
            });
        }

        /// <summary>
        /// Open the servo programmer
        /// </summary>
        private void CmdServoProgrammerClick(object sender, EventArgs e)
        {
            var dialog = new ServoProgrammer(lb);
            dialog.Show();
        }

        private void CmdServoTesterClick(object sender, EventArgs e)
        {
            var dialog = new ServoTester(lb, lnState);
            dialog.Show();
        }

        private static void CmdAdvancedClick(object sender, EventArgs e)
        {
            using (var dialog = new LocoIODebugForm())
            {
                dialog.ShowDialog();
            }
        }
    }
}
