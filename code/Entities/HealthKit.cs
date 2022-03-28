using Hammer;

[Library( "dm_healthkit", Title = "HealthKit" )]
[Hammer.EditorModel( "models/gameplay/healthkit/healthkit.vmdl" )]
[EntityTool( "Health Kit", "DM98", "Health Kit Give 25 hp." )]
partial class HealthKit : DeathmatchWeapon, IRespawnableEntity
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/gameplay/healthkit/healthkit.vmdl" );

		PhysicsEnabled = true;
		UsePhysicsCollision = true;

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other is not DeathmatchPlayer pl ) return;
		if ( pl.Health >= pl.MaxHealth ) return;

		var newhealth = pl.Health + 25;

		newhealth = newhealth.Clamp( 0, pl.MaxHealth );

		pl.Health = newhealth;

		PickedUp(this);
		Log.Info( "Picked" );
	}

	public void PickedUp( Entity ent )
	{
		//As the Entity is not weapon, this is so the player does not have it in their inventory.
		Sound.FromWorld( "dm.item_health", ent.Position );
		ItemRespawn.Taken( ent );
		Delete();
	}
}
