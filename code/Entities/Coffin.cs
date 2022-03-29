partial class Coffin : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/dm_coffin.vmdl" );

	public List<string> Weapons = new List<string>();
	public List<int> Ammos = new List<int>();

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	public void Populate( DeathmatchPlayer player )
	{
		Ammos.AddRange( player.Ammo );

		foreach ( var child in player.Children.ToArray() )
		{
			if ( child is DeathmatchWeapon weapon )
			{
				Weapons.Add( weapon.ClassInfo.Name );
			}
		}
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( IsClient )
			return;

		if ( other is not DeathmatchPlayer player )
			return;

		if ( player.LifeState == LifeState.Dead )
			return;

		Sound.FromWorld( "dm.pickup_ammo", Position );

		foreach ( var weapon in Weapons )
		{
			player.Give( weapon );
		}

		for ( int i = 0; i < Ammos.Count; i++ )
		{
			int taken = player.GiveAmmo( (AmmoType)i, Ammos[i] );
			if ( taken > 0 )
			{
				PickupFeed.OnPickup( To.Single( player ), $"+{taken} {((AmmoType)i)}" );
			}
		}

		Log.Info( $"Touchy: {other}" );

		Delete();
	}

}
