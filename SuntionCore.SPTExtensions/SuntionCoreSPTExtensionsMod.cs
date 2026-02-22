using SPTarkov.Server.Core.Models.Spt.Mod;
using SuntionCore.Services.I18NUtil;
using SuntionCore.Services.LogUtils;

namespace SuntionCore.SPTExtensions
{
    public record SuntionCoreMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.suntion.suntioncore.sptexpand";
        public override string Name { get; init; } = "SuntionCore.SPTExtensions";
        public override string Author { get; init; } = "Suntion";
        public override List<string>? Contributors { get; init; } = [];
        public override SemanticVersioning.Version Version { get; init; } = new("0.1.0");
        public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.4");


        public override List<string>? Incompatibilities { get; init; }
        public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = new()
        {
            { "com.suntion.suntioncore", new SemanticVersioning.Range(">=1.0.0") }
        };
        public override string? Url { get; init; }
        public override bool? IsBundleMod { get; init; } = false;
        public override string License { get; init; } = "CC-BY-SA";
    }

    public static class SuntionCoreSPTExtensionsMod
    {
        /// <summary> 本模组使用的日志 </summary>
        public static readonly Lazy<ModLogger> Logger = new(() => ModLogger.GetOrCreateLogger("SuntionCore.SPTExtensions"));
        /// <summary> 本模组使用的本地化语言 </summary>
        public static readonly Lazy<I18N> I18NSPTExtensions = new(() => I18N.GetOrCreateI18N("SuntionCore.SPTExtensions"));
    }
}