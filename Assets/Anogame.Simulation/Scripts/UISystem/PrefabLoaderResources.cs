using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib
{
    public class PrefabLoaderResources : IPrefabLoader
    {
        public IEnumerator Load(string _path, PrefabReceiver _receiver)
        {
            ResourceRequest req = Resources.LoadAsync(_path);
            yield return req;

            _receiver.prefab = req.asset;
        }

        public void Release(string _path, Object _prefab)
        {
        }
    }
}


