using JetBrains.Annotations;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Dialog;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Services;

namespace SuntionCore.Services.SPTUtils;

[Injectable]
public class ModMailService(
    ItemHelper itemHelper,
    ProfileHelper profileHelper,
    PaymentService paymentService,
    MailSendService mailSendService,
    EventOutputHolder eventOutputHolder
)
{
    /// <summary> 发送信息单条长度限制 </summary>
    public const int SendLimit = 490;
    
    /// <summary>
    /// 分隔要发送的字符串, 避免客户端无法完整显示
    /// </summary>
    public static string[] SplitStringByNewlines(string str)
    {
        switch (str.Length)
        {
            case 0:
                return [];
            // 如果字符串长度不超过限制，直接返回包含原字符串的数组
            case <= SendLimit:
                return [str];
        }

        // 用换行符分割
        string[] segments = str.Split('\n');
        List<string> result = [];
        string currentSegment = "";

        foreach (string segment in segments)
        {
            string potentialSegment = currentSegment != "" ? currentSegment + "\n" + segment : segment;

            if (potentialSegment.Length < SendLimit)
            {
                currentSegment = potentialSegment;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentSegment))
                {
                    result.Add(currentSegment);
                }
                currentSegment = segment;
            }
        }

        // 添加最后一个分段
        if (!string.IsNullOrEmpty(currentSegment))
        {
            result.Add(currentSegment);
        }

        return result.ToArray();
    }
    
    /// <summary>
    /// 将消息通过注册的聊天机器人发给对应sessionId的客户端
    /// </summary>
    public void SendMessage(string sessionId, string message, UserDialogInfo chatBot)
    {
        var details = new SendMessageDetails
        {
            RecipientId = sessionId,
            MessageText = message,
            Sender = MessageType.UserMessage,
            SenderDetails = chatBot
        };
        mailSendService.SendMessageToPlayer(details);
    }

    /// <summary>
    /// 将消息通过注册的聊天机器人异步发送大量消息
    /// <br />
    /// 可以自动处理长文本的分隔, 避免客户端聊天窗口无法显示
    /// </summary>
    [UsedImplicitly]
    public async Task SendAllMessageAsync(string sessionId, string message, UserDialogInfo chatBot)
    {
        string[] messages = SplitStringByNewlines(message);
        switch (messages.Length)
        {
            case 0:
                return;
            case 1:
                await Task.Delay(1000);
                SendMessage(sessionId, messages[0], chatBot);
                return;
        }

        await Task.Delay(750);

        // 同时有多条消息被启用时, 用来唯一标记
        var messageTag = $"[{messages[0][new Range(0, Math.Min(16, messages[0].Length))]}...]";

        for (var i = 0; i < messages.Length; i++)
        {
            SendMessage(sessionId, messages[i] + $"\n{i + 1}/{messages.Length} tag: {messageTag}", chatBot);
            if (i < messages.Length - 1)
            {
                await Task.Delay(1250);
            }
        }
    }
    
    /// <summary>
    /// 按照指定数额进行扣费
    /// </summary>
    /// <returns>如果未成功则返回警告列表</returns>
    [UsedImplicitly]
    public List<Warning>? Payment(MongoId sessionId, long amount, PmcData? pmcData = null)
    {
        ItemEventRouterResponse output = eventOutputHolder.GetOutput(sessionId);
        pmcData ??= profileHelper.GetPmcProfile(sessionId);

        if (pmcData == null)
        {
            return
            [
                new Warning
                {
                    ErrorMessage = $"未获取到Session {sessionId} 对应的玩家存档信息"
                }
            ];
        }

        try
        {
            paymentService.AddPaymentToOutput(
                pmcData,
                Money.ROUBLES,
                amount,
                sessionId,
                output);
        }
        catch (Exception e)
        {
            output.Warnings ??= [];
            output.Warnings.Add(new Warning
            {
                ErrorMessage = $"扣费时出现错误: {e.Message} {e.StackTrace}"
            });
        }

        return output.Warnings;
    }

    /// <summary>
    /// 给玩家发送钱, 且为FIR状态(对于钱来说不重要)
    /// </summary>
    /// <returns>如果未成功则返回警告列表</returns>
    [UsedImplicitly]
    public List<Warning>? SendMoney(MongoId sessionId, string msg, double amount)
    {
        ItemEventRouterResponse output = eventOutputHolder.GetOutput(sessionId);

        try
        {
            List<Warning>? warnings = SendItemsToPlayer(
                sessionId,
                msg,
                itemHelper.SplitStackIntoSeparateItems(new Item
                {
                    Id = new MongoId(),
                    Template = Money.ROUBLES,
                    Upd = new Upd
                    {
                        StackObjectsCount = amount,
                        SpawnedInSession = true
                    }
                }).SelectMany(x => x).ToList());
            if (warnings != null)
            {
                foreach (Warning warning in warnings)
                {
                    output.Warnings ??= [];
                    output.Warnings.Add(warning);
                }
                return warnings;
            }
        }
        catch (Exception e)
        {
            output.Warnings ??= [];
            output.Warnings.Add(new Warning
            {
                ErrorMessage = $"扣费时出现错误: {e.Message} {e.StackTrace}"
            });
        }

        return output.Warnings;
    }

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
        long? maxStorageTimeSeconds = 172800L)
    {
        try
        {
            if (items?.Count <= 0)
            {
                return
                [
                    new Warning
                    {
                        ErrorMessage = "未指定物品"
                    }
                ];
            }
            if (modGiveIsFIR)
            {
                foreach (Item item in items ?? [])
                {
                    item.Upd ??= new Upd();
                    item.Upd.SpawnedInSession = modGiveIsFIR;
                }
            }
            mailSendService.SendSystemMessageToPlayer(
                sessionId,
                msg,
                items,
                maxStorageTimeSeconds);
            return null;
        }
        catch (Exception e)
        {
            return
            [
                new Warning
                {
                    ErrorMessage = $"发送物品时出现错误: {e.Message} {e.StackTrace}"
                }
            ];
        }
    }
}