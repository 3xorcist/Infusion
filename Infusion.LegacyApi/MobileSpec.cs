using System;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class MobileSpec
    {
        private readonly MobileSpec[] childSpecs;

        public MobileSpec(string name)
        {
            Specificity = SpecSpecificity.Name;
            Name = name;
        }

        public MobileSpec(ModelId type, Color? color = null)
        {
            Specificity = !string.IsNullOrEmpty(Name) ? SpecSpecificity.Name : 
                color.HasValue ? SpecSpecificity.TypeAndColor : SpecSpecificity.Type;

            Type = type;
            Color = color;
        }

        internal MobileSpec(params MobileSpec[] childSpecs)
        {
            Specificity = SpecSpecificity.CompositeSpecificity;
            this.childSpecs = childSpecs;
        }

        private string Name { get; }

        private ModelId? Type { get; }
        private Color? Color { get; }
        public SpecSpecificity Specificity { get; }

        public bool Matches(Mobile mobile)
        {
            if (Type.HasValue)
                return mobile.Type == Type && (!Color.HasValue || Color == mobile.Color);
            if (childSpecs != null && childSpecs.Length > 0)
                return childSpecs.Any(s => s.Matches(mobile));

            if (Name != null)
                return mobile.Name == Name;

            throw new NotImplementedException();
        }

        public bool Matches(Corpse corpse)
        {
            if (Type.HasValue)
                return corpse.CorpseType == Type && (!Color.HasValue || Color == corpse.Color);
            if (childSpecs != null && childSpecs.Length > 0)
                return childSpecs.Any(s => s.Matches(corpse));

            if (Name != null)
                return corpse.Name == Name;

            throw new NotImplementedException();
        }

        public bool Matches(ModelId type)
        {
            if (Type.HasValue)
                return type == Type && !Color.HasValue;

            return childSpecs.Any(s => s.Matches(type));
        }

        public MobileSpec Including(params MobileSpec[] childSpecs)
        {
            return new MobileSpec(childSpecs.Concat(new[] { this }).ToArray());
        }

        public static implicit operator MobileSpec(ushort[] types)
        {
            return new MobileSpec(types.Select(t => new MobileSpec(t)).ToArray());
        }

        public static implicit operator MobileSpec(ModelId[] types)
        {
            return new MobileSpec(types.Select(t => new MobileSpec(t)).ToArray());
        }

        public static implicit operator MobileSpec(int[] types)
        {
            return new MobileSpec(types.Select(t => new MobileSpec((ushort)t)).ToArray());
        }

        public static implicit operator MobileSpec(MobileSpec[] specs)
        {
            return new MobileSpec(specs);
        }

        public static implicit operator MobileSpec(ushort type)
        {
            return new MobileSpec(type);
        }
    }
}