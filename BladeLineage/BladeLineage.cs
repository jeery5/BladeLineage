using HarmonyLib;
using LOR_DiceSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UI;
using UnityEngine;
#pragma warning disable IDE0017



namespace BladeLineageInitializer
{
    public class BladeLineageInitializer : ModInitializer
    {
        public override void OnInitializeMod()
        {
            base.OnInitializeMod();
            Harmony harmony = new Harmony("LOR.BladeLineage_MOD");
            MethodInfo method = typeof(BladeLineageInitializer).GetMethod("BookModel_SetXmlInfo");
            harmony.Patch(typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("BookModel_GetThumbSprite");
            harmony.Patch(typeof(BookModel).GetMethod("GetThumbSprite", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIStoryProgressPanel_SetStoryLine");
            harmony.Patch(typeof(UIStoryProgressPanel).GetMethod("SetStoryLine", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UISpriteDataManager_GetStoryIcon");
            harmony.Patch(typeof(UISpriteDataManager).GetMethod("GetStoryIcon", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UISettingInvenEquipPageListSlot_SetBooksData");
            harmony.Patch(typeof(UISettingInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIInvenEquipPageListSlot_SetBooksData");
            harmony.Patch(typeof(UIInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIStoryProgressPanel_SetStoryLine");
            harmony.Patch(typeof(UIStoryProgressPanel).GetMethod("SetStoryLine", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UISpriteDataManager_GetStoryIcon");
            harmony.Patch(typeof(UISpriteDataManager).GetMethod("GetStoryIcon", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIInvitationRightMainPanel_SetCustomInvToggle");
            harmony.Patch(typeof(UIInvitationRightMainPanel).GetMethod("SetCustomInvToggle", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIBattleStoryInfoPanel_SetData");
            harmony.Patch(typeof(UIBattleStoryInfoPanel).GetMethod("SetData", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIStoryProgressPanel_SelectedSlot");
            harmony.Patch(typeof(UIStoryProgressPanel).GetMethod("SelectedSlot", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIBookStoryChapterSlot_SetEpisodeSlots");
            harmony.Patch(typeof(UIBookStoryChapterSlot).GetMethod("SetEpisodeSlots", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIBookStoryPanel_OnSelectEpisodeSlot");
            harmony.Patch(typeof(UIBookStoryPanel).GetMethod("OnSelectEpisodeSlot", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BladeLineageInitializer).GetMethod("UIInvitationRightMainPanel_SendInvitation");
            harmony.Patch(typeof(UIInvitationRightMainPanel).GetMethod("SendInvitation", AccessTools.all), new HarmonyMethod(method), null, null, null, null);


            BladeLineageInitializer.GetArtWorks(new DirectoryInfo(BladeLineageInitializer.path + "/ArtWork"));
            BladeLineageInitializer.StoryInit = true;
            BladeLineageInitializer.Init = true;
            BladeLineageInitializer.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        }

        public static void GetArtWorks(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    BladeLineageInitializer.GetArtWorks(directories[i]);
                }
            }
            foreach (System.IO.FileInfo fileInfo in dir.GetFiles())
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                BladeLineageInitializer.ArtWorks[fileNameWithoutExtension] = value;
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

                if (IsAttackDice(behavior.Detail) && emotionLevel >= 5)
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
        public static string Desc = "적 처치시 힘 3을 얻음";

        public override void OnKill(BattleUnitModel target)
        {
            if (target.faction != owner.faction)
            {
                owner.battleCardResultLog?.SetPassiveAbility(this);
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 3, owner);
            }
        }
    }

    public class PassiveAbility_ColdBlood : PassiveAbilityBase
    {
        public static string Desc = "합 승리시 호흡 횟수 1 증가";

        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            base.OnWinParrying(behavior);
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 1);
        }
    }

    /// <summary>
    /// DiceCardSelfAbilitys
    /// </summary>

    public class DiceCardSelfAbility_Poise7Dmg1Draw1 : DiceCardSelfAbilityBase
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

    public class DiceCardAbility_Bleeding3Paralysis2 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 출혈 3과 마비 2 부여";

        public override void OnSucceedAttack(BattleUnitModel target)
        {
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 3, base.owner);
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 2, base.owner);
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

    public class DiceCardAbility_AddCard : DiceCardAbilityBase // 고장남
    {
        public static string Desc = "[합 패배] 자신의 패에 '골단' 추가";

        public override void OnLoseParrying()
        {
            base.owner.allyCardDetail.AddNewCard(new LorId(BladeLineageInitializer.packageId, 7), false);
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
    
    public class DiceCardAbility_Bleeding3 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 다음 막에 출혈 3 부여";

        public override void OnSucceedAttack()
        {
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 3, base.owner);
        }
    }

    public class DiceCardAbility_Bleeding5 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 다음 막에 출혈 5 부여";

        public override void OnSucceedAttack()
        {
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 5, base.owner);
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

    public class DiceCardAbility_SlashPowerUp2_OnWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 다음 막에 참격 위력 증가 2을 얻음";

        public override void OnWinParrying()
        {
            base.card.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.SlashPowerUp, 2, base.owner);
        }
    }

    public class DiceCardAbility_PenetratePowerUp_OnWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 다음 막에 참격 위력 증가 1을 얻음";

        public override void OnWinParrying()
        {
            base.card.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.PenetratePowerUp, 1, base.owner);
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
    
    public class DiceCardAbility_Poise5 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 5를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 5);
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
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineageInitializer.ArtWorks["SwordManship1"]);
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
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineageInitializer.ArtWorks["SwordManship2"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
            this.stack = 0;
        }
    }
    
    // 호흡딜 1.5배로 버프 - 추가의견
    
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
                    dmgRate = 50
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
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineageInitializer.ArtWorks["Respiration"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
        }
    }
}
