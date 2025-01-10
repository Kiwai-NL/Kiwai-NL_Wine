using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Life;
using Life.DB;
using Life.Network;
using Life.UI;
using UnityEngine;
using Mirror;

namespace KiwaïNLWine
{
    public class Main : Plugin
    {
        public Main(IGameAPI api) : base(api)
        {
        }
        public override void OnPluginInit()
        {
            Console.WriteLine("KiwaïNLWine loaded");
        }
        public override void OnPlayerInput(Player player, KeyCode keyCode, bool onUI)
        {
            base.OnPlayerInput(player, keyCode, onUI);
            if (keyCode == KeyCode.Y)
            {
                UIPanel vigneron = new UIPanel("Marché des vignerons", UIPanel.PanelType.TabPrice);
                vigneron.title = "Marché des vignerons";
                vigneron.AddTabLine("Vente de vin", ui =>
                {
                });

                vigneron.AddTabLine("Achat de matériels", ui =>
                {
                });

                vigneron.AddButton("Fermer", ui =>
                {
                    player.ClosePanel(vigneron);
                    player.Notify("Menu", "Vous avez fermé le marché des vignerons", NotificationManager.Type.Success, 5);
                });

                vigneron.AddButton("Choisir", ui =>
                {

                });

                player.ShowPanelUI(vigneron);
            }
        }
        public void vente(Player player)
        {
            UIPanel vente = new UIPanel("Vente de vin", UIPanel.PanelType.TabPrice);
            vente.title = "Vente de vin";

            vente.AddButton("Fermer", ui =>
            {
                player.ClosePanel(vente);
                player.Notify("Menu", "Vous avez fermé le marché des vignerons", NotificationManager.Type.Success, 5);
            });

            vente.AddButton("Prendre", ui =>
            {

            });
        }
    }
}
