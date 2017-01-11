﻿using UltimaRX.Packets;

public static class ItemTypes
{
    public static ModelId Hatchet = (ModelId)0x0F44;
    public static ModelId Hatchet2 = (ModelId)0x0F43;
    public static ModelId CopperExecutionersAxe = (ModelId)0x0F45;
    public static ModelId CopperVikingSword = (ModelId) 0x13B9;

    public static ModelId PickAxe = (ModelId)0xE86;

    public static ModelId FishingPole = (ModelId)0x0DBF;

    public static ModelId[] Hatchets = { Hatchet, Hatchet2, CopperExecutionersAxe };
    public static ModelId[] Knives = { CopperVikingSword };

    public static ModelId Fish1 = (ModelId)0x09CF;
    public static ModelId Fish2 = (ModelId)0x09CD;
    public static ModelId Fish3 = (ModelId)0x09CC;
    public static ModelId Fish4 = (ModelId)0x09CE;

    public static ModelId[] Fishes = {Fish1, Fish2, Fish3, Fish4};

    public static ModelId RawFishSteak = (ModelId)0x097A;

    public static ModelId Feathers = (ModelId)0x1BD1;
    public static ModelId Bird = (ModelId) 0x0006;
    public static ModelId BodyOfBird = (ModelId)0x2006;
    public static ModelId RawBird = (ModelId)0x09B9;
    public static ModelId RawRibs = (ModelId)0x09F1;

    public static ModelId Rabbit = (ModelId) 0x00CD;
    public static ModelId BodyOfRabbit = (ModelId) 0x2006;

    public static ModelId[] MassKillSubjects = {Bird, Rabbit};
    public static ModelId[] RippableBodies = {BodyOfBird, BodyOfRabbit};

    public static ModelId[] RawFood = { RawBird, RawFishSteak, RawRibs };

    public static ModelId Campfire = (ModelId)0x0DE3;

    public static ModelId BackPack = (ModelId)0xE75;
}

