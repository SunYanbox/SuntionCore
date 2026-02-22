using System.Collections.ObjectModel;
using JetBrains.Annotations;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Servers;
using SuntionCore.Services.I18NUtil;

namespace SuntionCore.SPTExpand.Services;

[Injectable(InjectionType = InjectionType.Singleton)]
public class ProfileAndAccountService(
    SaveServer saveServer,
    ProfileHelper profileHelper
): IOnLoad
{
    private const string KeyNoPmcDataInstance = nameof(KeyNoPmcDataInstance);
    private const string KeyLoadAccountData = nameof(KeyLoadAccountData);
    private const string KeyAccountIds = nameof(KeyAccountIds);
    
    /// <summary> Pmc/Scav id 到账户id的映射 </summary>
    private readonly Dictionary<MongoId, MongoId> _playerId2Account = new();

    /// <summary> 所有已存在的账号id的集合 </summary>
    private HashSet<MongoId> _accountIds = [];

    /// <summary> 所有已存在的账号id的只读视图集合 </summary>
    [UsedImplicitly] public ReadOnlySet<MongoId> AccountIds => new(_accountIds);
    
    /// <summary> 通过Pmc或Scav Id获取账号Id </summary>
    [UsedImplicitly] public MongoId? GetAccount(MongoId playerId)
    {
        return _playerId2Account.GetValueOrDefault(playerId);
    }
    
    /// <summary> 通过Account, Pmc, Session或Scav Id获取PmcData实例 </summary>
    [UsedImplicitly] public PmcData GetPmcDataByPlayerId(MongoId playerId)
    {
        if (!_playerId2Account.TryGetValue(playerId, out MongoId account))
            throw new Exception(KeyNoPmcDataInstance.Translate(SuntionCoreSPTExpandMod.I18NSPTExpand.Value, new { PlayerId = playerId }));
        SptProfile sptProfile = profileHelper.GetFullProfile(account);
        /*
         * SessionId, AccountId和PmcId相同, ScavId为PmcId+1
         */
        return (playerId == account
            ? sptProfile.CharacterData!.PmcData
            : sptProfile.CharacterData!.ScavData)!;
    }

    /// <summary> 维护常用Id映射 </summary>
    public void UpdateAccountData()
    {
        Dictionary<MongoId, SptProfile> profiles = saveServer.GetProfiles();
        HashSet<MongoId> accounts = profiles.Keys.ToHashSet();
        _accountIds = accounts;
        // 更新Pmc/Scav id 到账户id的映射
        string msg = KeyLoadAccountData.Translate(SuntionCoreSPTExpandMod.I18NSPTExpand.Value);
        // string msg = "从SPT加载账户数据: ";
        foreach (MongoId accountId in accounts)
        {
            SptProfile sptProfile = profiles[accountId];
            MongoId? pmcId = sptProfile.CharacterData?.PmcData?.Id;
            MongoId? scavId = sptProfile.CharacterData?.ScavData?.Id;
            if (pmcId is not null) _playerId2Account[pmcId.Value] = accountId;
            if (scavId is not null) _playerId2Account[scavId.Value] = accountId;
            // msg += $"\n\tAccount: {accountId}, PmcId: {pmcId}, ScavId: {scavId}";
            msg += KeyAccountIds.Translate(SuntionCoreSPTExpandMod.I18NSPTExpand.Value, new
            {
                Account = accountId,
                PmcId = pmcId,
                ScavId = scavId
            });
        }
        SuntionCoreSPTExpandMod.Logger.Value.Debug(msg);
    }
    
    public Task OnLoad()
    {
        SuntionCoreSPTExpandMod.I18NSPTExpand.Value.Expand("ch", new Dictionary<string, string>
        {
            { KeyNoPmcDataInstance, "未找到{{PlayerId}}对应的PmcData实例" },
            { KeyLoadAccountData, "从SPT加载账户数据: " },
            { KeyAccountIds, "\n\t账户Id: {{Account}}, PmcId: {{PmcId}}, ScavId: {{ScavId}}" }
        });
        SuntionCoreSPTExpandMod.I18NSPTExpand.Value.Expand("en", new Dictionary<string, string>
        {
            { KeyNoPmcDataInstance, "No PmcData instance found for {{PlayerId}}" },
            { KeyLoadAccountData, "Loading account data from SPT: " },
            { KeyAccountIds, "\n\tAccount ID: {{Account}}, Pmc ID: {{PmcId}}, Scav ID: {{ScavId}}" }
        });
        UpdateAccountData();
        SuntionCoreSPTExpandMod.Logger.Value.Info("服务ProfileAndAccountService已加载完成");
        return Task.CompletedTask;
    }
}