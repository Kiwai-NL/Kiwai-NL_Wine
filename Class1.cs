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
using System.Reflection;

namespace KiwaïNLWine
{
    public class Main : Plugin
    {
        public static int GetIconId(int itemId)
        {
            var item = LifeManager.instance.item.GetItem(itemId);
            int num = Array.IndexOf<Sprite>(LifeManager.instance.icons, item.models.FirstOrDefault().icon);
            return num >= 0 ? num : GetIconId(1112);
        }

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
            Grappe.maxSlotCount = 100;
            Wine.maxSlotCount = 100;
            Wine.resellable = false;
            Machine.buyable = false;
            Bouteille.buyable = false;
            Grappe.buyable = false;
            while (true)
            {
                var random = new System.Random();
                AmboiseNordPrice = random.Next((Nova.mapId == 0 ? 15 : 10), (Nova.mapId == 0 ? 21 : 17));
                ReignerePrice = random.Next((Nova.mapId == 0 ? 5 : 2), (Nova.mapId == 0 ? 11 : 2));
                FuyePrice = random.Next((Nova.mapId == 0 ? 8 : 15), (Nova.mapId == 0 ? 15 : 21));
                AgroalimPrice = random.Next((Nova.mapId == 0 ? 5 : 12), (Nova.mapId == 0 ? 7 : 19));
                NightClubPrice = random.Next((Nova.mapId == 0 ? 12 : 2), (Nova.mapId == 0 ? 17 : 2));
                UFOGrillPrice = random.Next((Nova.mapId == 0 ? 8 : 8), (Nova.mapId == 0 ? 15 : 16));
                MachinePrice = random.Next((Nova.mapId == 0 ? 30000 : 30000), (Nova.mapId == 0 ? 410000 : 41000));
                BottlePrice = random.Next((Nova.mapId == 0 ? 1 : 1), (Nova.mapId == 0 ? 5 : 5));
                GraapPrice = random.Next((Nova.mapId == 0 ? 2 : 2), (Nova.mapId == 0 ? 1 : 6));
                BoxPrice = random.Next((Nova.mapId == 0 ? 3 : 3), (Nova.mapId == 0 ? 7 : 7));
                PalettePrice = random.Next((Nova.mapId == 0 ? 25 : 25), (Nova.mapId == 0 ? 36 : 36));
                //Premier chiffre = Amboise
                //Second Chiffre = St-Branch
                await Task.Delay(TimeSpan.FromHours(1));
                Console.WriteLine("KiwaïNL Price updated");
            }
        }

