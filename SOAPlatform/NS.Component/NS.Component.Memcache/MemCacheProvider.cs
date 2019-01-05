using NS.Framework.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.Framework.Attributes;
using NS.Component.Memcache.Caching;

namespace NS.Component.Memcache
{
    /// <summary>
    /// 基于MemCache实现的缓存。
    /// </summary>
    [Export(typeof(ICacheProvider))]
    public class MemCacheProvider : ICacheProvider
    {
        MemcachedClient cache = null;
        public MemCacheProvider()
        {
            MemcachedClient cache = new MemcachedClient("MyCache");
            //cache.SendReceiveTimeout = 5000;
            //cache.ConnectTimeout = 5000;
            //cache.MinPoolSize = 10;
            //cache.MaxPoolSize = 25;
        }

        public void Add(string key, string valKey, object value)
        {
            cache.Store(Caching.Memcached.StoreMode.Add,key, value);
        }

        public void Put(string key, string valKey, object value)
        {
            cache.Store(Caching.Memcached.StoreMode.Set, key, value);
        }

        public object Get(string key, string valKey)
        {
            return cache.Get(key);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public bool Exists(string key)
        {
            return cache.Get(key) == null;
        }

        public bool Exists(string key, string valKey)
        {
            return cache.Get(key) == null;
        }
    }

    /// <summary>
    /// 基于MemCache实现的缓存。
    /// </summary>
    [Export(typeof(ICacheProviderEx))]
    public class MemCacheProviderEx : ICacheProviderEx
    {
        MemcachedClient cache = null;
        public MemCacheProviderEx()
        {
            MemcachedClient cache = new MemcachedClient("MyCache");
            //cache.SendReceiveTimeout = 5000;
            //cache.ConnectTimeout = 5000;
            //cache.MinPoolSize = 10;
            //cache.MaxPoolSize = 25;
        }

        public void Add(string key, string valKey, object value)
        {
            cache.Store(Caching.Memcached.StoreMode.Add, key, value);
        }

        public void Put(string key, string valKey, object value)
        {
            cache.Store(Caching.Memcached.StoreMode.Set, key, value);
        }

        public object Get(string key, string valKey)
        {
            return cache.Get(key);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public bool Exists(string key)
        {
            return cache.Get(key) == null;
        }

        public bool Exists(string key, string valKey)
        {
            return cache.Get(key) == null;
        }

        public void Add(string key, string valKey, object value, DateTime expireTime)
        {
            cache.Store(Caching.Memcached.StoreMode.Add, key, value, expireTime);
        }

        public void Put(string key, string valKey, object value, DateTime expireTime)
        {
            cache.Store(Caching.Memcached.StoreMode.Set, key, value,expireTime);
        }
    }
}
