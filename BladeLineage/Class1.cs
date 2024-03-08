using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Battle.DiceAttackEffect;
using Battle.CreatureEffect;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using Sound;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Workshop;
using HarmonyLib;
using static UnityEngine.GraphicsBuffer;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Data.SqlTypes;
using System.ComponentModel;
using UnityEngine.Assertions;
using System.Security.AccessControl;
using BTAI;
using Spine;
using static UnityEngine.UI.CanvasScaler;
using Steamworks;
using System.Security.Policy;
using HyperCard;


namespace BladeLineage
{
    public class BladeLineageInitializer : ModInitializer
    {
        public override void OnInitializeMod()
        {
            try
            {
                base.OnInitializeMod();
                Harmony harmorny = new Harmony("LOR_BladeLineage_MOD");
                MethodInfo method = typeof(BladeLineageInitializer).GetMethod("BookModel_SetXmlInfo");
                harmorny.Patch(typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
                BladeLineageInitializer.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
                BladeLineageInitializer.language = GlobalGameManager.Instance.CurrentOption.language;
                BladeLineageInitializer.GetArtWorks(new DirectoryInfo(BladeLineageInitializer.path + "/ArtWork"));
                BladeLineageInitializer.AddLocalize();
            }
            catch (Exception ex)
            {
                using (StreamWriter streamWriter = File.AppendText(Application.dataPath + "/Mods/BladeLineageError.txt"))
                {
                    TextWriter textWriter = streamWriter;
                    Exception ex2 = ex;
                    textWriter.WriteLine(((ex2 != null) ? ex2.ToString() : null) + ex.StackTrace);
                }
            }
        }

        public static void BookModel_SetXmlInfo(BookModel __instance,BookXmlInfo ____classInfo, ref List<DiceCardXmlInfo> ____onlyCards)
        {
            if (__instance.BookId.packageId == BladeLineageInitializer.packageId)
            {
                foreach (int id in ____classInfo.EquipEffect.OnlyCard)
                {
                    DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId(BladeLineageInitializer.packageId, id), false);
                    ____onlyCards.Add(cardItem);
                }
            }
        }

        public static void AddLocalize()
        {
            Dictionary<string, BattleEffectText> dictionary = typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all).GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as Dictionary<string, BattleEffectText>;
            System.IO.FileInfo[] files = new DirectoryInfo(BladeLineageInitializer.path + "/Localize/" + BladeLineageInitializer.language + "/EffectTexts").GetFiles();
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
        public static string packageId = "BladeLineage";
        public static string path;
        public static string language;
        public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();  
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

