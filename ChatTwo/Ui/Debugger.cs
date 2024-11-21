﻿using System.Numerics;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;

namespace ChatTwo.Ui;

public class DebuggerWindow : Window
{
    private readonly Plugin Plugin;
    private readonly ChatLogWindow ChatLogWindow;

    public DebuggerWindow(Plugin plugin) : base($"Debugger###chat2-debugger")
    {
        Plugin = plugin;
        ChatLogWindow = plugin.ChatLogWindow;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(475, 600),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        RespectCloseHotkey = false;
        DisableWindowSounds = true;

        #if DEBUG
        Plugin.Commands.Register("/chat2Debugger", showInHelp: false).Execute += Toggle;
        #endif
    }

    public void Dispose()
    {
        #if DEBUG
        Plugin.Commands.Register("/chat2Debugger", showInHelp: false).Execute -= Toggle;
        #endif
    }

    private void Toggle(string _, string __) => Toggle();

    public override unsafe void Draw()
    {
        var agent = (nint) AgentItemDetail.Instance();
        ImGui.TextUnformatted($"Current Cursor Pos: {ChatLogWindow.CursorPos}");
        if (ImGui.Selectable($"Agent Address: {agent:X}"))
            ImGui.SetClipboardText(agent.ToString("X"));

        ImGui.TextUnformatted($"Handle Tooltips: {ChatLogWindow.PayloadHandler.HandleTooltips}");
        ImGui.TextUnformatted($"Hovered Item: {ChatLogWindow.PayloadHandler.HoveredItem}");
        ImGui.TextUnformatted($"Hover Counter: {ChatLogWindow.PayloadHandler.HoverCounter}");
        ImGui.TextUnformatted($"Last Hover Counter: {ChatLogWindow.PayloadHandler.LastHoverCounter}");
    }
}
