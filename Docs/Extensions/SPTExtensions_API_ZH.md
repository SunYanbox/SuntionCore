## SuntionCoreSPTExtensionsMod - 静态类公有接口

```csharp
public static class SuntionCoreSPTExtensionsMod
{
    /// <summary> 本模组使用的日志 </summary>
    public static readonly Lazy<ModLogger> Logger;
    /// <summary> 本模组使用的本地化语言 </summary>
    public static readonly Lazy<I18N> I18NSPTExtensions;
}
```

## ModMailService - 公开接口

```csharp
[Injectable(InjectionType.Singleton)]
public class ModMailService(
    ItemHelper itemHelper,
    ProfileHelper profileHelper,
    PaymentService paymentService,
    MailSendService mailSendService,
    EventOutputHolder eventOutputHolder
): IOnLoad
{
    /// <summary> 发送信息单条长度限制 </summary>
    public const int SendLimit = 490;
    
    /// <summary>
    /// 分隔要发送的字符串, 避免客户端无法完整显示
    /// </summary>
    public static string[] SplitStringByNewlines(string str);
    
    /// <summary>
    /// 将消息通过注册的聊天机器人发给对应sessionId的客户端
    /// </summary>
    public void SendMessage(string sessionId, string message, UserDialogInfo chatBot);

    /// <summary>
    /// 将消息通过注册的聊天机器人异步发送大量消息
    /// <br />
    /// 可以自动处理长文本的分隔, 避免客户端聊天窗口无法显示
    /// </summary>
    [UsedImplicitly]
    public async Task SendAllMessageAsync(string sessionId, string message, UserDialogInfo chatBot);
    
    /// <summary>
    /// 按照指定数额进行扣费
    /// </summary>
    /// <param name="sessionId">玩家账户Id / sessionId</param>
    /// <param name="moneyId">钱的类型的模板Id</param>
    /// <param name="amount">消耗的指定类型钱的金额</param>
    /// <param name="pmcData">提供后忽略sessionId参数, 没有时通过sessionId消费</param>
    /// <exception cref="ArgumentException">货币模板Id不是欧元、卢布、GP币、美元之一时</exception>
    /// <returns>如果未成功则返回警告列表</returns>
    [UsedImplicitly]
    public List<Warning>? Payment(MongoId sessionId, MongoId moneyId, long amount, PmcData? pmcData = null);

    /// <summary>
    /// 给玩家发送钱, 且为FIR状态(对于钱来说不重要)
    /// </summary>
    /// <param name="sessionId">玩家账户Id / sessionId</param>
    /// <param name="moneyId">钱的类型的模板Id</param>
    /// <param name="msg">发送物品时附带的消息</param>
    /// <param name="amount">消耗的指定类型钱的金额</param>
    /// <exception cref="ArgumentException">货币模板Id不是欧元、卢布、GP币、美元之一时</exception>
    /// <returns>如果未成功则返回警告列表</returns>
    [UsedImplicitly]
    public List<Warning>? SendMoney(MongoId sessionId, MongoId moneyId, string msg, double amount);

    /// <summary>
    /// 将物品以System账户发送给玩家
    /// </summary>
    /// <param name="sessionId">玩家sessionId</param>
    /// <param name="msg">提示信息</param>
    /// <param name="items">物品列表</param>
    /// <param name="modGiveIsFIR">是否使得给予的物品是FIR状态(对局中发现)</param>
    /// <param name="maxStorageTimeSeconds">默认为2天</param>
    /// <returns>如果未成功则返回警告列表</returns>
    public List<Warning>? SendItemsToPlayer(
        MongoId sessionId,
        string msg,
        List<Item>? items,
        bool modGiveIsFIR = true,
        long? maxStorageTimeSeconds = 172800L);

    public Task OnLoad()
    {
        I18N i18N = SuntionCoreSPTExtensionsMod.I18NSPTExtensions.Value;

        i18N.Expand("ch", new Dictionary<string, string>
        {
            { KeyInvalidMoneyId, "货币模板 Id 必须在 [{{MoneyIds}}] 中，但传入的值为：{{CurrentId}}" },
            { KeyProfileNotFound, "未获取到 Session {{SessionId}} 对应的玩家存档信息" },
            { KeyPaymentError, "扣费时出现错误：{{Error}}" },
            { KeySendMoneyError, "理赔时出现错误：{{Error}}" },
            { KeyNoItemsSpecified, "未指定物品" },
            { KeySendItemError, "发送物品时出现错误：{{Error}}" }
        });

        i18N.Expand("en", new Dictionary<string, string>
        {
            { KeyInvalidMoneyId, "Money template ID must be one of [{{MoneyIds}}], but received: {{CurrentId}}" },
            { KeyProfileNotFound, "Failed to retrieve player profile for Session {{SessionId}}" },
            { KeyPaymentError, "Error occurred during payment deduction: {{Error}}" },
            { KeySendMoneyError, "An error occurred during payment processing: {{Error}}" },
            { KeyNoItemsSpecified, "No items specified" },
            { KeySendItemError, "Error occurred while sending items: {{Error}}" }
        });

        SuntionCoreSPTExtensionsMod.Logger.Value.Info("服务 ModMailService 已加载完成并初始化国际化");
        return Task.CompletedTask;
    }
}
```

## ProfileAndAccountService - 公开接口

```csharp
[Injectable(InjectionType = InjectionType.Singleton)]
public class ProfileAndAccountService(
    SaveServer saveServer,
    ProfileHelper profileHelper
): IOnLoad
{
    /// <summary> 所有已存在的账号id的只读视图集合 </summary>
    [UsedImplicitly] public ReadOnlySet<MongoId> AccountIds;
    
    /// <summary> 通过Pmc或Scav Id获取账号Id </summary>
    [UsedImplicitly] public MongoId? GetAccount(MongoId playerId);
    
    /// <summary> 通过Account, Pmc, Session或Scav Id获取PmcData实例 </summary>
    [UsedImplicitly] public PmcData GetPmcDataByPlayerId(MongoId playerId);

    /// <summary> 维护常用Id映射 </summary>
    public void UpdateAccountData();
    
    public Task OnLoad()
    {
        SuntionCoreSPTExtensionsMod.I18NSPTExtensions.Value.Expand("ch", new Dictionary<string, string>
        {
            { KeyNoPmcDataInstance, "未找到{{PlayerId}}对应的PmcData实例" },
            { KeyLoadAccountData, "从SPT加载账户数据: " },
            { KeyAccountIds, "\n\t账户Id: {{Account}}, PmcId: {{PmcId}}, ScavId: {{ScavId}}" }
        });
        SuntionCoreSPTExtensionsMod.I18NSPTExtensions.Value.Expand("en", new Dictionary<string, string>
        {
            { KeyNoPmcDataInstance, "No PmcData instance found for {{PlayerId}}" },
            { KeyLoadAccountData, "Loading account data from SPT: " },
            { KeyAccountIds, "\n\tAccount ID: {{Account}}, Pmc ID: {{PmcId}}, Scav ID: {{ScavId}}" }
        });
        UpdateAccountData();
        SuntionCoreSPTExtensionsMod.Logger.Value.Info("服务ProfileAndAccountService已加载完成");
        return Task.CompletedTask;
    }
}
```

