using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class PickupFeed : Panel
{
	public static PickupFeed Current;

	public PickupFeed()
	{
		Current = this;
	}

	/// <summary>
	/// An RPC which can be called from the server 
	/// </summary>
	[ClientRpc]
	public static void OnPickup( string text )
	{
		Current?.AddEntry( text );
	}

	/// <summary>
	/// An RPC which can be called from the server 
	/// </summary>
	[ClientRpc]
	public static void OnPickupWeapon( string text )
	{
		Current?.AddWeaponEntry( text );
	}

	/// <summary>
	/// Spawns a label, waits for half a second and then deletes it
	/// The :outro style in the scss keeps it alive and fades it out
	/// </summary>
	private async Task AddEntry( string text )
	{
		var panel = Current.Add.Panel( "entry" );

		panel.Add.Label( text );
		await Task.DelayRealtimeSeconds( 2.0f );
		panel.Delete();
	}

	private async Task AddWeaponEntry( string text )
	{
		var panel = Current.Add.Panel( "entry" );
		panel.AddClass( text ); ;
		panel.Add.Panel( "icon" );
		await Task.DelayRealtimeSeconds( 2.0f );
		panel.Delete();
	}
}
