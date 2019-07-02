namespace Data
{
    using System;
    using System.Collections.Generic;

    public class DictionaryTree<T, Y> : Dictionary<Y, DictionaryTree<T, Y>> where Y : class
    {
        public DictionaryTree<T, Y> Parent { get; private set; }
        public T Data { get; private set; }
        protected readonly Func<T, Y> getKey;
        private Y key => getKey(Data);
        protected List<Y> GetKeyList()
        {
            var ret = new List<Y>();

            if (Parent?.key != null)
                ret.AddRange(Parent.GetKeyList());

            ret.Add(key);
            return ret;
        }

        public Y[] Key => GetKeyList().ToArray();

        public DictionaryTree(Func<T, Y> getKey, T root = default, DictionaryTree<T, Y> parent = null)
            : base()
        {
            this.getKey = getKey;
            this.Data = root;
            this.Parent = parent;
        }

        public DictionaryTree<T, Y> this[T data]
        {
            get
            {
                var key = getKey(data);
                return this[key];
            }
        }

        public DictionaryTree<T, Y> this[Y[] keys]
        {
            get
            {
                DictionaryTree<T, Y> lastDictionary = this;
                foreach (var key in keys)
                {
                    if (lastDictionary.ContainsKey(key))
                    {
                        lastDictionary = lastDictionary[key];
                    }
                    else
                    {
                        return null;
                    }
                }
                return lastDictionary;
            }
        }

        public void Add(T data)
            => New(data);

        public void Add(DictionaryTree<T, Y> dictionaryTree)
        {
            if (dictionaryTree.Parent != this)
                throw new ArgumentException("Not in the same parent");

            this.Add(getKey(dictionaryTree.Data), dictionaryTree);
        }

        public bool ContainsKey(T data)
        {
            var key = getKey(data);
            return this.ContainsKey(key);
        }

        public virtual DictionaryTree<T, Y> New(T data)
        {
            Y key = getKey(data);
            var newDictionary = new DictionaryTree<T, Y>(this.getKey, data, this);
            this.Add(key, newDictionary);

            return newDictionary;
        }

        public virtual DictionaryTree<T, Y> AddIfNew(params T[] datas)
        {
            var lastDictionary = this;
            foreach (var data in datas)
            {
                var key = getKey(data);
                if (lastDictionary.ContainsKey(key))
                    lastDictionary = lastDictionary[key];
                else
                    lastDictionary = lastDictionary.New(data);
            }

            return lastDictionary;
        }
    }
}
