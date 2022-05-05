using System;
using System.Collections.Generic;
using System.Linq;
using SDG.Unturned;
using Rocket.Core.Plugins;
using Steamworks;
using UnityEngine;
using System.Collections;
using Rocket.API.Collections;
using Rocket.Unturned.Chat;

namespace DeathBox
{
    public class Main : RocketPlugin<Configuration>
    {
        public static bool DebugMode { get; set; } = false;
        private static EItemType[] PriorityItemTypes => new EItemType[5]
        {
            EItemType.SHIRT, EItemType.PANTS, EItemType.BACKPACK, EItemType.VEST, EItemType.GUN
        };
        public Dictionary<Transform, Coroutine> CooldownManager { get; set; }
        protected override void Load()
        {
            CooldownManager = new Dictionary<Transform, Coroutine>();
            BarricadeManager.onDamageBarricadeRequested += new DamageBarricadeRequestHandler(DamageBarricadeRequestHandler);
            PlayerEquipment.OnPunch_Global += new Action<PlayerEquipment, EPlayerPunch>(OnPunch_Global);
            PlayerLife.OnPreDeath += PlayerLife_OnPreDeath;

            if (Level.isLoaded)
                LevelLoaded(1);
            else
                Level.onLevelLoaded += new LevelLoaded(LevelLoaded);
        }
        protected override void Unload()
        {
            Level.onLevelLoaded -= new LevelLoaded(LevelLoaded);
            BarricadeManager.onDamageBarricadeRequested -= new DamageBarricadeRequestHandler(DamageBarricadeRequestHandler);
            PlayerEquipment.OnPunch_Global -= new Action<PlayerEquipment, EPlayerPunch>(OnPunch_Global);
            PlayerLife.OnPreDeath -= PlayerLife_OnPreDeath;

            foreach (var Par in CooldownManager)
                StopCoroutine(Par.Value);
        }

