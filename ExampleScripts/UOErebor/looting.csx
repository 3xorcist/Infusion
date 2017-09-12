#load "Specs.csx"
#load "ignore.csx"
#load "equip.csx"

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Infusion.Commands;

public static class Looting
{
    // Create a "private" instance of journal for cooking. If you delete this journal it
    // doesn't affect either UO.Journal or other instances of GameJournal so
    // you can try looting and healing in parallel
    // and you can still use journal.Delete method in both scripts at the same time.
    // It means, that you don't need tricks like UO.SetJournalLine(number,text) in
    // Injection.
    private static GameJournal journal = UO.CreateJournal();

    public static ItemSpec UselessLoot { get; } = new[] { Specs.Torsos, Specs.Rocks, Specs.Corpse };
    public static ItemSpec IgnoredLoot { get; set; } = UselessLoot;
    public static ItemSpec OnGroundLoot { get; set; } = new[] { Specs.Gold, Specs.Regs, Specs.Gem, Specs.Bolt };
    public static ObjectId? LootContainerId { get; set; }
    
    public static IgnoredItems ignoredItems = new IgnoredItems();

    public static IEnumerable<Item> GetLootableCorpses()
    {
        var corpses = UO.Items
            .Matching(Specs.Corpse)
            .MaxDistance(20)
            .Where(x => !ignoredItems.IsIgnored(x))
            .OrderByDistance().ToArray();

        return corpses;
    }
    
    public static void RipAndLootNearest()
    {
        LootGround();

        var corpses = GetLootableCorpses().ToArray();
        var corpse = corpses.MaxDistance(3).FirstOrDefault();

        if (corpse != null)
        {
            var handEquipment = Equip.GetHand();

            try
            {
                Rip(corpse);
                Loot(corpse);
                if (corpses.Length - 1 > 0)
                    HighlightLootableCorpses(corpses.Except(new[] { corpse }));
                else
                    UO.ClientPrint($"No more corpses to loot remaining");
            }
            finally
            {
            
                // It seems that re-wearing an item directly
                // after ripping a body and right before
                // looting may crash the game client.
                Equip.Set(handEquipment);
            }
        }
        else
        {
            if (corpses.Length > 0)
            {
                HighlightLootableCorpses(corpses);
                UO.ClientPrint($"No corpse reacheable but {corpses.Length} corpses to loot remaining");
            }
            else
                UO.ClientPrint("no corpse found");
        }
    }

    public static void HighlightLootableCorpses(IEnumerable<Item> lootableCorpses)
    {
        UO.ClientPrint($"{lootableCorpses.Count()} corpses to loot remaining");
        foreach (var corpse in lootableCorpses)
        {
            int lootableItemsCount = UO.Items.InContainer(corpse)
                .NotMatching(IgnoredLoot).Count();
            UO.ClientPrint($"--{lootableItemsCount}--", "System", corpse.Id, corpse.Type,
                SpeechType.Speech, Colors.Green, log: false);
        }
    }

    public static void LootNearest()
    {
        LootGround();

        var corpses = GetLootableCorpses().ToArray();
        var corpse = corpses.MaxDistance(3).FirstOrDefault();

        if (corpse != null)
        {
            Loot(corpse);
            if (corpses.Length - 1 > 0)
                HighlightLootableCorpses(corpses.Except(new[] { corpse }));
            else
                UO.ClientPrint($"No more corpses to loot remaining");
        }
        else
        {
            if (corpses.Length > 0)
            {
                UO.ClientPrint($"No corpse reacheable but {corpses.Length} corpses to loot remaining");
                HighlightLootableCorpses(corpses);
            }
            else
                UO.ClientPrint("no corpse found");
        }
    }

    public static Item LootContainer => LootContainerId.HasValue ? UO.Items[LootContainerId.Value] ?? UO.Me.BackPack : UO.Me.BackPack;

    public static void LootGround()
    {
        var itemsOnGround = UO.Items.Matching(OnGroundLoot).MaxDistance(3).OrderByDistance().ToArray();
        if (itemsOnGround.Length > 0)
            UO.ClientPrint("Looting ground");

        foreach (var item in itemsOnGround)
        {
            if (!UO.TryMoveItem(item, LootContainer))
            {
                UO.ClientPrint("Cannot pickup item, cancelling ground loot");
                break;
            }
        }
    }

    public static void Loot()
    {
        var container = UO.AskForItem();
        if (container != null)
        {
            Loot(container);
        }
        else
            UO.ClientPrint("no container for loot");
    }

    public static void Loot(ObjectId containerId)
    {
        var container = UO.Items[containerId];
        if (container == null)
            UO.ClientPrint($"Cannot find {containerId} container");

        Loot(container);
    }

    public static void Loot(Item container)
    {
        var originalLocation = UO.Me.Location;

        UO.OpenContainer(container);

        UO.ClientPrint($"Number of items in container: {UO.Items.InContainer(container).Count()}");

        foreach (var itemToPickup in UO.Items.InContainer(container).ToArray())
        {
            if (!IgnoredLoot.Matches(itemToPickup))
            {
                UO.ClientPrint($"Looting {Specs.TranslateToName(itemToPickup)} ({itemToPickup.Amount})");
                if (!UO.TryMoveItem(itemToPickup, LootContainer))
                {
                    UO.ClientPrint("Cannot pickup an item, cancelling loot");
                    return;
                }

                UO.Wait(25);
                if (journal.Contains("Ne tak rychle!"))
                {
                    journal.Delete();
                    UO.Wait(4000);
                }
            }
            else
            {
                UO.ClientPrint($"Ignoring {Specs.TranslateToName(itemToPickup)} ({itemToPickup.Amount})");
            }
        }

        UO.ClientPrint($"Looting finished, ignoring corpse {container.Id:X8}");
        ignoredItems.Ignore(container);
    }

    public static void Rip(Item container)
    {
        UO.ClientPrint("Ripping");
        UO.Use(Specs.Knives);

        UO.WaitForTarget();
        UO.Target(container);
        journal.WaitAny("Rozrezal jsi mrtvolu.");
    }

}

UO.RegisterCommand("ripandloot", Looting.RipAndLootNearest);
UO.RegisterCommand("loot", Looting.LootNearest);
