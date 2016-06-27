using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common;
//using SharpDX.Direct3D9;
using SharpDX;
using Ensage.Common.Menu;

namespace NecrophosRework
{
    class Reaper
    {
        static Ability Qskill, Rskill;
        static Item _item_blink, _item_dagon, _item_ethereal, _item_veil, _item_cyclone, _item_force_staff, _item_shivas_guard, _item_orchid;
        static bool combo_state = false;
        static Hero me, target;
        static double[] UltDmg = new double[3] { 0.4, 0.6, 0.9 };
        static double[] AUltDmg = new double[3] { 0.6, 0.9, 1.2 };
        static int[] DDamage = new int[5] { 400, 500, 600, 700, 800 };
        static readonly Menu Menu = new Menu("Necrophos", "Necrophos", true, "npc_dota_hero_Necrolyte", true);

        static readonly Dictionary<string, bool> _items_combo = new Dictionary<string, bool>
            {
                {"item_blink",true},
                {"item_dagon",true},
                {"item_ethereal_blade",true},
                {"item_shivas_guard",true},
                {"item_orchid",true},
                {"item_veil_of_discord",true}
            };
        static readonly Dictionary<string, bool> _items_pop_sphere = new Dictionary<string, bool>
            {
                {"item_dagon",true},
                {"item_cyclone",true},
                {"item_ethereal_blade",true},
                {"item_orchid",true},
                {"item_force_staff",true}
            };
        static readonly Dictionary<string, bool> _skills_combo = new Dictionary<string, bool>
            {
                {"necrolyte_death_pulse",true},
                {"necrolyte_reapers_scythe",true}
            };

