

using System.Text;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SuntionCore.Services.LogUtils;

namespace SuntionCore
{
    public record SuntionCoreMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.suntion.suntioncore";
        public override string Name { get; init; } = SuntionCoreLoad.ModName;
        public override string Author { get; init; } = "Suntion";
        public override List<string>? Contributors { get; init; } = [];
        public override SemanticVersioning.Version Version { get; init; } = new("0.1.0");
        public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.4");


        public override List<string>? Incompatibilities { get; init; }
        public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
        public override string? Url { get; init; }
        public override bool? IsBundleMod { get; init; } = false;
        public override string License { get; init; } = "CC-BY-SA";
    }

    [Injectable(InjectionType.Singleton, TypePriority = int.MaxValue)]
    public class SuntionCoreLoad(ISptLogger<SuntionCoreLoad> logger): IOnLoad
    {
        public const string ModName = "SuntionCore";
        
        public Task OnLoad()
        {
            // 仅有本模组日志不输出
            if (ModLogger.LoggerCount == 1 && ModLogger.GetLogger(ModName) is not null) return Task.CompletedTask;
            // 否则输出日志路径
            if (ModLogger.LoggerCount > 0)
            {
                ModLogger suntionCoreModLogger = ModLogger.GetOrCreateLogger(ModName);
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine($"默认日志路径为: \"{Path.GetFullPath(ModLogger.DefaultLogFolderPath)}\"");
                Dictionary<string, HashSet<ModLogger>> loggers = new();
                foreach (ModLogger modLogger in ModLogger.Items.Values)
                {
                    if (!loggers.TryGetValue(modLogger.FolderPath, out HashSet<ModLogger>? value))
                    {
                        value = [];
                        loggers.Add(modLogger.FolderPath, value);
                    }

                    value.Add(modLogger);
                }

                foreach ((string folderPath, HashSet<ModLogger> modLoggers) in loggers)
                {
                    stringBuilder.AppendLine($" - {folderPath}:");
                    foreach (ModLogger modLogger in modLoggers)
                    {
                        stringBuilder.AppendLine($"   - {modLogger.ModName}({modLogger.Strategy})");
                    }
                }

                logger.Info(suntionCoreModLogger.Info(stringBuilder.ToString()));
            }
            return Task.CompletedTask;
        }
    }
}