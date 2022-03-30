using Sandbox.UI;
using System.Threading.Tasks;

public partial class DamageIndicator : Panel
{
	public static DamageIndicator Current;

	public DamageIndicator()
	{
		Current = this;
	}

	public void OnHit( Vector3 pos )
	{
		var p = new HitPoint( pos );
		p.Parent = this;
	}

	public class HitPoint : Panel
	{
		public Vector3 Position;

		public HitPoint( Vector3 pos )
		{
			Position = pos;

			_ = Lifetime();
		}

		public override void Tick()
		{
			base.Tick();

			var wpos = CurrentView.Rotation.Inverse * (Position.WithZ( 0 ) - CurrentView.Position.WithZ( 0 )).Normal;
			wpos = wpos.WithZ( 0 ).Normal;

			var angle = MathF.Atan2( wpos.y, -1.0f * wpos.x );

			var pt = new PanelTransform();

			pt.AddTranslateX( Length.Percent( -50.0f ) );
			pt.AddTranslateY( Length.Percent( -0.0f ) );
			pt.AddRotation( 0, 0, angle.RadianToDegree() );

			Style.Transform = pt;

		}

		async Task Lifetime()
		{
			await Task.Delay( 200 );
			AddClass( "dying" );
			await Task.Delay( 200 );
			Delete();
		}


	}
}