        static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("Combo", "Combo: ").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Drawing.Info", "Enable Drawing Info").SetValue(true)).DontSave().SetTooltip("Shows the combo damage information.(Recommended)");
            Menu.AddItem(new MenuItem("Type", "Combo Type: ").SetValue(new StringList(new[] { "secure type", "secure force combo type", "total force combo type" })));
            Menu.AddItem(new MenuItem("items", "Combo Items:").SetValue(new AbilityToggler(_items_combo)));
            Menu.AddItem(new MenuItem("Items2", "PoP Linkens: ").SetValue(new AbilityToggler(_items_pop_sphere)));
            Menu.AddItem(new MenuItem("skills", "Skills: ").SetValue(new AbilityToggler(_skills_combo)));
            Menu.AddToMainMenu();
            Game.OnWndProc += reaper;
            Drawing.OnDraw += damagecalc;
            PrintSuccess(string.Format("> Necrophos Script Loaded!"));
        }
        public static void damagecalc(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Necrolyte)
                return;
            if (Menu.Item("Drawing.Info").GetValue<bool>())
            {
                if (Utils.SleepCheck("locktarget"))
                    target = me.ClosestToMouseTarget(1000);
                if (target == null)
                    return;
                if (target != null)
                {
                    var combodamage = ComboDamage();
                    var BarSize_Requiem = (HUDInfo.GetHPBarSizeX(target) / ((float)(target.MaximumHealth - combodamage) / (target.Health - combodamage)));
                    if (BarSize_Requiem > HUDInfo.GetHPBarSizeX(target))
                        BarSize_Requiem = HUDInfo.GetHPBarSizeX(target);
                    Drawing.DrawText((target.Health - combodamage) > 0 ? "" + (int)(target.Health - combodamage) : "Killable", new Vector2(HUDInfo.GetHPbarPosition(target).X, HUDInfo.GetHPbarPosition(target).Y - 42), new Vector2(15, 20), combodamage < target.Health ? Color.White : Color.Red, FontFlags.Additive);
                    Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(target).X, HUDInfo.GetHPbarPosition(target).Y - 1), new Vector2(combodamage >= target.MaximumHealth ? 0 : BarSize_Requiem, HUDInfo.GetHpBarSizeY(target) - 8), Color.GreenYellow);
                    Drawing.DrawRect(new Vector2(HUDInfo.GetHPbarPosition(target).X, HUDInfo.GetHPbarPosition(target).Y - 1), new Vector2(HUDInfo.GetHPBarSizeX(target), HUDInfo.GetHpBarSizeY(target) - 8), Color.Black, true);
                }
            }
        }
        private static void reaper(EventArgs args)
        {
            if (Game.IsPaused || !Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Necrolyte) return;
            if (Game.IsKeyDown(Menu.Item("Combo").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
            {
                if (Utils.SleepCheck("locktarget"))
                    target = me.ClosestToMouseTarget(1000);
                if (target == null) return;
                InitialValues();
                bool combo_type = true;
                if (Menu.Item("Type").GetValue<StringList>().SelectedIndex == 0)
                {
                    bool _Is_in_Advantage = (target.Modifiers.Any(x =>
                    x.Name == "modifier_item_blade_mail_reflect"
                    || x.Name == "modifier_item_lotus_orb_active"
                    || x.Name == "modifier_nyx_assassin_spiked_carapace"
                    || x.Name == "modifier_templar_assassin_refraction_damage"
                    || x.Name == "modifier_ursa_enrage"
                    || x.Name == "modifier_abaddon_borrowed_time"));
                    combo_type = target.Health <= ComboDamage() && !_Is_in_Advantage && target.CanDie();
                }
                else if (Menu.Item("Type").GetValue<StringList>().SelectedIndex == 1)
                {
                    bool _Is_in_Advantage = (target.Modifiers.Any(x =>
                    x.Name == "modifier_item_blade_mail_reflect"
                    || x.Name == "modifier_item_lotus_orb_active"
                    || x.Name == "modifier_nyx_assassin_spiked_carapace"
                    || x.Name == "modifier_templar_assassin_refraction_damage"
                    || x.Name == "modifier_ursa_enrage"
                    || x.Name == "modifier_abaddon_borrowed_time"));
                    combo_type = target.Health <= ComboDamage() && !_Is_in_Advantage;
                }
                else
                    combo_type = true;
                if ((combo_type || combo_state) && !target.IsInvul())
                {
                    Bool WindWalkMod = me.Modifiers.Any(x =>
                x.Name == "modifier_item_silver_edge_windwalk"
                || x.Name == "modifier_item_invisibility_edge_windwalk");
                    Utils.Sleep(500, "locktarget");
                    if (WindWalkMod)
                    {
                        if (Utils.SleepCheck("attacking"))
                        {
                            me.Attack(target);
                            Utils.Sleep(150, "attacking");
                        }
                    }
                    else
                    {
                        if (!combo_state)
                        {
                            combo_state = true;
                            Utils.Sleep(3000, "MAXCOMBOTIME");
                        }
                        if (Utils.SleepCheck("MAXCOMBOTIME"))
                        {
                            combo_state = false;
                        }
                        if (target.IsLinkensProtected())
                        {
                            if (Utils.SleepCheck("LINKENS"))
                            {
                                if (_item_cyclone.CanBeCasted() && item_state(_item_cyclone.Name, 2))
                                {
                                    _item_cyclone.UseAbility(target, false);
                                    Utils.Sleep(150, "LINKENS");
                                }
                                else if (_item_force_staff.CanBeCasted() && item_state(_item_force_staff.Name, 2))
                                {
                                    _item_force_staff.UseAbility(target, false);
                                    Utils.Sleep(150, "LINKENS");
                                }
                                else if (_item_orchid.CanBeCasted() && item_state(_item_orchid.Name, 2))
                                {
                                    _item_orchid.UseAbility(target, false);
                                    Utils.Sleep(150, "LINKENS");
                                }
                                else if (_item_ethereal.CanBeCasted() && item_state(_item_ethereal.Name, 2))
                                {
                                    _item_ethereal.UseAbility(target, false);
                                    Utils.Sleep(150, "LINKENS");
                                }
                                else if (_item_dagon.CanBeCasted() && item_state(_item_dagon.Name, 2))
                                {
                                    _item_dagon.UseAbility(target, false);
                                    Utils.Sleep(150, "LINKENS");
                                }
                            }
                        }
                        else
                        {
                            if (Rskill.CanBeCasted() && item_state(Rskill.Name, 3) && Utils.SleepCheck("Ulting"))
                            {
                                Rskill.UseAbility(target, false);
                                Utils.Sleep(150, "Ulting");
                            }
                            else if (!Rskill.CanBeCasted() || !item_state(Rskill.Name, 3))
                            {
                                bool CanDamage = (!_item_veil.CanBeCasted() || !item_state(_item_veil.Name, 1)) && (!_item_ethereal.CanBeCasted() || !item_state(_item_ethereal.Name, 1)) && (!_item_orchid.CanBeCasted() || !item_state(_item_orchid.Name, 1));
                                if (!CanDamage)
                                {
                                    if (Utils.SleepCheck("Amply"))
                                    {
                                        if (_item_orchid.CanBeCasted() && item_state(_item_orchid.Name, 1))
                                        {
                                            _item_orchid.UseAbility(target, false);
                                            Utils.Sleep(150, "Amply");
                                        }
                                        if (_item_veil.CanBeCasted() && item_state(_item_veil.Name, 1))
                                        {
                                            _item_veil.UseAbility(target.Position, false);
                                            Utils.Sleep(150, "Amply");
                                        }
                                        if (_item_ethereal.CanBeCasted() && item_state(_item_ethereal.Name, 1))
                                        {
                                            _item_ethereal.UseAbility(target, false);
                                            Utils.Sleep(150, "Amply");
                                            Utils.Sleep((me.Distance2D(target) / 1200) + 170, "DAMAGE");
                                        }
                                    }
                                }
                                else
                                {
                                    if (Utils.SleepCheck("DAMAGE"))
                                    {
                                        if (Qskill.CanBeCasted() && item_state(Qskill.Name, 3) && me.NetworkPosition.Distance2D(target.NetworkPosition) <= 450)
                                        {
                                            Qskill.UseAbility(false);
                                            Utils.Sleep(150, "DAMAGE");
                                        }
                                        if (_item_dagon.CanBeCasted() && item_state(_item_dagon.Name, 1))
                                        {
                                            _item_dagon.UseAbility(target, false);
                                            Utils.Sleep(150, "DAMAGE");
                                        }
                                        if (_item_shivas_guard.CanBeCasted() && item_state(_item_shivas_guard.Name, 1) && me.NetworkPosition.Distance2D(target.NetworkPosition) <= 800)
                                        {
                                            _item_shivas_guard.UseAbility(false);
                                            Utils.Sleep(150, "DAMAGE");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void InitialValues()
        {
            if (Utils.SleepCheck("initialvalues"))
            {
                Qskill = me.Spellbook.SpellQ;
                Rskill = me.Spellbook.SpellR;
                _item_blink = me.FindItem("item_blink");
                _item_dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
                _item_ethereal = me.FindItem("item_ethereal_blade");
                _item_veil = me.FindItem("item_veil_of_discord");
                _item_cyclone = me.FindItem("item_cyclone");
                _item_force_staff = me.FindItem("item_force_staff");
                _item_shivas_guard = me.FindItem("item_shivas_guard");
                _item_orchid = me.FindItem("item_orchid");
                Utils.Sleep(150, "initialvalues");
            }
        }
        private static bool item_state(string item, uint index)
        {
            bool value;
            if (item == null) return false;
            if (item.Contains("item_dagon"))
                item = "item_dagon";
            if (index == 1)
            {
                if (_items_combo.TryGetValue(item, out value))
                    return value;
                else
                    return false;
            }
            else if (index == 2)
            {
                if (_items_pop_sphere.TryGetValue(item, out value))
                    return value;
                else
                    return false;
            }
            else if (index == 3)
            {
                if (_skills_combo.TryGetValue(item, out value))
                    return value;
                else
                    return false;
            }
            else
                return false;
        }
        private static int ComboDamage()
        {
            InitialValues();
            if (target == null || me == null)
                return 0;
            float damage = 0;
            if (Rskill.CanBeCasted() && !me.AghanimState() && item_state(Rskill.Name, 3))
                damage = (int)Math.Floor((UltDmg[Rskill.Level - 1] / (1 + UltDmg[Rskill.Level - 1])) * target.MaximumHealth);
            else if (Rskill.CanBeCasted() && me.AghanimState() && item_state(Rskill.Name, 3))
                damage = (int)Math.Floor((AUltDmg[Rskill.Level - 1] / (1 + AUltDmg[Rskill.Level - 1])) * target.MaximumHealth);
            else
                damage = 0;
            if (_item_dagon.CanBeCasted() && item_state(_item_dagon.Name, 1))
                damage += DDamage[_item_dagon.Level - 1];
            if (_item_shivas_guard.CanBeCasted() && item_state(_item_shivas_guard.Name, 1))
                damage += 200;
            if (Qskill.Level > 0 && Qskill.CanBeCasted() && item_state(Qskill.Name, 3) && (me.Distance2D(target) < 450))
            {
                int[] Qskilldamage = new int[4] { 125, 175, 225, 275 };
                damage += Qskilldamage[Qskill.Level - 1];
            }
            double multiplier = 1;
            //Bonus ethereal and veil
            if (_item_veil.CanBeCasted() && !target.Modifiers.Any(x => x.Name == "modifier_item_veil_of_discord_debuff") && item_state(_item_veil.Name, 1))
                multiplier *= 1.25;
            if (_item_ethereal.CanBeCasted() && !target.Modifiers.Any(x => x.Name == "modifier_item_ethereal_blade_ethereal") && item_state(_item_ethereal.Name, 1))
                multiplier *= 1.40;
            multiplier = (multiplier - 1) * 100;
            damage = target.DamageTaken(damage, DamageType.Magical, me, false, 0, 0, multiplier);
            if (_item_ethereal.CanBeCasted() && item_state(_item_ethereal.Name, 1))
            {
                int damageethereal = (int)Math.Floor(((me.TotalIntelligence * 2) + 75));
                damage += (int)(damageethereal * (1 - target.MagicDamageResist));
            }
            bool WindWalkMod = me.Modifiers.Any(x => x.Name == "modifier_item_silver_edge_windwalk" || x.Name == "modifier_item_invisibility_edge_windwalk");
            if (WindWalkMod)
            {
                if (me.Modifiers.Any(x => x.Name == "modifier_item_silver_edge_windwalk"))
                    damage += (int)((me.MinimumDamage * (1 - (target.DamageResist))) + 225);
                else if (me.Modifiers.Any(x => x.Name == "modifier_item_invisibility_edge_windwalk"))
                    damage += (int)((me.MinimumDamage * (1 - (target.DamageResist))) + 175);
            }
            return (int)damage;
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
    }
}
