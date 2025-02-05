using UnityEngine;

public interface Filter 
{
    void FilterLiquid();

    void ApplyFilter(Becher becher);
    void RemoveFilter();
}
