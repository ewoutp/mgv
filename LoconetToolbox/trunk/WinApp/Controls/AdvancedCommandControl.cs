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

using LocoNetToolBox.Devices.LocoIO;
using LocoNetToolBox.Protocol;
using Message = LocoNetToolBox.Protocol.Message;

namespace LocoNetToolBox.WinApp.Controls
{
    public partial class AdvancedCommandControl : UserControl
    {
        private LocoBuffer lb;

        /// <summary>
        /// Default ctor
        /// </summary>
        public AdvancedCommandControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Connect to locobuffer
        /// </summary>
        internal LocoBuffer LocoBuffer { set { lb = value; } }

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

        /// <summary>
        /// Cause all devices to identify themselves.
        /// </summary>
        private void cmdDiscover_Click(object sender, EventArgs e)
        {
            var cmd = new PeerXferRequest1()
            {
                Command = PeerXferRequest.Commands.Discover
            };
            Execute(cmd);
        }

        /// <summary>
        /// Read Single SV
        /// </summary>
        private void cmdRead_Click(object sender, EventArgs e)
        {
            var programmer = new Programmer(lb, new LocoNetAddress((byte)tbDstL.Value, (byte)tbDstH.Value));
            var configs = new SVConfig[] { new SVConfig((int)tbSvAddress.Value) };
            programmer.Read(configs);

            if (configs[0].Valid)
            {
                MessageBox.Show("Value = " + configs[0].Value);
            }
            else
            {
                MessageBox.Show("Cannot read SV");
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

        private void cmdBusy_Click(object sender, EventArgs e)
        {
            Execute(new Busy());
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

        private void cmdSwitchRequest_Click(object sender, EventArgs e)
        {
            Execute(new SwitchRequest()
            {
                Address = (int)tbAddress.Value - 1,
                Direction = cbDirection.Checked,
                Output = cbOutput.Checked
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
    }
}
