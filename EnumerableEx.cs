using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

   public static class EnumerableEx
    {
        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            var list = source as IList<T>;
            if (list != null)
                return list.IndexOf(value);

            var list2 = source as IList;
            if (list2 != null)
                return list2.IndexOf(value);

            return IndexOf(source, value, null);
        }
        public static int IndexOf(this IEnumerable source, object value)
        {

            var list2 = source as IList;
            if (list2 != null)
                return list2.IndexOf(value);

            return IndexOf(source, value, null);
        }

        internal static int IndexOf(this IEnumerable source, object value, IEqualityComparer equalityComparer)
        {
            if (equalityComparer == null)
            {
                equalityComparer = ReferenceEqualityComparer.Default;
            }

            int index = 0;
            foreach (var item in source)
            {
                if (equalityComparer.Equals(item, value))
                    return index;
                index++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> equalityComparer)
        {
            if (equalityComparer == null)
            {
                equalityComparer = EqualityComparer<T>.Default;
            }
            int index = 0;
            foreach (T item in source)
            {
                if (equalityComparer.Equals(item, value))
                    return index;
                index++;
            }
            return -1;
        }
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static IEnumerator<T> ToIterator<T>(this T obj)
        {
            yield return obj;
        }
    
}
public class EnumerableWrapperList<T> : IList<T>, IList
{
    private IEnumerable<T> items;
    public EnumerableWrapperList(IEnumerable<T> items) : this(items, false)
    {
    }
    public EnumerableWrapperList(IEnumerable<T> items, bool fastMode) : this(items, fastMode, false)
    {
    }

    public EnumerableWrapperList(IEnumerable<T> items, bool fastMode, bool fixedSize)
    {
        if (fastMode)
        {
            if ((items as IList<T>) == null && (items as IList<T>) == null)
            {
                if (fixedSize)
                {
                    items = items.ToArray();
                }
                else
                {
                    items = new List<T>(items);
                }
            }
        }
        this.items = items;
    }

    public int Count
    {
        get
        {
            return items.Count();
        }
    }

    public bool IsReadOnly
    {
        get
        {
            var collection = items as ICollection<T>;
            if (collection != null)
            {
                return collection.IsReadOnly;
            }
            var list = items as IList;
            if (list != null)
            {
                return list.IsReadOnly;
            }
            return true;
        }
    }

    public bool IsFixedSize
    {
        get
        {
            var list = items as IList;
            if (list != null)
                return list.IsFixedSize;
            else
                return true;
        }
    }

    public bool IsSynchronized
    {
        get
        {
            var collection = items as ICollection;
            if (collection != null)
                return collection.IsSynchronized;
            return false;
        }
    }

    public object SyncRoot
    {
        get
        {
            var collection = items as ICollection;
            if (collection != null)
                return collection.SyncRoot;
            throw new NotSupportedException();
        }
    }

    object IList.this[int index]
    {
        get
        {
            var list = items as IList;
            if (list != null)
                return list[index];

            return items.ElementAt(index);
        }

        set
        {
            this[index] = (T)value;
        }
    }

    public T this[int index]
    {
        get
        {
            return items.ElementAt(index);
        }

        set
        {
            var list = items as IList<T>;
            if (list != null)
            {
                list[index] = value;
                return;
            }
            SetNonGeneric(index, value);
        }
    }

    private void SetNonGeneric(int index, object value)
    {
        var list2 = items as IList;
        if (list2 != null)
        {
            list2[index] = value;
            return;
        }
        throw new NotSupportedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }

    public void Add(T item)
    {
        var collection = items as ICollection<T>;
        if (collection != null)
        {
            collection.Add(item);
            return;
        }
        var list = items as IList;
        if (list != null)
        {
            list.Add(item);
            return;
        }
        items = items.Concat(item.ToEnumerable());
    }

    public void Clear()
    {
        var collection = items as ICollection<T>;
        if (collection != null)
            collection.Clear();
        throw new NotSupportedException();
    }

