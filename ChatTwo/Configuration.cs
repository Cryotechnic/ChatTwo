using System.Collections;
using ChatTwo.Code;
using ChatTwo.GameFunctions.Types;
using ChatTwo.Resources;
using ChatTwo.Util;
using Dalamud;
using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.FontIdentifier;
using ImGuiNET;

namespace ChatTwo;

[Serializable]
internal class ConfigKeyBind
{
    public ModifierFlag Modifier;
    public VirtualKey Key;

    public override string ToString()
    {
        var modString = "";
        if (Modifier.HasFlag(ModifierFlag.Ctrl))
            modString += Language.Keybind_Modifier_Ctrl + " + ";
        if (Modifier.HasFlag(ModifierFlag.Shift))
            modString += Language.Keybind_Modifier_Shift + " + ";
        if (Modifier.HasFlag(ModifierFlag.Alt))
            modString += Language.Keybind_Modifier_Alt + " + ";
        return modString+Key.GetFancyName();
    }
}

[Serializable]
internal class Configuration : IPluginConfiguration
{
    private const int LatestVersion = 5;

    public int Version { get; set; } = LatestVersion;

    public bool HideChat = true;
    public bool HideDuringCutscenes = true;
    public bool HideWhenNotLoggedIn = true;
    public bool HideWhenUiHidden = true;
    public bool HideInLoadingScreens;
    public bool HideInBattle;
    public bool HideWhenInactive;
    public int InactivityHideTimeout = 10;
    public bool InactivityHideActiveDuringBattle = true;
    public Dictionary<ChatType, ChatSource>? InactivityHideChannels;
    public bool InactivityHideExtraChatAll = true;
    public HashSet<Guid> InactivityHideExtraChatChannels = [];
    public bool ShowHideButton = true;
    public bool NativeItemTooltips = true;
    public bool PrettierTimestamps = true;
    public bool MoreCompactPretty;
    public bool HideSameTimestamps;
    public bool ShowNoviceNetwork;
    public bool SidebarTabView;
    public bool PrintChangelog = true;
    public bool OnlyPreviewIf;
    public int PreviewMinimum = 1;
    public PreviewPosition PreviewPosition = PreviewPosition.Inside;
    public CommandHelpSide CommandHelpSide = CommandHelpSide.None;
    public KeybindMode KeybindMode = KeybindMode.Strict;
    public LanguageOverride LanguageOverride = LanguageOverride.None;
    public bool CanMove = true;
    public bool CanResize = true;
    public bool ShowTitleBar;
    public bool ShowPopOutTitleBar = true;
    public bool DatabaseBattleMessages;
    public bool LoadPreviousSession;
    public bool FilterIncludePreviousSessions;
    public bool SortAutoTranslate;
    public bool CollapseDuplicateMessages;
    public bool PlaySounds = true;
    public bool KeepInputFocus = true;
    public int MaxLinesToRender = 10_000;

    public bool ShowEmotes = true;
    public HashSet<string> BlockedEmotes = [];

    public bool FontsEnabled = true;
    public ExtraGlyphRanges ExtraGlyphRanges = 0;
    public float FontSizeV2 = 12.75f;
    public float SymbolsFontSizeV2 = 12.75f;
    public SingleFontSpec GlobalFontV2 = new()
    {
        // dalamud only ships KR as regular, which chat2 used previously for global fonts
        FontId = new DalamudAssetFontAndFamilyId(DalamudAsset.NotoSansKrRegular),
        SizePt = 12.75f,
    };
    public SingleFontSpec JapaneseFontV2 = new()
    {
        FontId = new DalamudAssetFontAndFamilyId(DalamudAsset.NotoSansJpMedium),
        SizePt = 12.75f,
    };
    public bool ItalicEnabled = false;
    public SingleFontSpec ItalicFontV2 = new()
    {
        FontId = new DalamudAssetFontAndFamilyId(DalamudAsset.NotoSansKrRegular),
        SizePt = 12.75f,
    };

