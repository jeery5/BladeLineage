using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mod;
using TMPro;

#pragma warning disable IDE0017



namespace BladeLineage
{
    public class BladeLineageInit : ModInitializer
    {
        public override void OnInitializeMod()
        {
            base.OnInitializeMod();
            var harmony = new Harmony("LOR.BladeLineage.MOD");
            var method = typeof(BladeLineageInit).GetMethod("BookModel_SetXmlInfo");
            harmony.Patch(typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), null, new HarmonyMethod(method),null, null, null);
            BladeLineageInit.Path = System.IO.Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            BladeLineageInit.Language = GlobalGameManager.Instance.CurrentOption.language;
            BladeLineageInit.GetArtWorks(new DirectoryInfo(BladeLineageInit.Path + "/ArtWork"));
            BladeLineageInit.AddLocalize();
        }

        public static void AddLocalize()
        {
            Dictionary<string, BattleEffectText> dictionary = typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all).GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as Dictionary<string, BattleEffectText>;
            FileInfo[] files = new DirectoryInfo(BladeLineageInit.Path + "/Localize/" + BladeLineageInit.Language + "/EffectTexts").GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(files[i].FullName)))
                {
                    BattleEffectTextRoot battleEffectTextRoot = (BattleEffectTextRoot)new XmlSerializer(typeof(BattleEffectTextRoot)).Deserialize(stringReader);
                    for (int j = 0; j < battleEffectTextRoot.effectTextList.Count; j++)
                    {
                        BattleEffectText battleEffectText = battleEffectTextRoot.effectTextList[j];
                        dictionary.Add(battleEffectText.ID, battleEffectText);
                    }
                }
            }
            files = new DirectoryInfo(BladeLineageInit.Path + "/Localize/" + BladeLineageInit.Language + "/BattleDialogues").GetFiles();
            for (int k = 0; k < files.Length; k++)
            {
                using (StringReader stringReader2 = new StringReader(File.ReadAllText(files[k].FullName)))
                {
                    BattleDialogRoot battleDialogRoot = (BattleDialogRoot)new XmlSerializer(typeof(BattleDialogRoot)).Deserialize(stringReader2);
                    ((Dictionary<string, BattleDialogRoot>)typeof(BattleDialogXmlList).GetField("_dictionary", AccessTools.all).GetValue(Singleton<BattleDialogXmlList>.Instance))[battleDialogRoot.groupName] = battleDialogRoot;
                }
            }
        }

        public static void GetArtWorks(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    BladeLineageInit.GetArtWorks(directories[i]);
                }
            }
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                Texture2D texture2D = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture2D, File.ReadAllBytes(fileInfo.FullName));
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName);
                BladeLineageInit.ArtWorks[fileNameWithoutExtension] = value;
            }
        }
        public static void BookModel_SetXmlInfo(BookModel __instance, BookXmlInfo ____classInfo, ref List<DiceCardXmlInfo> ____onlyCards)
        {
            if (__instance.BookId.packageId == BladeLineageInit.PackageId)
            {
                foreach (int id in ____classInfo.EquipEffect.OnlyCard)
                {
                    DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId(BladeLineageInit.PackageId, id), false);
                    ____onlyCards.Add(cardItem);
                }
            }
        }
        public static string Path;
        public static string Language;
        public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
        public static string PackageId = "BladeLineage";
    }

    /// <summary>
    /// PassiveAbilities
    /// </summary>

    public class PassiveAbility_Coldness : PassiveAbilityBase
    {
        public override string debugDesc => "합 승리시 호흡 1 얻음";

        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 1);
        }
    }


    public class PassiveAbility_Unbending : PassiveAbilityBase
    {
        public override string debugDesc => "참격 위력+1, 참격 피해량+1";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail != BehaviourDetail.Slash) return;
            owner.ShowPassiveTypo(this);
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 1,
                dmg = 1
            });
        }
    }

    public class PassiveAbility_SwordPlay : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            if (battleUnitBuf.stack >= 13)
            {
                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(owner.faction);
                int num = 2; 
                while (aliveList.Count > 0 && num > 0)
                {
                    var battleUnitModel = RandomUtil.SelectOne(aliveList);
                    aliveList.Remove(battleUnitModel);
                    battleUnitModel.bufListDetail.AddReadyBuf(new BattleUnitBuf_SwordPlayPowerUp2());
                    num--;
                }
            }
            else if (battleUnitBuf.stack >= 8)
            {
                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(owner.faction);
                int num = 2;
                while (aliveList.Count > 0 && num > 0)
                {
                    var battleUnitModel = RandomUtil.SelectOne(aliveList);
                    aliveList.Remove(battleUnitModel);
                    battleUnitModel.bufListDetail.AddReadyBuf(new BattleUnitBuf_SwordPlayPowerUp1());
                    num--;
                }
            }
        }
    }
    public class PassiveAbility_Stand : PassiveAbilityBase
    {
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId(BladeLineageInit.PackageId, 10), false);
            List<BattleDiceBehavior> list = new List<BattleDiceBehavior>();
            int num = 0;
            foreach (DiceBehaviour diceBehaviour in cardItem.DiceBehaviourList)
            {
                BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
                battleDiceBehavior.behaviourInCard = diceBehaviour.Copy();
                battleDiceBehavior.SetIndex(num++);
                list.Add(battleDiceBehavior);
            }
            this.owner.cardSlotDetail.keepCard.AddBehaviours(cardItem, list);
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
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            if (battleUnitBuf.stack >= 7)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }

    public class DiceCardSelfAbility_Poise7Energy1Draw1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 보유한 호흡 7당 책장을 뽑음 (최대 3장)";

        public override void OnUseCard()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            int num = Mathf.Min(3, battleUnitBuf.stack / 6);
            base.owner.allyCardDetail.DrawCards(num);
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
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            if (battleUnitBuf.stack >= 7)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }

    public class DiceCardSelfAbility_Poise5Dmg1Energy2 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 빛 2 회복, 자신의 호흡이 5 이상이라면 이 책장의 모든 주사위 위력 +1";

        public override void OnUseCard()
        {
            base.owner.cardSlotDetail.RecoverPlayPointByCard(2);
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            if (battleUnitBuf.stack >= 5)
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
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            if (battleUnitBuf.stack >= 8)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }
    public class DiceCardSelfAbility_Poise8Dmg1Draw2Energy1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 보유한 호흡의 절반만큼 흐트러짐 회복";
        public override void OnUseCard()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            base.owner.breakDetail.RecoverBreak(battleUnitBuf.stack / 2);
        }
    }

    public class DiceCardSelfAbility_PoisePer7Power : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 보유한 호흡 수치 7당 이 책장의 모든 주사위 위력 증가 (최대 2)";

        public override void OnUseCard()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            int num = Mathf.Min(2, battleUnitBuf.stack / 7);
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = num
            });

        }
    }

    public class DiceCardSelfAbility_PoisePer4Power : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 보유한 호흡 수치 4당 이 책장의 모든 주사위 위력 증가 (최대 5)";

        public override void OnUseCard()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            int num = Mathf.Min(5, battleUnitBuf.stack / 4);
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = num
            });
        }
    }

    public class DiceCardSelfAbility_Poise2onUse : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 호흡 2 얻음";

        public override void OnUseCard()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
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

    public class DiceCardSelfAbility_Flash : DiceCardSelfAbilityBase
    {
        public static string Desc = "합 패배시 골단 사용";

        public override void OnLoseParrying()
        {
            this.isLoseParrying = true;
        }

        public override void OnEndBattle()
        {
            if (this.isLoseParrying)
            {
                BattleUnitModel target = this.card.target;
                BattleDiceCardModel card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId(BladeLineageInit.PackageId, 6), false));
                BattlePlayingCardDataInUnitModel battlePlayingCardDataInUnitModel = new BattlePlayingCardDataInUnitModel();
                battlePlayingCardDataInUnitModel.card = card;
                battlePlayingCardDataInUnitModel.owner = base.owner;
                battlePlayingCardDataInUnitModel.target = target;
                battlePlayingCardDataInUnitModel.targetSlotOrder = 0;
                Singleton<StageController>.Instance.AddAllCardListInBattle(battlePlayingCardDataInUnitModel, target, -1);
            }
            this.isLoseParrying = false;
        }
        private bool isLoseParrying = false;
    }

    /// <summary>
    /// DiceCardAbilitys
    /// </summary>

    public class DiceCardAbility_Bleeding8Paralysis5 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 다음막에 출혈 8, 마비 5 부여";

        public override string[] Keywords => new string[2] { "Paralysis_Keyword", "exhaust" };

        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.card.target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 5, base.owner);
            base.card.target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 8, base.owner);
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
        public static string Desc = "[합 승리] 호흡 4 얻고, 다음 주사위 위력 +2";

        public override void OnWinParrying()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 4);
            base.card.AddDiceAdder(DiceMatch.NextDice, 2);
        }
    }

    public class DiceCardAbility_Bleeding3Paralysis2 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 다음 막에 출혈 3, 마비 2 부여";

        public override void OnSucceedAttack()
        {
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 3, base.owner);
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 2, base.owner);
        }
    }

    public class DiceCardAbiltiy_Poise5onWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 호흡 5 얻음";

        public override void OnWinParrying()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 5);

        }
    }
    public class DiceCardAbility_Bleeding2 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 다음 막에 출혈 2 부여";

        public override void OnSucceedAttack()
        {
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 2, base.owner);
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

    public class DiceCardAbility_SlashPowerUpOnWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 다음 막에 참격 위력 증가 1 얻음";

        public override void OnWinParrying()
        {
            base.card.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.SlashPowerUp, 1, base.owner);
        }
    }

    public class DiceCardAbility_SlashPowerUp2OnWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 다음 막에 참격 위력 증가 2 얻음";

        public override void OnWinParrying()
        {
            base.card.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.SlashPowerUp, 2, base.owner);
        }
    }

    public class DiceCardAbility_Poise1 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 1 얻음";

        public override void OnSucceedAttack()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 1);
        }
    }

    public class DiceCardAbility_Poise2 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 2 얻음";

        public override void OnSucceedAttack()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 2);
        }
    }

    public class DiceCardAbility_Poise3Break : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 3 얻음. 보유한 호흡의 절반만큼 대상에게 흐트러짐 피해";

        public override void OnSucceedAttack()
        {
            var target = base.card.target;
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 3);
            target.TakeBreakDamage(battleUnitBuf.stack, 0, null, (AtkResist)3, 0);
        }
    }

    public class DiceCardAbility_Poise4 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 4 얻음";
        public override void OnSucceedAttack()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 4);
        }
    }
    
    public class DiceCardAbility_Poise5 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 5 얻음";

        public override void OnSucceedAttack()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 5);
        }
    }

    public class DiceCardAbility_Poise4onWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 호흡 4 얻음";

        public override void OnWinParrying()
        {
            var battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Poise);
            BattleUnitBuf_Poise.AddPoise(base.owner, 4);
        }
    }

    /// <summary>
    /// BattleUnitBufs
    /// </summary>

    public class BattleUnitBuf_SwordPlayPowerUp1 : BattleUnitBuf // 본국검 세법전수
    {
        protected override string keywordId => "SwordPlay1";
        public override void OnRoundEnd()
        {
            this.Destroy();
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail != BehaviourDetail.Slash && behavior.Detail != BehaviourDetail.Penetrate) return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 1
            });
        }
        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineageInit.ArtWorks["SwordPlay1"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
            this.stack = 0;
        }
    }

