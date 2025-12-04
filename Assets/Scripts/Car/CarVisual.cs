using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarVisual : MonoBehaviour
{
    [SerializeField] private List<Transform> partsForPainting;
    private CarSettings settings;

    public void Initialize(CarSettings settings)
    {
        this.settings = settings;
    }

    public void SetColor(MyColor color)
    {
        var pair = settings.colorMaterialPairs.FirstOrDefault(x => x.color == color);
        if (pair.material == null) return;

        foreach (var part in partsForPainting)
        {
            part.GetComponent<MeshRenderer>().material = pair.material;
        }
    }
}
