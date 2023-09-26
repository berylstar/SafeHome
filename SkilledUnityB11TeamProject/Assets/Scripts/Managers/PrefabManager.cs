using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Pool
{
    public PoolType type;       // Ǯ���� ������Ʈ
    public GameObject prefab;   // ������
    public Transform holder;    // ������Ʈ ���� �θ�. PrefabMangaer ������Ʈ �Ʒ��� �� ���ӿ�����Ʈ�� ���� �޾��ֱ�. Hierarchy ������

    public Pool(PoolType _type, GameObject _prefab, Transform _holder)
    {
        type = _type;
        prefab = _prefab;
        holder = _holder;
    }
}

public enum PoolType            // PoolType�� ����ŭ ������Ʈ Ǯ��
{
    Monster = 30,               // ����
    SFXAudio = 30,              // ȿ����
}

public class PrefabManager : MonoBehaviour
{
    public List<Pool> pools;
    public Dictionary<PoolType, Queue<GameObject>> poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();

    private void Awake()
    {
        foreach (Pool pool in pools)
        {
            AddOnDictionary(pool);
        }
    }

    private void AddOnDictionary(Pool pool)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < (int)pool.type; i++)
        {
            GameObject _object = Instantiate(pool.prefab, pool.holder);
            _object.SetActive(false);
            objectPool.Enqueue(_object);
        }

        poolDictionary.Add(pool.type, objectPool);
    }

    public void AddAtPoolsOnScript(PoolType _type, GameObject _object)      // holder ���������� ������ GameManager.PrefabManager ������ Ȧ���� ����
    {
        Transform tf = new GameObject("Holder").transform;
        tf.SetParent(transform);

        Pool newPool = new Pool(_type, _object, tf);

        pools.Add(newPool);
        AddOnDictionary(newPool);
    }

    public void AddAtPoolsOnScript(PoolType type, GameObject obj, Transform holder)
    {
        Pool newPool = new Pool(type, obj, holder);

        pools.Add(newPool);
        AddOnDictionary(newPool);
    }

    public GameObject SpawnFromPool(PoolType type)
    {
        if (!poolDictionary.ContainsKey(type))
            return null;

        GameObject _object = poolDictionary[type].Dequeue();
        poolDictionary[type].Enqueue(_object);

        _object.SetActive(true);

        return _object;
    }
}
