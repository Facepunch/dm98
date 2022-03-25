using Sandbox.UI;

public class HudRootPanel : RootPanel
{
	public HudRootPanel()
	{
		StyleSheet.Load( "/resource/styles/hud.scss" );
		SetTemplate( "/resource/templates/hud.html" );

		AddChild<DamageIndicator>();
		AddChild<HitIndicator>();

		AddChild<InventoryBar>();
		AddChild<PickupFeed>();

		AddChild<ChatBox>();
		AddChild<KillFeed>();
		AddChild<Scoreboard>();
		AddChild<VoiceList>();
	}

	protected override void UpdateScale( Rect screenSize )
	{
		base.UpdateScale( screenSize );
	}
}
