using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public interface Filter 
{
    SubstanceMixture FilterLiquid(SubstanceMixture mix);
    void ApplyFilter(Becher becher);
    void RemoveFilter();
}
