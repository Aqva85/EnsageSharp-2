using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using System;
using SharpDX;


namespace PerfectOverlay.scripts
{
    class OverlayMenu
    {
        private static Hero me;

        private static Player player;

        private static float boxSizeX;

        private static IEnumerable<Ability> Herospells;

        private static IEnumerable<Item> DangerItems;

        private static List<Hero> heroes;

        private static float boxSizeY;

        private static Vector2 TOP_HEALTH_BAR_POS, TOP_MANA_BAR_POS, TOP_HEALTH_BAR_SIZE, TOP_MANA_BAR_SIZE, TOP_BAR_FULLSIZE, TOP_ULT_BAR_SIZE, TOP_ULT_BAR_POS;

        static float ScaleX, ScaleY;

        private static float i, i2, i3;

        public static void start(/*bool topmenu*/)
        {
            me = ObjectMgr.LocalHero;
            if (me == null && !Game.IsInGame) return;
            GetHPbarSize();
            ScaleX = (float)Drawing.Width / 1400;
            ScaleY = (float)Drawing.Height / 900;
            heroes = ObjectMgr.GetEntities<Hero>().Where(x => x.IsAlive && !x.IsIllusion && x.IsVisible && x.IsValid).ToList();
            try
            {
                try
                {
                    //for (uint t = 0; t < 10; t++)
                    //{
                    //    player = ObjectMgr.GetPlayerById(t);
                    //    if (player != null && player.Team != me.Team)
                    //    {
                    //        Hero x = player.Hero;
                    //        if (x != null && x.IsValid && x.Health != 0)
                    //        {
                    //            if (topmenu)
                    //            {
                    //                TOP_HEALTH_BAR_POS = new Vector2(HUDInfo.GetTopPanelPosition(x).X, (float)HUDInfo.GetTopPanelSizeY(x) + (1 * ScaleY));
                    //                TOP_HEALTH_BAR_SIZE = new Vector2(((float)HUDInfo.GetTopPanelSizeX(x) / (x.MaximumHealth / (float)x.Health)) - (2 * ScaleX), boxSizeY - (13 * ScaleY));
                    //                TOP_MANA_BAR_POS = new Vector2(HUDInfo.GetTopPanelPosition(x).X, (float)HUDInfo.GetTopPanelSizeY(x) + 6);
                    //                TOP_MANA_BAR_SIZE = new Vector2(((float)HUDInfo.GetTopPanelSizeX(x) / (x.MaximumMana / x.Mana)) - (2 * ScaleX), boxSizeY - (13 * ScaleY));
                    //                TOP_BAR_FULLSIZE = new Vector2((float)HUDInfo.GetTopPanelSizeX(x) - (2 * ScaleX), boxSizeY - (13 * ScaleY));
                    //                TOP_ULT_BAR_POS = new Vector2(HUDInfo.GetTopPanelPosition(x).X, (float)HUDInfo.GetTopPanelSizeY(x) + (12 * ScaleY));
                    //                TOP_ULT_BAR_SIZE = new Vector2(((float)HUDInfo.GetTopPanelSizeX(x) / (x.Spellbook.SpellR.CooldownLength / x.Spellbook.SpellR.Cooldown)) - (2 * ScaleX), boxSizeY - (13 * ScaleY));
                    //                if (MouseOnSpot(TOP_HEALTH_BAR_POS, new Vector2((float)HUDInfo.GetTopPanelSizeX(x), 100 * ScaleY)))
                    //                    Drawing.DrawText("HP: " + (int)x.Health + " / MANA: " + (int)x.Mana + " / ULT: " + (int)x.Spellbook.SpellR.Cooldown, TOP_HEALTH_BAR_POS + new Vector2(-5 * ScaleX, 17 * ScaleY), new Vector2(18 * ScaleX, 15 * ScaleY), Color.White, FontFlags.AntiAlias | FontFlags.StrikeOut | FontFlags.Additive);
                    //                Drawing.DrawRect(TOP_HEALTH_BAR_POS, TOP_HEALTH_BAR_SIZE, Color.Green, false);
                    //                Drawing.DrawRect(TOP_HEALTH_BAR_POS, TOP_BAR_FULLSIZE, Color.Black, true);
                    //                Drawing.DrawRect(TOP_MANA_BAR_POS, TOP_MANA_BAR_SIZE, new Color(0x00, 0x80, 0xF8, 0xFF), false);
                    //                Drawing.DrawRect(TOP_MANA_BAR_POS, TOP_BAR_FULLSIZE, Color.Black, true);
                    //                if (TOP_ULT_BAR_SIZE.X > 0)
                    //                {
                    //                    Drawing.DrawRect(TOP_ULT_BAR_POS, TOP_ULT_BAR_SIZE, Color.Gold, false);
                    //                    Drawing.DrawRect(TOP_ULT_BAR_POS, TOP_BAR_FULLSIZE, Color.Black, true);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    foreach (Hero x in heroes)
                    {
                        if (x == null) continue;
                        if (x == me) continue;
                        Herospells = x.Spellbook.Spells;
                        DangerItems = x.Inventory.Items;
                        i = 0;
                        i2 = 0;
                        i3 = 0;
                        if (x.Mana > 0 && x.Team != me.Team && !x.UnitState.HasFlag(UnitState.NoHealthbar))
                        {
                            Drawing.DrawRect(new Vector2((float)(HUDInfo.GetHPbarPosition(x).X - (0.5 * ScaleX)), HUDInfo.GetHPbarPosition(x).Y + (9 * ScaleY)), new Vector2((boxSizeX * 6) + (2 * ScaleX), boxSizeY - (13 * ScaleY)), new Color(0x00, 0x80, 0xF8, 0x32), false);
                            Drawing.DrawRect(new Vector2((float)(HUDInfo.GetHPbarPosition(x).X - (0.5 * ScaleX)), HUDInfo.GetHPbarPosition(x).Y + (9 * ScaleY)), new Vector2((boxSizeX * 6) + (2 * ScaleX), boxSizeY - (13 * ScaleY)), Color.Black, true);
                            Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + (1 * ScaleX), HUDInfo.GetHPbarPosition(x).Y + (10 * ScaleY)), new Vector2(((boxSizeX * 6) / (x.MaximumMana / x.Mana)) - (1 * ScaleX), boxSizeY - (15 * ScaleY)), new Color(0x00, 0x80, 0xF8, 0xFF), false);
                        }
                        foreach (Ability y in Herospells)
                        {
                            if (y != null && !y.Name.Contains("empty") && (x.Name.Contains("rubick") ? !y.Name.Contains("hidden") : true) && !x.UnitState.HasFlag(UnitState.NoHealthbar))
                            {
                                if (y.AbilitySlot == AbilitySlot.Slot_1ULL || y.AbilitySlot == AbilitySlot.Slot_2 || y.AbilitySlot == AbilitySlot.Slot_3 || y.AbilitySlot == AbilitySlot.Slot_4 || y.AbilitySlot == AbilitySlot.Slot_5 || y.AbilitySlot == AbilitySlot.Slot_6)
                                {
                                    Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y - (25 * ScaleY)), new Vector2(boxSizeX, boxSizeY), Drawing.GetTexture("materials/ensage_ui/spellicons/" + y.Name + ".vmat"));
                                    Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y - (25 * ScaleY)), new Vector2(boxSizeX, boxSizeY), Color.Black, true);
                                    if (y.Cooldown > 0 || (y.ManaCost > x.Mana && y.ManaCost > 0))
                                    {
                                        Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y - (25 * ScaleY)), new Vector2(boxSizeX, boxSizeY), new Color(0x5F, 0x00, 0xCE, 0x9E), false);
                                        Drawing.DrawText((int)(y.Cooldown == 0 ? (y.ManaCost - x.Mana) : y.Cooldown) + "", new Vector2((float)(HUDInfo.GetHPbarPosition(x).X + (0.8 + (i * 20) * ScaleX)), HUDInfo.GetHPbarPosition(x).Y - (17 * ScaleY)), new Vector2(10 * ScaleX, 20 * ScaleY), y.Cooldown == 0 ? Color.LightSkyBlue : Color.White, FontFlags.AntiAlias | FontFlags.StrikeOut | FontFlags.Additive);
                                    }
                                    if (y.Level <= 0)
                                        Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y - (25 * ScaleY)), new Vector2(boxSizeX, boxSizeY), new Color(0x5F, 0x00, 0x00, 0x9E), false);
                                    else
                                    {
                                        Drawing.DrawRect(new Vector2((HUDInfo.GetHPbarPosition(x).X + ((i * 20) * ScaleX)), HUDInfo.GetHPbarPosition(x).Y - (25 * ScaleY)), new Vector2(boxSizeX - ((y.Level < 10 ? 8 : 5) * ScaleX), boxSizeY - (9 * ScaleY)), Color.Black, false);
                                        Drawing.DrawText((int)y.Level + "", new Vector2((float)(HUDInfo.GetHPbarPosition(x).X + (((y.Level < 10 ? 0.8 : 0) + (i * 20)) * ScaleX)), HUDInfo.GetHPbarPosition(x).Y - (25 * ScaleY)), new Vector2(10 * ScaleX, 20 * ScaleY), Color.YellowGreen, FontFlags.AntiAlias | FontFlags.StrikeOut | FontFlags.Additive);
                                    }
                                    i += 1 * ScaleX;
                                }
                                else
                                {
                                    if (y.Cooldown != 0 || y.ManaCost > x.Mana)
                                    {
                                        Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i2 * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y - (50 * ScaleY)), new Vector2(boxSizeX, boxSizeY), Drawing.GetTexture("materials/ensage_ui/spellicons/" + y.Name + ".vmat"));
                                        Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i2 * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y - (50 * ScaleY)), new Vector2(boxSizeX, boxSizeY), Color.Black, true);
                                        if (y.Cooldown > 0 || (y.ManaCost > x.Mana && y.ManaCost > 0))
                                        {
                                            Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i2 * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y - (50 * ScaleY)), new Vector2(boxSizeX, boxSizeY), new Color(0x5F, 0x00, 0xCE, 0x9E), false);
                                            Drawing.DrawText((int)(y.Cooldown == 0 ? (y.ManaCost - x.Mana) : y.Cooldown) + "", new Vector2((float)(HUDInfo.GetHPbarPosition(x).X + ((0.8 + (i2 * 20)) * ScaleX)), HUDInfo.GetHPbarPosition(x).Y - (42 * ScaleY)), new Vector2(10 * ScaleX, 20 * ScaleY), y.Cooldown == 0 ? Color.LightSkyBlue : Color.White, FontFlags.AntiAlias | FontFlags.StrikeOut | FontFlags.Additive);
                                        }
                                        i2 += 1 * ScaleX;
                                    }
                                }
                            }
                        }
                        foreach (Item z in DangerItems)
                        {
                            if (z != null && !x.UnitState.HasFlag(UnitState.NoHealthbar) && (z.Name == "item_gem" || z.Name == "item_dust" || z.Name == "item_sphere" || z.Name == "item_blink" || z.Name == "item_ward_observer" || z.Name == "item_ward_sentry" || z.Name == "item_black_king_bar" || z.Name == "item_ward_dispenser" || z.Name == "item_sheepstick" || z.Name == "item_blade_mail" || z.Name == "item_rapier" || z.Name == "item_cyclone" || z.Name == "item_shadow_amulet" || z.Name == "item_invis_sword" || z.Name == "item_silver_edge" || z.Name == "item_glimmer_cape" || z.Name == "item_lotus_orb" || z.Name == "item_orchid" || z.Name.Contains("item_dagon") || z.Name == "item_manta" || z.Name == "item_aegis" || z.Name == "item_cheese" || z.Name == "item_faerie_fire" || z.Name == "item_ghost" || z.Name == "item_infused_raindrop" || z.Name == "item_magic_stick" || z.Name == "item_smoke_of_deceit" || z.Name == "item_magic_wand" || z.Name == "item_ultimate_scepter" || z.Name == "item_bloodthorn" || z.Name == "item_bloodstone" || z.Name == "item_ethereal_blade" || z.Name == "item_echo_sabre" || z.Name == "item_guardian_greaves" || z.Name == "item_force_staff" || z.Name == "item_hand_of_midas" || z.Name == "item_heavens_halberd" || z.Name == "item_hurricane_pike" || z.Name == "item_mask_of_madness" || z.Name == "item_abyssal_blade" || z.Name == "item_basher" || z.Name == "item_solar_crest" || z.Name == "item_soul_ring" || z.Name == "item_urn_of_shadows" || z.Name == "item_veil_of_discord" || z.Name == "item_enchanted_mango" || z.Name == "item_tpscroll" || z.Name == "item_aether_lens" || z.Name == "item_arcane_boots" || z.Name == "item_diffusal_blade" || z.Name == "item_diffusal_blade_2" || z.Name == "item_skadi" || z.Name == "item_medallion_of_courage" || z.Name == "item_mekansm" || z.Name == "item_monkey_king_bar" || z.Name == "item_mjollnir" || z.Name == "item_moon_shard" || z.Name == "item_necronomicon" || z.Name == "item_necronomicon_2" || z.Name == "item_necronomicon_3" || z.Name == "item_pipe" || z.Name == "item_refresher" || z.Name == "item_rod_of_atos" || z.Name == "item_satanic" || z.Name == "item_shivas_guard" || z.Name == "item_satanic" || z.Name == "item_basher" || z.Name == "item_tranquil_boots" || z.Name == "item_basher" || z.Name == "item_phase_boots" || z.Name == "item_travel_boots" || z.Name == "item_travel_boots_2" || z.Name == "item_boots" || z.Name == "item_power_treads"))
                            {
                                Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i3 * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y + (15 * ScaleY)), new Vector2(boxSizeX + (6 * ScaleX), boxSizeY), Drawing.GetTexture("materials/ensage_ui/items/" + z.Name.Remove(0, 5)));
                                Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i3 * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y + (15 * ScaleY)), new Vector2(boxSizeX, boxSizeY), Color.Black, true);
                                if (z.Cooldown > 0 || (z.ManaCost > x.Mana && z.ManaCost > 0))
                                {
                                    Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(x).X + ((i3 * 20) * ScaleX), HUDInfo.GetHPbarPosition(x).Y + (15 * ScaleY)), new Vector2(boxSizeX, boxSizeY), new Color(0x5F, 0x00, 0xCE, 0x9E), false);
                                    Drawing.DrawText((int)(z.Cooldown == 0 ? (z.ManaCost - x.Mana) : z.Cooldown) + "", new Vector2((float)(HUDInfo.GetHPbarPosition(x).X + ((1.2 + (i3 * 20)) * ScaleX)), HUDInfo.GetHPbarPosition(x).Y + (23 * ScaleY)), new Vector2(10 * ScaleX, 20 * ScaleY), z.Cooldown == 0 ? Color.LightSkyBlue : Color.White, FontFlags.AntiAlias | FontFlags.StrikeOut | FontFlags.Additive);
                                }
                                i3 += 1 * ScaleX;
                            }
                        }
                    }
                }
                catch (EntityNotFoundException)
                {

                }
            }
            catch (NullReferenceException)
            {

            }
        }
        private static bool MouseOnSpot(Vector2 InitialPOS, Vector2 SIZEPOS)
        {
            Vector2 mouse = Game.MouseScreenPosition;
            if (mouse.X >= InitialPOS.X && mouse.X >= InitialPOS.Y)
            {
                if (mouse.X <= InitialPOS.X + SIZEPOS.X && mouse.Y <= InitialPOS.Y + SIZEPOS.Y)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        private static void GetHPbarSize()
        {
            boxSizeX = (HUDInfo.GetHPBarSizeX() / 6);
            boxSizeY = (HUDInfo.GetHpBarSizeY() + 7);
        }
    }
}
