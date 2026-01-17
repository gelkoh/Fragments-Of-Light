using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UICursorTrigger : MonoBehaviour
{
    private CursorManager m_cursorManager;
    private bool m_isOverButton = false;

    void Start()
    {
        m_cursorManager = ManagersManager.Get<CursorManager>();
    }

    void Update()
    {
        CheckPointerOverUI();
    }

    private void CheckPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool foundButton = false;
        foreach (var result in results)
        {
            // Prüfen, ob das getroffene Objekt oder ein Parent ein Button ist
            if (result.gameObject.GetComponentInParent<Selectable>() != null)
            {
                foundButton = true;
                break;
            }
        }

        if (foundButton && !m_isOverButton)
        {
            m_isOverButton = true;
            m_cursorManager.SetCursorState(CursorState.Pointer);
        }
        else if (!foundButton && m_isOverButton)
        {
            m_isOverButton = false;
            // Hier wichtig: Nur zurück auf Default, wenn nicht gerade rotiert wird!
            m_cursorManager.SetCursorState(CursorState.Default);
        }
    }
}