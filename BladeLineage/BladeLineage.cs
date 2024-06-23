using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Battle.DiceAttackEffect;

using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;

using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Workshop;


namespace BladeLineage
{
    public class BladeLineage : ModInitializer
    {
        public override void OnInitializeMod()
        {
            base.OnInitializeMod();
            Harmony harmony = new Harmony("LOR.BladeLineage_MOD");
            MethodInfo method = typeof(BladeLineage).GetMethod("BookModel_SetXmlInfo");
            harmony.Patch(typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineage).GetMethod("BookModel_GetThumbSprite");
            harmony.Patch(typeof(BookModel).GetMethod("GetThumbSprite", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineage).GetMethod("UIStoryProgressPanel_SetStoryLine");
            harmony.Patch(typeof(UIStoryProgressPanel).GetMethod("SetStoryLine", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineage).GetMethod("UISpriteDataManager_GetStoryIcon");
            harmony.Patch(typeof(UISpriteDataManager).GetMethod("GetStoryIcon", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineage).GetMethod("UISettingInvenEquipPageListSlot_SetBooksData");
            harmony.Patch(typeof(UISettingInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineage).GetMethod("UIInvenEquipPageListSlot_SetBooksData");
            harmony.Patch(typeof(UIInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineage).GetMethod("UIStoryProgressPanel_SetStoryLine");
            harmony.Patch(typeof(UIStoryProgressPanel).GetMethod("SetStoryLine", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineage).GetMethod("UISpriteDataManager_GetStoryIcon");
            harmony.Patch(typeof(UISpriteDataManager).GetMethod("GetStoryIcon", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineage).GetMethod("UIInvitationRightMainPanel_SetCustomInvToggle");
            harmony.Patch(typeof(UIInvitationRightMainPanel).GetMethod("SetCustomInvToggle", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineage).GetMethod("UIBattleStoryInfoPanel_SetData");
            harmony.Patch(typeof(UIBattleStoryInfoPanel).GetMethod("SetData", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineage).GetMethod("UIStoryProgressPanel_SelectedSlot");
            harmony.Patch(typeof(UIStoryProgressPanel).GetMethod("SelectedSlot", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineage).GetMethod("UIBookStoryChapterSlot_SetEpisodeSlots");
            harmony.Patch(typeof(UIBookStoryChapterSlot).GetMethod("SetEpisodeSlots", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineage).GetMethod("UIBookStoryPanel_OnSelectEpisodeSlot");
            harmony.Patch(typeof(UIBookStoryPanel).GetMethod("OnSelectEpisodeSlot", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineage).GetMethod("UIInvitationRightMainPanel_SendInvitation");
            harmony.Patch(typeof(UIInvitationRightMainPanel).GetMethod("SendInvitation", AccessTools.all), new HarmonyMethod(method), null, null, null, null);



            BladeLineage.StoryInit = true;
            BladeLineage.Init = true;
            BladeLineage.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            BladeLineage.GetArtWorks(new DirectoryInfo(BladeLineage.path + "/ArtWork"));
        }

        public static void SlotCopying(UIStoryProgressIconSlot slot, UIStoryProgressIconSlot newslot)
        {
            Assembly assembly = Assembly.LoadFile(Application.dataPath + "/Managed/Assembly-CSharp.dll");
            newslot.currentStory = UIStoryLine.Rats;
            GameObject gameObject = newslot.transform.GetChild(1).gameObject;
            newslot.GetType().GetField("closeRect", AccessTools.all).SetValue(newslot, gameObject);
            Animator component = newslot.transform.GetChild(1).gameObject.GetComponent<Animator>();
            newslot.GetType().GetField("anim_closeRect", AccessTools.all).SetValue(newslot, component);
            CanvasGroup component2 = newslot.transform.GetChild(1).gameObject.GetComponent<CanvasGroup>();
            newslot.GetType().GetField("cg_closeRect", AccessTools.all).SetValue(newslot, component2);
            UnityEngine.UI.Image component3 = newslot.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
            newslot.GetType().GetField("img_highlighted", AccessTools.all).SetValue(newslot, component3);
            object obj = Activator.CreateInstance(assembly.GetType("UI.StoryIconSet"));
            GameObject gameObject2 = gameObject.transform.GetChild(1).gameObject;
            obj.GetType().GetField("root").SetValue(obj, gameObject2);
            Image component4 = gameObject2.transform.GetChild(0).gameObject.GetComponent<Image>();
            obj.GetType().GetField("img_iconbg").SetValue(obj, component4);
            Image component5 = gameObject2.transform.GetChild(1).gameObject.GetComponent<Image>();
            obj.GetType().GetField("img_iconFrame").SetValue(obj, component5);
            Image component6 = gameObject2.transform.GetChild(2).gameObject.GetComponent<Image>();
            obj.GetType().GetField("img_iconContent").SetValue(obj, component6);
            newslot.GetType().GetField("closeIconset", AccessTools.all).SetValue(newslot, obj);
            GameObject gameObject3 = newslot.transform.GetChild(2).gameObject;
            newslot.GetType().GetField("openRect", AccessTools.all).SetValue(newslot, gameObject3);
            GameObject gameObject4 = gameObject3.transform.GetChild(0).GetChild(0).gameObject;
            newslot.GetType().GetField("openFrameTarget", AccessTools.all).SetValue(newslot, gameObject4);
            object obj2 = Activator.CreateInstance(assembly.GetType("UI.StoryIconSet"));
            gameObject2 = gameObject3.transform.GetChild(1).gameObject;
            obj2.GetType().GetField("root").SetValue(obj2, gameObject2);
            component4 = gameObject2.transform.GetChild(0).gameObject.GetComponent<Image>();
            obj2.GetType().GetField("img_iconbg").SetValue(obj2, component4);
            obj2.GetType().GetField("img_iconFrame").SetValue(obj2, null);
            component6 = gameObject2.transform.GetChild(1).gameObject.GetComponent<Image>();
            obj2.GetType().GetField("img_iconContent").SetValue(obj2, component6);
            newslot.GetType().GetField("openIconset", AccessTools.all).SetValue(newslot, obj2);
            Array array = Array.CreateInstance(assembly.GetType("UI.storyIconLevel"), 3);
            object obj3 = Activator.CreateInstance(assembly.GetType("UI.storyIconLevel"));
            gameObject2 = gameObject3.transform.GetChild(3).GetChild(0).gameObject;
            obj3.GetType().GetField("root").SetValue(obj3, gameObject2);
            component4 = gameObject2.transform.GetChild(0).gameObject.GetComponent<Image>();
            obj3.GetType().GetField("img_iconbg").SetValue(obj3, component4);
            component5 = gameObject2.transform.GetChild(1).gameObject.GetComponent<Image>();
            obj3.GetType().GetField("img_iconFrame").SetValue(obj3, component5);
            TextMeshProUGUI component7 = gameObject2.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
            obj3.GetType().GetField("txt_iconContent").SetValue(obj3, component7);
            UICustomSelectable component8 = gameObject2.transform.GetChild(3).gameObject.GetComponent<UICustomSelectable>();
            obj3.GetType().GetField("selectable").SetValue(obj3, component8);
            object obj4 = Activator.CreateInstance(assembly.GetType("UI.storyIconLevel"));
            gameObject2 = gameObject3.transform.GetChild(3).GetChild(1).gameObject;
            obj4.GetType().GetField("root").SetValue(obj4, gameObject2);
            component4 = gameObject2.transform.GetChild(0).gameObject.GetComponent<Image>();
            obj4.GetType().GetField("img_iconbg").SetValue(obj4, component4);
            component5 = gameObject2.transform.GetChild(1).gameObject.GetComponent<Image>();
            obj4.GetType().GetField("img_iconFrame").SetValue(obj4, component5);
            component7 = gameObject2.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
            obj4.GetType().GetField("txt_iconContent").SetValue(obj4, component7);
            component8 = gameObject2.transform.GetChild(3).gameObject.GetComponent<UICustomSelectable>();
            obj4.GetType().GetField("selectable").SetValue(obj3, component8);
            object obj5 = Activator.CreateInstance(assembly.GetType("UI.storyIconLevel"));
            gameObject2 = gameObject3.transform.GetChild(3).GetChild(2).gameObject;
            obj5.GetType().GetField("root").SetValue(obj5, gameObject2);
            component4 = gameObject2.transform.GetChild(0).gameObject.GetComponent<Image>();
            obj5.GetType().GetField("img_iconbg").SetValue(obj5, component4);
            component5 = gameObject2.transform.GetChild(1).gameObject.GetComponent<Image>();
            obj5.GetType().GetField("img_iconFrame").SetValue(obj5, component5);
            component7 = gameObject2.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
            obj5.GetType().GetField("txt_iconContent").SetValue(obj5, component7);
            component8 = gameObject2.transform.GetChild(3).gameObject.GetComponent<UICustomSelectable>();
            obj5.GetType().GetField("selectable").SetValue(obj3, component8);
            array.SetValue(obj3, 0);
            array.SetValue(obj4, 1);
            array.SetValue(obj5, 2);
            newslot.GetType().GetField("iconset_Level", AccessTools.all).SetValue(newslot, array);
            List<GameObject> list = new List<GameObject>();
            GameObject gameObject5 = ((List<GameObject>)slot.GetType().GetField("connectLineList", AccessTools.all).GetValue(slot))[0];
            GameObject item = UnityEngine.Object.Instantiate<GameObject>(gameObject5, gameObject5.transform.parent);
            list.Add(item);
            newslot.GetType().GetField("connectLineList", AccessTools.all).SetValue(newslot, list);
            newslot.GetType().GetField("isChapterIcon", AccessTools.all).SetValue(newslot, false);
            newslot.GetType().GetField("currentGrade", AccessTools.all).SetValue(newslot, Grade.grade1);
            GameObject gameObject6 = null;
            for (int i = 0; i < gameObject3.transform.childCount; i++)
            {
                if (gameObject3.transform.GetChild(i).gameObject.name.Contains("[text]chaptername"))
                {
                    gameObject6 = gameObject3.transform.GetChild(i).gameObject;
                    break;
                }
            }
            TextMeshProUGUI value = null;
            if (gameObject6 != null)
            {
                value = gameObject6.GetComponent<TextMeshProUGUI>();
            }
            newslot.GetType().GetField("txt_openChapterName", AccessTools.all).SetValue(newslot, value);
            TextMeshProUGUI component9 = newslot.transform.GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            newslot.GetType().GetField("txt_chaptergrade", AccessTools.all).SetValue(newslot, component9);
        }

        public static void GetArtWorks(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    BladeLineage.GetArtWorks(directories[i]);
                }
            }
            foreach (System.IO.FileInfo fileInfo in dir.GetFiles())
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                BladeLineage.ArtWorks[fileNameWithoutExtension] = value;
            }
        }

        public static bool UISpriteDataManager_GetStoryIcon(string story, ref UIIconManager.IconSet __result)
        {
            bool flag = BladeLineage.ArtWorks.ContainsKey(story);
            bool flag2 = flag;
            bool result;
            if (flag2)
            {
                __result = new UIIconManager.IconSet
                {
                    type = story,
                    icon = BladeLineage.ArtWorks[story],
                    iconGlow = BladeLineage.ArtWorks[story]
                };
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

        public static void CreateStory(UIStoryProgressPanel __instance)
        {
            bool flag = false;
            bool flag2 = false;
            UIStoryProgressIconSlot uiStoryProgressIconSlot = null;
            UIStoryProgressIconSlot uiStoryProgressIconSlot2 = new UIStoryProgressIconSlot();
            foreach(UIStoryProgressIconSlot uiStoryProgressIconSlot3 in ((List<UIStoryProgressIconSlot>)__instance.GetType().GetField("iconList", AccessTools.all).GetValue(__instance)))
            {
                if (uiStoryProgressIconSlot3.currentStory == UIStoryLine.RCorp1)
                {
                    flag = true;
                    uiStoryProgressIconSlot = uiStoryProgressIconSlot3;
                }
                if (uiStoryProgressIconSlot3.currentStory == UIStoryLine.LiuAssociation1)
                {
                    flag2 = true;
                    uiStoryProgressIconSlot2 = uiStoryProgressIconSlot3;
                }
                if (flag && flag2)
                {
                    UIStoryProgressIconSlot uiStoryProgressIconSlot4 = UnityEngine.Object.Instantiate<UIStoryProgressIconSlot>(uiStoryProgressIconSlot, uiStoryProgressIconSlot.transform.parent);
                    BladeLineage.SlotCopying(uiStoryProgressIconSlot, uiStoryProgressIconSlot4);
                    uiStoryProgressIconSlot4.Initialized(__instance);
                    uiStoryProgressIconSlot4.transform.localPosition -= new Vector3(200f, 0f);
                    List<GameObject> list = (List<GameObject>)uiStoryProgressIconSlot4.GetType().GetField("connectLineList", AccessTools.all).GetValue(uiStoryProgressIconSlot4);
                    list[0].transform.localPosition += new Vector3(-300f, 130f);
                    list[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    List<StageClassInfo> list2 = Singleton<StageClassInfoList>.Instance.GetAllDataList().FindAll((StageClassInfo x) => x.storyType == "blade1");
                    uiStoryProgressIconSlot4.SetSlotData(list2);
                    BladeLineage.Storyslots[list2] = uiStoryProgressIconSlot4;
                    break;
                }
            }
        }

        public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
        public static Dictionary<List<StageClassInfo>, UIStoryProgressIconSlot> Storyslots;
        public static UIPhase uIPhase;
        public static string packageId = "BladeLineage";
        public static string path;
        public static string language;
        public static bool Init;
        public static bool StoryInit;
        public static bool BladeStory;
        public static LorId storyId;

    }

    /// <summary>
    /// PassiveAbilitys For All
    /// </summary>
    
    public class PassiveAbility_GhostBlade : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            switch (behavior.Detail)
            {
                case BehaviourDetail.Slash:
                    owner.battleCardResultLog?.SetPassiveAbility(this);
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        power = 4,
                        dmgRate = 10
                    });
                    break;
                case BehaviourDetail.Penetrate:
                case BehaviourDetail.Hit:
                    owner.battleCardResultLog?.SetPassiveAbility(this);
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        power = -3
                    });
                    break;
            }
        }
    }

    public class PassiveAbility_CondensedSadness : PassiveAbilityBase
    {
        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Sadness);
            BattleUnitBuf_Sadness.AddSadness(base.owner, dmg);
        }
    }


    /// <summary>
    /// PassiveAbilitys For Librarian
    /// </summary> 

    public class PassiveAbiltiy_SwordOfTheHomeland : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(owner.faction);
            int num = 2;
            while (aliveList.Count > 0 && num > 0)
            {
                BattleUnitModel battleUnitModel = RandomUtil.SelectOne(aliveList);
                aliveList.Remove(battleUnitModel);
                battleUnitModel.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.SlashPowerUp, 1);
                num--;
            }
        }
    }

