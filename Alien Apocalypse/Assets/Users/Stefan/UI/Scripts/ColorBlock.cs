using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorBlock
{
    public delegate void ColorChanged(Color color);

    public ColorChanged onColorChangedEditor;

    [SerializeField]
    private Color m_DefaultColor;

    [SerializeField]
    private Color m_HighlightedColor;

    [SerializeField]
    private Color m_ClickedColor;

    [SerializeField]
    private Color m_SelectedColor;

    [SerializeField]
    private Color m_DisabledColor;

    public Color DefaultColor
    {
        get
        {
            return m_DefaultColor;
        }
        set
        {
            if (value == m_DefaultColor) return;
            m_DefaultColor = value;

            OnColorChanged(m_DefaultColor);
        }
    }

    public Color HoveredColor
    {
        get
        {
            return m_HighlightedColor;
        }
        set
        {
            if (value == m_HighlightedColor) return;
            m_HighlightedColor = value;

            OnColorChanged(m_HighlightedColor);
        }
    }

    public Color ClickedColor
    {
        get
        {
            return m_ClickedColor;
        }
        set
        {
            if (value == m_ClickedColor) return;
            m_ClickedColor = value;

            OnColorChanged(m_ClickedColor);
        }
    }

    public Color SelectedColor
    {
        get
        {
            return m_SelectedColor;
        }
        set
        {
            if (value == m_SelectedColor) return;
            m_SelectedColor = value;

            OnColorChanged(m_SelectedColor);
        }
    }

    public Color DisabledColor
    {
        get
        {
            return m_DisabledColor;
        }
        set
        {
            if (value == m_DisabledColor) return;
            m_DisabledColor = value;

            OnColorChanged(m_DisabledColor);
        }
    }

    private void OnColorChanged(Color changedColor)
    {
        if (Application.isEditor)
        {
            onColorChangedEditor?.Invoke(changedColor);
        }

    }

    public Color GetColor(UISelectable selectable)
    {
        // Order of colors to return
        // Disabled
        // Clicked
        // Hovered
        // Selected
        // Default

        if (!selectable.Interactable)
            return DisabledColor;
        if (selectable.TriesClicking)
            return ClickedColor;
        if (selectable.Hovered)
            return HoveredColor;
        if (selectable.Selected)
            return SelectedColor;

        return DefaultColor;
    }
}
