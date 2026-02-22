## SuntionCore.SPTExtensions

This module consolidates utility classes that depend on SPT-related classes. It serves as an extension to the [SuntionCore](https://forge.sp-tarkov.com/mod/2600/suntioncore) module. By default, this module supports both Chinese ("ch") and English ("en") and utilizes the default logging path provided by SuntionCore.

> **To change the display language:**  
> `SuntionCoreSPTExtensionsMod.I18NSPTExtensions.Value.CurrentLang = "target_language"`
>
> **To extend language support:**  
> Fork my repository, refer to the content within the `OnLoad` function in the C# code under `SuntionCore.SPTExtensions/Services` to add your extensions, and then submit a Pull Request. Please avoid making extensive modifications to files, as this makes the review process difficult.

### Features Provided by SuntionCore.SPTExtensions

- [**ModMailService**: A tool for sending bulk messages, deducting funds, and processing claims on the server side](#modmailservice---a-tool-for-sending-bulk-messages-deducting-funds-and-processing-claims-on-the-server-side)
    - Provides `SendMessage` and `SendAllMessageAsync` for sending messages.
    - Provides `SplitStringByNewlines` to assist in splitting long strings into lists suitable for client delivery (`SendAllMessageAsync` includes this functionality automatically).
    - Uses `Payment` and `SendMoney` for deducting and adding funds, respectively.
    - Provides `SendItemsToPlayer` for sending items (automatically splits items based on stack data).
- [**ProfileAndAccountService**: Player profile and account mapping service](#profileandaccountservice---player-profile-and-account-mapping-service)

## How to Reference This Library in Your Project

1. Create a `libs` folder within your mod project directory (optional).
2. Install both this module and the SuntionCore module into SPT (this module's releases are packaged by default for universal mod installation). Copy `"YourGameFolder\SPT\user\mods\SuntionCore\SuntionCore.dll"` and `"YourGameFolder\SPT\user\mods\SuntionCore.SPTExpand\SuntionCore.SPTExpand.dll"` to the `libs` folder mentioned above.
3. Add the following content to your `YourProjectName.csproj` file:
   ```xml
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
   > Note: `HintPath` should be the relative path of the actual DLL files relative to your `.csproj` file.  
   > `Private` must be set to `False` to prevent incorrectly bundling all referenced libraries during the build output.
4. Your compiler will now be able to correctly reference and use the contents of this library.

## ModMailService - A Tool for Sending Bulk Messages, Deducting Funds, and Processing Claims on the Server Side

[API Reference](SPTExtensions_API_EN.md#modmailservice---public-api)

Primarily tested and utilized in the **RaidRecord** module.

## ProfileAndAccountService - Player Profile and Account Mapping Service

[API Reference](SPTExtensions_API_EN.md#profileandaccountservice---public-api)

Primarily tested and utilized in the **RaidRecord** module.

## Acknowledgements

Inspiration sources:
- [MassivesoftCore](https://forge.sp-tarkov.com/mod/2587/massivesoftcore) | **Regarding the creation of SPT class libraries**
- [MassivesoftWeapons](https://forge.sp-tarkov.com/mod/2588/massivesoftweapons) | **Regarding referencing SPT class libraries**
- [Raid Record](https://forge.sp-tarkov.com/mod/2341/raid-record) |
    - **Fund deduction/claim processing/message sending/item sending functionalities**
    - **Retrieving player saves/handling conversions for sessions, profileIds, etc.**
- [SPT.Server](https://github.com/sp-tarkov/server-csharp) | **Logic for splitting items, deducting funds, adding money, etc.**