    public class PassiveAbility_StrongWilled : PassiveAbilityBase
    {
        private int _activated;

        private bool _recoverBreak;

        public int activated => _activated;

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            bool result = false;
            if (owner.UnitData.floorBattleData.param1 == 0 && (int)(owner.hp - (float)dmg) < 1)
            {
                owner.battleCardResultLog?.SetTakeDamagedEvent(PrintEffect);
                owner.battleCardResultLog?.SetPassiveAbility(this);
                owner.RecoverHP(owner.MaxHp / 2);
                result = true;
                _activated++;
                owner.UnitData.floorBattleData.param1 = _activated;
                _recoverBreak = true;
            }
            return result;
        }

        public override bool BeforeTakeBreakDamage(BattleUnitModel attacker, int dmg)
        {
            bool result = false;
            if(owner.UnitData.floorBattleData.param1==0 && (int)(owner.breakDetail.breakLife -(float)dmg) < 1)
            {
                owner.battleCardResultLog?.SetTakeDamagedEvent(PrintEffect);
                owner.battleCardResultLog?.SetPassiveAbility(this);
                owner.RecoverBreakLife(owner.MaxBreakLife / 2);
                result = true;
                _activated++;
                owner.UnitData.floorBattleData.param1 = _activated;
                _recoverBreak = true;
            }
            return result;
        }

