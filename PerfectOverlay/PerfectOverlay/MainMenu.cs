using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using PerfectOverlay.scripts;
using Ensage;
//using Ensage.Common.Extensions;
//using Ensage.Common;
//using SharpDX.Direct3D9;
//using SharpDX;
using Ensage.Common.Menu;

namespace PerfectOverlay
{
    class MainMenu
    {
        static readonly Menu Menu = new Menu("Perfect Overlay!", "Perfect Overlay!", true);
        static readonly Menu Menu_options = new Menu("Options", "Options");
        static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("ShowMeMore", "ShowMeMore").SetValue(true).SetTooltip("Show all skills range."));
            Menu.AddItem(new MenuItem("Overlay Menu", "Overlay Menu").SetValue(true).SetTooltip("Show Overlay Menu, with skills, cooldowns, dangerous items.."));
            Menu.AddItem(new MenuItem("Show Illusions", "Show Illusions").SetValue(true).SetTooltip("Show an enemy Illusion."));
            Menu.AddItem(new MenuItem("Show Wards/Mines", "Show Wards/Mines").SetValue(true).SetTooltip("Show all enemies wards and techies mines with ranges.(you need to see them at least one time to work."));
            //Menu.AddItem(new MenuItem("Show TOP Overlay", "Show TOP Overlay").SetValue(true).SetTooltip("Top health bar, top mana bars, to ultimate timing."));
            Menu.AddToMainMenu();
            PrintSuccess("> Perfect Overlay !");
            Game.OnUpdate += Playing;
            Drawing.OnDraw += Desenho;
            Unit.OnModifierAdded += ModifierAdded;
        }
        static void ModifierAdded(Unit Sender, ModifierChangedEventArgs args)
        {
            if (Menu.Item("ShowMeMore").GetValue<bool>())
                ShowMeMore.init(Sender, args);
        }
        public static void Desenho(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            if (Menu.Item("Overlay Menu").GetValue<bool>())
                OverlayMenu.start(/*Menu.Item("Show TOP Overlay").GetValue<bool>()*/);
            if (Menu.Item("Show Wards/Mines").GetValue<bool>())
                wards.start();
        }
        public static void Playing(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            if (Menu.Item("Show Illusions").GetValue<bool>())
                Showillusions.illusionstart();
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