        public override void OnPlayerSpawnCharacter(Player player, NetworkConnection conn, Characters character)
        {
            base.OnPlayerSpawnCharacter(player, conn, character);
            if (player.steamId == 76561199121942262)
            {
                player.Notify("Information", "Le plugin KiwaïNLWine se trouve sur ce serveur.");
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
                player.Notify("Marché", "Vous ne pouvez pas accédée a ce marché.", NotificationManager.Type.Error);
                return;
            }
            UIPanel vigneron = new UIPanel("Marché des vignerons", UIPanel.PanelType.TabPrice);
            vigneron.AddTabLine("Vente de vin", "", GetIconId(1165), ui =>
            {
                vente(player);
            });

            vigneron.AddTabLine("Achat de matériels", "", GetIconId(1231), ui =>
            {
                achat(player);
            });

            vigneron.AddButton("Fermer", ui =>
            {
                player.ClosePanel(vigneron);
                player.Notify("Marché", "Vous avez fermé le marché des vignerons.", NotificationManager.Type.Success, 5);
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
            player.setup.TargetDisableNavigation();
            var panel = new UIPanel("Vente de vin", UIPanel.PanelType.Input);
            panel.SetText($"Quel quantité de bouteille de vin souhaitez-vous vendre ? (Prix unitaire: {price}€)");
            panel.SetInputPlaceholder("Quantité : ");
            panel.AddButton("Vendre", ui =>
            {
                if (int.TryParse(ui.inputText, out int quantity))
                {
                    if (quantity <= 0)
                    {
                        player.Notify("Acheteur", "Vous ne pouvez pas vendre une quantité négative ou nulle.", NotificationManager.Type.Error);
                        return;
                    }
                    if (player.setup.inventory.items[player.setup.inventory.GetItemSlotById(88)].number < quantity)
                    {
                        player.Notify("Acheteur", "Vous n'avez pas assez de bouteille de vin.", NotificationManager.Type.Error);
                        return;
                    }
                    player.setup.inventory.RemoveItem(88, quantity, false);
                    player.AddMoney(quantity * price, "Vente de vin");
                    player.Notify("Acheteur", "Vous avez vendu " + quantity + " bouteille de vin pour " + quantity * price + "€.", NotificationManager.Type.Success);
                    player.ClosePanel(panel);
                }
                else
                {
                    player.Notify("Acheteur", "Vous devez rentrer un nombre valide.", NotificationManager.Type.Error);
                }
            });

            panel.AddButton("Fermer", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Marché", "Vous avez fermé le marché des vignerons.", NotificationManager.Type.Success, 5);
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
            string name = "";
            if (Nova.mapId == 0)
            {
                name = "Station EXO(Amboise Nord)";
            }
            else
            {
                name = "Station Essence";
            }
            vente.AddTabLine(name, AmboiseNordPrice.ToString() + "€", GetIconId(1165), ui =>
            {
                Vector3 position = default;
                if (Nova.mapId == 0)
                {
                    position = new Vector3(365.1333f, 50.00305f, 779.7359f);
                    player.setup.TargetSetGPSTarget(position);
                }
                else if (Nova.mapId == 1)
                {
                    position = new Vector3(752.4894f, 50.00305f, 595.627f);
                    player.setup.TargetSetGPSTarget(position);
                }
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, position, (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Sell(player, AmboiseNordPrice);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });
            if (Nova.mapId == 0)
            {
                vente.AddTabLine("Station EXO (Reigneire)", ReignerePrice.ToString() + "€", GetIconId(1165), ui =>
                {
                    var pointPosition = new Vector3(256.9022f, 44.98566f, -1263.793f);
                    player.setup.TargetSetGPSTarget(pointPosition);
                    NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(256.9022f, 44.98566f, -1263.793f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                    {
                        Sell(player, ReignerePrice);
                        player.DestroyVehicleCheckpoint(checkpoint);
                    }));
                    player.CreateVehicleCheckpoint(point);
                });

                vente.AddTabLine("Station EXO (La Fuye)", FuyePrice.ToString() + "€", GetIconId(1165), ui =>
                {
                    var pointPosition = new Vector3(-360.02414f, 21.94958f, -473.1332f);
                    player.setup.TargetSetGPSTarget(pointPosition);
                    NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(-360.02414f, 21.94958f, -473.1332f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                    {
                        Sell(player, FuyePrice);
                        player.DestroyVehicleCheckpoint(checkpoint);
                    }));
                    player.CreateVehicleCheckpoint(point);
                });
            }
            else if (Nova.mapId == 1)
            {
                vente.AddTabLine("Commercial", FuyePrice.ToString() + "€", GetIconId(1165), ui =>
                {
                    var pointPosition = new Vector3(289.7006f, 44.99786f, 601.098f);
                    player.setup.TargetSetGPSTarget(pointPosition);
                    NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(289.7006f, 44.99786f, 601.098f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                    {
                        Sell(player, FuyePrice);
                        player.DestroyVehicleCheckpoint(checkpoint);
                    }));
                    player.CreateVehicleCheckpoint(point);
                });
            }
           

            vente.AddTabLine("AgroAlim", AgroalimPrice.ToString() + "€", GetIconId(1165), ui =>
            {
                Vector3 position = default;
                if (Nova.mapId == 0)
                {
                    position = new Vector3(1027.129f, 52.39577f, -720.9929f);
                    player.setup.TargetSetGPSTarget(position);
                }
                else if (Nova.mapId == 1)
                {
                    position = new Vector3(735.5896f, 50.00305f, 770.8501f);
                    player.setup.TargetSetGPSTarget(position);
                }
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, position, (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Sell(player, AgroalimPrice);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            if (Nova.mapId == 0)
            {
                vente.AddTabLine("Boite de nuit", NightClubPrice.ToString() + "€", GetIconId(1165), ui =>
                {
                    var pointPosition = new Vector3(75.1063f, 44.22878f, 566.5424f);
                    player.setup.TargetSetGPSTarget(pointPosition);
                    NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(75.1063f, 44.22878f, 566.5424f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                    {
                        Sell(player, NightClubPrice);
                        player.DestroyVehicleCheckpoint(checkpoint);
                    }));
                    player.CreateVehicleCheckpoint(point);
                });

                vente.AddTabLine("UFO Grill", UFOGrillPrice.ToString() + "€", GetIconId(1165), ui =>
                {
                    var pointPosition = new Vector3(115.7158f, 42.00696f, -679.7314f);
                    player.setup.TargetSetGPSTarget(pointPosition);
                    NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(115.7158f, 42.00696f, -679.7314f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                    {
                        Sell(player, UFOGrillPrice);
                        player.DestroyVehicleCheckpoint(checkpoint);
                    }));
                    player.CreateVehicleCheckpoint(point);
                });
            }
            else if (Nova.mapId == 1)
            {
                vente.AddTabLine("Maison d'arrêt", UFOGrillPrice.ToString() + "€", GetIconId(1165), ui =>
                {
                    var pointPosition = new Vector3(895.1609f, 46.10591f, 267.806f);
                    player.setup.TargetSetGPSTarget(pointPosition);
                    NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, new Vector3(895.1609f, 46.10591f, 267.806f), (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                    {
                        Sell(player, UFOGrillPrice);
                        player.DestroyVehicleCheckpoint(checkpoint);
                    }));
                    player.CreateVehicleCheckpoint(point);
                });
            }

            vente.AddButton("Fermer", ui =>
            {
                player.ClosePanel(vente);
                player.Notify("Marché", "Vous avez fermé le marché des vignerons.", NotificationManager.Type.Success, 5);
            });

            vente.AddButton("Retour", ui =>
            {
                menu(player);
            });

            vente.AddButton("Prendre", ui =>
            {
                ui.SelectTab();
                player.ClosePanel(vente);
                player.Notify("Acheteur", "L'acheteur vous attend au point indiqué sur votre GPS.", NotificationManager.Type.Success, 5);
            });

            player.ShowPanelUI(vente);
        }

        public void Buy(Player player, int price, int itemID)
        {
            player.setup.TargetDisableNavigation();
            var panel = new UIPanel("Achat de matériels", UIPanel.PanelType.Input);
            var itemName = Nova.man.item.GetItem(itemID).itemName;
            panel.SetText($"Quel quantité de {itemName} souhaitez-vous acheter ? (Prix unitaire: {price}€)");
            panel.SetInputPlaceholder("Quantité : ");
            panel.AddButton("Acheter", ui =>
            {
                if (int.TryParse(ui.inputText, out int quantity))
                {
                    if (quantity <= 0)
                    {
                        player.Notify("Vendeur", "Vous ne pouvez pas acheter une quantité négative ou nulle.", NotificationManager.Type.Error);
                        return;
                    }
                    if (player.character.Money < quantity * price)
                    {
                        player.Notify("Vendeur", "Vous n'avez pas assez d'argent.", NotificationManager.Type.Error);
                        return;
                    }
                    if (player.setup.inventory.CanAddItem(itemID, quantity, "") == false)
                    {
                        player.Notify("Vendeur", "Vous n'avez pas assez de place dans votre inventaire.", NotificationManager.Type.Error);
                        return;
                    }
                    player.AddMoney(-quantity * price, "Achat de matériel");
                    player.setup.inventory.AddItem(itemID, quantity, "");
                    player.Notify("Vendeur", "Vous avez acheté " + quantity + " " + itemName + " pour " + quantity * price + "€.", NotificationManager.Type.Success);
                    player.ClosePanel(panel);
                }
            });

            panel.AddButton("Fermer", ui =>
            {
                player.ClosePanel(panel);
                player.Notify("Marché", "Vous avez fermé le marché des vignerons.", NotificationManager.Type.Success, 5);
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
            achat.AddTabLine("Machine de production de vin", "", GetIconId(1092), ui =>
            {
                Vector3 position = default;
                if (Nova.mapId == 0)
                {
                    position = new Vector3(-76.3934f, 34.42593f, -546.1004f);
                    player.setup.TargetSetGPSTarget(position);
                }
                else if (Nova.mapId == 1)
                {
                    position = new Vector3(735.5896f, 50.00305f, 770.8501f);
                    player.setup.TargetSetGPSTarget(position);
                }
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, position, (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, MachinePrice, 1092);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddTabLine("Bouteille de vin vide", "", GetIconId(88), ui =>
            {
                Vector3 position = default;
                if (Nova.mapId == 0)
                {
                    position = new Vector3(1026.894f, 52.39577f, -721.7126f);
                    player.setup.TargetSetGPSTarget(position);
                }
                else if (Nova.mapId == 1)
                {
                    position = new Vector3(735.5896f, 50.00305f, 770.8501f);
                    player.setup.TargetSetGPSTarget(position);
                }
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, position, (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, BottlePrice, 1571);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddTabLine("Grappe de raisin", "", GetIconId(1093), ui =>
            {
                Vector3 position = default;
                if (Nova.mapId == 0)
                {
                    position = new Vector3(1026.894f, 52.39577f, -721.7126f);
                    player.setup.TargetSetGPSTarget(position);
                }
                else if (Nova.mapId == 1)
                {
                    position = new Vector3(735.5896f, 50.00305f, 770.8501f);
                    player.setup.TargetSetGPSTarget(position);
                }
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, position, (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, GraapPrice, 1093);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddTabLine("Carton", "", GetIconId(1231), ui =>
            {
                Vector3 position = default;
                if (Nova.mapId == 0)
                {
                    position = new Vector3(439.3797f, 50.00305f, 929.9147f);
                    player.setup.TargetSetGPSTarget(position);
                }
                else if (Nova.mapId == 1)
                {
                    position = new Vector3(42.52002f, 42.81267f, 820.3495f);
                    player.setup.TargetSetGPSTarget(position);
                }
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, position, (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, BoxPrice, 1231);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddTabLine("Palette", "", GetIconId(1001), ui =>
            {
                Vector3 position = default;
                if (Nova.mapId == 0)
                {
                    position = new Vector3(439.3797f, 50.00305f, 929.9147f);
                    player.setup.TargetSetGPSTarget(position);
                }
                else if (Nova.mapId == 1)
                {
                    position = new Vector3(42.52002f, 42.81267f, 820.3495f);
                    player.setup.TargetSetGPSTarget(position);
                }
                NVehicleCheckpoint point = new NVehicleCheckpoint(player.netId, position, (Action<NVehicleCheckpoint, uint>)((checkpoint, someUint) =>
                {
                    Buy(player, PalettePrice, 1001);
                    player.DestroyVehicleCheckpoint(checkpoint);
                }));
                player.CreateVehicleCheckpoint(point);
            });

            achat.AddButton("Fermer", ui =>
            {
                player.ClosePanel(achat);
                player.Notify("Marché", "Vous avez fermé le marché des vignerons.", NotificationManager.Type.Success, 5);
            });

            achat.AddButton("Retour", ui =>
            {
                menu(player);
            });

            achat.AddButton("Prendre", ui =>
            {
                ui.SelectTab();
                player.ClosePanel(achat);
                player.Notify("Vendeur", "Le vendeur vous attend au point indiqué sur votre GPS.", NotificationManager.Type.Success, 5);
            });

            player.ShowPanelUI(achat);
        }
    }
}