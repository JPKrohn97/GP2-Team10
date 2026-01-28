using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;


public class ManagerObjectPool : Singleton<ManagerObjectPool>
{
    public List<ObjectPool> ObjectPoolsList;

    protected override void Awake()
    {
        base.Awake();
        InitObjectPools();
    }

    private void InitObjectPools()
    {
        for (int i = 0; i < ObjectPoolsList.Count; i++)
        {
            ObjectPoolsList[i].InitializePool();
        }
    }

    public GameObject Spawn(ObjectPoolType poolType)
    {
        ObjectPool pool = GetObjectPool(poolType);
        if (pool == null) return null;

        GameObject clone = pool.GetNextObject();
        if (clone == null) return null;

        clone.SetActive(true);
        //clone.transform.localRotation = Quaternion.identity;

        return clone;
    }
    public GameObject Spawn(ObjectPoolType poolType, Transform t)
    {
        ObjectPool pool = GetObjectPool(poolType);
        if (pool == null) return null;

        GameObject clone = pool.GetNextObject();
        if (clone == null) return null;

        clone.transform.position = t.position;
        clone.SetActive(true);
        //t.transform.localRotation = Quaternion.identity;
        return clone;
    }
    public GameObject Spawn(ObjectPoolType poolType, Vector3 pos,Quaternion rot)
    {
        ObjectPool pool = GetObjectPool(poolType);
        if (pool == null) return null;

        GameObject clone = pool.GetNextObject();
        if (clone == null) return null;

        clone.transform.position = pos;
        clone.transform.rotation = rot;
        clone.SetActive(true);
        //t.transform.localRotation = Quaternion.identity;
        return clone;
    }

    public void Despawn(ObjectPoolType poolType, GameObject obj)
    {
        ObjectPool objectPool = GetObjectPool(poolType);

        obj.transform.SetParent(objectPool.ParentGameObject.transform);

        if (!objectPool.InactiveObjectsDictionary.ContainsKey(obj.GetInstanceID()))
        {
            objectPool.InactiveObjectsDictionary.Add(obj.GetInstanceID(), obj);
        }
        obj.transform.localRotation = Quaternion.identity;
        obj.SetActive(false);
    }


    public ObjectPool GetObjectPool(ObjectPoolType poolType)
    {
        for (int i = 0; i < ObjectPoolsList.Count; i++)
        {
            if (ObjectPoolsList[i].ObjectPoolType == poolType)
            {
                return ObjectPoolsList[i];
            }
        }

        return null;
    }


    [System.Serializable]
    public class ObjectPool
    {


        private List<Queue<GameObject>> _levelPools;
        public ObjectPoolType ObjectPoolType;
        public GameObject Prefab;
        public int MaximumInstanceCount;

        [HideInInspector]
        public Dictionary<int, GameObject> InactiveObjectsDictionary;

        [HideInInspector]
        public GameObject ParentGameObject;

        public void InitializePool()
        {
            InactiveObjectsDictionary = new Dictionary<int, GameObject>();
            ParentGameObject = new GameObject("[" + ObjectPoolType + "]");
            DontDestroyOnLoad(ParentGameObject);

            GameObject cloneGameObject;

            for (int i = 0; i < MaximumInstanceCount; i++)
            {
                cloneGameObject = Instantiate(Prefab);
                cloneGameObject.SetActive(false);
                cloneGameObject.transform.SetParent(ParentGameObject.transform);
                InactiveObjectsDictionary.Add(cloneGameObject.GetInstanceID(), cloneGameObject);
            }
        }


        public GameObject GetNextObject()
        {
            GameObject tempObject;

            if (InactiveObjectsDictionary.Count > 0)
            {
                tempObject = InactiveObjectsDictionary.Values.ElementAt(0);
                InactiveObjectsDictionary.Remove(InactiveObjectsDictionary.Keys.ElementAt(0));
                return tempObject;
            }
            else
            {
                Debug.Log(string.Format("PoolManager: {0} - passiveObjectsDictionary is empty. Instantiating new one.", ObjectPoolType));
                GameObject clone;
                clone = Instantiate(Prefab);
                clone.SetActive(false);
                clone.transform.SetParent(ParentGameObject.transform);
                InactiveObjectsDictionary.Add(clone.GetInstanceID(), clone);

                tempObject = InactiveObjectsDictionary.Values.ElementAt(0);
                InactiveObjectsDictionary.Remove(InactiveObjectsDictionary.Keys.ElementAt(0));
                return tempObject;
            }
        }


    }


}


