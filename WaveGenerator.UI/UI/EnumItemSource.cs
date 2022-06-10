using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WaveGenerator.UI
{
    public class EnumItemSource<TEnum> : IList<EnumValueReference<TEnum>> where TEnum : struct, Enum
    {
        private List<EnumValueReference<TEnum>> _data;
        public EnumItemSource()
        {
            _data = Enum.GetNames(typeof(TEnum)).Select((x) => (EnumValueReference<TEnum>)x).ToList();
        }

        public EnumValueReference<TEnum> this[int index] { get => _data[index]; set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool Contains(EnumValueReference<TEnum> item) => _data.Contains(item);

        public void CopyTo(EnumValueReference<TEnum>[] array, int arrayIndex) => _data.CopyTo(array, arrayIndex);

        public int IndexOf(EnumValueReference<TEnum> item) => _data.IndexOf(item);

        public IEnumerator<EnumValueReference<TEnum>> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

        public bool IsReadOnly => true;

        public void Add(EnumValueReference<TEnum> item) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        public void Insert(int index, EnumValueReference<TEnum> item) => throw new NotImplementedException();

        public bool Remove(EnumValueReference<TEnum> item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();
    }

    public class EnumValueReference<TEnum> where TEnum : struct, Enum
    {
        private EnumValueReference(string value)
        {
            this.Value = Enum.Parse<TEnum>(value);
        }

        public static explicit operator EnumValueReference<TEnum>(string value) => new(value);

        public TEnum Value { get; protected set; }
        public override string ToString() => Value.ToString();
    }
}
