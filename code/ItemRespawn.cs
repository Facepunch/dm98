/// <summary>
/// Any entities that implement this interface are added as a record and respawned
/// So it should really just be weapons, ammo and healthpacks etc
/// </summary>
public interface IRespawnableEntity
{

}

public class ItemRespawn
{
	/// <summary>
	/// A record of an entity and its position
	/// </summary>
	public class Record
	{
		public Transform Transform;
		public string ClassName;
	}

	/// <summary>
	/// a list of entity records
	/// </summary>
	static Dictionary<Entity, Record> Records = new();

	/// <summary>
	/// Create a bunch of records from the existing entities. This should be called before
	/// any players are spawned, but right after the level is loaded.
	/// </summary>
	public static void Init()
	{
		Records = new();

		foreach ( var entity in Entity.All )
		{
			if ( entity is IRespawnableEntity )
			{
				AddRecordFromEntity( entity );
			}
		}
	}

	/// <summary>
	/// Respawn this entity if it gets deleted (and Taken is called before!)
	/// </summary>
	/// <param name="ent"></param>
	public static void AddRecordFromEntity( Entity ent )
	{
		var record = new Record
		{
			Transform = ent.Transform,
			ClassName = ent.ClassInfo.Name
		};

		Records[ent] = record;
	}

	/// <summary>
	/// Entity has been picked up, or deleted or something.
	/// If it was in our records, start a respawn timer
	/// </summary>
	public static void Taken( Entity ent )
	{
		if ( Records.Remove( ent, out var record ) )
		{
			_ = RespawnAsync( record );
		}
	}

	/// <summary>
	/// Async Respawn timer. Wait 30 seconds, spawn the entity, add a record for it.
	/// </summary>
	static async Task RespawnAsync( Record record )
	{
		await GameTask.Delay( 1000 * 30 );

		// TODO - find a sound that sounds like the echoey crazy truck horn sound that played in HL1 when items spawned
		Sound.FromWorld( "dm.item_respawn", record.Transform.Position + Vector3.Up * 50 );

		var ent = Library.Create<Entity>( record.ClassName );
		ent.Transform = record.Transform;

		Records[ent] = record;
	}
}
