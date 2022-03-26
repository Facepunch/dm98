using Hammer;

/// <summary>
/// This charger gradually replenishes the player's health on use. Once out of charge, it will recharge after a set time.
/// </summary>
[Library( "dm_healthcharger" )]
[Hammer.SupportsSolid]
[Hammer.EditorModel( "models/gameplay/charger/charger_station.vmdl" )]
[Model( Model = "models/gameplay/charger/charger_station.vmdl" )]

partial class ChargerStation : KeyframeEntity, IUse
{
	/// <summary>
	/// This controls the amount of charge in the station, Default Value is 50.
	/// </summary>
	[Net]
	[Property( "chargerpower", Title = "Charger Power" )]
	public float DefaultChargerPower { get; set; } = 50f;

	[Net]
	public float ChargerPower { get; set; }

	/// <summary>
	/// This controls the time it takes for the charger to refill, Default Value is 60.
	/// </summary>
	[Net]
	[Property( "chargerresettime", Title = "Charger Reset Time" )]
	public float ChargerResetTime { get; set; } = 60f;

	private TimeSince TimeSinceUsed;

	public override void Spawn()
	{
		base.Spawn();

		ChargerPower = DefaultChargerPower;

		SetupPhysicsFromModel( PhysicsMotionType.Static );
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		// no power, no health
		if ( ChargerPower <= 0 )
		{
			SetState( false );
			return false;
		}
		if ( user is DeathmatchPlayer player )
		{
			if ( player.Health >= 100 ) return false;

			// standard rate of 10 health per second
			var add = 10 * Time.Delta;

			// check if charger has enough power to heal
			if ( add > ChargerPower )
				add = ChargerPower;

			TimeSinceUsed = 0;

			// fill up til full health
			if ( player.Health + add >= 100 )
			{
				add = 100 - player.Health;

				player.Health += add;
				ChargerPower -= add;

				// stop using if we finished healing
				return false;
			}

			player.Health += add;
			ChargerPower -= add;

			return true;
		}

		return false;
	}

	public void SetState( bool state )
	{
		if ( state )
		{
			//Full
			PlaySound( "dm.item_charger_full" );
		}
		else
		{
			//Empty
			PlaySound( "dm.item_charger_empty" );
		}
	}

	[Event.Tick.Server]
	private void Tick()
	{
		if ( TimeSinceUsed >= ChargerResetTime && ChargerPower <= 0 )
		{
			SetState( true );
			ChargerPower = DefaultChargerPower;
		}
	}

	[Event.Tick.Client]
	private void ClientTick()
	{

		SceneObject?.Attributes.Set( "PowerCharge", (ChargerPower / DefaultChargerPower) * .5f );
	}

}
