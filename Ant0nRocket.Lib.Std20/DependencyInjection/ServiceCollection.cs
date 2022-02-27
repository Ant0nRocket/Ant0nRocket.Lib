using Microsoft.Extensions.DependencyInjection;

using System.Collections;
using System.Collections.Generic;

namespace Ant0nRocket.Lib.Std20.DependencyInjection
{
    public class ServiceCollection : IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable
    {
        private readonly List<ServiceDescriptor> _descriptors = new();

        public int Count => _descriptors.Count;

        public bool IsReadOnly => false;

        public ServiceDescriptor this[int index]
        {
            get => _descriptors[index];
            set => _descriptors[index] = value;
        }

        public void Clear() =>
            _descriptors.Clear();

        public bool Contains(ServiceDescriptor item) =>
            _descriptors.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) =>
            _descriptors.CopyTo(array, arrayIndex);

        public bool Remove(ServiceDescriptor item) =>
            _descriptors.Remove(item);

        public IEnumerator<ServiceDescriptor> GetEnumerator() =>
            _descriptors.GetEnumerator();

        void ICollection<ServiceDescriptor>.Add(ServiceDescriptor item) =>
            _descriptors.Add(item);

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public int IndexOf(ServiceDescriptor item) =>
            _descriptors.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item) =>
            _descriptors.Insert(index, item);

        public void RemoveAt(int index) =>
            _descriptors.RemoveAt(index);
    }
}
