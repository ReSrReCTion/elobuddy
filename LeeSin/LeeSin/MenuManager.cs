using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace LeeSin
{
    public static class MenuManager
    {
        public static Menu AddonMenu;
        public static Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>();
        public static void Init()
        {
            var AddonName = Champion.AddonName;
            var Author = Champion.Author;
            AddonMenu = MainMenu.AddMenu(AddonName, AddonName + " by " + Author + " v6.4.0");
            AddonMenu.AddLabel(AddonName + " made by " + Author);

            SubMenu["Prediction"] = AddonMenu.AddSubMenu("예측", "Prediction3");
            SubMenu["Prediction"].AddGroupLabel("Q 설정");
            SubMenu["Prediction"].Add("QCombo", new Slider("콤보 떄리는기회(적중률?) %", 65));
            SubMenu["Prediction"].Add("QHarass", new Slider("견제 떄리는기회(적중률?) %", 70));

            //Combo
            SubMenu["Combo"] = AddonMenu.AddSubMenu("콤보 설정", "Combo");
            SubMenu["Combo"].Add("Q", new CheckBox("Q 사용", true));
            SubMenu["Combo"].Add("W", new CheckBox("GapClose W 사용", true));
            SubMenu["Combo"].Add("E", new CheckBox("E 사용", true));
            SubMenu["Combo"].Add("Smite", new CheckBox("강타 사용", false));
            SubMenu["Combo"].Add("Items", new CheckBox("공격 아이템 사용", true));
            var switcher = SubMenu["Combo"].Add("Switcher", new KeyBind("Combo Switcher", false, KeyBind.BindTypes.HoldActive, (uint)'K'));
            switcher.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (args.NewValue)
                {
                    var cast = GetSubMenu("Combo")["Mode"].Cast<Slider>();
                    if (cast.CurrentValue == cast.MaxValue)
                    {
                        cast.CurrentValue = 0;
                    }
                    else
                    {
                        cast.CurrentValue++;
                    }
                }
            };
            SubMenu["Combo"].AddStringList("Mode", "콤보 모드", new[] { "기본 콤보", "시작 콤보", "갱 콤보" }, 0);
            SubMenu["Combo"]["Mode"].Cast<Slider>().CurrentValue = 0; //E L I M I N A R

            SubMenu["Combo"].AddGroupLabel("기본 콤보");
            SubMenu["Combo"].Add("Normal.R", new CheckBox("R 타켓에게 사용", false));
            SubMenu["Combo"].Add("Normal.Ward", new CheckBox("와드 사용", false));
            SubMenu["Combo"].Add("Normal.Stack", new Slider("패시브 상용전에 다른스킬 사용", 1, 0, 2));
            SubMenu["Combo"].Add("Normal.W", new Slider("W 사용 체력 %", 25, 0, 100));
            SubMenu["Combo"].Add("Normal.R.Hit", new Slider("R 맞는 수 >=", 3, 1, 5));

            SubMenu["Combo"].AddSeparator();

            SubMenu["Combo"].AddGroupLabel("시작 콤보");
            SubMenu["Combo"].Add("Star.Ward", new CheckBox("와드 사용", true));
            SubMenu["Combo"].Add("Star.Stack", new Slider("패시브 사용전에 다른스킬 사용", 0, 0, 2));
            SubMenu["Combo"].AddStringList("Star.Mode", "시작 콤보 설정", new[] { "Q음파 R Q날아가기", "R Q음파 Q날아가기" }, 0);

            SubMenu["Combo"].AddSeparator();

            SubMenu["Combo"].AddGroupLabel("갱 콤보");
            SubMenu["Combo"].Add("Gank.R", new CheckBox("R 사용", true));
            SubMenu["Combo"].Add("Gank.Ward", new CheckBox("와드 사용", true));
            SubMenu["Combo"].Add("Gank.Stack", new Slider("패시브 사용전에 다른스킬 사용", 1, 0, 2));
            
            //Insec
            SubMenu["Insec"] = AddonMenu.AddSubMenu("인섹킥", "Insec");
            SubMenu["Insec"].Add("Key", new KeyBind("인섹킥 키설정 (이키가 맞는지 확인)", false, KeyBind.BindTypes.HoldActive, (uint)'R'));
            SubMenu["Insec"].Add("Object", new CheckBox("Q 사용 적 챔피언/미니언 만약 떄릴수없는타켓일떄", true));
            SubMenu["Insec"].AddSeparator(0);
            SubMenu["Insec"].Add("Flash.Return", new CheckBox("적챔피언 뒤로 플레쉬", false));
            SubMenu["Insec"].AddStringList("Priority", "우선순위", new[] { "와드점프 > 점멸", "점멸 > 와드점멸" }, 1);
            SubMenu["Insec"].AddStringList("Flash.Priority", "점멸 우선순위", new[] { "오직 R -> 점멸", "오직 점멸 -> R", "R -> 점멸 그리고 점멸 -> R" }, 0);
            SubMenu["Insec"].AddStringList("Position", "인섹킥 한후 포지션", new[] { "아군 선택 > 포지션 선택 > 포탑 > 가까운 아군 > 상황 포지션", "마우스 포지션", "상황 포지션" }, 0);
            SubMenu["Insec"].Add("DistanceBetweenPercent", new Slider("타켓과 와드의 거리 %", 20, 0, 100));
            SubMenu["Insec"].AddGroupLabel("팁");
            SubMenu["Insec"].AddLabel("아군에게 왼쪽클릭을 사용하여 아군선택.");
            SubMenu["Insec"].AddLabel("타켓에게 왼쪽클릭을 사용하여 타켓선택.");
            SubMenu["Insec"].AddLabel("자기위치에서 왼쪽클릭을 사용하여 위치선택.");
            
            SubMenu["Harass"] = AddonMenu.AddSubMenu("견제", "Harass");
            SubMenu["Harass"].Add("Q", new CheckBox("Q 사용", true));
            SubMenu["Harass"].Add("W", new CheckBox("W 도망 사용", true));
            SubMenu["Harass"].Add("E", new CheckBox("E 사용", true));

            SubMenu["Smite"] = AddonMenu.AddSubMenu("강타", "Smite");
            SubMenu["Smite"].Add("Q.Combo", new CheckBox("Q 사용 - 콤보 강타", true));
            SubMenu["Smite"].Add("Q.Harass", new CheckBox("Q사용 - 견제 강타", false));
            SubMenu["Smite"].Add("Q.Insec", new CheckBox("Q 사용 - 인섹킥 강타", true));
            SubMenu["Smite"].Add("DragonSteal", new CheckBox("강타 사용(용,바론)", true));
            SubMenu["Smite"].Add("KillSteal", new CheckBox("강타로 킬스틸 사용", true));
            
            SubMenu["JungleClear"] = AddonMenu.AddSubMenu("정글링", "JungleClear");
            SubMenu["JungleClear"].Add("Q", new CheckBox("Q 사용", true));
            SubMenu["JungleClear"].Add("W", new CheckBox("W 사용", true));
            SubMenu["JungleClear"].Add("E", new CheckBox("E 사용", true));
            SubMenu["JungleClear"].Add("Smite", new CheckBox("강타 사용(용,바론)", true));

            SubMenu["KillSteal"] = AddonMenu.AddSubMenu("킬스틸 설정", "KillSteal");
            SubMenu["KillSteal"].Add("Ward", new CheckBox("GapClose 와드점프", false));
            SubMenu["KillSteal"].Add("Q", new CheckBox("Q 사용", false));
            SubMenu["KillSteal"].Add("W", new CheckBox("GapClose W 사용", true));
            SubMenu["KillSteal"].Add("E", new CheckBox("E 사용", true));
            SubMenu["KillSteal"].Add("R", new CheckBox("R 사용", false));
            SubMenu["KillSteal"].Add("Ignite", new CheckBox("점화 사용", true));
            SubMenu["KillSteal"].Add("Smite", new CheckBox("강타 사용", true));

            SubMenu["Drawings"] = AddonMenu.AddSubMenu("표시", "Drawings");
            SubMenu["Drawings"].Add("Disable", new CheckBox("모든 표시 끄기", false));
            SubMenu["Drawings"].Add("Q", new CheckBox("Q 사거리 표시", true));
            SubMenu["Drawings"].Add("W", new CheckBox("W 사거리 표시", true));
            SubMenu["Drawings"].Add("E", new CheckBox("E 사거리 표시", false));
            SubMenu["Drawings"].Add("R", new CheckBox("R 사거리 표시", false));
            SubMenu["Drawings"].Add("Combo.Mode", new CheckBox("콤보모드 표시 사용", true));
            SubMenu["Drawings"].Add("Insec.Line", new CheckBox("인섹킥 라인 표시", true));
            SubMenu["Drawings"].Add("Target", new CheckBox("원형표시 타켓 사용(뭔지모름)", true));

            SubMenu["Flee"] = AddonMenu.AddSubMenu("도망/와드점프", "Flee");
            SubMenu["Flee"].Add("WardJump", new CheckBox("와드점프 사용", true));
            SubMenu["Flee"].Add("W", new CheckBox("오브젝트에 W사용(마우스위치)", true));

            SubMenu["Misc"] = AddonMenu.AddSubMenu("기타", "Misc");
            SubMenu["Misc"].Add("Interrupter", new CheckBox("대기시간있는 스킬에 R 사용(예:케이틀린Q,R)", true));
            SubMenu["Misc"].Add("Overkill", new Slider("오버킬 대미지 %", 10, 0, 100));
            SubMenu["Misc"].Add("R.Hit", new Slider("R 맞는수 >=", 3, 1, 5));

        }
        
        public static int GetSliderValue(this Menu m, string s)
        {
            if (m != null)
                return m[s].Cast<Slider>().CurrentValue;
            return -1;
        }
        public static bool GetCheckBoxValue(this Menu m, string s)
        {
            if (m != null)
                return m[s].Cast<CheckBox>().CurrentValue;
            return false;
        }
        public static bool GetKeyBindValue(this Menu m, string s)
        {
            if (m != null)
                return m[s].Cast<KeyBind>().CurrentValue;
            return false;
        }
        public static void AddStringList(this Menu m, string uniqueId, string displayName, string[] values, int defaultValue = 0)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = displayName + ": " + values[args.NewValue];
            };
        }
        public static Menu GetSubMenu(string s)
        {
            foreach (KeyValuePair<string, Menu> t in SubMenu)
            {
                if (t.Key.Equals(s))
                {
                    return t.Value;
                }
            }
            return null;
        }
        public static Menu MiscMenu
        {
            get
            {
                return GetSubMenu("Misc");
            }
        }
        public static Menu PredictionMenu
        {
            get
            {
                return GetSubMenu("Prediction");
            }
        }
        public static Menu DrawingsMenu
        {
            get
            {
                return GetSubMenu("Drawings");
            }
        }
        public static Menu SmiteMenu
        {
            get
            {
                return GetSubMenu("Smite");
            }
        }
    }
}
