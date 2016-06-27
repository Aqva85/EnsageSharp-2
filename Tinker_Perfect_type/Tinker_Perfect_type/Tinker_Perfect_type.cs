using System;
using System.Linq;
using System.Collections.Generic;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;
using System.Drawing;

namespace Tinker_Perfect_type
{
    class Tinker_Perfect_type
    {
        private static Ability Laser, Rocket, Refresh, March;
        private static Item Blink, Dagon, Hex, Soulring, Ethereal, Shiva, ghost, euls, forcestaff, glimmer, bottle, travel, veil;
        private static Hero me, target;
        private static ParticleEffect SmartBlinkEffect;
        private static Vector3 mousepos;
        //private static Vector3 Radiant = new Vector3(-7472, -6938, 528), Dire = new Vector3(7472, 6192, 512);
        //private static int stage = 0;
        //private static bool auto_attack, auto_attack_after_spell;
        private static bool smartblink_option = false;
        private static readonly Menu Menu = new Menu("Tinker Perfect", "Tinker Perfect", true, "npc_dota_hero_tinker", true);
        private static readonly Menu _skills = new Menu("Skills", "Skills");
        private static readonly Menu _items = new Menu("Items", "Items");
        private static readonly Menu _items2 = new Menu("Don't Use Combo on:", "Don't Use Combo on:");
        private static readonly Dictionary<string, bool> Skills = new Dictionary<string, bool>
            {
                {"tinker_laser",true},
                {"tinker_heat_seeking_missile",true},
                {"tinker_rearm",true},
                //{"tinker_march_of_the_machines",true}
            };
        private static readonly Dictionary<string, bool> Items = new Dictionary<string, bool>
            {
                //{"item_blink",true},
                {"item_dagon",true},
                {"item_sheepstick",true},
                {"item_soul_ring",true},
                {"item_ethereal_blade",true},
                {"item_shivas_guard",true}
            };
        private static readonly Dictionary<string, bool> Items2 = new Dictionary<string, bool>
            {
                {"item_ghost",true},
                {"item_cyclone",true},
                {"item_force_staff",true},
                //{"item_bottle",true},
                {"item_glimmer_cape",true},
                {"item_veil_of_discord",true}
            };
        private static readonly Dictionary<string, bool> Items3 = new Dictionary<string, bool>
            {
                {"item_blade_mail",true},
                {"item_lotus_orb",true}
            };

