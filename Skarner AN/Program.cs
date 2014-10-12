using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

using SharpDX;
using Color = System.Drawing.Color;

namespace ANSkarner
{
    class Program
    {
        public static string ChampName = "Skarner";
        public static Orbwalking.Orbwalker Orbwalker; //orbwalker

        public static Obj_AI_Base Player = ObjectManager.Player;
        //le spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        //items
        public static Items.Item DFG;

        //menu
        public static Menu AN;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 350);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 350);

            E.SetSkillshot(0.5f, 60, 1200f, false, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(E);
            SpellList.Add(R);

            //Big menu
            AN = new Menu("Animated's" + ChampName, ChampName, true);
            // le orbwalker
            AN.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(AN.SubMenu("Orbwalker"));
            //TS
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            AN.AddSubMenu(ts);
            //combo
            AN.AddSubMenu(new Menu("Combo", "Combo"));
            AN.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use W").SetValue(true));
            AN.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            AN.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E").SetValue(true));
            AN.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            AN.SubMenu("Combo").AddItem(new MenuItem("useUlt", "Use Ultimate On Target").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            //KillSteal
            AN.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            AN.SubMenu("KillSteal").AddItem(new MenuItem("useQKS", "Use Q").SetValue(true));
            AN.SubMenu("killSteal").AddItem(new MenuItem("useEKS", "Use E").SetValue(true));

            //Clear/Farm
           AN.AddSubMenu(new Menu("Farming", "Lane/Jungle Clear"));
            AN.SubMenu("Farming").AddItem(new MenuItem("useQF", "Use Q").SetValue(true));
            AN.SubMenu("Farming").AddItem(new MenuItem("useEF", "Use E").SetValue(true));
           AN.SubMenu("Farming").AddItem(new MenuItem("LaneClear", "LaneClear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //Misc
           AN.AddSubMenu(new Menu("Misc", "Misc"));
           AN.SubMenu("Misc").AddItem(new MenuItem("turretR", "Auto R Under turret").SetValue(true));




            AN.AddToMainMenu();

            //draw
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(Player.Position, Q.Range, Color.Azure);
            Utility.DrawCircle(Player.Position, E.Range, Color.Ivory);

        }
        static void Game_OnGameUpdate(EventArgs args)
        {
            var useQKS = AN.Item("useQKS").GetValue<bool>() && Q.IsReady();
            var useEKS = AN.Item("useEKS").GetValue<bool>() && E.IsReady();

            if (AN.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (AN.Item("useUlt").GetValue<KeyBind>().Active)
            {
                Ultimate();
            }

            if (AN.Item("LaneClear").GetValue<KeyBind>().Active)
            {
                Farming();
            }

            if (AN.Item("towerR").GetValue<bool>())
                TurretR();



        }
        private static void Combo()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;



            if (target.IsValidTarget(Q.Range) && Q.IsReady()) ;
            {
                Q.Cast(target);
            }

            if (target.IsValidTarget(E.Range) && E.IsReady()) ;
            {
                E.Cast(target);
            }

            if (target.IsValidTarget(E.Range) && E.IsReady()) ;
            {
                W.Cast();
            }


        }

        private static void Farming()
        {

            var useQ = AN.Item("UseQF").GetValue<bool>();
            var useE = AN.Item("UseEF").GetValue<bool>();

            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width, MinionTypes.All, MinionTeam.NotAlly);
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (useQ && Q.IsReady())
            {
                var qPos = Q.GetCircularFarmLocation(allMinionsQ);
                if (qPos.MinionsHit >= 2)
                    Q.Cast(qPos.Position, true);
            }

            if (useE && E.IsReady())
            {
              var ePos = E.GetLineFarmLocation(allMinionsE);
              if (ePos.MinionsHit >= 3)
                    E.Cast(ePos.Position, true);
            }


        }

        public static void Ultimate()
        {
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);

            if (target.IsValidTarget(R.Range) && R.IsReady());
            {
                R.Cast(target);
            }

        }
        public static void TurretR()
        {
        foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
     {
                if (enemy.IsEnemy && Player.Distance(enemy.ServerPosition) <= R.Range && enemy != null)
     {
        foreach (var turret in ObjectManager.Get<Obj_AI_Turret>())
     {
           if (turret != null && turret.IsValid && turret.IsAlly && turret.Health > 0)
          {
         if (Vector2.Distance(enemy.Position.To2D(), turret.Position.To2D()) < 950)
     {
        R.CastOnUnit(enemy);
             }
            }
               }
              }
           }
       }
    }
}