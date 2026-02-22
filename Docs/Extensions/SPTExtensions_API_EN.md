## SuntionCoreSPTExtensionsMod - Static Class Public API

```csharp
public static class SuntionCoreSPTExtensionsMod
{
    /// <summary> The logger used by this module. </summary>
    public static readonly Lazy<ModLogger> Logger;
    
    /// <summary> The localization (I18N) instance used by this module. </summary>
    public static readonly Lazy<I18N> I18NSPTExtensions;
}
```

## ModMailService - Public API

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
    /// <summary> Maximum length limit for a single message. </summary>
    public const int SendLimit = 490;
    
    /// <summary>
    /// Splits the string to be sent to prevent incomplete display on the client side.
    /// </summary>
    public static string[] SplitStringByNewlines(string str);
    
    /// <summary>
    /// Sends a message to the client associated with the specified sessionId via a registered chat bot.
    /// </summary>
    public void SendMessage(string sessionId, string message, UserDialogInfo chatBot);

    /// <summary>
    /// Asynchronously sends bulk messages to the client via a registered chat bot.
    /// <br />
    /// Automatically handles splitting long text to prevent overflow in the client chat window.
    /// </summary>
    [UsedImplicitly]
    public async Task SendAllMessageAsync(string sessionId, string message, UserDialogInfo chatBot);
    
    /// <summary>
    /// Deducts funds based on the specified amount.
    /// </summary>
    /// <param name="sessionId">Player account ID / sessionId.</param>
    /// <param name="moneyId">Template ID of the currency type.</param>
    /// <param name="amount">The amount of the specified currency to deduct.</param>
    /// <param name="pmcData">If provided, the sessionId parameter is ignored; otherwise, deduction occurs via sessionId.</param>
    /// <exception cref="ArgumentException">Thrown if the currency template ID is not Euro, Ruble, GP Coin, or USD.</exception>
    /// <returns>Returns a list of warnings if unsuccessful; otherwise null.</returns>
    [UsedImplicitly]
    public List<Warning>? Payment(MongoId sessionId, MongoId moneyId, long amount, PmcData? pmcData = null);

    /// <summary>
    /// Sends money to the player with FIR status (Found In Raid). 
    /// (Note: FIR status is generally insignificant for currency).
    /// </summary>
    /// <param name="sessionId">Player account ID / sessionId.</param>
    /// <param name="moneyId">Template ID of the currency type.</param>
    /// <param name="msg">Message attached when sending items/funds.</param>
    /// <param name="amount">The amount of the specified currency to send.</param>
    /// <exception cref="ArgumentException">Thrown if the currency template ID is not Euro, Ruble, GP Coin, or USD.</exception>
    /// <returns>Returns a list of warnings if unsuccessful; otherwise null.</returns>
    [UsedImplicitly]
    public List<Warning>? SendMoney(MongoId sessionId, MongoId moneyId, string msg, double amount);

    /// <summary>
    /// Sends items to the player from the "System" account.
    /// </summary>
    /// <param name="sessionId">Player sessionId.</param>
    /// <param name="msg">Notification message.</param>
    /// <param name="items">List of items to send.</param>
    /// <param name="modGiveIsFIR">Whether the given items should have FIR (Found In Raid) status.</param>
    /// <param name="maxStorageTimeSeconds">Maximum storage time in seconds. Defaults to 2 days.</param>
    /// <returns>Returns a list of warnings if unsuccessful; otherwise null.</returns>
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

        SuntionCoreSPTExtensionsMod.Logger.Value.Info("Service ModMailService loaded and initialized successfully.");
        return Task.CompletedTask;
    }
}
```

## ProfileAndAccountService - Public API

```csharp
[Injectable(InjectionType = InjectionType.Singleton)]
public class ProfileAndAccountService(
    SaveServer saveServer,
    ProfileHelper profileHelper
): IOnLoad
{
    /// <summary> A read-only view collection of all existing account IDs. </summary>
    [UsedImplicitly] public ReadOnlySet<MongoId> AccountIds;
    
    /// <summary> Gets the Account ID using a PMC or Scav ID. </summary>
    [UsedImplicitly] public MongoId? GetAccount(MongoId playerId);
    
    /// <summary> Gets the PmcData instance using an Account, PMC, Session, or Scav ID. </summary>
    [UsedImplicitly] public PmcData GetPmcDataByPlayerId(MongoId playerId);

    /// <summary> Maintains common ID mappings. </summary>
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
        SuntionCoreSPTExtensionsMod.Logger.Value.Info("Service ProfileAndAccountService loaded successfully.");
        return Task.CompletedTask;
    }
}
```