using UnityEngine;

public class ContextUIController : MonoBehaviour
{
    private TMPro.TextMeshProUGUI contextText;

    private void Start()
    {
        contextText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public void SetContextActive(string contextInteractionText)
    {
        contextText.text = contextInteractionText;
        gameObject.SetActive(true);
    }

    public void HideContextUI()
    {
        gameObject.SetActive(false);
    }

    public void CheckContextState(Grabbable grabbedObject, RaycastHit hit)
    {
        //non ho oggetti in mano -> grab o interazione con strumenti
        if (grabbedObject == null)
        {
            if (hit.transform.TryGetComponent<Grabbable>(out var grabbable))
            {
                //SetContextActive("Pick Up");
                if (grabbable.TryGetComponent<SubstanceVial>(out var vial))
                {
                    SetContextActive("Pick Up: " + vial.SubstanceContained());
                }
                else
                {
                    SetContextActive("Pick Up");
                }
            }
            else if (hit.transform.TryGetComponent<BalanceButton>(out var balance))
            {
                SetContextActive("Tare");
            }
            else if (hit.transform.TryGetComponent<Oven>(out var oven))
            {
                if (oven.IsOpen && !oven.IsMoving)
                {
                    SetContextActive("Close oven");
                }
                else if (!oven.IsOpen && !oven.IsMoving)
                {
                    SetContextActive("Open oven");
                }
                else
                {
                    HideContextUI();
                }
            }
            else if (hit.transform.TryGetComponent<Fridge>(out var fridge))
            {
                if (fridge.IsOpen && !fridge.IsMoving)
                {
                    SetContextActive("Close fridge");
                }
                else if (!fridge.IsOpen && !fridge.IsMoving)
                {
                    SetContextActive("Open fridge");
                }
                else
                {
                    HideContextUI();
                }
            }
            else
            {
                HideContextUI();
            }
        }
        //ho un oggetto in mano -> azione contestuale

        //guardo un oggetto fillable -> posso versare con un pourable o un dropper, oppure mischiare con una bacchetta
        else if (hit.transform.TryGetComponent<Fillable>(out var fillable))
        {
            if (grabbedObject.TryGetComponent<Pourable>(out var pourable) && fillable != pourable && fillable.GetRemainingVolume() != 0 
                && pourable.GetCurrentVolume() != 0)
            {
                SetContextActive("Pour");
            }
            else if (grabbedObject.TryGetComponent<Dropper>(out var dropper) && dropper.IsFull() && fillable.GetRemainingVolume() != 0)
            {
                SetContextActive("Drop liquid");
            }
            else if (hit.transform.TryGetComponent<StirringRod>(out var rod))
            {
                SetContextActive("Mix");
            }
            else if (hit.transform.TryGetComponent<Pourable>(out var pourable2) && grabbedObject.TryGetComponent<Dropper>(out var dropper2)
            && !dropper2.IsFull() && pourable2.GetCurrentVolume() > 0)
            {
                SetContextActive("Suck");
            }
            else if (hit.transform.TryGetComponent<Becher>(out var becher) && grabbedObject.TryGetComponent<StirringRod>(out var stirringRod))
            {
                SetContextActive("Mix");
            }
            else
            {
                HideContextUI();
            }

        }
        //guardo un oggetto pourable -> posso solo raccogliere con un dropper
        else if (hit.transform.TryGetComponent<Pourable>(out var pourable) && grabbedObject.TryGetComponent<Dropper>(out var dropper)
            && !dropper.IsFull() && pourable.GetCurrentVolume() > 0)
        {
            SetContextActive("Suck");
        }
        else
        {
            HideContextUI();
        }
    }
}
