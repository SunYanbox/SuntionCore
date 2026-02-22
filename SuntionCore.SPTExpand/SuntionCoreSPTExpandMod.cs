using SPTarkov.Server.Core.Models.Spt.Mod;

namespace SuntionCore.SPTExpand
{
    public record SuntionCoreMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.suntion.suntioncore.sptexpand";
        public override string Name { get; init; } = "SuntionCore.SPTExpand";
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
    
    
}