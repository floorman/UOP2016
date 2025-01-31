﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eItemState
{
    INACTIVE,
    ACTIVE
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class ScoreItem : MonoBehaviour
{
	#region Fields
	public int points;
	public float absorbDistance;

	private Transform mTransform;
	private Vector3 mSuckDirection;

	private float mSpeed;
	private float mSuckPower;

	private Rigidbody mRigidbody;
	private Collider mCollider;
	#endregion

    #region Properties
    public eItemState mState;
    #endregion

    #region Unity Methods
    private void Start()
	{
		mRigidbody = GetComponent<Rigidbody>();
		mCollider = GetComponent<Collider>();

		mTransform = this.transform;
		mSpeed = 2f;
		mSuckPower = 4f;

		SetState( eItemState.INACTIVE );
	}

	private void Update()
	{
        switch (mState)
        {
            case eItemState.INACTIVE:
                // TODO: ...Only Cthulhu knows.
                StateInactive();
                break;

            case eItemState.ACTIVE:
                StateActive();
                break;
        }

//         Debug.Log(mState);
	}

	private void OnCollisionEnter( Collision col )
	{
		switch ( col.gameObject.tag )
		{
			case Tags.PLAYER:
				{
					// Get the score component.
					ScoreManager sm = col.gameObject.GetComponent<ScoreManager>();

					// It should have a score manager though...
					if ( sm )
					{
// 		 				Debug.Log("Incrementing score to ScoreManager.");
						sm.Increase( points );

						//Debug.Log(sm.Score);
					}

					// Temporary step for getting rid of it.
					Destroy( this.gameObject );
				}
				break;

			case Tags.PLAYER_WHEEL:
				Physics.IgnoreCollision( mCollider, col.collider );
				break;

			case Tags.SHELF:
				if ( mState == eItemState.ACTIVE )
				{
					Physics.IgnoreCollision( mCollider, col.collider );
				}
				break;
		}
	}
	#endregion

    #region Methods
	public eItemState GetState()
	{
		return mState;
	}

	public void SetState( eItemState state )
	{
		mState = state;

		switch ( state ) {
			case eItemState.INACTIVE:
				if ( mRigidbody )
				{
					mRigidbody.isKinematic = true;
				}
				break;

			case eItemState.ACTIVE:
				if ( mRigidbody )
				{
					mRigidbody.isKinematic = false;
				}
				break;
		}
	}

    private void StateInactive()
    {
    }

    private void StateActive()
    {
        if (GameManager.Singleton().GetState() == GameManager.eState.Playing)
        {
            foreach (Car player in GameManager.Singleton().GetPlayers())
            {
                // Check the distance of each player from the item.
                float dist = Vector3.Distance(mTransform.position, player.transform.position);

                if (dist < absorbDistance)
                {
                    float suckSpeed = (mSpeed / dist) * mSuckPower;
                    mTransform.position = Vector3.MoveTowards(mTransform.position, player.transform.position, suckSpeed * Time.deltaTime);
                }
            }
        }
    }
    #endregion
}