        private void PrintEffect()
        {
        }
    }

    /// <summary>
    /// PassiveAbilitys For Enemy
    /// </summary> 
    
    public class PassiveAbility_DistortedEmotion : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            int emotionLevel = base.owner.emotionDetail.EmotionLevel;
            if (IsAttackDice(behavior.Detail) && emotionLevel >= 3)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -1,
                });

                if(IsAttackDice(behavior.Detail) && emotionLevel >= 5)
                {
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        power = -3,
                    });
                }
            }
        }
    }

    public class PassiveAbility_LostedHonor : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            switch (behavior.Detail)
            {
                case BehaviourDetail.Slash:
                    owner.battleCardResultLog?.SetPassiveAbility(this);
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        power = 1,
                    });
                    break;
            }

            int emotionLevel = base.owner.emotionDetail.EmotionLevel;

            if (emotionLevel >= 5)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -2,
                });

            }
        }
    }

    public class PassiveAbility_MemoriesOfTheDay : PassiveAbilityBase
    {
        public override void OnKill(BattleUnitModel target)
        {
            if (target.faction != owner.faction)
            {
                owner.battleCardResultLog?.SetPassiveAbility(this);
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 3, owner);
            }
        }
    }

    /// <summary>
    /// DiceCardSelfAbilitys
    /// </summary>

    public class DiceCardSelfAbility_Poise7Dmg1Draw1Page : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 책장을 1장 뽑음, 자신의 호흡이 7 이상이라면 이 책장의 모든 주사위 위력 +1";

        public override void OnUseCard()
        {
            base.owner.allyCardDetail.DrawCards(1);
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            if (battleUnitBuf.stack >= 7)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }

    public class DiceCardSelfAbility_Poise7Dmg1Energy1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 빛 1 회복, 자신의 호흡이 7 이상이라면 이 책장의 모든 주사위 위력 +1";

        public override void OnUseCard()
        {
            base.owner.cardSlotDetail.RecoverPlayPointByCard(1);
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            if (battleUnitBuf.stack >= 7)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }

    public class DiceCardSelfAbility_Poise8Dmg1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "자신의 호흡이 8 이상이라면 이 책장의 모든 주사위 위력 +1";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            if (battleUnitBuf.stack >= 8)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }
    
    public class DiceCardSelfAbility_PowerMinus12 : DiceCardSelfAbilityBase
    {
        public static string Desc = "감정 단계가 4 이상일 때, 합을 하는 동안 이 책장의 모든 주사위 위력 -12";

        public override void OnUseCard()
        {
            int emotionLevel = base.owner.emotionDetail.EmotionLevel;
            if (emotionLevel >= 4)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = -12
                });
            }
        }
    }

    public class DiceCardSelfAbility_PoisePer7Power : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 보유한 호흡 수치 7당 이 책장의 모든 주사위 위력 증가 (최대 2)";

        public override void OnUseCard()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = battleUnitBuf.stack / 7
            });

        }
    }

    public class DiceCardSelfAbility_PoisePer4Power : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 보유한 호흡 수치 4당 이 책장의 모든 주사위 위력 증가 (최대 5)";

        public override void OnUseCard()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = battleUnitBuf.stack / 4
            });
        }
    }

    public class DiceCardSelfAbility_Poise2onUse : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 호흡 2를 얻음";

        public override void OnUseCard()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 2);
        }
    }

    public class DiceCardSelfAbility_Energy2onUse : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 빛 2 회복";

        public override void OnUseCard()
        {
            base.owner.cardSlotDetail.RecoverPlayPoint(2);
        }
    }

    public class DiceCardSelfAbility_Rending : DiceCardSelfAbilityBase
    {
        public static string Desc = "[전투 시작] 무작위 아군 2명에게 '본국검 - 세법 전수' 부여";

        public override void OnStartBattle()
        {
            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
            List<BattleUnitModel> list = new List<BattleUnitModel>();
            aliveList.RemoveAll((BattleUnitModel x) => x == base.owner);
            int num = 2;
            while (aliveList.Count > 0 && num > 0)
            {
                BattleUnitModel item = RandomUtil.SelectOne(aliveList);
                aliveList.Remove(item);
                list.Add(item);
                num--;
            }
            foreach (BattleUnitModel item2 in list)
            {
                BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Rending);
                item2.bufListDetail.AddReadyBuf(new BattleUnitBuf_Rending());
            }
        }
    }

    public class DiceCardSelfAbility_Penetrating : DiceCardSelfAbilityBase
    {
        public static string Desc = "[전투 시작] 무작위 아군 2명에게 '본국검 - 자법 전수' 부여";

        public override void OnStartBattle()
        {
            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
            List<BattleUnitModel> list = new List<BattleUnitModel>();
            aliveList.RemoveAll((BattleUnitModel x) => x == base.owner);
            int num = 2;
            while (aliveList.Count > 0 && num > 0)
            {
                BattleUnitModel item = RandomUtil.SelectOne(aliveList);
                aliveList.Remove(item);
                list.Add(item);
                num--;
            }
            foreach (BattleUnitModel item2 in list)
            {
                BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Penetrating);
                item2.bufListDetail.AddReadyBuf(new BattleUnitBuf_Penetrating());
            }
        }
    }

    /// <summary>
    /// DiceCardAbilitys
    /// </summary>
    
    public class DiceCardAbility_Poise1atk : DiceCardAbilityBase
    {
        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 1);
        }
    }

    public class DiceCardAbility_Bleeding3Paralysis1 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 출혈 3과 마비 1 부여";

        public override void OnSucceedAttack(BattleUnitModel target)
        {
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 3, base.owner);
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 1, base.owner);
        }
    }
    
    public class DiceCardAbility_Paralysis5 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 마비 5 부여";

        public override void OnSucceedAttack(BattleUnitModel target)
        {
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 1, base.owner);
        }
    }
    
    public class DiceCardAbility_Bleeding2 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 출혈 2 부여";

        public override void OnSucceedAttack(BattleUnitModel target)
        {
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 2, base.owner); 
        }
    }
    
    public class DiceCardAbility_Energy2 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 빛 2 회복";

        public override void OnSucceedAttack()
        {
            base.owner.cardSlotDetail.RecoverPlayPointByCard(2);
        }
    }

    public class DiceCardAbility_Poise4AddNextDice2onWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 호흡 4를 얻고, 다음 주사위 위력 +2";

        public override void OnWinParrying()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 4);
            base.card.AddDiceAdder(DiceMatch.NextDice, 2);
        }
    }

    public class DiceCardAbility_AddCard : DiceCardAbilityBase
    {
        public static string Desc = "[합 패배] 자신의 패에 '골단' 추가";

        public override void OnLoseParrying()
        {
            base.owner.allyCardDetail.AddNewCard(new LorId(BladeLineage.packageId, 7), false);
        }
    }

    public class DiceCardAbility_Bleeding3Paralysis5 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 다음 막에 출혈 3, 마비 5 부여";

        public override void OnSucceedAttack()
        {
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 3, base.owner);
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 5, base.owner);
        }
    }

    public class DiceCardAbiltiy_Poise5onWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 호흡 5를 얻음";

        public override void OnWinParrying()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 5);

        }
    }

    public class DiceCardAbility_SlashPowerUp_OnWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 다음 막에 참격 위력 증가 1을 얻음";

        public override void OnWinParrying()
        {
            base.card.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.SlashPowerUp, 1, base.owner);
        }
    }

    public class DiceCardAbility_Poise1 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 1를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 1);
        }
    }

    public class DiceCardAbility_Poise2 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 2를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 2);
        }
    }

    public class DiceCardAbility_Poise3 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 3를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 3);
        }
    }

    public class DiceCardAbility_Poise4 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 4를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 4);
        }
    }

    public class DiceCardAbility_Poise4onWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 호흡 4를 얻음";

        public override void OnWinParrying()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 4);
        }
    }

    /// <summary>
    /// BattleUnitBufs
    /// </summary>

    public class BattleUnitBuf_Rending : BattleUnitBuf
    {
        protected override string keywordId
        {
            get
            {
                return "Rending";
            }
        }

        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            this.Destroy();
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                min = 2
            });
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineage.ArtWorks["SwordManship1"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
            this.stack = 0;
        }
    }

    public class BattleUnitBuf_Penetrating : BattleUnitBuf
    {
        protected override string keywordId
        {
            get
            {
                return "Penetrating";
            }
        }

        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            this.Destroy();
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                max = 2
            });
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineage.ArtWorks["SwordManship2"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
            this.stack = 0;
        }
    }

    public class BattleUnitBuf_Poise : BattleUnitBuf 
    {
        protected override string keywordId
        {
            get
            {   
                return "Respiration_Buf";
            }
        }

        public override void OnRoundEnd()
        {
            this.stack--;
            if (this.stack <= 0)
            {
                this.Destroy();
                return;
            }
            if (this._owner.IsImmune(this.bufType))
            {
                this.Destroy();
            }
        }

        public override void BeforeGiveDamage(BattleDiceBehavior behavior)
        {
            base.BeforeGiveDamage(behavior);
            if (RandomUtil.valueForProb < 0.05f * this.stack)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmgRate = 40
                });
                this.stack--;
            }
        }

        public static BattleUnitBuf_Poise IshaveBuf(BattleUnitModel target, bool findready = false)
        {
            foreach (BattleUnitBuf battleUnitBuf in target.bufListDetail.GetActivatedBufList())
            {
                if (battleUnitBuf is BattleUnitBuf_Poise)
                {
                    return battleUnitBuf as BattleUnitBuf_Poise;
                }
            }

            if (findready)
            {
                foreach (BattleUnitBuf battleUnitBuf2 in target.bufListDetail.GetReadyBufList())
                {
                    if (battleUnitBuf2 is BattleUnitBuf_Poise)
                    {
                        return battleUnitBuf2 as BattleUnitBuf_Poise;
                    }
                }
            }
            return null;
        }

        public static void AddPoise(BattleUnitModel target, int stack)
        {
            BattleUnitBuf_Poise battleUnitBuf_Poise = BattleUnitBuf_Poise.IshaveBuf(target, true);
            if (battleUnitBuf_Poise != null)
            {
                battleUnitBuf_Poise.stack += stack;
                if (battleUnitBuf_Poise.stack > 20)
                {
                    battleUnitBuf_Poise.stack = 20;
                    return;
                }
            }
            else
            {
                BattleUnitBuf_Poise battleUnitBuf_Poise2 = new BattleUnitBuf_Poise();
                battleUnitBuf_Poise2.stack = stack;
                battleUnitBuf_Poise2.Init(target);
                target.bufListDetail.AddBuf(battleUnitBuf_Poise2);
            }
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineage.ArtWorks["Respiration"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
        }
    }

    public class BattleUnitBuf_Sadness : BattleUnitBuf
    {
        public static BattleUnitBuf_Sadness IshaveBuf(BattleUnitModel target, bool findready = false)
        {
            foreach (BattleUnitBuf battleUnitBuf in target.bufListDetail.GetActivatedBufList())
            {
                if (battleUnitBuf is BattleUnitBuf_Sadness)
                {
                    return battleUnitBuf as BattleUnitBuf_Sadness;
                }
            }
            if (findready)
            {
                foreach (BattleUnitBuf battleUnitBuf2 in target.bufListDetail.GetReadyBufList())
                {
                    if (battleUnitBuf2 is BattleUnitBuf_Sadness)
                    {
                        return battleUnitBuf2 as BattleUnitBuf_Sadness;
                    }
                }
            }
            return null;
        }

        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            base.OnTakeDamageByAttack(atkDice, dmg);
            this.Destroy();
        }

        public override void OnSuccessAttack(BattleDiceBehavior behavior)
        {
            base.OnSuccessAttack(behavior);
            this._owner.RecoverHP(this.stack);
            this.Destroy();
        }

        public static void AddSadness(BattleUnitModel target, int stack)
        {
            BattleUnitBuf_Sadness battleUnitBuf_Sadness = BattleUnitBuf_Sadness.IshaveBuf(target, true);
            if (battleUnitBuf_Sadness != null)
            {
                battleUnitBuf_Sadness.stack += stack;
                if (battleUnitBuf_Sadness.stack > 50)
                {
                    battleUnitBuf_Sadness.stack = 50;
                    return;
                }
            }
            else
            {
                BattleUnitBuf_Sadness battleUnitBuf_Sadness2 = new BattleUnitBuf_Sadness();
                battleUnitBuf_Sadness2.stack = stack;
                battleUnitBuf_Sadness2.Init(target);
                target.bufListDetail.AddBuf(battleUnitBuf_Sadness2);
            }
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineage.ArtWorks["Sadness"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
        }
    }
}