    // TODO Remove after 24.08
    public float FontSize = 17f;
    public float JapaneseFontSize = 17f;
    public float SymbolsFontSize = 17f;
    public string GlobalFont = "Chat 2: Noto Sans";
    public string JapaneseFont = "Chat 2: Noto Sans JP";

    public float TooltipOffset;
    public float WindowAlpha = 100f;
    public Dictionary<ChatType, uint> ChatColours = new();
    public List<Tab> Tabs = [];

    public bool OverrideStyle;
    public string? ChosenStyle;

    public ConfigKeyBind? ChatTabForward;
    public ConfigKeyBind? ChatTabBackward;

    internal void UpdateFrom(Configuration other, bool backToOriginal)
    {
        if (backToOriginal)
            foreach (var tab in Tabs.Where(t => t.PopOut))
                tab.PopOut = false;

        HideChat = other.HideChat;
        HideDuringCutscenes = other.HideDuringCutscenes;
        HideWhenNotLoggedIn = other.HideWhenNotLoggedIn;
        HideWhenUiHidden = other.HideWhenUiHidden;
        HideInLoadingScreens = other.HideInLoadingScreens;
        HideInBattle = other.HideInBattle;
        HideWhenInactive = other.HideWhenInactive;
        InactivityHideTimeout = other.InactivityHideTimeout;
        InactivityHideActiveDuringBattle = other.InactivityHideActiveDuringBattle;
        InactivityHideChannels = other.InactivityHideChannels.ToDictionary(entry => entry.Key, entry => entry.Value);
        InactivityHideExtraChatAll = other.InactivityHideExtraChatAll;
        InactivityHideExtraChatChannels = other.InactivityHideExtraChatChannels.ToHashSet();
        ShowHideButton = other.ShowHideButton;
        NativeItemTooltips = other.NativeItemTooltips;
        PrettierTimestamps = other.PrettierTimestamps;
        MoreCompactPretty = other.MoreCompactPretty;
        HideSameTimestamps = other.HideSameTimestamps;
        ShowNoviceNetwork = other.ShowNoviceNetwork;
        SidebarTabView = other.SidebarTabView;
        PrintChangelog = other.PrintChangelog;
        OnlyPreviewIf = other.OnlyPreviewIf;
        PreviewMinimum = other.PreviewMinimum;
        PreviewPosition = other.PreviewPosition;
        CommandHelpSide = other.CommandHelpSide;
        KeybindMode = other.KeybindMode;
        LanguageOverride = other.LanguageOverride;
        CanMove = other.CanMove;
        CanResize = other.CanResize;
        ShowTitleBar = other.ShowTitleBar;
        ShowPopOutTitleBar = other.ShowPopOutTitleBar;
        DatabaseBattleMessages = other.DatabaseBattleMessages;
        LoadPreviousSession = other.LoadPreviousSession;
        FilterIncludePreviousSessions = other.FilterIncludePreviousSessions;
        SortAutoTranslate = other.SortAutoTranslate;
        CollapseDuplicateMessages = other.CollapseDuplicateMessages;
        PlaySounds = other.PlaySounds;
        KeepInputFocus = other.KeepInputFocus;
        MaxLinesToRender = other.MaxLinesToRender;
        ShowEmotes = other.ShowEmotes;
        BlockedEmotes = other.BlockedEmotes;
        FontsEnabled = other.FontsEnabled;
        ItalicEnabled = other.ItalicEnabled;
        ExtraGlyphRanges = other.ExtraGlyphRanges;
        FontSizeV2 = other.FontSizeV2;
        GlobalFontV2 = other.GlobalFontV2;
        JapaneseFontV2 = other.JapaneseFontV2;
        ItalicFontV2 = other.ItalicFontV2;
        SymbolsFontSizeV2 = other.SymbolsFontSizeV2;
        TooltipOffset = other.TooltipOffset;
        WindowAlpha = other.WindowAlpha;
        ChatColours = other.ChatColours.ToDictionary(entry => entry.Key, entry => entry.Value);
        Tabs = other.Tabs.Select(t => t.Clone()).ToList();
        OverrideStyle = other.OverrideStyle;
        ChosenStyle = other.ChosenStyle;
        ChatTabForward = other.ChatTabForward;
        ChatTabBackward = other.ChatTabBackward;
    }
}

