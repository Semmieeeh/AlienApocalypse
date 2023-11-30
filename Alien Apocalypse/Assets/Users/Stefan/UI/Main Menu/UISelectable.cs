using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Derive from this class if you want to have UI objects with mouse/pointer interactions events
/// </summary>
public class UISelectable : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{

    [SerializeField]
    SelectableEvents interactionEvents;

    [SerializeField]
    SelectableSFX interactionSoundEffects;

    [SerializeField]
    bool m_Interactable = true;

    public bool Interactable
    {
        get { return m_Interactable; }
        set
        {
            if (value == m_Interactable) return;
            m_Interactable = value;

            OnInteractionStatusChanged();
        }
    }

    [SerializeField]
    bool m_deriveFromParentSelectable = false;

    public bool DeriveFromParentSelectable
    {
        get
        {
            return m_deriveFromParentSelectable;
        }
        set
        {
            m_deriveFromParentSelectable = value;
        }
    }

    private bool calledFromParent;

    private AudioSource m_audioSource;

    public AudioSource AudioSource
    {
        get
        {
            if (m_audioSource == null) m_audioSource = transform.root.GetOrAddComponent<AudioSource>();

            return m_audioSource;
        }
    }

    [SerializeField]
    private bool m_Hovered;
    public bool Hovered { get { return m_Hovered; } private set { m_Hovered = value; } }
    public bool TriesClicking { get; private set; }

    private bool m_selected;

    public bool Selected
    {
        get
        {
            return m_selected;
        }
        set
        {
            m_selected = value;
        }
    }

    UISelectable[] m_derivedSelectables;

    public UISelectable[] DerivedSelectables
    {
        get
        {
            if(m_derivedSelectables == null)
            {
                List<UISelectable> temp = new();

                var selectables = transform.GetComponentsInHierarchy<UISelectable>();

                foreach (UISelectable selectable in selectables)
                {
                    if (selectable.DeriveFromParentSelectable)
                    {
                        temp.Add(selectable);
                    }
                }

                m_derivedSelectables = temp.ToArray();
            }
            return m_derivedSelectables;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnPointerExit(new PointerEventData(EventSystem.current));
    }
    /// <summary>
    /// Called whenever the user clicks on this UISelectable
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (DeriveFromParentSelectable && calledFromParent == false) return;
        if (calledFromParent) calledFromParent = false;

        TriesClicking = true;

        interactionEvents.OnPointerDown.Invoke ( );


        PlaySound (interactionSoundEffects.OnClickEnter);

        for (int i = 0; i < DerivedSelectables.Length; i++)
        {
            DerivedSelectables[i].calledFromParent = true;
            DerivedSelectables[i].OnPointerDown(eventData);
        }

        OnClickStart();
    }

    /// <summary>
    /// Called whenever the user cancels/finishes the click
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (DeriveFromParentSelectable && calledFromParent == false) return;
        if (calledFromParent) calledFromParent = false;

        TriesClicking = false;
        PlaySound(interactionSoundEffects.OnClickExit);

        interactionEvents.OnPointerUp.Invoke( );

        for (int i = 0; i < DerivedSelectables.Length; i++)
        {
            DerivedSelectables[i].calledFromParent = true;
            DerivedSelectables[i].OnPointerUp(eventData);
        }

        OnClickEnd();
    }

    /// <summary>
    /// Called whenever the pointer enters the hitbox of an UISelectable
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (DeriveFromParentSelectable && calledFromParent == false) return;
        if (calledFromParent) calledFromParent = false;

        Hovered = true;
        PlaySound(interactionSoundEffects.OnHoverEnter);

        interactionEvents.OnHoverEnter?.Invoke ( );
        
        for ( int i = 0; i < DerivedSelectables.Length; i++)
        {
            DerivedSelectables[i].calledFromParent = true;
            DerivedSelectables[i].OnPointerEnter(eventData);
        }

        OnHoverEnter();
    }

    /// <summary>
    /// Called whenever the pointer leaves the hitbox of an UISelectable
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (DeriveFromParentSelectable && calledFromParent == false) return;
        if (calledFromParent) calledFromParent = false;

        Hovered = false;
        TriesClicking = false;

        interactionEvents.OnHoverExit?.Invoke ( );

        PlaySound(interactionSoundEffects.OnHoverExit);

        for (int i = 0; i < DerivedSelectables.Length; i++)
        {
            DerivedSelectables[i].calledFromParent = true;
            DerivedSelectables[i].OnPointerExit(eventData);
        }

        OnHoverExit();
    }

    /// <summary>
    /// Called whenever the user clicks this button. 
    /// Use this function for proper click interaction
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (DeriveFromParentSelectable && calledFromParent == false) return;
        if (calledFromParent) calledFromParent = false;

        PlaySound(interactionSoundEffects.OnClick);

        interactionEvents.OnPointerClick.Invoke ( );



        for ( int i = 0; i < DerivedSelectables.Length; i++)
        {
            DerivedSelectables[i].calledFromParent = true;
            DerivedSelectables[i].OnPointerClick(eventData);
        }

        OnClick();

    }

    public virtual void OnInteractionStatusChanged()
    {
        throw new System.NotImplementedException();
    }

    protected void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource.clip = clip;
        AudioSource.Play();
    }

    protected virtual void Update()
    {
        if (Hovered)
            OnHoverStay();

        if (TriesClicking)
            OnClickStay();
    }

    #region Overridable Functions

    /// <summary>
    ///// Overridable function to add logic to your selectable item whenever the user / pointer started hovering over this interface element
    /// </summary>
    public virtual void OnHoverEnter() { }

    /// <summary>
    /// Overridable function to add logic to your selectable item whenever the user / pointer stopped hovering over this interface element
    /// </summary>
    public virtual void OnHoverExit() { }

    /// <summary> 
    /// Overridable function to add logic to your selectable item whenever the user / pointer is hovered over this interface element(like update, but only if this element is hovered)
    /// </summary>
    public virtual void OnHoverStay() { }

    /// <summary>
    /// Overridable function to add logic to your selectable item whenever the user / pointer started clicking this interface element
    /// </summary>
    public virtual void OnClickStart() { }

    /// <summary>
    /// Overridable function to add logic to your selectable item whenever the user / pointer stopped clicking this interface element
    /// </summary>
    public virtual void OnClickEnd() { }

    /// <summary>
    /// Overridable function to add logic to your selectable item whenever the user / pointer is clicked over this interface element(like update, but only if this element is clicked)
    /// </summary>
    public virtual void OnClickStay() { }

    /// <summary>
    /// Overridable functen whenever this element is clicked
    /// </summary>
    public virtual void OnClick() { }

    /// <summary>
    /// Ovveridable function whenever this element is selected
    /// </summary>
    public virtual void OnSelect() { }

    /// <summary>
    /// Overridable function whenever this element is deselected
    /// </summary>
    public virtual void OnDeselect() { }

    #endregion

}

[System.Serializable]
public struct SelectableEvents
{
    [Header("Hover Events")]
    public UnityEvent OnHoverEnter;
    public UnityEvent OnHoverExit;

    [Space(8)]
    [Header("Click Interactions")]
    public UnityEvent OnPointerUp;
    public UnityEvent OnPointerDown;
    public UnityEvent OnPointerClick;
}

[System.Serializable]
public struct SelectableSFX
{
    [Header("Hover Sound Effects")]
    public AudioClip OnHoverEnter;
    public AudioClip OnHoverExit;

    [Space(8)]
    [Header("Click Sound Effects")]
    public AudioClip OnClickEnter;
    public AudioClip OnClickExit;
    public AudioClip OnClick;
}