    public bool Contains(T item)
    {
        var collection = items as ICollection<T>;
        if (collection != null)
        {
            return collection.Contains(item);
        }
        var list = items as IList;
        if (list != null)
        {
            return list.Contains(item);
        }
        return items.Contains(item, null);
    }

    public void CopyTo(T[] array, int index)
    {
        var collection = items as ICollection<T>;
        if (collection != null)
            collection.CopyTo(array, index);
        else
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (array.Length - index < this.Count)
            {
                throw new ArgumentException();
            }
            foreach (var item in items)
            {
                array[index++] = item;
            }
        }

    }

    public bool Remove(T item)
    {
        var list = items as ICollection<T>;
        if (list != null)
        {
            return list.Remove(item);
        }
        var list2 = items as IList;
        if (list2 != null)
        {
            list2.Remove(item);
            return true;
        }
        items = items.Except(item.ToEnumerable());
        return true;
    }

    public int IndexOf(T item)
    {
        return items.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        var list = items as IList<T>;
        if (list != null)
        {
            list.Insert(index, item);
            return;
        }
        var list2 = items as IList;
        if (list2 != null)
        {
            list2.Insert(index, item);
            return;
        }
        throw new NotSupportedException();
    }

    public void RemoveAt(int index)
    {
        var list = items as IList<T>;
        if (list != null)
        {
            list.RemoveAt(index);
            return;
        }
        var list2 = items as IList;
        if (list2 != null)
        {
            list2.RemoveAt(index);
            return;
        }
        throw new NotSupportedException();
    }

    public int Add(object value)
    {
        var list = items as IList;
        if (list != null)
        {
            return list.Add(value);
        }
        var collection = items as ICollection<T>;
        if (collection != null)
        {
            collection.Add((T)value);
            return Count;
        }
        throw new NotSupportedException();
    }

    public bool Contains(object value)
    {
        var list = items as IList;
        if (list != null)
        {
            return list.Contains(value);
        }
        var collection = items as ICollection<T>;
        if (collection != null)
        {
            return collection.Contains((T)value);
        }
        foreach (var x in (IEnumerable)items)
        {
            if (x.Equals(value))
            {
                return true;
            }
        }
        return false;
    }

    public int IndexOf(object value)
    {
        return IndexOf((T)value);
    }

    public void Insert(int index, object value)
    {
        var list2 = items as IList;
        if (list2 != null)
        {
            list2.Insert(index, value);
            return;
        }
        var list = items as IList<T>;
        if (list != null)
        {
            list.Insert(index, (T)value);
            return;
        }
        throw new NotSupportedException();
    }

    public void Remove(object value)
    {
        var list = items as ICollection<T>;
        if (list != null)
        {
            list.Remove((T)value);
            return;
        }
        var list2 = items as IList;
        if (list2 != null)
        {
            list2.Remove(value);
            return;
        }
        throw new NotSupportedException();
    }

    public void CopyTo(Array array, int index)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        if (index < 0 || index > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (array.Length - index < this.Count)
        {
            throw new ArgumentException();
        }
        var collection = items as ICollection;
        if (collection != null)
        {
            collection.CopyTo(array, index);
            return;
        }

        var collection2 = items as ICollection<T>;
        if (collection2 != null)
        {
            var array2 = array as T[];
            if (array2 != null)
            {
                collection2.CopyTo(array2, index);
                return;
            }
            else
            {
                var array3 = array as object[];
                if (array3 != null)
                {
                    foreach (var item in items)
                    {
                        array3[index++] = item;
                    }
                    return;
                }
            }
        }

        var array4 = array as T[];
        if (array4 != null)
        {
            foreach (var item in items)
            {
                array4[index++] = item;
            }
            return;
        }
        else
        {
            var array3 = array as object[];
            if (array3 != null)
            {
                foreach (var item in items)
                {
                    array3[index++] = item;
                }
                return;
            }
        }
    }
}
