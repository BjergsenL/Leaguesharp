using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;
using Color = System.Drawing.Color;

//Let's roll
namespace Jax
{
   
    class Program
    {
        public static string ChampionName = "Jax";
        public static Orbwalking.Orbwalker Orbwalker;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public static Items.Item BoRK;
        public static Items.Item Hydra;
        public static Items.Item Tiamat;
        public static Items.Item Hextech;
        public static Items.Item Cutlass;

        //MMMMMEENUUU
        public static Menu Config;
        private static Obj_AI_Hero Player;


       private static void Main(string[] args)
        {
            {
                CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            }
        }

       private static void Game_OnGameLoad(EventArgs args)
       {
           Player = ObjectManager.Player;
           if (Player.BaseSkinName != ChampionName) return;
           Q = new Spell(SpellSlot.Q, 700);
           W = new Spell(SpellSlot.W, 0);
           E = new Spell(SpellSlot.E, 450);
           R = new Spell(SpellSlot.R, 0);

           SpellList.Add(Q);
           SpellList.Add(W);
           SpellList.Add(E);
           SpellList.Add(R);
            //menu
           Config = new Menu(ChampionName, ChampionName, true);
           //orbwalker
           Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
           Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
           //ts
           var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
           SimpleTs.AddToMenu(targetSelectorMenu);
           Config.AddSubMenu(targetSelectorMenu);

           //combo menu

           Config.AddSubMenu(new Menu("Combo", "Combo"));
           Config.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
           Config.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
           Config.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E").SetValue(true));
           Config.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));
           Config.SubMenu("Combo").AddItem(new MenuItem("combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

           //Extras

           //Config.AddSubMenu(new Menu("Extras", "Extras"));
           //Config.SubMenu("Extras").AddItem(new MenuItem("escape", "Ward Jump With Q").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press))); //needs to be done.

           //drawing
           Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q Range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
           Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E Range").SetValue(new Circle(true, Color.FromArgb(150, Color.Azure))));

       }

       private static void Game_OnGameUpdate(EventArgs args)
       {
           if (Config.Item("combo").GetValue<KeyBind>().Active)
           {
               Combo();
           }
       }


       private static void Combo()
       {
           var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
           if (target == null) return;

           if (target.IsValidTarget(Q.Range) && Q.IsReady());
           {
               Q.Cast(target);
           }

           if (target.IsValidTarget(Q.Range) && E.IsReady());
           {
               E.Cast();
           }

           if (target.IsValidTarget(Q.Range) && W.IsReady());
           {
               W.Cast();
           }
       }
    }
}
