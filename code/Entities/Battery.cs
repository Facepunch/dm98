using Hammer;

[Library( "dm_battery", Title = "HealthKit" )]
[Hammer.EditorModel( "models/dm_battery.vmdl" )]
[EntityTool( "Battery", "DM98", "Gives 25 Armour" )]
partial class Battery : DeathmatchWeapon, IRespawnableEntity
{
	public static readonly Model WorldModel = Model.Load( "models/dm_battery.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = true;
		UsePhysicsCollision = true;

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other is not DeathmatchPlayer player ) return;
		if ( player.Armour >= player.Armour ) return;

		var newhealth = player.Armour + 25;

		newhealth = newhealth.Clamp( 0, 100 );

		player.Armour = newhealth;

		Sound.FromWorld( "dm.item_battery", Position );
		ItemRespawn.Taken( this );
		Delete();
	}
}
