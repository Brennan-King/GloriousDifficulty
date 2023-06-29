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

        private const int ENEMY_LEVEL_CAP = 125;

        public void Awake()
        {
            EDMLogger = Logger;
            AddDifficulty();
            FillTokens();
            Run.onRunSetRuleBookGlobal += Run_onRunSetRuleBookGlobal;
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
            GloriousDifficultyDef = new(3.5f, "GLORIOUS_DIFFICULTY", "EXAMPLEDIFFICULTYMOD_ICON", "GLORIOUS_DIFFICULTY_DESCRIPTION", new Color32(193, 117, 255, 255), "GD", true);
            GloriousDifficultyDef.iconSprite = null;
            GloriousDifficultyDef.foundIconSprite = true;
            GloriousDifficultyDiffIndex = DifficultyAPI.AddDifficulty(GloriousDifficultyDef);
        }
        // TODO: Figure out how to boost spawn rate of enemies
        private void Run_onRunSetRuleBookGlobal(Run arg1, RuleBook arg2)
        {
            Run.ambientLevelCap = (arg1.selectedDifficulty == GloriousDifficultyDiffIndex) ? ENEMY_LEVEL_CAP : Run.ambientLevelCap;
        }
    }
}