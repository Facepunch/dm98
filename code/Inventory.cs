partial class DmInventory : BaseInventory
{


	public DmInventory( Player player ) : base( player )
	{

	}

	public override bool Add( Entity ent, bool makeActive = false )
	{
		var player = Owner as DeathmatchPlayer;
		var weapon = ent as DeathmatchWeapon;
		var notices = !player.SupressPickupNotices;

		if ( weapon == null )
			return false;
		//
		// We don't want to pick up the same weapon twice
		// But we'll take the ammo from it Winky Face
		//
		if ( weapon != null && IsCarryingType( ent.GetType() ) )
		{
			var ammo = weapon.AmmoClip;
			var ammoType = weapon.AmmoType;

			if ( ammo > 0 )
			{
				var taken = player.GiveAmmo( ammoType, ammo );
				if ( taken == 0 )
					return false;

				if ( notices && taken > 0 )
				{
					Sound.FromWorld( "dm.pickup_ammo", ent.Position );
					PickupFeed.OnPickup( To.Single( player ), $"+{taken} {ammoType}" );
				}
			}

			ItemRespawn.Taken( ent );

			// Despawn it
			ent.Delete();
			return false;
		}

		if ( !base.Add( ent, makeActive ) )
			return false;

		if ( weapon != null && notices )
		{
			var display = DisplayInfo.For( ent );

			Sound.FromWorld( "dm.pickup_weapon", ent.Position );
			PickupFeed.OnPickupWeapon( To.Single( player ), display.Name );
		}

		ItemRespawn.Taken( ent );

		return true;
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.IsValid() && x.GetType() == t );
	}
}
