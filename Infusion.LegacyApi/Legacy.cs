﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infusion.Commands;
using Infusion.Gumps;
using Infusion.Logging;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class Legacy
    {
        private readonly ItemsObservers itemsObserver;
        private readonly JournalObservers journalObservers;
        private readonly BlockedPacketsFilters blockedPacketsFilters;
        private readonly GumpObservers gumpObservers;

        private readonly ThreadLocal<CancellationToken?> cancellationToken =
            new ThreadLocal<CancellationToken?>(() => null);

        private readonly Targeting targeting;

        private readonly JournalSource journalSource;
        private readonly PlayerObservers playerObservers;

        private readonly ILogger logger;

        public LegacyEvents Events { get; }

        public Configuration Configuration { get; }

        public Gump CurrentGump => gumpObservers.CurrentGump;

        public CancellationToken? CancellationToken
        {
            get => cancellationToken.Value;
            set => cancellationToken.Value = value;
        }

        public CommandHandler CommandHandler { get; }

        public UltimaMap Map { get; } = new UltimaMap();

        public ItemCollection Items { get; }

        public Player Me { get; }

        public GameJournal Journal { get; }

        public ISet<uint> IgnoredItems { get; } = new HashSet<uint>();

        public UltimaServer Server { get; }
        public UltimaClient Client { get; }

        public void OpenContainer(Item container, TimeSpan? timeout = null)
        {
            OpenContainer(container.Id);
        }

        public void OpenContainer(uint containerId, TimeSpan? timeout = null)
        {
            Use(containerId);

            itemsObserver.WaitForContainerOpened(timeout);
        }

        public Command RegisterCommand(string name, Action commandAction) => CommandHandler.RegisterCommand(name,
            commandAction);

        public Command RegisterCommand(string name, Action<string> commandAction) => CommandHandler.RegisterCommand(name,
            commandAction);

        public void Alert(string message)
        {
            logger.Critical(message);
        }

        private void RegisterDefaultCommands()
        {
            CommandHandler.RegisterCommand(new Command("walkto", parameters => WalkTo(parameters),
                "Walks to the specified location.", "Example: ,walkto 1234 432"));
            CommandHandler.RegisterCommand(new Command("info", InfoCommand,
                "Shows information about selected item or tile."));
            CommandHandler.RegisterCommand(new Command("lastgumpinfo", LastGumpInfo,
                "Shows information about the last gump dispalyed in Ultima Online client."));
            CommandHandler.RegisterCommand(new Command("opendoor", OpenDoor,
                "Opens neares closed doors. This is analogue to UO client's 'opendoor' macro."));
            CommandHandler.RegisterCommand(new Command("waron", WarModeOn,
                "War mode on."));
            CommandHandler.RegisterCommand(new Command("waroff", WarModeOff,
                "War mode off."));
            CommandHandler.RegisterCommand(new Command("terminate", Terminate,
                "Terminates all running commands and scripts.", executionMode: CommandExecutionMode.Direct));
        }

        public GameJournal CreateJournal()
        {
            return new GameJournal(journalSource, this);
        }

        public void Say(string message)
        {
            journalSource.NotifyLastAction();

            Server.Say(message);
        }

        public Gump WaitForGump(TimeSpan? timeout = null)
        {
            return gumpObservers.WaitForGump(timeout);
        }

        public Legacy(Configuration configuration, CommandHandler commandHandler,
            UltimaServer ultimaServer, UltimaClient ultimaClient, ILogger logger)
        {
            Me = new Player(() => Items.OnLayer(Layer.Mount).First() != null, Server, this);
            gumpObservers = new GumpObservers(ultimaServer, ultimaClient, this);
            Items = new ItemCollection(Me);
            itemsObserver = new ItemsObservers(Items, ultimaServer, this);
            Me.LocationChanged += itemsObserver.OnPlayerPositionChanged;
            journalSource = new JournalSource();
            Journal = new GameJournal(journalSource, this);
            journalObservers = new JournalObservers(journalSource, ultimaServer);
            playerObservers = new PlayerObservers(Me, ultimaClient, ultimaServer, logger, this);
            playerObservers.WalkRequestDequeued += Me.OnWalkRequestDequeued;
            targeting = new Targeting(ultimaServer, ultimaClient, this);

            blockedPacketsFilters = new BlockedPacketsFilters(ultimaServer);

            Events = new LegacyEvents(itemsObserver);

            this.logger = logger;
            Server = ultimaServer;
            Client = ultimaClient;

            CommandHandler = commandHandler;
            RegisterDefaultCommands();
            CommandHandler.CancellationTokenCreated += (sender, token) => CancellationToken = token;

            Configuration = configuration;
        }

        public void Use(uint objectId)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            Server.DoubleClick(objectId);
        }

        internal void CheckCancellation()
        {
            cancellationToken.Value?.ThrowIfCancellationRequested();
        }

        public void RequestStatus(Item item)
        {
            Server.RequestStatus(item.Id);
        }

        public void Use(Item item)
        {
            Use(item.Id);
        }

        public void Click(Item item)
        {
            Server.Click(item.Id);
        }

        public bool Use(ItemSpec spec)
        {
            CheckCancellation();

            var item = Items.Matching(spec).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.Matching(spec).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.Matching(spec).InContainer(Me.BackPack).First()
                       ?? Items.Matching(spec).OnLayer(Layer.Backpack).First();

            if (item != null)
                Use(item);
            else
                return false;

            return true;
        }

        public bool UseType(ModelId type)
        {
            CheckCancellation();

            var item = Items.OfType(type).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.OfType(type).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.OfType(type).InContainer(Me.BackPack).First()
                       ?? Items.OfType(type).OnLayer(Layer.Backpack).First();
            if (item != null)
            {
                Use(item);
            }
            else
                return false;

            return true;
        }

        public bool UseType(params ModelId[] types)
        {
            CheckCancellation();

            var item = Items.OfType(types).InContainer(Me.BackPack).First()
                       ?? Items.OfType(types).OnLayer(Layer.OneHandedWeapon).First()
                       ?? Items.OfType(types).OnLayer(Layer.TwoHandedWeapon).First()
                       ?? Items.OfType(types).OnLayer(Layer.Backpack).First();

            if (item != null)
            {
                Use(item);
                return true;
            }

            var typesString = types.Select(u => u.ToString()).Aggregate(string.Empty, (l, r) => l + ", " + r);
            Log($"Item of any type {typesString} not found.");

            return false;
        }

        public void Wait(int milliseconds)
        {
            while (milliseconds > 0)
            {
                CheckCancellation();
                Thread.Sleep(50);
                milliseconds -= 50;
            }
        }

        public void Wait(TimeSpan span)
        {
            Wait((int) span.TotalMilliseconds);
        }

        public void WaitToAvoidFastWalk(MovementType movementType)
        {
            Me.WaitToAvoidFastWalk(movementType);
        }

        public void WaitWalkAcknowledged()
        {
            CheckCancellation();
            Me.WaitWalkAcknowledged();
        }

        public void Walk(Direction direction, MovementType movementType = MovementType.Walk)
        {
            CheckCancellation();

            Me.Walk(direction, movementType);
        }

        public void WarModeOn()
        {
            Server.RequestWarMode(WarMode.Fighting);
        }

        public void WarModeOff()
        {
            Server.RequestWarMode(WarMode.Normal);
        }

        public AttackResult Attack(Item target, TimeSpan? timeout = null)
        {
            return playerObservers.Attack(target.Id, timeout);
        }

        public void TargetTile(string tileInfo)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.TargetTile(tileInfo);
        }

        public void Target(Item item)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.Target(item);
        }

        public void Target(Player player)
        {
            CheckCancellation();

            journalSource.NotifyLastAction();
            targeting.Target(player);
        }

        public void Terminate(string parameters)
        {
            try
            {
                Log("Terminate attempt");
                if (string.IsNullOrEmpty(parameters))
                    CommandHandler.Terminate();
                else
                    CommandHandler.Terminate(parameters);
            }
            finally
            {
                Log("Terminate attempt finished");
            }
        }

        public string Info()
        {
            return targeting.Info();
        }

        private void InfoCommand()
        {
            var info = Info();
            ClientPrint(!string.IsNullOrEmpty(info) ? info : "Targeting cancelled.");
        }

        public Item ItemInfo()
        {
            var itemId = targeting.ItemIdInfo();

            Item item;
            if (!Items.TryGet(itemId, out item))
                return null;

            return item;
        }

        public void WaitForTarget()
        {
            CheckCancellation();

            targeting.WaitForTarget();
        }

        public void DropItem(Item item, Item targetContainer)
        {
            CheckCancellation();

            Server.DropItem(item.Id, targetContainer.Id);
        }

        public void DragItem(Item item)
        {
            DragItem(item, item.Amount);
        }

        public void DragItem(Item item, ushort amount)
        {
            CheckCancellation();
            itemsObserver.DraggedItemId = item.Id;

            Server.DragItem(item.Id, amount);
        }

        public bool MoveItem(Item item, Item targetContainer, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
        {
            return MoveItem(item, item.Amount, targetContainer, timeout, dropDelay);
        }

        public bool MoveItem(Item item, ushort amount, Item targetContainer, TimeSpan? timeout = null,
            TimeSpan? dropDelay = null)
        {
            DragItem(item, amount);
            if (WaitForItemDragged(timeout) != DragResult.Success)
                return false;

            if (dropDelay.HasValue)
                Wait(dropDelay.Value);

            DropItem(item, targetContainer);

            return true;
        }

        public DragResult WaitForItemDragged(TimeSpan? timeout = null)
        {
            return itemsObserver.WaitForItemDragged(timeout);
        }

        public void Log(string message)
        {
            logger.Info(message);
        }

        public void TriggerGump(uint triggerId)
        {
            gumpObservers.TriggerGump(triggerId);
        }

        public GumpResponseBuilder GumpResponse()
        {
            return gumpObservers.GumpResponse();
        }

        public void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            gumpObservers.SelectGumpButton(buttonLabel, labelPosition);
        }

        public void LastGumpInfo()
        {
            var gumpInfo = gumpObservers.LastGumpInfo();
            Log(gumpInfo);
        }

        public void CloseGump()
        {
            gumpObservers.CloseGump();
        }

        public void StepToward(Location2D currentLocation, Location2D targetLocation)
        {
            var walkVector = (targetLocation - currentLocation).Normalize();
            if (walkVector != Vector.NullVector)
            {
                var movementType = Me.CurrentStamina > Me.MaxStamina / 10 ? MovementType.Run : MovementType.Walk;

                var direction = walkVector.ToDirection();
                if (Me.Movement.Direction == direction)
                    WaitToAvoidFastWalk(movementType);

                Walk(direction, movementType);
                WaitWalkAcknowledged();
            }
        }

        public void StepToward(Item item)
        {
            StepToward(item.Location);
        }

        public void StepToward(Location2D targetLocation)
        {
            StepToward(Me.Location, targetLocation);
        }

        public void WalkTo(Location2D targetLocation)
        {
            while (Me.Location != targetLocation)
            {
                StepToward(targetLocation);
            }
        }

        public void WalkTo(ushort xloc, ushort yloc)
        {
            WalkTo(new Location2D(xloc, yloc));
        }

        internal void WalkTo(string parameters)
        {
            var parser = new CommandParameterParser(parameters);
            WalkTo((ushort) parser.ParseInt(), (ushort) parser.ParseInt());
        }

        public bool Wear(Item item, Layer layer, TimeSpan? timeout = null)
        {
            DragItem(item, 1);
            if (WaitForItemDragged(timeout) != DragResult.Success)
                return false;

            Server.Wear(item.Id, layer, Me.PlayerId);

            return true;
        }

        public void CastSpell(Spell spell)
        {
            journalSource.NotifyLastAction();

            Server.CastSpell(spell);
        }

        public void UseSkill(Skill skill)
        {
            journalSource.NotifyLastAction();

            Server.UseSkill(skill);
        }

        public void OpenDoor()
        {
            Server.OpenDoor();
        }

        public void Ignore(Item item)
        {
            IgnoredItems.Add(item.Id);
        }

        public void ClientPrint(string message, string name, uint itemId, ModelId itemModel, SpeechType type,
            Color color,  bool log = true)
        {
            Client.SendSpeech(message, name, itemId, itemModel, type, color);
            if (log)
                Log(message);
        }

        public void ClientPrint(string message, bool log = true)
        {
            ClientPrint(message, "System", 0, 0, SpeechType.Normal, (Color) 0x03B2, log);
        }

        public void ClientPrint(string message, string name, Player onBehalfPlayer, bool log = true)
        {
            ClientPrint(message, name, onBehalfPlayer.PlayerId, onBehalfPlayer.BodyType, SpeechType.Speech,
                (Color) 0x0026, log);
        }

        public void ClientPrint(string message, string name, Item onBehalfItem, bool log = true)
        {
            ClientPrint(message, name, onBehalfItem.Id, onBehalfItem.Type, SpeechType.Speech, (Color) 0x0026, log);
        }

        public void CloseContainer(Item container)
        {
            Client.CloseContainer(container.Id);
        }
    }
}