[Serializable]
internal enum UnreadMode
{
    All,
    Unseen,
    None,
}

internal static class UnreadModeExt
{
    internal static string Name(this UnreadMode mode) => mode switch
    {
        UnreadMode.All => Language.UnreadMode_All,
        UnreadMode.Unseen => Language.UnreadMode_Unseen,
        UnreadMode.None => Language.UnreadMode_None,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };

    internal static string? Tooltip(this UnreadMode mode) => mode switch
    {
        UnreadMode.All => Language.UnreadMode_All_Tooltip,
        UnreadMode.Unseen => Language.UnreadMode_Unseen_Tooltip,
        UnreadMode.None => Language.UnreadMode_None_Tooltip,
        _ => null,
    };
}

[Serializable]
internal class Tab
{
    public string Name = Language.Tab_DefaultName;
    public Dictionary<ChatType, ChatSource> ChatCodes = new();
    public bool ExtraChatAll;
    public HashSet<Guid> ExtraChatChannels = [];

    public UnreadMode UnreadMode = UnreadMode.Unseen;
    public bool UnhideOnActivity;
    public bool DisplayTimestamp = true;
    public InputChannel? Channel;
    public bool PopOut;
    public bool IndependentOpacity;
    public float Opacity = 100f;
    public bool InputDisabled;

    [NonSerialized]
    public uint Unread;

    [NonSerialized]
    public long LastActivity;

    [NonSerialized]
    public MessageList Messages = new();

    [NonSerialized]
    public InputChannel? PreviousChannel;

    [NonSerialized]
    public Guid Identifier = Guid.NewGuid();

    internal bool Matches(Message message) => message.Matches(ChatCodes, ExtraChatAll, ExtraChatChannels);

    internal void AddMessage(Message message, bool unread = true)
    {
        Messages.AddPrune(message, MessageManager.MessageDisplayLimit);
        if (!unread)
            return;
        Unread += 1;

        if (message.Matches(Plugin.Config.InactivityHideChannels!, Plugin.Config.InactivityHideExtraChatAll, Plugin.Config.InactivityHideExtraChatChannels))
            LastActivity = Environment.TickCount64;
    }

    internal void Clear() => Messages.Clear();

    internal Tab Clone()
    {
        return new Tab
        {
            Name = Name,
            ChatCodes = ChatCodes.ToDictionary(entry => entry.Key, entry => entry.Value),
            ExtraChatAll = ExtraChatAll,
            ExtraChatChannels = ExtraChatChannels.ToHashSet(),
            UnreadMode = UnreadMode,
            UnhideOnActivity = UnhideOnActivity,
            DisplayTimestamp = DisplayTimestamp,
            Channel = Channel,
            PopOut = PopOut,
            IndependentOpacity = IndependentOpacity,
            Opacity = Opacity,
            Identifier = Identifier,
            InputDisabled = InputDisabled,
        };
    }

    /// <summary>
    /// MessageList provides an ordered list of messages with duplicate ID
    /// tracking, sorting and mutex protection.
    /// </summary>
    internal class MessageList
    {
        private ReaderWriterLock rwl = new();

        private readonly List<Message> messages;
        private readonly HashSet<Guid> trackedMessageIds;

        public MessageList()
        {
            messages = new();
            trackedMessageIds = new();
        }

        public MessageList(int initialCapacity)
        {
            messages = new(initialCapacity);
            trackedMessageIds = new(initialCapacity);
        }