public class BattleUnitBuf_SwordPlayPowerUp2 : BattleUnitBuf // 본국검 자법전수
{
    protected override string keywordId => "SwordPlay2";

    public override void OnRoundEnd()
    {
        this.Destroy();
    }

    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (behavior.Detail != BehaviourDetail.Slash && behavior.Detail != BehaviourDetail.Penetrate) return;
        behavior.ApplyDiceStatBonus(new DiceStatBonus
        {
            power = 2
        });
    }

    public override void Init(BattleUnitModel owner)
    {
        base.Init(owner);
        typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineageInit.ArtWorks["SwordPlay2"]);
        typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
        this.stack = 0;
        }
    }

public class BattleUnitBuf_Poise : BattleUnitBuf
    {
        protected override string keywordId => "Poise_Buf";
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
            if (RandomUtil.valueForProb > 0.05f * this.stack) return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                dmgRate = 30
            });
            this.stack--;
        }

        public static BattleUnitBuf_Poise IshaveBuf(BattleUnitModel target, bool findready = false)
        {
            foreach (var battleUnitBuf in target.bufListDetail.GetActivatedBufList())
            {
                if (battleUnitBuf is BattleUnitBuf_Poise)
                {
                    return battleUnitBuf as BattleUnitBuf_Poise;
                }
            }

            if (findready)
            {
                foreach (var battleUnitBuf2 in target.bufListDetail.GetReadyBufList())
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
            var battleUnitBufPoise = BattleUnitBuf_Poise.IshaveBuf(target, true);
            if (battleUnitBufPoise != null)
            {
                battleUnitBufPoise.stack += stack;
                if (battleUnitBufPoise.stack > 20)
                {
                    battleUnitBufPoise.stack = 20;
                }
            }
            else
            {
                var battleUnitBufPoise2 = new BattleUnitBuf_Poise();
                battleUnitBufPoise2.stack = stack;
                battleUnitBufPoise2.Init(target);
                target.bufListDetail.AddBuf(battleUnitBufPoise2);
            }
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineageInit.ArtWorks["Poise"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
        }
    }
}