    public class PassiveAbiltiy_SwordsManship : PassiveAbilityBase
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
                owner.RecoverHP(50);
                result = true;
                _activated++;
                owner.UnitData.floorBattleData.param1 = _activated;
                _recoverBreak = true;
            }
            return result;
        }

        public override void OnRoundEndTheLast()
        {
            if (_recoverBreak)
            {
                if (owner.breakDetail.IsBreakLifeZero())
                {
                    owner.RecoverBreakLife(owner.MaxBreakLife);
                    owner.breakDetail.nextTurnBreak = false;
                }
                owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
                _recoverBreak = false;
            }
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

    public class DiceCardSelfAbility_Res7Dmg1Draw1Page : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 책장을 1장 뽑음, 자신의 호흡이 7 이상이라면 이 책장의 모든 주사위 위력 +1";

        public override void OnUseCard()
        {
            base.owner.allyCardDetail.DrawCards(1);
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            if (battleUnitBuf.stack >= 7)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }

    public class DiceCardSelfAbility_Res7Dmg1Energy1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 빛 1 회복, 자신의 호흡이 7 이상이라면 이 책장의 모든 주사위 위력 +1";

        public override void OnUseCard()
        {
            base.owner.cardSlotDetail.RecoverPlayPointByCard(1);
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            if (battleUnitBuf.stack >= 7)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }

    public class DiceCardSelfAbility_Res8Dmg1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "자신의 호흡이 8 이상이라면 이 책장의 모든 주사위 위력 +1";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
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

    public class DiceCardSelfAbility_ResPerDmg2 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 보유한 호흡 수치 7당 이 책장의 모든 주사위 위력 증가 (최대 2)";

        public override void OnUseCard()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = battleUnitBuf.stack / 7
            });

        }
    }

    public class DiceCardSelfAbility_Respiration2 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[사용시] 호흡 2를 얻음";

        public override void OnUseCard()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            BattleUnitBuf_Respiration.AddRespiration(base.owner, 2);
        }
    }

    public class DiceCardSelfAbility_SwordManship1 : DiceCardSelfAbilityBase
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
                BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_SwordManship1);
                item2.bufListDetail.AddReadyBuf(new BattleUnitBuf_SwordManship1());
            }
        }
    }

    public class DiceCardSelfAbility_SwordManship2 : DiceCardSelfAbilityBase
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
                BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_SwordManship2);
                item2.bufListDetail.AddReadyBuf(new BattleUnitBuf_SwordManship2());
            }
        }
    }

    /// <summary>
    /// DiceCardAbilitys
    /// </summary>
    
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

    public class DiceCardAbility_Res4AddNextDice2_OnWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 호흡 4를 얻고, 다음 주사위 위력 +2";

        public override void OnWinParrying()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            BattleUnitBuf_Respiration.AddRespiration(base.owner, 4);
            base.card.AddDiceAdder(DiceMatch.NextDice, 2);
        }
    }

    public class DiceCardAbility_AddCard : DiceCardAbilityBase
    {
        public static string Desc = "[합 패배] 자신의 패에 '골단' 추가";

        public override void OnLoseParrying()
        {
            base.owner.allyCardDetail.AddNewCard(512006);
        }
    }

    public class DiceCardAbility_Bleeding3Paralysis5 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 다음 막에 출혈 3과 마비 5 부여";

        public override void OnSucceedAttack()
        {
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 3, base.owner);
            base.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 5, base.owner);
        }
    }

    public class DiceCardAbiltiy_Respiration5_OnWinParrying : DiceCardAbilityBase
    {
        public static string Desc = "[합 승리] 호흡 5를 얻음";

        public override void OnWinParrying()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            BattleUnitBuf_Respiration.AddRespiration(base.owner, 5);

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

    public class DiceCardAbility_Respiration1 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 1를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            BattleUnitBuf_Respiration.AddRespiration(base.owner, 1);
        }
    }

    public class DiceCardAbility_Respiration2 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 2를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            BattleUnitBuf_Respiration.AddRespiration(base.owner, 2);
        }
    }

    public class DiceCardAbility_Respiration3 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 3를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            BattleUnitBuf_Respiration.AddRespiration(base.owner, 3);
        }
    }

    public class DiceCardAbility_Respiration4 : DiceCardAbilityBase
    {
        public static string Desc = "[적중] 호흡 4를 얻음";

        public override void OnSucceedAttack()
        {
            BattleUnitBuf battleUnitBuf = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Respiration);
            BattleUnitBuf_Respiration.AddRespiration(base.owner, 4);
        }
    }

    /// <summary>
    /// BattleUnitBufs
    /// </summary>

    public class BattleUnitBuf_SwordManship1 : BattleUnitBuf
    {
        protected override string keywordId
        {
            get
            {
                return "SwordManship1";
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

    public class BattleUnitBuf_SwordManship2 : BattleUnitBuf
    {
        protected override string keywordId
        {
            get
            {
                return "SwordManship2";
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

    public class BattleUnitBuf_Respiration : BattleUnitBuf 
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

        public static BattleUnitBuf_Respiration IshaveBuf(BattleUnitModel target, bool findready = false)
        {
            foreach (BattleUnitBuf battleUnitBuf in target.bufListDetail.GetActivatedBufList())
            {
                if (battleUnitBuf is BattleUnitBuf_Respiration)
                {
                    return battleUnitBuf as BattleUnitBuf_Respiration;
                }
            }

            if (findready)
            {
                foreach (BattleUnitBuf battleUnitBuf2 in target.bufListDetail.GetReadyBufList())
                {
                    if (battleUnitBuf2 is BattleUnitBuf_Respiration)
                    {
                        return battleUnitBuf2 as BattleUnitBuf_Respiration;
                    }
                }
            }
            return null;
        }

        public static void AddRespiration(BattleUnitModel target, int stack)
        {
            BattleUnitBuf_Respiration battleUnitBuf_Respiration = BattleUnitBuf_Respiration.IshaveBuf(target, true);
            if (battleUnitBuf_Respiration != null)
            {
                battleUnitBuf_Respiration.stack += stack;
                if (battleUnitBuf_Respiration.stack > 20)
                {
                    battleUnitBuf_Respiration.stack = 20;
                    return;
                }
            }
            else
            {
                BattleUnitBuf_Respiration battleUnitBuf_Respiration2 = new BattleUnitBuf_Respiration();
                battleUnitBuf_Respiration2.stack = stack;
                battleUnitBuf_Respiration2.Init(target);
                target.bufListDetail.AddBuf(battleUnitBuf_Respiration2);
            }
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineageInitializer.ArtWorks["Respiration"]);
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
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, BladeLineageInitializer.ArtWorks["Sadness"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
        }
    }
}
