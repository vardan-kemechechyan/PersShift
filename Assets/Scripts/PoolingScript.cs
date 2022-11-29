using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingScript : Singleton<PoolingScript>
{
	[Header( "Object Prefab Declaration" )]
	public List<PullableObjectClass> AllPoolableObjectDefinition;

	[Header("Pool instantiation z-location")]
	public float gapBetweenObjects = 3f, z_poisition = 0f;
	
	public Dictionary<string, PullableObjectClass> THE_POOL = new Dictionary<string, PullableObjectClass>();

	bool initialised = false;

	void Initialize()
	{
		foreach ( var PoolableObjectInfo in AllPoolableObjectDefinition )
		{
			THE_POOL.Add( PoolableObjectInfo.PullableObject.name, PoolableObjectInfo );
		}

		InitializeThePool();

		initialised = true;
	}

	public GameObject Pull( string ObjectTagOrName )
	{
		if(!initialised) Initialize();

		PullableObjectClass QueriedObjectPoolInfo;
		
		if( THE_POOL.TryGetValue( ObjectTagOrName, out QueriedObjectPoolInfo ) )
		{
			if ( QueriedObjectPoolInfo.PullableObjectPool.Count != 0 )
			{
				var tGO = QueriedObjectPoolInfo.PullableObjectPool.Dequeue();

				if ( tGO == null ) Pull( ObjectTagOrName );

				tGO.SetActive(true);

				return tGO;
			}
			else
			{
				var t_go = Instantiate(Instantiate(QueriedObjectPoolInfo.PullableObject, transform.position + Vector3.forward * z_poisition, Quaternion.identity, transform));

				t_go.name = QueriedObjectPoolInfo.PullableObject.name;

				QueriedObjectPoolInfo.PullableObjectPool.Enqueue(t_go);
				z_poisition -= gapBetweenObjects;

				t_go.SetActive(true);

				return QueriedObjectPoolInfo.PullableObjectPool.Dequeue();
			}
		}
		else
			Debug.LogError( "Couldn't find the corresponding pool of objects!\nCurrent queried string: " + ObjectTagOrName + "\nReturning NULL!" );

		return null;
	}

	public void ReturnToPool( GameObject ObjectReturningToThePool )
	{
		PullableObjectClass QueriedObjectPoolInfo;

		string ObjectTagOrName = ObjectReturningToThePool.name;

		if ( THE_POOL.TryGetValue( ObjectTagOrName, out QueriedObjectPoolInfo ) )
		{
			QueriedObjectPoolInfo.PullableObjectPool.Enqueue( ObjectReturningToThePool );
			ObjectReturningToThePool.transform.position = transform.position + Vector3.forward * z_poisition;
			z_poisition -= gapBetweenObjects;
			ObjectReturningToThePool.SetActive( false );
		}
		else
			Debug.LogWarning( "Couldn't find the corresponding pool of objects!\nCurrent queried string: " + ObjectTagOrName + "\nReturning NULL!" );
	}

	public void InitializeThePool()
	{
		int LOOP_CYCLE_COUNT = 0;

		foreach ( var PoolObjectInfo in AllPoolableObjectDefinition )
			if ( PoolObjectInfo.PullableObjectCount >= LOOP_CYCLE_COUNT ) LOOP_CYCLE_COUNT = PoolObjectInfo.PullableObjectCount;

		foreach ( var EachPool in THE_POOL )
		{
			for ( int i = 0; i < LOOP_CYCLE_COUNT; ++i )
			{
				if( i < EachPool.Value.PullableObjectCount )
				{
					var t_go = Instantiate(EachPool.Value.PullableObject, transform.position + Vector3.forward * z_poisition, Quaternion.identity, transform);

					t_go.name = EachPool.Value.PullableObject.name;

					EachPool.Value.PullableObjectPool.Enqueue( t_go );

					z_poisition -= gapBetweenObjects;
				}
			}
		}
	}
	
	public void ClearAll()
	{
		foreach ( var queue in THE_POOL ) queue.Value.PullableObjectPool.Clear();

		z_poisition = 0f;

		InitializeThePool();
	}
}

[Serializable]
public class PullableObjectClass
{
	public GameObject PullableObject;
	public int PullableObjectCount;
	[HideInInspector] public Queue<GameObject> PullableObjectPool = new Queue<GameObject>();
}