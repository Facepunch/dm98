using Sandbox.UI;
using Sandbox.UI.Construct;

/// <summary>
/// When a player is within radius of the camera we add this to their entity.
/// We remove it again when they go out of range.
/// </summary>
internal class NameTagComponent : EntityComponent<DeathmatchPlayer>
{
	NameTag NameTag;

	protected override void OnActivate()
	{
		NameTag = new NameTag( Entity.Client?.Name ?? Entity.Name, Entity.Client?.SteamId );
	}

	protected override void OnDeactivate()
	{
		NameTag?.Delete();
		NameTag = null;
	}

	/// <summary>
	/// Called for every tag, while it's active
	/// </summary>
	[Event.Client.Frame]
	public void FrameUpdate()
	{
		var tx = Entity.GetAttachment( "hat" ) ?? Entity.Transform;
		tx.Position += Vector3.Up * 10.0f;
		tx.Rotation = Rotation.LookAt( -Camera.Rotation.Forward );

		NameTag.Transform = tx;
	}

	/// <summary>
	/// Called once per frame to manage component creation/deletion
	/// </summary>
	[Event.Client.Frame]
	public static void SystemUpdate()
	{
		foreach ( var player in Sandbox.Entity.All.OfType<DeathmatchPlayer>() )
		{
			if ( player.IsLocalPawn && player.IsFirstPersonMode )
			{
				var c = player.Components.Get<NameTagComponent>();
				c?.Remove();
				continue;
			}

			var shouldRemove = player.Position.Distance( Camera.Position ) > 500;
			shouldRemove = shouldRemove || player.LifeState != LifeState.Alive;
			shouldRemove = shouldRemove || player.IsDormant;

			if ( shouldRemove )
			{
				var c = player.Components.Get<NameTagComponent>();
				c?.Remove();
				continue;
			}

			// Add a component if it doesn't have one
			player.Components.GetOrCreate<NameTagComponent>();
		}
	}
}

/// <summary>
/// A nametag panel in the world
/// </summary>
public class NameTag : WorldPanel
{
	public Panel Avatar;
	public Label NameLabel;

	internal NameTag( string title, long? steamid )
	{
		StyleSheet.Load( "Resource/styles/nametag.scss" );

		if ( steamid != null )
		{
			Avatar = Add.Panel( "avatar" );
			Avatar.Style.SetBackgroundImage( $"avatar:{steamid}" );
		}

		NameLabel = Add.Label( title, "title" );

		// this is the actual size and shape of the world panel
		PanelBounds = new Rect( -500, -100, 1000, 200 );
	}
}
