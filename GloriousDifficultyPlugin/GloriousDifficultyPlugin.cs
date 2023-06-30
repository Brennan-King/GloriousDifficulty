using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace GloriousDifficulty
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(DifficultyAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GloriousDifficultyPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "Brennan";
        public const string PluginName = "GloriousDifficulty";
        public const string PluginVersion = "1.0.0";

        public static ManualLogSource EDMLogger;
        public static DifficultyDef GloriousDifficultyDef;
        public static DifficultyIndex GloriousDifficultyDiffIndex;
        public static bool shouldRun = false;

        private const int ENEMY_LEVEL_CAP = 125;

        internal static bool IncreaseSpawnCap = true;
        internal static PluginInfo pluginInfo;

        public AssetBundle GloriousDifficultyAssetbundle;

        public void Awake()
        {
            pluginInfo = Info;
            EDMLogger = Logger;
            GloriousDifficultyAssetbundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(pluginInfo.Location), "gloriousdifficultyicon"));

            AddDifficulty();
            FillTokens();
            Run.onRunSetRuleBookGlobal += Run_onRunSetRuleBookGlobal;

            Run.onRunStartGlobal += (Run run) =>
            {
                shouldRun = false;
                if (run.selectedDifficulty == GloriousDifficultyDiffIndex)
                {
                    shouldRun = true;
                    if (IncreaseSpawnCap) On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
                }
            };
            Run.onRunDestroyGlobal += (Run run) =>
            {
                shouldRun = false;
                if (IncreaseSpawnCap) On.RoR2.CombatDirector.Awake -= CombatDirector_Awake;
            };
        }

        public void FillTokens()
        {
            LanguageAPI.Add("GLORIOUS_DIFFICULTY", "Glorious Difficulty");
            LanguageAPI.Add("GLORIOUS_DIFFICULTY_DESCRIPTION", "For veterans looking for a further challenge...\n\n" +
                                                                "<style=cStack>>Player Health Regeneration: <style=cIsHealth>-40%</style>\n" +
                                                                ">Difficulty Scaling: <style=cIsHealth>+75%</style>\n" +
                                                                ">Enemy Level Cap: <style=cIsHealth>125</style>\n" +
                                                                ">Enemy Spawn Rate: <style=cIsHealth>+10%</style>\n");
        }

        public void AddDifficulty()
        {
            GloriousDifficultyDef = new(3.5f, "GLORIOUS_DIFFICULTY", "GLORIOUS_DIFFICULTY_ICON", "GLORIOUS_DIFFICULTY_DESCRIPTION", new Color32(193, 117, 255, 255), "GD", true);
            GloriousDifficultyDef.iconSprite = GloriousDifficultyAssetbundle.LoadAsset<Sprite>("belt_icon.png");
            GloriousDifficultyDef.foundIconSprite = true;
            GloriousDifficultyDiffIndex = DifficultyAPI.AddDifficulty(GloriousDifficultyDef);
        }

        private void Run_onRunSetRuleBookGlobal(Run arg1, RuleBook arg2)
        {
            Run.ambientLevelCap = (arg1.selectedDifficulty == GloriousDifficultyDiffIndex) ? ENEMY_LEVEL_CAP : Run.ambientLevelCap;
        }

        public static void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
        {
            if (IncreaseSpawnCap) self.creditMultiplier *= 1.1f;
            orig(self);
        }
    }
}