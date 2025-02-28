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
using InsaneSystems.RoadNavigator;
using Life.CheckpointSystem;
using Life.InventorySystem;

namespace KiwaïNLWine
{
    public class Main : Plugin
    {
        public Main(IGameAPI api) : base(api)
        {
        }
        public override async void OnPluginInit()
        {
            Console.WriteLine("KiwaïNLWine loaded (made by MediaGamings)");
            var Wine = Nova.man.item.GetItem(88);
            var Machine = Nova.man.item.GetItem(1092);
            var Bouteille = Nova.man.item.GetItem(1571);
            var Grappe = Nova.man.item.GetItem(1093);
            Wine.resellable = false;
            Machine.buyable = false;
            Bouteille.buyable = false;
            Grappe.buyable = false;
            while (true) 
            {
                var random = new System.Random();
                AmboiseNordPrice = random.Next(15, 21);
                ReignerePrice = random.Next(5, 11);
                FuyePrice = random.Next(8, 15);
                AgroalimPrice = random.Next(5, 7);
                NightClubPrice = random.Next(12, 17);
                UFOGrillPrice = random.Next(8, 15);
                MachinePrice = random.Next(30000, 41000);
                BottlePrice = random.Next(1, 5);
                GraapPrice = random.Next(2, 6);
                BoxPrice = random.Next(3, 7);
                PalettePrice = random.Next(25, 36);
                await Task.Delay(TimeSpan.FromHours(1));
                Console.WriteLine("KiwaïNL Price updated");
            }
        }
        public override void OnPlayerInput(Player player, KeyCode keyCode, bool onUI)
        {
            base.OnPlayerInput(player, keyCode, onUI);
            if (keyCode == KeyCode.Y && onUI == false)
            {
                menu(player);
            }
        }

        public void menu(Player player)
        {
            if (player.GetActivity() != Life.BizSystem.Activity.Type.Chef)
            {
                player.Notify("Menu", "Vous ne pouvez pas accédée a ce marché", NotificationManager.Type.Error);
                return;
            }
            UIPanel vigneron = new UIPanel("Marché des vignerons", UIPanel.PanelType.TabPrice);
            vigneron.AddTabLine("Vente de vin", ui =>
            {
                vente(player);
            });

            vigneron.AddTabLine("Achat de matériels", ui =>
            {
                achat(player);
            });

            vigneron.AddButton("Fermer", ui =>
            {
                player.ClosePanel(vigneron);
                player.Notify("Menu", "Vous avez fermé le marché des vignerons", NotificationManager.Type.Success, 5);
            });

            vigneron.AddButton("Choisir", ui =>
            {
                ui.SelectTab();
            });

            player.ShowPanelUI(vigneron);
        }

        public int AmboiseNordPrice = 0;
        public int ReignerePrice = 0;
        public int FuyePrice = 0;
        public int AgroalimPrice = 0;
        public int NightClubPrice = 0;
        public int UFOGrillPrice = 0;

        public void Sell(Player player, int price)
        {
            var panel = new UIPanel("Vente de vin", UIPanel.PanelType.Input);
            panel.SetText("Quel quantité de bouteille de vin souhaitez-vous vendre ?");
            panel.SetInputPlaceholder("Quantité : ");
            panel.AddButton("Vendre", ui =>
            {
                if (int.TryParse(ui.inputText, out int quantity))
                {
                    if (quantity <= 0)
                    {
                        player.Notify("Vente", "Vous ne pouvez pas vendre une quantité négative ou nulle", NotificationManager.Type.Error);
                        return;
                    }
                    if (player.setup.inventory.items[player.setup.inventory.GetItemSlotById(88)].number < quantity)
                    {
                        player.Notify("Vente", "Vous n'avez pas assez de bouteille de vin", NotificationManager.Type.Error);
                        return;
                    }
                    player.setup.inventory.RemoveItem(88, quantity, false);
                    player.AddMoney(quantity * price, "Vente de vin");
                    player.Notify("Vente", "Vous avez vendu " + quantity + " bouteille de vin pour " + quantity * price + "€", NotificationManager.Type.Success);
                    player.ClosePanel(panel);
                }
                else
                {
                    player.Notify("Vente", "Vous devez rentrer un nombre valide", NotificationManager.Type.Error);
                }
            });

            panel.AddButton("Fermer", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Vente", "Vous avez fermé le marché des vignerons", NotificationManager.Type.Success, 5);
            });

            panel.AddButton("Retour", ui =>
            {
                vente(player);
            });

            player.ShowPanelUI(panel);
        }

