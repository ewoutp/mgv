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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LocoNetToolBox.Protocol;
using Message = LocoNetToolBox.Protocol.Message;

namespace LocoNetToolBox.WinApp.Controls
{
    public partial class CommandControl : UserControl
    {
        private LocoBuffer lb;
        private LocoNetAddress currentAddress;

        /// <summary>
        /// Default ctor
        /// </summary>
        public CommandControl()
        {
            InitializeComponent();
            UpdateUI();
        }

        /// <summary>
        /// Connect to locobuffer
        /// </summary>
        internal LocoBuffer LocoBuffer { set { lb = value; } }

        /// <summary>
        /// Sets the current loconet address (null means no selection)
        /// </summary>
        internal LocoNetAddress CurrentAddress
        {
            set
            {
                currentAddress = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Program LocoIO module
        /// </summary>
        internal void ProgramLocoIO()
        {
            if (currentAddress != null)
            {
                var dialog = new LocoIOConfigurationForm();
                dialog.Initialize(lb, currentAddress);
                dialog.Show();
            }
        }

        /// <summary>
        /// Update the controls
        /// </summary>
        private void UpdateUI()
        {
            cmdProgramLocoIO.Enabled = (currentAddress != null);
        }

        /// <summary>
        /// Execute the given request.
        /// </summary>
        private void Execute(Request request)
        {
            try
            {
                request.Execute(lb);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdGpOn_Click(object sender, EventArgs e)
        {
            Execute(new GlobalPowerOn());
        }

        private void cmdGpOff_Click(object sender, EventArgs e)
        {
            Execute(new GlobalPowerOff());
        }

        private void cmdQuery_Click(object sender, EventArgs e)
        {
            Execute(new PeerXferRequest1()
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
        private void cmdServoProgrammer_Click(object sender, EventArgs e)
        {
            var dialog = new ServoProgrammer(this.lb);
            dialog.Show();
        }

        private void cmdServoTester_Click(object sender, EventArgs e)
        {
            var dialog = new ServoTester(this.Execute);
            dialog.Show();
        }

        private void cmdAdvanced_Click(object sender, EventArgs e)
        {
            using (var dialog = new LocoIODebugForm())
            {
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Program the LocoIO on the current address.
        /// </summary>
        private void cmdProgramLocoIO_Click(object sender, EventArgs e)
        {
            ProgramLocoIO();
        }
    }
}
