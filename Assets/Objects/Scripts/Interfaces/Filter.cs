using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public interface Filter 
{
    void FilterLiquid(List<Substance> toFilterSubstances);
    void ApplyFilter(Becher becher);
    void RemoveFilter();
}