        public void vente(Player player)
        {
            UIPanel vente = new UIPanel("Vente de vin", UIPanel.PanelType.TabPrice);
            vente.AddTabLine("Station EXO (Amboise Nord)", AmboiseNordPrice.ToString() + "€", -1, ui =>
            {
                var pointPosition = new Vector3(365.1333f, 50.00305f, 779.7359f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(365.1333f, 50.00305f, 779.7359f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Sell(player, AmboiseNordPrice);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            vente.AddTabLine("Station EXO (Reigneire)", ReignerePrice.ToString() + "€", 0, ui =>
            {
                var pointPosition = new Vector3(256.9022f, 44.98566f, -1263.793f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(365.1333f, 50.00305f, 779.7359f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Sell(player, ReignerePrice);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            vente.AddTabLine("Station EXO (La Fuye)", FuyePrice.ToString() + "€", 0, ui =>
            {
                var pointPosition = new Vector3(-360.02414f, 21.94958f, -473.1332f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(365.1333f, 50.00305f, 779.7359f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Sell(player, FuyePrice);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            vente.AddTabLine("AgroAlim", AgroalimPrice.ToString() + "€", 0, ui =>
            {
                var pointPosition = new Vector3(1027.129f, 52.39577f, -720.9929f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(365.1333f, 50.00305f, 779.7359f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Sell(player, AgroalimPrice);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            vente.AddTabLine("Boite de nuit", NightClubPrice.ToString() + "€", 0, ui =>
            {
                var pointPosition = new Vector3(75.1063f, 44.22878f, 566.5424f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(365.1333f, 50.00305f, 779.7359f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Sell(player, NightClubPrice);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            vente.AddTabLine("UFO Grill", UFOGrillPrice.ToString() + "€", 0, ui =>
            {
                var pointPosition = new Vector3(115.7158f, 42.00696f, -679.7314f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(365.1333f, 50.00305f, 779.7359f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Sell(player, UFOGrillPrice);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            vente.AddButton("Fermer", ui =>
            {
                player.ClosePanel(vente);
                player.Notify("Menu", "Vous avez fermé le marché des vignerons", NotificationManager.Type.Success, 5);
            });
            
            vente.AddButton("Retour", ui =>
            {
                menu(player);
            });

            vente.AddButton("Prendre", ui =>
            {
                ui.SelectTab();
            });

            player.ShowPanelUI(vente);
        }

        public void Buy(Player player, int price, int itemID)
        {
            var panel = new UIPanel("Achat de matériels", UIPanel.PanelType.Input);
            panel.SetText("Quel quantité de matériels souhaitez-vous acheter ?");
            panel.SetInputPlaceholder("Quantité : ");
            panel.AddButton("Acheter", ui =>
            {
                if (int.TryParse(ui.inputText, out int quantity))
                {
                    if (quantity <= 0)
                    {
                        player.Notify("Achat", "Vous ne pouvez pas acheter une quantité négative ou nulle", NotificationManager.Type.Error);
                        return;
                    }
                    if (player.character.Money < quantity * price)
                    {
                        player.Notify("Achat", "Vous n'avez pas assez d'argent", NotificationManager.Type.Error);
                        return;
                    }
                    if (player.setup.inventory.CanAddItem(itemID, quantity, "") == false)
                    {
                        player.Notify("Achat", "Vous n'avez pas assez de place dans votre inventaire", NotificationManager.Type.Error);
                        return;
                    }
                    player.AddMoney(-quantity * price, "Achat de matériel");
                    player.setup.inventory.AddItem(itemID, quantity, "");
                    player.Notify("achat", "Vous avez acheté " + quantity + " " + Nova.man.item.GetItem(itemID).itemName + " pour " + quantity * price + "€", NotificationManager.Type.Success);
                    player.ClosePanel(panel);
                }
            });

            panel.AddButton("Fermer", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Achat", "Vous avez fermé le marché des vignerons", NotificationManager.Type.Success, 5);
            });

            panel.AddButton("Retour", ui =>
            {
                achat(player);
            });

            player.ShowPanelUI(panel);
        }

        public int MachinePrice = 0;
        public int BottlePrice = 0;
        public int GraapPrice = 0;
        public int BoxPrice = 0;
        public int PalettePrice = 0;

        public void achat(Player player)
        {
            UIPanel achat = new UIPanel("Achat de matériels", UIPanel.PanelType.TabPrice);
            achat.AddTabLine("Machine de production de vin", ui =>
            {
                var pointPosition = new Vector3(-76.3934f, 34.42593f, -546.1004f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(-76.3934f, 34.42593f, -546.1004f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, MachinePrice, 1092);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddTabLine("Bouteille de vin vide", ui =>
            {
                var pointPosition = new Vector3(1026.894f, 52.39577f, -721.7126f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(1026.894f, 52.39577f, -721.7126f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, BottlePrice, 1571);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddTabLine("Grappe de raisin", ui =>
            {
                var pointPosition = new Vector3(1026.894f, 52.39577f, -721.7126f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(1026.894f, 52.39577f, -721.7126f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, GraapPrice, 1093);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddTabLine("Carton", ui =>
            {
                var pointPosition = new Vector3(439.3797f, 50.00305f, 929.9147f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(439.3797f, 50.00305f, 929.9147f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, BoxPrice, 1231);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddTabLine("Palette", ui =>
            {
                var pointPosition = new Vector3(439.3797f, 50.00305f, 929.9147f);
                player.setup.TargetSetGPSTarget(pointPosition);
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(439.3797f, 50.00305f, 929.9147f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, PalettePrice, 1001);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddButton("Fermer", ui =>
            {
                player.ClosePanel(achat);
                player.Notify("Menu", "Vous avez fermé le marché des vignerons", NotificationManager.Type.Success, 5);
            });

            achat.AddButton("Retour", ui =>
            {
                menu(player);
            });

            achat.AddButton("Prendre", ui =>
            {
                ui.SelectTab();
            });

            player.ShowPanelUI(achat);
        }
    }
}