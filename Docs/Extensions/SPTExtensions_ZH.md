## SuntionCore.SPTExtensions

本模组是对于依赖SPT相关类才可使用的工具类的整合, 是对[SuntionCore](https://forge.sp-tarkov.com/mod/2600/suntioncore)模组的扩展, 本模组默认支持中英文("ch", "en"), 使用SuntionCore提供的默认日志路径

> 需要更改显示的语言: `SuntionCoreSPTExtensionsMod.I18NSPTExtensions.Value.CurrentLang = "目标语言"`
>
> 需要扩展语言: fork我的仓库, 在`SuntionCore.SPTExtensions/Services`的C#代码下参考OnLoad函数中的内容扩展, 然后创建一个推送请求; 请不要大量修改文件, 那会很难审核

### SuntionCore.SPTExtensions提供的功能

- [ModMailService 服务端发送大量消息/消费/理赔的工具](#modmailservice-服务端发送大量消息消费理赔的工具)
    - 提供SendMessage、SendAllMessageAsync用于发送消息
    - 提供SplitStringByNewlines辅助拆分长字符串为适合发送给客户端的列表(SendAllMessageAsync自带此功能)
    - 使用Payment和SendMoney分别用于消耗/获取
    - 提供SendItemsToPlayer用于发送物品(会按照物品堆叠数据自动分隔)
- [ProfileAndAccountService 玩家档案与账户映射服务](#ProfileAndAccountService-玩家档案与账户映射服务)

## 如何在你的项目引用这个库

1. 在模组项目文件夹下创建libs文件夹(可选)
2. 安装本模组与SuntionCore模组到SPT(本模组Releases默认打包为适配通用模组安装方式的格式), 把`你的游戏文件夹\SPT\user\mods\SuntionCore\SuntionCore.dll"`和`"你的游戏文件夹\SPT\user\mods\SuntionCore.SPTExpand\SuntionCore.SPTExpand.dll"`复制到上述libs文件夹下
3. 在`你的项目名称.csproj`中写入以下内容
   ```css
    <ItemGroup>
        <Reference Include="SuntionCore.dll">
            <HintPath>libs\SuntionCore.dll</HintPath>
            <Private>False</Private>
        </Reference>
        <Reference Include="SuntionCore.SPTExpand.dll">
            <HintPath>libs\SuntionCore.SPTExpand.dll</HintPath>
            <Private>False</Private>
        </Reference>
    </ItemGroup>
   ```
   > HintPath为你实际的dll文件相对于csproj文件的相对路径
   > Private必须设置为false以避免在打包输出时错误的包含所有引用过的库
4. 随后你的编译器就可以正确引用与使用该库中的内容了

## ModMailService 服务端发送大量消息/消费/理赔的工具

[API参考](SPTExtensions_API_ZH.md#modmailservice---公开接口)

主要测试与使用在RaidRecord模组中

## ProfileAndAccountService 玩家档案与账户映射服务

[API参考](SPTExtensions_API_ZH.md#profileandaccountservice---公开接口)

主要测试与使用在RaidRecord模组中

## 致谢

灵感来源：
- [MassivesoftCore](https://forge.sp-tarkov.com/mod/2587/massivesoftcore) | **关于制作SPT类库**
- [MassivesoftWeapons](https://forge.sp-tarkov.com/mod/2588/massivesoftweapons) | **关于引用SPT类库**
- [Raid Record](https://forge.sp-tarkov.com/mod/2341/raid-record) | 
  - **消费/理赔/消息发送/物品发送功能**
  - **获取玩家存档/处理session,profileId等的转换**
- [SPT.Server](https://github.com/sp-tarkov/server-csharp) | **拆分物品/消费/加钱等逻辑**