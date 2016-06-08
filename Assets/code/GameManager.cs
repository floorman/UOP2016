﻿using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public enum eState
	{
		Countdown,
		Playing,
		GameOver,
		Paused,
	}
	private eState mState;

	public const float GAME_TIME_PLAYING = 45f; // This will probably get tweaked all the time. 15 for testing. Maybe 45 for real thing?
	public const float GAME_TIME_COUNTDOWN = 3f;
	private float mGameTimer;

	private List<Car> mPlayers = new List<Car>();
	private List<NPC> mNPCs = new List<NPC>();
	private List<Transform> mPlayerSpawns = new List<Transform>();
	private List<Transform> mNPCSpawns = new List<Transform>();

	private GameObject mNPCPrefab;

	// Singleton.
	private static GameManager sSingleton;
	public static GameManager Singleton()
	{
		return sSingleton;
	}

	void Awake()
	{
		sSingleton = this;

		mPlayers = new List<Car>();
		mNPCs = new List<NPC>();

		mNPCPrefab = (GameObject) Resources.Load( "prefabs/NPC" );

		// This is a really shit way of doing it but it's the only way in Unity I know how.
		// Because we need the Transforms not the GameObjects.
		GameObject[] spawnGameObjects = GameObject.FindGameObjectsWithTag( Tags.PLAYER_SPAWN );
		foreach ( GameObject go in spawnGameObjects )
		{
			mPlayerSpawns.Add( go.transform );
		}

		spawnGameObjects = GameObject.FindGameObjectsWithTag( Tags.NPC_SPAWN );
		foreach ( GameObject go in spawnGameObjects )
		{
			mNPCSpawns.Add( go.transform );
		}

		for ( int i = 0; i < 3; i++ ) {
			SpawnNPC();
		}

		// Keep this last.
		SetState( eState.Countdown );
	}

	void Update()
	{
		switch ( mState )
		{
			case eState.Countdown:
				StateCountdown();
				break;

			case eState.Playing:
				StatePlaying();
				break;

			case eState.GameOver:
				StateGameOver();
				break;

			case eState.Paused:
				StatePaused();
				break;
		}
	}

	public eState GetState()
	{
		return mState;
	}

	public void SetState( eState state )
	{
		mState = state;

		// Ideally only set stuff in here.
		// State functions that are called per-frame is done in Update().
		switch ( state )
		{
			case eState.Countdown:
				print( "Get ready!" );

				foreach ( Car car in mPlayers )
				{
					car.SetState( Car.eState.Countdown );
				}

				foreach ( NPC npc in mNPCs )
				{
					npc.SetState( NPC.eState.Waiting );
				}

				mGameTimer = GAME_TIME_COUNTDOWN;
				break;

			case eState.Playing:
				print( "GO!" );

				foreach ( Car car in mPlayers )
				{
					car.SetState( Car.eState.Playing );
				}

				foreach ( NPC npc in mNPCs )
				{
					npc.SetState( NPC.eState.Alive );
				}

				mGameTimer = GAME_TIME_PLAYING;
				break;

			case eState.GameOver:
				{
					foreach ( Car car in mPlayers )
					{
						car.SetState( Car.eState.GameOver );
					}

					foreach ( NPC npc in mNPCs )
					{
						npc.SetState( NPC.eState.Waiting );
					}

					// Determine winner.
					Car winner = GetWinningPlayer();
					print( "Player " + winner.GetPlayerNumber() + " wins!" );
					print( "Game over! Press Enter (temporary) to restart game." );
				}
				break;

			case eState.Paused:
				break;
		}
	}

	private void StateCountdown()
	{
		mGameTimer = Mathf.Max( mGameTimer - Time.deltaTime, 0f );
		// 		print( mGameTimer );
		if ( mGameTimer <= 0f )
		{
			SetState( eState.Playing );
		}
	}

	private void StatePlaying()
	{
		mGameTimer = Mathf.Max( mGameTimer - Time.deltaTime, 0f );
		// 		print( mGameTimer );
		if ( mGameTimer <= 0f )
		{
			SetState( eState.GameOver );
		}
	}

	private void StateGameOver()
	{
		if ( Input.GetKey( KeyCode.Return ) )
		{
			Application.LoadLevel( Application.loadedLevelName );
		}
	}

	private void StatePaused()
	{
		// Press escape or start to unpause or whatever...
	}

	public List<Car> GetPlayers()
	{
		return mPlayers;
	}

	/// <summary>
	/// <para>Returns the player from the list of players.</para>
	/// </summary>
	/// <param name="index">The zero-oriented player number.</param>
	/// <returns>The player from the list of players.</returns>
	public Car GetPlayer( int index )
	{
		return mPlayers[index - 1];
	}

	public Transform GetPlayerSpawn( int index )
	{
		return mPlayerSpawns[index - 1];
	}

	private Car GetWinningPlayer()
	{
		Car winning = null;
		int highestScore = 0;
		foreach ( Car car in mPlayers )
		{
			if ( car.GetScoreManager().Score > highestScore )
			{
				winning = car;
				winning.GetScoreManager().Score = car.GetScoreManager().Score;
				highestScore = car.GetScoreManager().Score;
			}
		}

		return winning;
	}

	public List<NPC> GetNPCs()
	{
		return mNPCs;
	}

	public void SpawnNPC()
	{
		// Old impl.
		//Transform npcSpawn = GetRandomNPCSpawn();
		//GameObject npc = (GameObject) Instantiate( mNPCPrefab, npcSpawn.position, npcSpawn.rotation );

		// New impl.
		float radius = 15f; // ARBITRARY!
		Vector2 unitCircle = Random.insideUnitCircle;
		Vector3 circle = new Vector3( unitCircle.x, 0f, unitCircle.y ) * radius;

		NavMeshHit hit;
		NavMesh.SamplePosition( circle, out hit, radius, 1 );
		Vector3 euler = new Vector3( 0f, Random.Range( -180f, 180f ), 0f );

		GameObject npc = Instantiate( mNPCPrefab, hit.position, Quaternion.Euler( euler ) ) as GameObject;
        npc.GetComponent<NPC>().SetState(NPC.eState.Alive);
	}

	private Transform GetRandomNPCSpawn()
	{
		return mNPCSpawns[Random.Range( 0, mNPCSpawns.Count )];
	}

	public void TogglePause()
	{
		bool currentlyPaused = ( Time.timeScale == 0f );
		if ( currentlyPaused )
		{
			Time.timeScale = 1f;

			// Disable pause menu.
		}
		else
		{
			Time.timeScale = 0f;

			// Enable pause menu.
		}
	}

    public float GetGameTime()
    {
        return mGameTimer;
    }
}