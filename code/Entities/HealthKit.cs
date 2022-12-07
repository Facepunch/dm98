/// <summary>
/// Gives 25 health points.
/// </summary>
[Library( "dm_healthkit" ), HammerEntity]
[EditorModel( "models/gameplay/healthkit/healthkit.vmdl" )]
[Title(  "Health Kit" )]
partial class HealthKit : ModelEntity, IRespawnableEntity
{
	public static readonly Model WorldModel = Model.Load( "models/gameplay/healthkit/healthkit.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		Tags.Add( "item" );
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not DeathmatchPlayer pl ) return;
		if ( pl.Health >= pl.MaxHealth ) return;

		var newhealth = pl.Health + 25;

		newhealth = newhealth.Clamp( 0, pl.MaxHealth );

		pl.Health = newhealth;

		Sound.FromWorld( "dm.item_health", Position );
		ItemRespawn.Taken( this );
		Delete();
	}
}
