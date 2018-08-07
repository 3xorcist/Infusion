#load "Specs.csx"
#load "items.csx"
#load "container.csx"

using System;
using System.Collections.Generic;
using System.Linq;

public class Warehouse
{
    private Dictionary<ItemSpec, IContainer> locations = new Dictionary<ItemSpec, IContainer>();
    
    public static Warehouse Global = new Warehouse();

    public void AddLocation(ItemSpec spec, IContainer container)
    {
        locations.Add(spec, container);
    }
    
    public IContainer GetContainer(ItemSpec requestedSpec)
    {
        foreach (var locationPair in locations)
        {
            if (requestedSpec.IsKindOf(locationPair.Key))
            {
                return locationPair.Value;
            }
        }
        
        throw new InvalidOperationException($"No registered container for {Specs.TranslateToName(requestedSpec)} in your warehouse."); 
    }
    
    public IContainer GetContainer(Item item)
    {
        if (TryGetContainer(item, out IContainer container))
            return container;
        
        throw new InvalidOperationException($"No registered container for {Specs.TranslateToName(item)}"); 
    }
    
    public bool TryGetContainer(Item item, out IContainer container)
    {
        foreach (var locationPair in locations)
        {
            if (locationPair.Key.Matches(item))
            {
                container = locationPair.Value;
                return true;
            }
        }
        
        container = null;
        return false;
    }
    
    public void Sort(ObjectId sourceContainerId)
    {
        var items = UO.Items.InContainer(sourceContainerId).ToArray();
        
        foreach (var item in items)
        {
            if (TryGetContainer(item, out IContainer container))
            {
                container.Open();
                Items.TryMoveItem(item, container.Id);
            }
        }
    }
    
    public void Reload(ObjectId targetContainerId, ItemSpec spec, int targetAmount)
    {
        var currentAmount = UO.Items.InContainer(targetContainerId)
            .Matching(spec)
            .Sum(x => x.Amount);
            
        if (currentAmount >= targetAmount)
            return;
            
        var container = GetContainer(spec);
        container.Open();
        
        Items.Reload(container.Id, targetContainerId, targetAmount, spec);
    }
}