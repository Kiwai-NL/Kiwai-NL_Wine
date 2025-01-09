using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Life;
using Life.DB;
using Life.Network;
using Life.UI;
using Mirror;
using UnityEngine;

namespace KiwaïNLWin
{
    public class Main : Plugin
    {
        public Main(IGameAPI api) : base(api)
        {
        }
        public override void OnPluginInit()
        {
            Console.WriteLine("KiwaïNLWin loaded");
        }
        public override void OnPlayerInput(Player player, KeyCode keyCode, bool onUI)
        {
            base.OnPlayerInput(player, keyCode, onUI);

            UIPanel panel = new UIPanel("Marché des vignerons", UIPanel.PanelType.Input);
            panel.title = "Marché des vignerons";
        }
    }
}
