using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace XinZhao
{
    class Program
    {
        public static string ChampName = "Xin Zhao";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q, W, E, R;
        public static Items.Item hydra = new Items.Item(3074, 400);
        public static Items.Item tiamat = new Items.Item(3077, 400);
        public static Items.Item BoRK = new Items.Item(3153, 400);
        private static SpellSlot IgniteSlot;
        

        public static Menu AN;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 180);
           

            //Base menu
            AN = new Menu("AN" + ChampName, ChampName, true);
            //Orbwalker and menu
            AN.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(AN.SubMenu("Orbwalker"));
            //Target selector and menu
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            AN.AddSubMenu(ts);
            //Combo menu
            AN.AddSubMenu(new Menu("Combo", "Combo"));
            AN.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q?").SetValue(true));
            AN.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W?").SetValue(true));
            AN.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E?").SetValue(true));
            AN.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            //KS Menu
            AN.AddSubMenu(new Menu("KillSteal", "Ks"));
            AN.SubMenu("Ks").AddItem(new MenuItem("ActiveKs", "Use KillSteal")).SetValue(true);
            AN.SubMenu("Ks").AddItem(new MenuItem("UseRKs", "Use R")).SetValue(true);
            


            Drawing.OnDraw += Drawing_OnDraw; // Add onDraw
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)

            Game.PrintChat(ChampName + " loaded! By Animated ;)");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (AN.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (AN.Item("ActivateKs").GetValue<bool>())
            {
                KillSteal();
            }

        }

        static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(Player.Position, E.Range, Color.Crimson);
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            if (target.IsValidTarget(hydra.Range) && hydra.IsReady())
                hydra.Cast();

            if (target.IsValidTarget(tiamat.Range) && tiamat.IsReady())
                tiamat.Cast();

            if (target.IsValidTarget(BoRK.Range) && BoRK.IsReady())
                BoRK.Cast(target);

            if (target.IsValidTarget(E.Range) && Q.IsReady())
            {
                Q.Cast();

            }
            if (target.IsValidTarget(E.Range) && W.IsReady())
            {
                W.Cast();
            }
            if (target.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(target);
            }
            //if (target.IsValidTarget(R.Range) && R.IsReady() && Player.Distance(target)>= R.Range)
              //  if (target.Health < Rdmg)
           // {
              //  R.Cast();
            }

        public static void Killsteal()
        {
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
            var igniteDmg = DamageLib.getDmg(target, DamageLib.SpellType.IGNITE);
            var RDmg = DamageLib.getDmg(target, DamageLib.SpellType.R);
             
            {
                if (AN.Item("UseRKS").GetValue<bool>() && target != null && R.IsReady() && Player.Distance(target)<= R.Range) {
                    if (target.Health < RDmg){
                        R.Cast();
                    }
                }
            }




        }

        }
    }
