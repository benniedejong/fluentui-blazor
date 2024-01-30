﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Microsoft.Fast.Components.FluentUI;

/// <summary>
/// Extends the OnKeyDown blazor event to provide a more fluent way to evaluate the key code.
/// The anchor must refer to the ID of an element (or sub-element) accepting the focus.
/// </summary>
public partial class FluentKeyCode
{
    private const string JAVASCRIPT_FILE = "./_content/Microsoft.Fast.Components.FluentUI/Components/Accessibility/FluentKeyCode.razor.js";
    private DotNetObjectReference<FluentKeyCode>? _dotNetHelper = null;
    private readonly KeyCode[] _Modifiers = new[] { KeyCode.Shift, KeyCode.Alt, KeyCode.Ctrl, KeyCode.Meta };

    /// <summary />
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary />
    private IJSObjectReference? Module { get; set; }

    /// <summary>
    /// Required. Gets or sets the control identifier associated with the KeyCode engine.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Anchor { get; set; } = string.Empty;

    /// <summary>
    /// Event triggered when a KeyDown event is raised.
    /// </summary>
    [Parameter]
    public EventCallback<FluentKeyCodeEventArgs> OnKeyDown { get; set; }

    /// <summary>
    /// Ignore modifier keys (Shift, Alt, Ctrl, Meta) when evaluating the key code.
    /// </summary>
    [Parameter]
    public bool IgnoreModifier { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of <see cref="KeyCode"/> to accept, and only this list, when evaluating the key code.
    /// </summary>
    [Parameter]
    public KeyCode[] Only { get; set; } = Array.Empty<KeyCode>();

    /// <summary>
    /// Gets or sets the list of <see cref="KeyCode"/> to ignore when evaluating the key code.
    /// </summary>
    [Parameter]
    public KeyCode[] Ignore { get; set; } = Array.Empty<KeyCode>();

    /// <summary>
    /// Gets or sets a way to prevent further propagation of the current event in the capturing and bubbling phases.
    /// </summary>
    [Parameter]
    public bool StopPropagation { get; set; } = false;

    /// <summary />
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Module ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", JAVASCRIPT_FILE);
            _dotNetHelper = DotNetObjectReference.Create(this);

            await Module.InvokeVoidAsync("RegisterKeyCode", Anchor, Only, IgnoreModifier ? Ignore.Union(_Modifiers) : Ignore, StopPropagation, _dotNetHelper);
        }
    }

    /// <summary>
    /// Internal method.
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="value"></param>
    /// <param name="ctrlKey"></param>
    /// <param name="shiftKey"></param>
    /// <param name="altKey"></param>
    /// <param name="metaKey"></param>
    /// <param name="location"></param>
    /// <param name="targetId"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task OnKeyDownRaisedAsync(int keyCode, string value, bool ctrlKey, bool shiftKey, bool altKey, bool metaKey, int location, string targetId)
    {
        if (OnKeyDown.HasDelegate)
        {
            await OnKeyDown.InvokeAsync(new FluentKeyCodeEventArgs
            {
                Location = Enum.IsDefined(typeof(KeyLocation), location) ? (KeyLocation)location : KeyLocation.Unknown,
                Key = Enum.IsDefined(typeof(KeyCode), keyCode) ? (KeyCode)keyCode : KeyCode.Unknown,
                KeyCode = keyCode,
                Value = value,
                CtrlKey = ctrlKey,
                ShiftKey = shiftKey,
                AltKey = altKey,
                MetaKey = metaKey,
                TargetId = targetId,
            });
        }
    }
}