        static void Main(string[] args)
        {
            // Menu Options
            Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Smart Blink", "Smart Blink").SetValue(new KeyBind('P', KeyBindType.Press)));
            //Menu.AddItem(new MenuItem("Farm Key", "Farm Key").SetValue(new KeyBind('F', KeyBindType.Press)));
            //Menu.AddItem(new MenuItem("Blink On/Off", "Blink On/Off").SetValue(new KeyBind('T', KeyBindType.Press)));
            Menu.AddSubMenu(_skills);
            Menu.AddSubMenu(_items);
            Menu.AddSubMenu(_items2);
            _skills.AddItem(new MenuItem("Skills: ", "Skills: ").SetValue(new AbilityToggler(Skills)));
            _items.AddItem(new MenuItem("Items: ", "Items 1:").SetValue(new AbilityToggler(Items)));
            _items.AddItem(new MenuItem("Items2: ", "Items 2: ").SetValue(new AbilityToggler(Items2)));
            _items2.AddItem(new MenuItem("Don't Use Combo on:", "Don't Use Combo on:").SetValue(new AbilityToggler(Items3)));
            Menu.AddToMainMenu();
            // Auto Attack Checker
            //if (Game.GetConsoleVar("dota_player_units_auto_attack_after_spell").GetInt() == 1)
            //    auto_attack_after_spell = true;
            //else
            //    auto_attack_after_spell = false;
            //if (Game.GetConsoleVar("dota_player_units_auto_attack").GetInt() == 1)
            //    auto_attack = true;
            //else
            //    auto_attack = false;
            // start
            PrintSuccess(string.Format("> Tinker Perfect Type Loaded!"));
            //Game.OnUpdate
            Game.OnWndProc += Tinker_In_Madness;
            Game.OnUpdate += SmartBlink;
            Drawing.OnDraw += markedfordeath;
        }
        public static void SmartBlink(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null)
                return;
            if (me.ClassID != ClassID.CDOTA_Unit_Hero_Tinker)
                return;
            if (((Game.IsKeyDown(Menu.Item("Smart Blink").GetValue<KeyBind>().Key)) && !Game.IsChatOpen) && Utils.SleepCheck("SmartblinkTime"))
            {
                smartblink_option = !smartblink_option;
                Blink = me.FindItem("item_blink");
                Utils.Sleep(300, "SmartblinkTime");
                mousepos = Game.MousePosition;
                if (SmartBlinkEffect != null)
                    SmartBlinkEffect.Dispose();
                SmartBlinkEffect = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", mousepos);
            }
            if (smartblink_option)
            {
                SmartBlinkEffect.SetControlPoint(2, new Vector3(50, 255, 400));
                SmartBlinkEffect.SetControlPoint(1, new Vector3(200, 34, 76));
                if (Blink != null && Blink.CanBeCasted() && !me.IsChanneling() && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase) && Utils.SleepCheck("blink"))
                {
                    Blink.UseAbility(me.Distance2D(mousepos) < 1200 ? mousepos : new Vector3(me.NetworkPosition.X + 1150 * (float)Math.Cos(me.NetworkPosition.ToVector2().FindAngleBetween(mousepos.ToVector2(), true)), me.NetworkPosition.Y + 1150 * (float)Math.Sin(me.NetworkPosition.ToVector2().FindAngleBetween(mousepos.ToVector2(), true)), 100), false);
                    Utils.Sleep(50, "blink");
                }
                if(Blink.Cooldown >= 11.5)
                    smartblink_option = false;
            }
            else if(SmartBlinkEffect != null)
            {
                SmartBlinkEffect.Dispose();
            }
        }
        public static void Tinker_In_Madness(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null)
                return;
            if (me.ClassID != ClassID.CDOTA_Unit_Hero_Tinker)
                return;
            //if (Game.IsKeyDown(Menu.Item("Blink On/Off").GetValue<KeyBind>().Key) && !Game.IsChatOpen && Utils.SleepCheck("BLINKTOGGLE"))
            //{
            //    Items["item_blink"] = !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_blink");
            //    Utils.Sleep(500, "BLINKTOGGLE");
            //}
            //if ((Game.IsKeyDown(Menu.Item("Farm Key").GetValue<KeyBind>().Key)) && !Game.IsChatOpen || (!Utils.SleepCheck("InCombo") && Refresh.IsChanneling))
            //{
            //    FindItems();
            //    autoattack(true);
            //    Vector3 POSMARCH = (Game.MousePosition - me.NetworkPosition) * 10 / Game.MousePosition.Distance2D(me.NetworkPosition) + me.NetworkPosition;
            //    if (stage == 0 && Utils.SleepCheck("FarmRefresh"))
            //    {
            //        if (Blink != null && Blink.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Blink.Name) && Utils.SleepCheck("REFRESHEER") && !Refresh.IsChanneling && Utils.SleepCheck("blink") && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_blink") && !me.IsChanneling())
            //        {
            //            Blink.UseAbility(Game.MousePosition);
            //            Utils.Sleep(100 - Game.Ping, "blink");
            //        }
            //        if (ghost != null && ghost.CanBeCasted() && !me.IsChanneling() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name) && Utils.SleepCheck("ghost_usage") && Utils.SleepCheck("REFRESHEEER"))
            //        {
            //            ghost.UseAbility(false);
            //            Utils.Sleep(600 - Game.Ping, "ghost_usage");
            //        }
            //        if (Soulring != null && Soulring.CanBeCasted() && !me.IsChanneling() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Soulring.Name) && Utils.SleepCheck("soul_ring_usage") && Utils.SleepCheck("REFRESHEEER"))
            //        {
            //            Soulring.UseAbility(false);
            //            Utils.Sleep(600 - Game.Ping, "soul_ring_usage");
            //        }
            //        if (bottle != null && bottle.CanBeCasted() && !me.IsChanneling() && !me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration") && bottle.CurrentCharges >= 0 && Utils.SleepCheck("bottle_CD") && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(bottle.Name) && Utils.SleepCheck("REFRESHEEER"))
            //        {
            //            bottle.UseAbility(false);
            //            Utils.Sleep(1000 - Game.Ping, "bottle_CD");
            //        }
            //        if (March != null && March.CanBeCasted() && !me.IsChanneling() && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(March.Name) && me.Mana >= March.ManaCost + 75 && Utils.SleepCheck("MarchUsage"))
            //        {
            //            March.UseAbility(POSMARCH, false);
            //            Utils.Sleep(800 - Game.Ping, "MarchUsage");
            //        }
            //        if ((Soulring == null || !Soulring.CanBeCasted() || !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Soulring.Name)) && (!March.CanBeCasted() || March.Level <= 0 || me.Mana <= March.ManaCost + 75 || !Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(March.Name)) && (Refresh.Level >= 0 && Refresh.CanBeCasted()) && !me.IsChanneling() && Utils.SleepCheck("REFRESHEEER") && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name))
            //        {
            //            Refresh.UseAbility(false);
            //            Utils.Sleep(900 - Game.Ping, "REFRESHEEER");
            //        }
            //        if (Refresh.IsChanneling)
            //        {
            //            stage = 1;
            //            Utils.Sleep(5000 - Game.Ping, "CD_COMBO_FARM");
            //        }
            //        if (me.Mana <= Refresh.ManaCost)
            //            stage = 1;
            //        Utils.Sleep(500 - Game.Ping, "InCombo");
            //    }
            //    if (stage == 1 && Utils.SleepCheck("FarmRefresh"))
            //    {
            //        if (Blink != null && Blink.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Blink.Name) && Utils.SleepCheck("REFRESHEER") && !Refresh.IsChanneling && Utils.SleepCheck("blink") && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_blink") && !me.IsChanneling())
            //        {
            //            Blink.UseAbility(Game.MousePosition);
            //            Utils.Sleep(300 - Game.Ping, "blink");
            //        }
            //        if (ghost != null && ghost.CanBeCasted() && !me.IsChanneling() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name) && Utils.SleepCheck("REFRESHEEER") && Utils.SleepCheck("ghost_usage"))
            //        {
            //            ghost.UseAbility(false);
            //            Utils.Sleep(600 - Game.Ping, "ghost_usage");
            //        }
            //        if (Soulring != null && Soulring.CanBeCasted() && !me.IsChanneling() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Soulring.Name) && Utils.SleepCheck("REFRESHEEER") && Utils.SleepCheck("soul_ring_usage"))
            //        {
            //            Soulring.UseAbility(false);
            //            Utils.Sleep(600 - Game.Ping, "soul_ring_usage");
            //        }
            //        if (bottle != null && bottle.CanBeCasted() && !me.IsChanneling() && !me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration") && bottle.CurrentCharges >= 0 && Utils.SleepCheck("bottle_CD") && Utils.SleepCheck("REFRESHEEER"))
            //        {
            //            bottle.UseAbility();
            //            Utils.Sleep(1000 - Game.Ping, "bottle_CD");
            //        }
            //        if (March != null && March.CanBeCasted() && !me.IsChanneling() && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(March.Name) && me.Mana >= March.ManaCost + 75 && Utils.SleepCheck("REFRESHEEER") && Utils.SleepCheck("MarchUsage"))
            //        {
            //            March.UseAbility(POSMARCH);
            //            Utils.Sleep(800 - Game.Ping, "MarchUsage");
            //        }
            //        if ((Soulring == null || !Soulring.CanBeCasted() || !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Soulring.Name)) && (!March.CanBeCasted() || March.Level <= 0 || me.Mana <= March.ManaCost + 75 || !Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(March.Name)) && Utils.SleepCheck("REFRESHEEER"))
            //        {
            //            if (travel.CanBeCasted() && !me.IsChanneling())
            //            {
            //                if (me.Team == Team.Dire)
            //                    travel.UseAbility(Dire);
            //                if (me.Team == Team.Radiant)
            //                    travel.UseAbility(Radiant);
            //                Utils.Sleep(500 - Game.Ping, "FarmRefresh");
            //            }
            //            if (travel.IsChanneling)
            //                stage = 0;
            //        }
            //    }
            //}
            //else
            //{
            //    autoattack(false);
            //    if (Utils.SleepCheck("CD_COMBO_FARM"))
            //        stage = 0;
            //}
            if ((Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)) && !Game.IsChatOpen)
            {
                target = me.ClosestToMouseTarget(1000);
                if (target != null && target.IsAlive && !target.IsIllusion && !me.IsChanneling() && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase) && !CanReflectDamage(target))
                {
                    //autoattack(true);
                    FindItems();
                    if (target.IsLinkensProtected())
                    {
                        if (euls != null && euls.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(euls.Name))
                        {
                            if (Utils.SleepCheck("TimingToLinkens"))
                            {
                                euls.UseAbility(target,false);
                                Utils.Sleep(200, "TimingToLinkens");
                            }
                        }
                        else if (forcestaff != null && forcestaff.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(forcestaff.Name))
                        {
                            if (Utils.SleepCheck("TimingToLinkens"))
                            {
                                forcestaff.UseAbility(target,false);
                                Utils.Sleep(200, "TimingToLinkens");
                            }
                        }
                        else if (Ethereal != null && Ethereal.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Ethereal.Name))
                        {
                            if (Utils.SleepCheck("TimingToLinkens"))
                            {
                                Ethereal.UseAbility(target,false);
                                Utils.Sleep(200, "TimingToLinkens");
                                Utils.Sleep(((me.NetworkPosition.Distance2D(target.NetworkPosition) / 1200) * 1000) + 200, "TimingToLinkens");
                            }
                        }
                        else if (Laser != null && Laser.CanBeCasted() && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Laser.Name))
                        {
                            if (Utils.SleepCheck("TimingToLinkens"))
                            {
                                Laser.UseAbility(target,false);
                                Utils.Sleep(200, "TimingToLinkens");
                            }
                        }
                        else if (Dagon != null && Dagon.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                        {
                            if (Utils.SleepCheck("TimingToLinkens"))
                            {
                                Dagon.UseAbility(target,false);
                                Utils.Sleep(200, "TimingToLinkens");
                            }
                        }
                        else if (Hex != null && Hex.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Hex.Name))
                        {
                            if (Utils.SleepCheck("TimingToLinkens"))
                            {
                                Hex.UseAbility(target,false);
                                Utils.Sleep(200, "TimingToLinkens");
                            }
                        }
                    }
                    else
                    {
                        uint elsecount = 0;
                        bool magicimune = (!target.IsMagicImmune() && !target.Modifiers.Any(x => x.Name == "modifier_eul_cyclone"));
                        uint[] dagondamage = new uint[5] { 400, 500, 600, 700, 800 };
                        // glimmer -> ghost -> soulring -> hex -> laser -> ethereal -> dagon -> rocket -> shivas -> euls -> refresh
                        //if(Blink != null && Blink.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Blink.Name) && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("blink") && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_blink") && !me.IsChanneling())
                        //{
                        //    Blink.UseAbility(Game.MousePosition);
                        //    Utils.Sleep(300 - Game.Ping, "blink");
                        //}
                        if (glimmer != null && glimmer.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(glimmer.Name) && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("glimmer"))
                        {
                            glimmer.UseAbility(me,false);
                            Utils.Sleep(200, "glimmer");
                        }
                        else
                            elsecount += 1;
                        if(veil != null && veil.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(veil.Name) && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("veil"))
                        {
                            veil.UseAbility(target.Position,false);
                            Utils.Sleep(200, "veil");
                        }
                        if (ghost != null && Ethereal == null && ghost.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name) && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("ghost"))
                        {
                            ghost.UseAbility(false);
                            Utils.Sleep(200, "ghost");
                        }
                        else
                            elsecount += 1;
                        if (Soulring != null && Soulring.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Soulring.Name) && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("soulring"))
                        {
                            Soulring.UseAbility(false);
                            Utils.Sleep(200, "soulring");
                        }
                        else
                            elsecount += 1;
                        if (Hex != null && Hex.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Hex.Name) && magicimune && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("Hex"))
                        {
                            Hex.UseAbility(target,false);
                            Utils.Sleep(200, "Hex");
                        }
                        else
                            elsecount += 1;
                        if (Laser != null && Laser.CanBeCasted() && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Laser.Name) && magicimune && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("laser"))
                        {
                            Utils.Sleep(200, "laser");
                            Laser.UseAbility(target,false);
                        }
                        else
                            elsecount += 1;
                        if (Ethereal != null && Ethereal.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Ethereal.Name) && magicimune && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("Ethereal") && me.Distance2D(target) <= Ethereal.CastRange && (Dagon != null ? target.Health >= target.DamageTaken(dagondamage[Dagon.Level - 1],DamageType.Magical,me,false,0,0,0) : true))
                        {
                            Ethereal.UseAbility(target,false);
                            Utils.Sleep(200, "Ethereal");
                            if (Utils.SleepCheck("EtherealTime"))
                            {
                                Utils.Sleep(((me.NetworkPosition.Distance2D(target.NetworkPosition) / 1200) * 1000) + 25, "EtherealTime");
                                Utils.Sleep(((me.NetworkPosition.Distance2D(target.NetworkPosition) / 1200) * 1000) + 200, "EtherealTime2");
                            }
                        }
                        else
                            elsecount += 1;
                        if (Dagon != null && Dagon.CanBeCasted() && (!Ethereal.CanBeCasted() || target.Health <= target.DamageTaken(dagondamage[Dagon.Level - 1], DamageType.Magical, me, false, 0, 0, 0)) && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_dagon") && magicimune && Utils.SleepCheck("Rearm") && Utils.SleepCheck("EtherealTime") && !Refresh.IsChanneling && Utils.SleepCheck("dagon"))
                        {
                            Dagon.UseAbility(target,false);
                            Utils.Sleep(200, "dagon");
                        }
                        else
                            elsecount += 1;
                        if (Rocket != null && Rocket.CanBeCasted() && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name) && magicimune && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("rocket") && me.Distance2D(target) <= Rocket.CastRange)
                        {
                            Rocket.UseAbility(false);
                            Utils.Sleep(200, "rocket");
                            if (Utils.SleepCheck("RocketTime"))
                            {
                                Utils.Sleep(((me.NetworkPosition.Distance2D(target.NetworkPosition) / 900) * 1000), "RocketTime");
                                Utils.Sleep(((me.NetworkPosition.Distance2D(target.NetworkPosition) / 900) * 1000) + 200, "RocketTime2");
                            }
                        }
                        else
                            elsecount += 1;
                        if (Shiva != null && Shiva.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Shiva.Name) && magicimune && Utils.SleepCheck("Rearm") && !Refresh.IsChanneling && Utils.SleepCheck("shiva"))
                        {
                            Shiva.UseAbility(false);
                            Utils.Sleep(200, "shiva");
                        }
                        else
                            elsecount += 1;
                        if (elsecount == 9 && euls != null && euls.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(euls.Name) && magicimune && Utils.SleepCheck("Rearm") && Utils.SleepCheck("EtherealTime2") && Utils.SleepCheck("RocketTime2") && Utils.SleepCheck("euls"))
                        {
                            euls.UseAbility(target,false);
                            Utils.Sleep(200, "euls");
                        }
                        else
                            elsecount += 1;
                        if (elsecount == 10 && Refresh != null && Refresh.CanBeCasted() && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name) && !Refresh.IsChanneling && Utils.SleepCheck("Rearm") && Ready_for_refresh())
                        {
                            Refresh.UseAbility(false);
                            Utils.Sleep(1000, "Rearm");
                        }
                        else
                        {
                            if (!me.IsChanneling() && me.CanAttack() && Utils.SleepCheck("Rearm") && Utils.SleepCheck("movesleep2"))
                            {
                                //Orbwalking.Orbwalk(target);
                                me.Attack(target);
                                Utils.Sleep(500, "movesleep2");
                            }
                        }
                    }
                }
                else
                {
                    //autoattack(false);
                    if (!me.IsChanneling() && Utils.SleepCheck("Rearm") && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase) && Utils.SleepCheck("movesleep1"))
                    {
                        me.Move(Game.MousePosition, false);
                        Utils.Sleep(500, "movesleep1");
                    }

                }
            }
            //else
            //    autoattack(false);
        }
        //static void autoattack(bool key)
        //{
        //    if (key)
        //    {
        //        if (auto_attack)
        //            Game.ExecuteCommand("dota_player_units_auto_attack 0");
        //        if (auto_attack_after_spell)
        //            Game.ExecuteCommand("dota_player_units_auto_attack_after_spell 0");
        //    }
        //    else
        //    {
        //        if (auto_attack)
        //            Game.ExecuteCommand("dota_player_units_auto_attack 1");
        //        if (auto_attack_after_spell)
        //            Game.ExecuteCommand("dota_player_units_auto_attack_after_spell 1");
        //    }

        //}
        static void markedfordeath(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null)
                return;
            if (me.ClassID != ClassID.CDOTA_Unit_Hero_Tinker)
                return;
            target = me.ClosestToMouseTarget(50000);
            if (target != null && !target.IsIllusion && target.IsAlive)
            {
                Vector2 target_health_bar = HeroPositionOnScreen(target);
                Drawing.DrawText("Marked for Death", target_health_bar, new Vector2(10, 200), me.Distance2D(target) < 1200 ? SharpDX.Color.Red : SharpDX.Color.Azure, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
            }
            //if (!Utils.SleepCheck("BLINKTOGGLE"))
            //    Drawing.DrawText(Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_blink") == true ? "BLINK ON" : "BLINK OFF", new Vector2(HUDInfo.ScreenSizeX() / 2, HUDInfo.ScreenSizeY() / 2), new Vector2(30, 200), Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_blink") == true ? Color.LimeGreen : Color.Red, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
        }
        static void FindItems()
        {
            //Skils
            Laser = me.Spellbook.SpellQ;
            Rocket = me.Spellbook.SpellW;
            Refresh = me.Spellbook.SpellR;
            March = me.Spellbook.SpellE;
            //Items
            Blink = me.FindItem("item_blink");
            Dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            Hex = me.FindItem("item_sheepstick");
            Soulring = me.FindItem("item_soul_ring");
            Ethereal = me.FindItem("item_ethereal_blade");
            Shiva = me.FindItem("item_shivas_guard");
            ghost = me.FindItem("item_ghost");
            euls = me.FindItem("item_cyclone");
            forcestaff = me.FindItem("item_force_staff");
            glimmer = me.FindItem("item_glimmer_cape");
            bottle = me.FindItem("item_bottle");
            veil = me.FindItem("item_veil_of_discord");
            travel = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_travel_boots"));
        }
        static Vector2 HeroPositionOnScreen(Hero x)
        {
            Vector2 PicPosition;
            PicPosition = new Vector2(HUDInfo.GetHPbarPosition(x).X - 1, HUDInfo.GetHPbarPosition(x).Y - 40);
            return PicPosition;
        }
        static bool Ready_for_refresh()
        {
            if ((ghost != null && ghost.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name))
                || (Soulring != null && Soulring.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Soulring.Name))
                || (Hex != null && Hex.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Hex.Name))
                || (Laser != null && Laser.CanBeCasted() && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Laser.Name))
                || (Ethereal != null && Ethereal.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Ethereal.Name))
                || (Dagon != null && Dagon.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                || (Rocket != null && Rocket.CanBeCasted() && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name))
                || (Shiva != null && Shiva.CanBeCasted() && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
                || (euls != null && euls.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(euls.Name))
                || (glimmer != null && glimmer.CanBeCasted() && Menu.Item("Items2: ").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)))
                return false;
            else
                return true;
        }
        private static void PrintSuccess(string text, params object[] arguments)
        {
            PrintEncolored(text, ConsoleColor.Green, arguments);
        }
        private static void PrintEncolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, arguments);
            Console.ForegroundColor = clr;
        }
        //static bool IsLinkensProtected(Hero x)
        //{
        //    if (x.Modifiers.Any(m => m.Name == "modifier_item_sphere_target") || x.FindItem("item_sphere") != null && x.FindItem("item_sphere").Cooldown <= 0)
        //        return true;
        //    else
        //        return false;
        //}
        static bool CanReflectDamage(Hero x)
        {
            if (x.Modifiers.Any(m => (m.Name == "modifier_item_blade_mail_reflect" && Menu.Item("Don't Use Combo on:").GetValue<AbilityToggler>().IsEnabled("item_blade_mail")) || (m.Name == "modifier_item_lotus_orb_active") && Menu.Item("Don't Use Combo on:").GetValue<AbilityToggler>().IsEnabled("item_lotus_orb")))
                return true;
            else
                return false;
        }
    }
}
