using System.Collections.Generic;
using HotChocolate.Types;

namespace StrawberryShake.Generators.Descriptors
{
    public class ClassDescriptor
        : IClassDescriptor
    {
        public ClassDescriptor(
            string name,
            INamedType type,
            IInterfaceDescriptor implements)
            : this(name, type, new[] { implements })
        {
        }

        public ClassDescriptor(
            string name,
            INamedType type,
            IReadOnlyList<IInterfaceDescriptor> implements)
        {
            Name = name;
            Type = type;
            Implements = implements;
            Fields = CreateFields(type, implements);
        }

        public string Name { get; }

        public INamedType Type { get; }

        public IReadOnlyList<IInterfaceDescriptor> Implements { get; }

        public IReadOnlyList<IFieldDescriptor> Fields { get; }

        IEnumerable<ICodeDescriptor> ICodeDescriptor.GetChildren() =>
            Implements;

        private static IReadOnlyList<IFieldDescriptor> CreateFields(
            INamedType type,
            IReadOnlyList<IInterfaceDescriptor> implements)
        {
            var handled = new HashSet<IInterfaceDescriptor>();
            var handledField = new HashSet<string>();
            var queue = new Queue<IInterfaceDescriptor>(implements);
            var list = new List<IFieldDescriptor>();

            if (type is IComplexOutputType complexType)
            {
                while (queue.Count > 0)
                {
                    IInterfaceDescriptor current = queue.Dequeue();
                    if (handled.Add(current))
                    {
                        foreach (IFieldDescriptor descriptor in current.Fields)
                        {
                            if (handledField.Add(descriptor.ResponseName)
                                && complexType.Fields.TryGetField(
                                    descriptor.Field.Name, out IOutputField field))
                            {
                                list.Add(new FieldDescriptor(
                                    field,
                                    descriptor.Selection,
                                    descriptor.Type,
                                    descriptor.Path));
                            }
                        }

                        foreach (IInterfaceDescriptor child in current.Implements)
                        {
                            queue.Enqueue(child);
                        }
                    }
                }
            }

            return list;
        }
    }
}