        public void AddPrune(Message message, int max)
        {
            rwl.AcquireWriterLock(-1);
            try
            {
                AddLocked(message);
                PruneMaxLocked(max);
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        public void AddSortPrune(IEnumerable<Message> messages, int max)
        {
            rwl.AcquireWriterLock(-1);
            try
            {
                foreach (var message in messages)
                    AddLocked(message);

                SortLocked();
                PruneMaxLocked(max);
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        private void AddLocked(Message message)
        {
            if (trackedMessageIds.Contains(message.Id))
                return;

            messages.Add(message);
            trackedMessageIds.Add(message.Id);
        }

        public void Clear()
        {
            rwl.AcquireWriterLock(-1);
            try
            {
                messages.Clear();
                trackedMessageIds.Clear();
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }

        private void SortLocked()
        {
            messages.Sort((a, b) => a.Date.CompareTo(b.Date));
        }

        private void PruneMaxLocked(int max)
        {
            while (messages.Count > max)
            {
                trackedMessageIds.Remove(messages[0].Id);
                messages.RemoveAt(0);
            }
        }

        /// <summary>
        /// GetReadOnly returns a read-only list of messages while holding a
        /// reader lock. The list should be used with a using statement.
        /// </summary>
        public RLockedMessageList GetReadOnly(int millisecondsTimeout = -1)
        {
            rwl.AcquireReaderLock(millisecondsTimeout);
            return new RLockedMessageList(rwl, messages);
        }

        internal class RLockedMessageList(ReaderWriterLock rwl, List<Message> messages) : IReadOnlyList<Message>, IDisposable
        {
            public IEnumerator<Message> GetEnumerator()
            {
                return messages.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count => messages.Count;

            public Message this[int index] => messages[index];

            public void Dispose()
            {
                rwl.ReleaseReaderLock();
            }
        }
    }
}

[Serializable]
internal enum PreviewPosition
{
    None,
    Inside,
    Top,
    Bottom,
    Tooltip,
}

internal static class PreviewPositionExt
{
    internal static string Name(this PreviewPosition position) => position switch
    {
        PreviewPosition.None => Language.Options_Preview_None,
        PreviewPosition.Inside => Language.Options_Preview_Inside,
        PreviewPosition.Top => Language.Options_Preview_Top,
        PreviewPosition.Bottom => Language.Options_Preview_Bottom,
        PreviewPosition.Tooltip => Language.Options_Preview_Tooltip,
        _ => throw new ArgumentOutOfRangeException(nameof(position), position, null),
    };
}

[Serializable]
internal enum CommandHelpSide
{
    None,
    Left,
    Right,
}

internal static class CommandHelpSideExt
{
    internal static string Name(this CommandHelpSide side) => side switch
    {
        CommandHelpSide.None => Language.CommandHelpSide_None,
        CommandHelpSide.Left => Language.CommandHelpSide_Left,
        CommandHelpSide.Right => Language.CommandHelpSide_Right,
        _ => throw new ArgumentOutOfRangeException(nameof(side), side, null),
    };
}

[Serializable]
internal enum KeybindMode
{
    Flexible,
    Strict,
}

internal static class KeybindModeExt
{
    internal static string Name(this KeybindMode mode) => mode switch
    {
        KeybindMode.Flexible => Language.KeybindMode_Flexible_Name,
        KeybindMode.Strict => Language.KeybindMode_Strict_Name,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };

    internal static string? Tooltip(this KeybindMode mode) => mode switch
    {
        KeybindMode.Flexible => Language.KeybindMode_Flexible_Tooltip,
        KeybindMode.Strict => Language.KeybindMode_Strict_Tooltip,
        _ => null,
    };
}

[Serializable]
internal enum LanguageOverride
{
    None,
    ChineseSimplified,
    ChineseTraditional,
    Dutch,
    English,
    French,
    German,
    Greek,

    // Italian,
    Japanese,

    // Korean,
    // Norwegian,
    PortugueseBrazil,
    Romanian,
    Russian,
    Spanish,
    Swedish,
}

internal static class LanguageOverrideExt
{
    internal static string Name(this LanguageOverride mode) => mode switch
    {
        LanguageOverride.None => Language.LanguageOverride_None,
        LanguageOverride.ChineseSimplified => "简体中文",
        LanguageOverride.ChineseTraditional => "繁體中文",
        LanguageOverride.Dutch => "Nederlands",
        LanguageOverride.English => "English",
        LanguageOverride.French => "Français",
        LanguageOverride.German => "Deutsch",
        LanguageOverride.Greek => "Ελληνικά",
        // LanguageOverride.Italian => "Italiano",
        LanguageOverride.Japanese => "日本語",
        // LanguageOverride.Korean => "한국어 (Korean)",
        // LanguageOverride.Norwegian => "Norsk",
        LanguageOverride.PortugueseBrazil => "Português do Brasil",
        LanguageOverride.Romanian => "Română",
        LanguageOverride.Russian => "Русский",
        LanguageOverride.Spanish => "Español",
        LanguageOverride.Swedish => "Svenska",
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };

    internal static string Code(this LanguageOverride mode) => mode switch
    {
        LanguageOverride.None => "",
        LanguageOverride.ChineseSimplified => "zh-hans",
        LanguageOverride.ChineseTraditional => "zh-hant",
        LanguageOverride.Dutch => "nl",
        LanguageOverride.English => "en",
        LanguageOverride.French => "fr",
        LanguageOverride.German => "de",
        LanguageOverride.Greek => "el",
        // LanguageOverride.Italian => "it",
        LanguageOverride.Japanese => "ja",
        // LanguageOverride.Korean => "ko",
        // LanguageOverride.Norwegian => "no",
        LanguageOverride.PortugueseBrazil => "pt-br",
        LanguageOverride.Romanian => "ro",
        LanguageOverride.Russian => "ru",
        LanguageOverride.Spanish => "es",
        LanguageOverride.Swedish => "sv",
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };
}

[Serializable]
[Flags]
internal enum ExtraGlyphRanges
{
    ChineseFull = 1 << 0,
    ChineseSimplifiedCommon = 1 << 1,
    Cyrillic = 1 << 2,
    Japanese = 1 << 3,
    Korean = 1 << 4,
    Thai = 1 << 5,
    Vietnamese = 1 << 6,
}

internal static class ExtraGlyphRangesExt
{
    internal static string Name(this ExtraGlyphRanges ranges) => ranges switch
    {
        ExtraGlyphRanges.ChineseFull => Language.ExtraGlyphRanges_ChineseFull_Name,
        ExtraGlyphRanges.ChineseSimplifiedCommon => Language.ExtraGlyphRanges_ChineseSimplifiedCommon_Name,
        ExtraGlyphRanges.Cyrillic => Language.ExtraGlyphRanges_Cyrillic_Name,
        ExtraGlyphRanges.Japanese => Language.ExtraGlyphRanges_Japanese_Name,
        ExtraGlyphRanges.Korean => Language.ExtraGlyphRanges_Korean_Name,
        ExtraGlyphRanges.Thai => Language.ExtraGlyphRanges_Thai_Name,
        ExtraGlyphRanges.Vietnamese => Language.ExtraGlyphRanges_Vietnamese_Name,
        _ => throw new ArgumentOutOfRangeException(nameof(ranges), ranges, null),
    };

    internal static nint Range(this ExtraGlyphRanges ranges) => ranges switch
    {
        ExtraGlyphRanges.ChineseFull => ImGui.GetIO().Fonts.GetGlyphRangesChineseFull(),
        ExtraGlyphRanges.ChineseSimplifiedCommon => ImGui.GetIO().Fonts.GetGlyphRangesChineseSimplifiedCommon(),
        ExtraGlyphRanges.Cyrillic => ImGui.GetIO().Fonts.GetGlyphRangesCyrillic(),
        ExtraGlyphRanges.Japanese => ImGui.GetIO().Fonts.GetGlyphRangesJapanese(),
        ExtraGlyphRanges.Korean => ImGui.GetIO().Fonts.GetGlyphRangesKorean(),
        ExtraGlyphRanges.Thai => ImGui.GetIO().Fonts.GetGlyphRangesThai(),
        ExtraGlyphRanges.Vietnamese => ImGui.GetIO().Fonts.GetGlyphRangesVietnamese(),
        _ => throw new ArgumentOutOfRangeException(nameof(ranges), ranges, null),
    };
}