        private void LevelLoaded(int __)
        {
            foreach (BarricadeRegion BR in BarricadeManager.BarricadeRegions)
                BR.drops.FindAll(X => X.asset.id == Configuration.Instance.DeathBoxID).ForEach(X => CooldownManager.Add(X.model, StartCoroutine(DeathBoxCoroutine(X.model, Configuration.Instance.DisappearCooldownAfterShutdown))));
        }
        private void PlayerLife_OnPreDeath(PlayerLife Player)
        {
            List<ItemJar> Items = new List<ItemJar>();
            #region GetItems
            for (byte Page = 0; Page < PlayerInventory.PAGES - 2; Page++)
            {
                if (Player.player.inventory.items[Page] == null)
                    continue;

                while (Player.player.inventory.getItemCount(Page) != 0)
                {
                    Items.Add(Player.player.inventory.items[Page].items[0]);
                    Player.player.inventory.items[Page].removeItem(0);
                }
            }
            if (Player.player.equipment.itemID != 0)
            {
                Items.Add(new ItemJar(new Item(Player.player.equipment.itemID, 1, Player.player.equipment.quality, Player.player.equipment.state)));
            }
            if (Player.player.clothing.backpack != 0)
            {
                Items.Add(new ItemJar(new Item(Player.player.clothing.backpack, 1, Player.player.clothing.backpackQuality, Player.player.clothing.backpackState)));
                Player.player.clothing.thirdClothes.backpack = 0;
                Player.player.clothing.askWearBackpack(0, 0, new byte[0], true);
            }
            if (Player.player.clothing.vest != 0)
            {
                Items.Add(new ItemJar(new Item(Player.player.clothing.vest, 1, Player.player.clothing.vestQuality, Player.player.clothing.vestState)));
                Player.player.clothing.thirdClothes.vest = 0;
                Player.player.clothing.askWearVest(0, 0, new byte[0], true);
            }
            if (Player.player.clothing.shirt != 0)
            {
                Items.Add(new ItemJar(new Item(Player.player.clothing.shirt, 1, Player.player.clothing.shirtQuality, Player.player.clothing.shirtState)));
                Player.player.clothing.thirdClothes.shirt = 0;
                Player.player.clothing.askWearShirt(0, 0, new byte[0], true);
            }
            if (Player.player.clothing.pants != 0)
            {
                Items.Add(new ItemJar(new Item(Player.player.clothing.pants, 1, Player.player.clothing.pantsQuality, Player.player.clothing.pantsState)));
                Player.player.clothing.thirdClothes.pants = 0;
                Player.player.clothing.askWearPants(0, 0, new byte[0], true);
            }
            if (Player.player.clothing.hat != 0)
            {
                Items.Add(new ItemJar(new Item(Player.player.clothing.hat, 1, Player.player.clothing.hatQuality, Player.player.clothing.hatState)));
                Player.player.clothing.thirdClothes.hat = 0;
                Player.player.clothing.askWearHat(0, 0, new byte[0], true);
            }
            if (Player.player.clothing.glasses != 0)
            {
                Items.Add(new ItemJar(new Item(Player.player.clothing.glasses, 1, Player.player.clothing.glassesQuality, Player.player.clothing.glassesState)));
                Player.player.clothing.thirdClothes.glasses = 0;
                Player.player.clothing.askWearGlasses(0, 0, new byte[0], true);
            }
            if (Player.player.clothing.mask != 0)
            {
                Items.Add(new ItemJar(new Item(Player.player.clothing.mask, 1, Player.player.clothing.maskQuality, Player.player.clothing.maskState)));
                Player.player.clothing.thirdClothes.mask = 0;
                Player.player.clothing.askWearMask(0, 0, new byte[0], true);
            }
            #endregion
            if (Items.Count(X => X != null) == 0)
                return;

            Transform TransformBarricade = BarricadeManager.dropNonPlantedBarricade(new Barricade(Assets.find(EAssetType.ITEM, Configuration.Instance.DeathBoxID) as ItemBarricadeAsset), Player.player.transform.position, Quaternion.LookRotation(LevelGround.getNormal(Player.player.transform.position)), 0, 0);
            InteractableStorage Storage = BarricadeManager.FindBarricadeByRootTransform(TransformBarricade).interactable as InteractableStorage;
            Storage.items.resize(Configuration.Instance.InitialDeathBoxHSize, 0);

            foreach (ItemJar ItemJar in Items)
            {
                byte rot;
                byte y;
                byte x;
                while (!Storage.items.tryFindSpace(ItemJar.size_x, ItemJar.size_y, out x, out y, out rot))
                    Storage.items.resize(Storage.items.width, (byte)(Storage.items.height + 1));

                Storage.items.addItem(x, y, rot, ItemJar.item);
            }
            CooldownManager.Add(TransformBarricade, StartCoroutine(DeathBoxCoroutine(TransformBarricade, Configuration.Instance.NormalDisappearCooldown)));
        }
        private void OnPunch_Global(PlayerEquipment Player, EPlayerPunch Type)
        {
            BarricadeDrop Drop = BarricadeManager.FindBarricadeByRootTransform(DamageTool.raycast(new Ray(Player.player.look.aim.position, Player.player.look.aim.forward), 3, RayMasks.BARRICADE | RayMasks.BARRICADE_INTERACT, Player.player).transform);
            if (!this.Configuration.Instance.PunchUtil || Drop == null || !CooldownManager.ContainsKey(Drop.model))
                return;

            Items Items = (Drop.interactable as InteractableStorage).items;
            foreach (ItemJar Item in Items.items.OrderByDescending(X => PriorityItemTypes.Contains((Assets.find(EAssetType.ITEM, X.item.id) as ItemAsset).type)))
            {
                if (DebugMode)
                    Rocket.Core.Logging.Logger.Log("Item Added: " + Assets.find(EAssetType.ITEM, Item.item.id).name);

                if (!Player.player.inventory.tryAddItemAuto(Item.item, true, true, true, false))
                {
                    if (Configuration.Instance.PunchUtil_DropWhenItemsDoesntFit)
                        ItemManager.dropItem(Item.item, Player.transform.position, false, true, true);
                    else
                        continue;
                }

                Items.removeItem(Items.getIndex(Item.x, Item.y));
            }

            if (Items.getItemCount() == 0)
            {
                BarricadeManager.tryGetRegion(Drop.model, out byte x, out byte y, out ushort plant, out BarricadeRegion Region);
                BarricadeManager.destroyBarricade(Drop, x, y, plant);
                StopCoroutine(CooldownManager[Drop.model]);
                CooldownManager.Remove(Drop.model);
            }
        }
        private void DamageBarricadeRequestHandler(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (damageOrigin != EDamageOrigin.Unknown && CooldownManager.ContainsKey(barricadeTransform))
                shouldAllow = Configuration.Instance.CanDamageDeathBox;

            if (shouldAllow && BarricadeManager.FindBarricadeByRootTransform(barricadeTransform).GetServersideData().barricade.health - pendingTotalDamage <= 0)
            {
                StopCoroutine(CooldownManager[barricadeTransform]);
                CooldownManager.Remove(barricadeTransform);
            }
        }
        private IEnumerator DeathBoxCoroutine(Transform Barricade, int Timer)
        {
            yield return new WaitForSeconds(Timer);
            BarricadeManager.tryGetRegion(Barricade, out byte x, out byte y, out ushort plant, out BarricadeRegion Region);
            BarricadeManager.destroyBarricade(Region.drops[Region.IndexOfBarricadeByRootTransform(Barricade)], x, y, plant);
            CooldownManager.Remove(Barricade);
            
            if (DebugMode)
                Rocket.Core.Logging.Logger.Log("Coroutine Exited");
        }
    }
}
